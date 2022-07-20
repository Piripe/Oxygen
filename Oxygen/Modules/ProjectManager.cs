using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace Oxygen.Modules
{
    internal static class ProjectManager
    {
        internal static void Save(string path)
        {
            XDocument doc = new XDocument(
                new XElement("save",
                    new XElement("origin", (int)Global.SkinConfig.SkinOrigin),
                    new XElement("path", Global.SkinConfig.skinPath),
                    new XElement("vars")
                )
            );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            XElement vars = doc.Root.Element("vars")??new XElement("","");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Add every vars in the <vars> element
            if (Global.JSEngine != null)
            foreach (string var in (Global.SkinData??new Data.SkinData()).Vars)
            {
                Jint.Native.JsValue jsValue = Global.JSEngine.Evaluate(var);
                if (jsValue.IsArray())
                {
                    XElement newElement = new XElement(var);
                    newElement.SetAttributeValue("type", "array");

                    foreach (object item in (object[])jsValue.ToObject())
                    {
                        newElement.Add(new XElement("item",item.ToString()));
                    }

                    vars.Add(newElement);
                }
                else
                {
                    if (jsValue.ToObject().GetType() == typeof(Color))
                    {
                        XElement newElement = new XElement(var, ((Color)jsValue.ToObject()).ToArgb());
                        newElement.SetAttributeValue("type", "color");
                        vars.Add(newElement);
                    }
                    else if (jsValue.ToObject().GetType() == typeof(double))
                    {
                        XElement newElement = new XElement(var, jsValue.ToObject().ToString());
                        newElement.SetAttributeValue("type", "double");
                        vars.Add(newElement);
                    }
                    else if (jsValue.ToObject().GetType() == typeof(bool))
                    {
                        XElement newElement = new XElement(var, jsValue.ToObject().ToString());
                        newElement.SetAttributeValue("type", "bool");
                        vars.Add(newElement);
                    }
                    else
                    {
                        XElement newElement = new XElement(var, jsValue.ToObject().ToString());
                        newElement.SetAttributeValue("type", "any");
                        vars.Add(newElement);
                    }
                }
            }
            doc.Save(path);
        }
        internal static void Load(Forms.ProgressBar progress, string path)
        {
            XDocument doc = XDocument.Load(path);
            if (doc.Root != null)
            {

                Global.SkinConfig.skinSave = path;
                Global.SkinConfig.skinPath = (doc.Root.Element("path") ?? new XElement("", "")).Value;
                Global.SkinConfig.SkinOrigin = (Data.SkinOrigin)int.Parse((doc.Root.Element("origin") ?? new XElement("", "")).Value);

                Dictionary<string, object> vars = new Dictionary<string, object>();

                foreach (XElement var in (doc.Root.Element("vars") ?? new XElement("", "")).Elements())
                {
                    switch ((var.Attribute("type") ?? new XAttribute("", "")).Value)
                    {
                        case "array":
                            List<string> items = new List<string>();
                            foreach (XElement item in var.Elements())
                            {
                                items.Add(item.Value);
                            }
                            vars.Add(var.Name.LocalName, items.ToArray());
                            break;
                        case "color":
                            if (int.TryParse(var.Value, out int resultColor)) vars.Add(var.Name.LocalName, Color.FromArgb(resultColor));
                            break;
                        case "double":
                            if (double.TryParse(var.Value, out double resultDouble)) vars.Add(var.Name.LocalName, resultDouble);
                            break;
                        case "bool":
                            if (bool.TryParse(var.Value, out bool resultBool)) vars.Add(var.Name.LocalName, resultBool);
                            break;
                        case "any":
                            vars.Add(var.Name.LocalName, var.Value);
                            break;
                    }
                }

                GenerateProject(progress, vars);
            }
        }
        internal static void GenerateProject(Forms.ProgressBar progress,Dictionary<string,object>? vars = null)
        {
            TaskScheduler context = TaskScheduler.FromCurrentSynchronizationContext();

            Task.Run(async () => {

                string rootWorkingPath = Path.Combine(Path.GetTempPath(), "Oxygen");
                
                void checkIfValidSkin()
                {
                    if (Directory.EnumerateFiles(Path.Combine(rootWorkingPath, "skin")).Count() == 0)
                    {
                        IEnumerable<string> dirs = Directory.EnumerateDirectories(Path.Combine(rootWorkingPath, "skin")).ToList().FindAll((string x) => x != ".git");
                        if (dirs.Count() >= 1)
                        {
                            try
                            {
                                if (Directory.Exists(Path.Combine(rootWorkingPath, "skin_temp"))) Directory.Delete(Path.Combine(rootWorkingPath, "skin_temp"), true);
                            }
                            catch { }
                                Directory.Move(dirs.First(), Path.Combine(rootWorkingPath, "skin_temp"));
                            try
                            {
                                Directory.Delete(Path.Combine(rootWorkingPath, "skin"), true);
                            }catch { }
                            Directory.Move(Path.Combine(rootWorkingPath, "skin_temp"), Path.Combine(rootWorkingPath, "skin"));
                        }
                        else
                        {
                            MessageBox.Show("Invalid skin.");
                            Task.Factory.StartNew(() => {
                                progress.DialogResult = DialogResult.Abort;
                            }, CancellationToken.None, TaskCreationOptions.None, context);

                            progress.CLIResult = DialogResult.Abort;
                            progress.drawCLIProgressbar("Error while importing skin: Invalid skin.", 100);
                        }
                    }
                }

                if (Global.SkinConfig.SkinOrigin == Data.SkinOrigin.Local)
                {
                    await Task.Factory.StartNew(() => {
                        progress.workingLabel.Text = "Extracting skin...";
                        if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    progress.drawCLIProgressbar("Extracting skin...", 0);

                    try
                    {
                        if (Directory.Exists(Path.Combine(rootWorkingPath, "skin"))) Directory.Delete(Path.Combine(rootWorkingPath, "skin"), true);
                    }
                    catch { }
                        Directory.CreateDirectory(Path.Combine(rootWorkingPath, "skin"));

                    if (Directory.Exists(Global.SkinConfig.skinPath))
                    {
                        Exporter.CopyDirectory(Global.SkinConfig.skinPath, Path.Combine(rootWorkingPath, "skin"));
                    }
                    else
                    {
                        System.IO.Compression.ZipFile.ExtractToDirectory(Global.SkinConfig.skinPath??"", Path.Combine(rootWorkingPath, "skin"));
                        checkIfValidSkin();
                    }
                }
                else
                {
                    await Task.Factory.StartNew(() => {
                        progress.workingLabel.Text = "Downloading skin...";
                        if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    progress.drawCLIProgressbar("Downloading skin...", 0);
                    try
                    {
                        if (Directory.Exists(Path.Combine(rootWorkingPath, "skin"))) Directory.Delete(Path.Combine(rootWorkingPath, "skin"), true);
                    }
                    catch { }
                    HttpClient http = new HttpClient();
                    Stream stream = await http.GetStreamAsync(Global.SkinConfig.skinPath??"");
                    FileStream fs = File.OpenWrite(Path.Combine(rootWorkingPath, "skin.zip"));
                    await stream.CopyToAsync(fs);
                    fs.Close();
                    await Task.Factory.StartNew(() => {
                        progress.workingLabel.Text = "Extracting skin...";
                        progress.progressBar1.Value = 20;
                        if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    progress.drawCLIProgressbar("Extracting skin...", 20);
                    System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(rootWorkingPath, "skin.zip"), Path.Combine(rootWorkingPath, "skin"));
                    File.Delete(Path.Combine(rootWorkingPath, "skin.zip"));
                    checkIfValidSkin();
                }

                Data.SkinData SkinData = new Data.SkinData();

                await Task.Factory.StartNew(() => {
                    progress.workingLabel.Text = "Loading JS Engine...";
                    progress.progressBar1.Value = 25;
                    if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                }, CancellationToken.None, TaskCreationOptions.None, context);
                progress.drawCLIProgressbar("Loading JS Engine...", 25);

                Jint.Engine JSEngine = new Jint.Engine(options =>
                {
                    // TODO : Add constraints to the JS engine
                });
                JSEngine.SetValue("Color", Color.Empty);
                JSEngine.SetValue("ColorTranslator", typeof(ColorTranslator));
                JSEngine.SetValue("matrix4x", new Func<Color, float[,], Color>((Color color, float[,] matrix) => {
                    float Rs = color.R / 255;
                    float Gs = color.G / 255;
                    float Bs = color.B / 255;
                    float As = color.A / 255;

                    float Rd = Math.Max(0, Math.Min(1, (matrix[0, 0] * Rs) + (matrix[0, 1] + Gs) + (matrix[0, 2] * Bs) + (matrix[0, 3] * As)));
                    float Gd = Math.Max(0, Math.Min(1, (matrix[1, 0] * Rs) + (matrix[1, 1] + Gs) + (matrix[1, 2] * Bs) + (matrix[1, 3] * As)));
                    float Bd = Math.Max(0, Math.Min(1, (matrix[2, 0] * Rs) + (matrix[2, 1] + Gs) + (matrix[2, 2] * Bs) + (matrix[2, 3] * As)));
                    float Ad = Math.Max(0, Math.Min(1, (matrix[3, 0] * Rs) + (matrix[3, 1] + Gs) + (matrix[3, 2] * Bs) + (matrix[3, 3] * As)));
                    return Color.FromArgb((byte)Math.Floor(Ad * 255), (byte)Math.Floor(Rd * 255), (byte)Math.Floor(Gd * 255), (byte)Math.Floor(Bd * 255));
                }));
                JSEngine.SetValue("multiplyRGB", new Func<Color, float, float, float, float, Color>((Color color, float r, float g, float b, float a) => {
                    return Color.FromArgb((byte)Math.Floor(Math.Max(0, Math.Min(255, color.A * a))), (byte)Math.Floor(Math.Max(0, Math.Min(255, color.R * r))), (byte)Math.Floor(Math.Max(0, Math.Min(255, color.G * g))), (byte)Math.Floor(Math.Max(0, Math.Min(255, color.B * b))));
                }));
                JSEngine.SetValue("multiplyHSL", new Func<Color, float, float, float, float, Color>((Color color, float h, float s, float l, float a) => {
                    return Libs.Color.HSLtoRGB((byte)Math.Floor(Math.Max(0, Math.Min(255, color.A * a))), (float)Math.Max(0, Math.Min(360, color.GetHue() / 360 * h)), (float)Math.Max(0, Math.Min(1, color.GetSaturation() * s)), (float)Math.Max(0, Math.Min(1, color.GetBrightness() * l)));
                }));
                JSEngine.SetValue("Gradient", typeof(Data.JS.Gradient));


                await Task.Factory.StartNew(() => {
                    progress.workingLabel.Text = "Loading Skin Infos...";
                    progress.progressBar1.Value = 25;
                    if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
                }, CancellationToken.None, TaskCreationOptions.None, context);
                progress.drawCLIProgressbar("Loading Skin Infos...", 25);

                if (File.Exists(Path.Combine(rootWorkingPath, "skin/infos.xml")))
                {
                    XDocument settingsDocument = XDocument.Load(Path.Combine(rootWorkingPath, "skin/infos.xml"));
                    if (settingsDocument.Root != null)
                    foreach (XElement info in settingsDocument.Root.Elements())
                    {
                        switch (info.Name.ToString())
                        {
                            case "title":
                                SkinData.Title = info.Value;
                                break;
                            case "description":
                                SkinData.Description = info.Value;
                                break;
                            case "thumbnail":
                                SkinData.Thumbnail = info.Value;
                                break;
                        }
                    }
                }

                await Task.Factory.StartNew(() => {
                    progress.workingLabel.Text = "Loading Skin Vars...";
                    progress.progressBar1.Value = 50;
                    if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                }, CancellationToken.None, TaskCreationOptions.None, context);
                progress.drawCLIProgressbar("Loading Skin Vars...", 50);

                if (File.Exists(Path.Combine(rootWorkingPath, "skin/vars.xml")))
                {
                    XDocument varsDocument = XDocument.Load(Path.Combine(rootWorkingPath, "skin/vars.xml"));
                    if (varsDocument.Root!=null)
                    foreach (XElement var in varsDocument.Root.Elements())
                    {
                        SkinData.Vars.Add(var.Value);
                    }
                }

                if (File.Exists(Path.Combine(rootWorkingPath, "skin/oxygen/oxygen.js")))
                {
                    try
                    {
                        JSEngine.Execute(File.ReadAllText(Path.Combine(rootWorkingPath, "skin/oxygen/oxygen.js")));
                    }
                    catch (Exception ex) {
                        ErrorManager.Error(ex.Message, "oxygen/oxygen.js", string.Join(":", ((ex.InnerException ?? new Exception()).StackTrace ?? "?:?:?").Split(':')[^2..^0]));
                    }
                }

                if (vars != null)
                {
                    foreach (var var in vars)
                    {
                        if (var.Value.GetType() == typeof(string[]))
                        {
                            try
                            {
                                JSEngine.Execute($"{var.Key}=[\"{string.Join("\",\"", (string[])var.Value)}\"]");
                            }
                            catch
                            {
                                ErrorManager.Error($"Variable \"{var.Key}\" not already declared.", "");
                            }
                        }
                        else if (var.Value.GetType() == typeof(Color))
                        {
                            try
                            {
                                Color colorVar = (Color)var.Value;
                            JSEngine.Execute($"{var.Key}=Color.FromArgb({colorVar.A},{colorVar.R},{colorVar.G},{colorVar.B})");
                        }
                            catch
                            {
                                ErrorManager.Error($"Variable \"{var.Key}\" not already declared.", "");
                            }
                    }
                        else
                        {
                            try
                            {
                                JSEngine.Execute($"{var.Key}=\"{var.Value}\"");
                    }
                            catch
                            {
                                ErrorManager.Error($"Variable \"{var.Key}\" not already declared.", "");
                            }
                }
                    }
                }

                await Task.Factory.StartNew(() => {
                    progress.workingLabel.Text = "Loading Skin Settings...";
                    progress.progressBar1.Value = 75;
                    if (progress.Cancelled) { progress.DialogResult = DialogResult.Cancel; }
                }, CancellationToken.None, TaskCreationOptions.None, context);
                progress.drawCLIProgressbar("Loading Skin Settings...",75);

                Data.JS.Document? Document = null;

                if (File.Exists(Path.Combine(rootWorkingPath, "skin/settings.xml")))
                {
                    try
                    {
                      XDocument settingsDocument = XDocument.Load(Path.Combine(rootWorkingPath, "skin/settings.xml"));

                        if (settingsDocument.Root != null)
                      Document = new Data.JS.Document(settingsDocument.Root, JSEngine);
                    }
                    catch (Exception ex) {
                        ErrorManager.Error(ex.Message,"settings.xml");
                    }
                }

                await Task.Factory.StartNew(() => {
                    Global.SkinData = SkinData;
                    Global.JSEngine = JSEngine;
                    Global.Document = Document;
                    progress.DialogResult = DialogResult.OK;
                }, CancellationToken.None, TaskCreationOptions.None, context);

                Global.SkinData = SkinData;
                Global.JSEngine = JSEngine;
                Global.Document = Document;
                progress.CLIResult = DialogResult.OK;
                progress.drawCLIProgressbar("Project Imported.", 100);
            });
        }
    }
}
