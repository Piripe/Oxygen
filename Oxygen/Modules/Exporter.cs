using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Text.RegularExpressions;

namespace Oxygen.Modules
{
    internal static class Exporter
    {
        /// <summary>
        /// Start the thread for the exporter
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="destination"></param>
        internal static void ExportSkin(Forms.ProgressBar progress, string destination)
        {
            if (Global.JSEngine != null)
            {
                TaskScheduler context = TaskScheduler.FromCurrentSynchronizationContext();

                Task.Run(() => { ExportThread(progress, destination, Global.JSEngine, context); });
            }
        }
        /// <summary>
        /// Export the skin (Warning: The code is very bad)
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="destination"></param>
        /// <param name="JSEngine"></param>
        /// <param name="context"></param>
        private static void ExportThread(Forms.ProgressBar progress, string destination, Jint.Engine JSEngine, TaskScheduler context)
        {
            string rootWorkingPath = Path.Combine(Path.GetTempPath(), "Oxygen");
            bool containsSfpFile = false;

            // Delete the temp export Directory if it already exists
            if (Directory.Exists(Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))))) { Directory.Delete(Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))), true); }

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Loading TGA Files List...";
                progress.progressBar1.Value = 0;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Loading TGA Files List...", 0);

            // Get TGA generation file (.tga.xml)
            string[] tgaFiles = GetTGAFiles(Path.Combine(rootWorkingPath, "skin"));

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Rendering TGA Images...";
                progress.progressBar1.Value = 5;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Rendering TGA Images...", 5);

            // Render all images
            for (int i = 0; i < tgaFiles.Length; i++)
            {
                GenTGAXML(rootWorkingPath, tgaFiles[i],JSEngine);
            }

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Loading VGUI Files List...";
                progress.progressBar1.Value = 50;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Loading VGUI Files List...", 50);

            // Get the VGUI files (.styles|.layout|.menu|.res) 
            string[] vguiFiles = GetVGUIFiles(Path.Combine(rootWorkingPath, "skin"));

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Preprocessing VGUI Files...";
                progress.progressBar1.Value = 55;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Preprocessing VGUI Files...", 55);

            // Apply the pre processor on all VGUI files
            for (int i = 0; i < vguiFiles.Length; i++)
            {
                string vguiFile = vguiFiles[i];
                string destinationFile = vguiFile.Replace(Path.Combine(rootWorkingPath, "skin"), Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData ?? new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))));

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile) ?? "");
                File.WriteAllText(destinationFile, Preprocessor.PreProcess(File.ReadAllText(vguiFile), vguiFile.Replace(Path.Combine(rootWorkingPath, "skin"), "").Remove(0, 1), JSEngine));
            }


            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Loading CSS Files List...";
                progress.progressBar1.Value = 60;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Loading CSS Files List...", 60);

            // Get the CSS files (.css) 
            string[] cssFiles = GetCSSFiles(Path.Combine(rootWorkingPath, "skin"));

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Preprocessing CSS Files...";
                progress.progressBar1.Value = 65;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Preprocessing CSS Files...", 65);

            // Apply the pre processor on all CSS files
            for (int i = 0; i < cssFiles.Length; i++)
            {
                string cssFile = cssFiles[i];
                string destinationFile = cssFile.Replace(Path.Combine(rootWorkingPath, "skin"), Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData ?? new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))));

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile) ?? "");
                File.WriteAllText(destinationFile, Preprocessor.PreProcess(File.ReadAllText(cssFile), cssFile.Replace(Path.Combine(rootWorkingPath, "skin"), "").Remove(0, 1), JSEngine,true));
            }

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Preprocessing SFP CSS Files...";
                progress.progressBar1.Value = 80;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Preprocessing SFP CSS Files...", 80);

            if (File.Exists(Path.Combine(rootWorkingPath, "skin", "oxygen", "sfp", "friends.custom.css")))
            {
                void installFriends()
                {
                    containsSfpFile = true;
                    File.WriteAllText(Path.Combine(Data.Steam.ClientUIDir, "friends.custom.css"), Preprocessor.PreProcess(File.ReadAllText(Path.Combine(rootWorkingPath, "skin", "oxygen", "sfp", "friends.custom.css")), Path.Combine("oxygen", "sfp", "friends.custom.css"), JSEngine, true));
                }
                if (File.Exists(Path.Combine(Data.Steam.ClientUIDir, "friends.custom.css")))
                {
                    if (Global.AutoExport)
                    {
                        File.Delete(Path.Combine(Data.Steam.ClientUIDir, "friends.custom.css"));
                        installFriends();
                    }
                    else
                    {
                        if (MessageBox.Show("Friends skin already installed, can Oxygen replace it?", "Friends skin already installed", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            File.Delete(Path.Combine(Data.Steam.ClientUIDir, "friends.custom.css"));
                            installFriends();
                        }
                    }
                }
                else
                {
                    installFriends();
                }
            }
            if (File.Exists(Path.Combine(rootWorkingPath, "skin", "oxygen", "sfp", "libraryroot.custom.css")))
            {
                void installLibrary()
                {
                    containsSfpFile = true;
                    File.WriteAllText(Path.Combine(Data.Steam.SteamUIDir, "libraryroot.custom.css"), Preprocessor.PreProcess(File.ReadAllText(Path.Combine(rootWorkingPath, "skin", "oxygen", "sfp", "libraryroot.custom.css")), Path.Combine("oxygen", "sfp", "libraryroot.custom.css"), JSEngine, true));
                }
                if (File.Exists(Path.Combine(Data.Steam.SteamUIDir, "libraryroot.custom.css")))
                {
                    if (Global.AutoExport)
                    {
                        File.Delete(Path.Combine(Data.Steam.SteamUIDir, "libraryroot.custom.css"));
                        installLibrary();
                    }
                    else
                    {
                        if (MessageBox.Show("Library skin already installed, can Oxygen replace it?", "Library skin already installed", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            File.Delete(Path.Combine(Data.Steam.SteamUIDir, "libraryroot.custom.css"));
                            installLibrary();
                        }
                    }
                }
                else
                {
                    installLibrary();
                }
            }

            Task.Factory.StartNew(() => {
                progress.workingLabel.Text = "Copying skin...";
                progress.progressBar1.Value = 90;
                if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
            }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.drawCLIProgressbar("Copying skin...", 90);

            // Set paths for copy
            string copySrc = Path.Combine(rootWorkingPath, "export", string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars())));
            string copyDest = Path.Combine(destination, string.Join("_", (Global.SkinConfig.skinSave == null ? (Global.SkinData??new Data.SkinData()).Title : Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(Global.SkinConfig.skinSave))).Split(Path.GetInvalidFileNameChars())));

            // Delete the export destination if it already exists
            try
            {
                if (Directory.Exists(copyDest)) Directory.Delete(copyDest, true);
            }
            catch (Exception ex)
            {
                while (true)
                {
                    Task.Factory.StartNew(() =>
                    {
                        progress.workingLabel.Text = "Error while copying skin : "+ex.Message;
                        progress.progressBar1.Value = 90;
                        if (progress.Cancelled) { progress.DialogResult = System.Windows.Forms.DialogResult.Cancel; }
                    }, CancellationToken.None, TaskCreationOptions.None, context);
                    progress.drawCLIProgressbar("Error while copying skin : " + ex.Message, 90);
                    Thread.Sleep(100);
                }
            }

            // Move or copy the temp export to the export destination
            if (new DirectoryInfo(copySrc).Root == new DirectoryInfo(copyDest).Root)
            {
                Directory.Move(copySrc, copyDest);
            }
            else {
                CopyDirectory(copySrc,copyDest);
                if (Directory.Exists(copySrc))
                    Directory.Delete(copySrc, true);
            }


            // Create a save file in the export to restore the current configuration later
            if (Global.SkinConfig.skinSave == null)
            {
                ProjectManager.Save(Path.Combine(copyDest, string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars())) + ".oxygen.xml"));
            }
            else
            {
                ProjectManager.Save(Path.Combine(copyDest, string.Join("_", Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(Global.SkinConfig.skinSave)).Split(Path.GetInvalidFileNameChars())) + ".oxygen.xml"));
            }

            Task.Factory.StartNew(() => { progress.DialogResult = System.Windows.Forms.DialogResult.OK; }, CancellationToken.None, TaskCreationOptions.None, context);
            progress.CLIResult = System.Windows.Forms.DialogResult.OK;
            progress.drawCLIProgressbar("Project Exported.", 100);

            if (!Global.AutoExport & containsSfpFile)
            {
                if (MessageBox.Show("This skin contains CSS files that have to be injected using SFP. Do you want to download it?","SFP Required",MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/PhantomGamers/SFP/releases/latest") { UseShellExecute = true });
                }
            }
        }
        /// <summary>
        /// Return all .tga.xlm files in the specified <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string[] GetTGAFiles(string path)
        {
            List<string> paths = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                if (file.EndsWith(".tga.xml")) { paths.Add(file); }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                paths.AddRange(GetTGAFiles(dir));
            }
            return paths.ToArray();
        }
        /// <summary>
        /// Return all .styles|.layout|.res|.menu files in the specified <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string[] GetVGUIFiles(string path)
        {
            string[] vgui_exts = new string[] { ".styles", ".layout", ".res", ".menu" };
            List<string> paths = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                if (vgui_exts.Any((x) => file.EndsWith(x))) { paths.Add(file); }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                paths.AddRange(GetVGUIFiles(dir));
            }
            return paths.ToArray();
        }
        /// <summary>
        /// Return all .css files in the specified <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string[] GetCSSFiles(string path, bool root = true)
        {
            string[] vgui_exts = new string[] { ".css" };
            List<string> paths = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                if (vgui_exts.Any((x) => file.EndsWith(x))) { paths.Add(file); }
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                if (!(root & Path.GetFileName(dir) == "oxygen"))
                {
                    paths.AddRange(GetCSSFiles(dir, false));
                }
            }
            return paths.ToArray();
        }
        /// <summary>
        /// Generate images from a .tga.xml file
        /// </summary>
        /// <param name="rootWorkingPath"></param>
        /// <param name="tgaFile"></param>
        /// <param name="JSEngine"></param>
        private static void GenTGAXML(string rootWorkingPath, string tgaFile, Jint.Engine JSEngine)
        {

            string destinationFile = Path.Combine(Path.GetDirectoryName(tgaFile)??"", Path.GetFileNameWithoutExtension(tgaFile)).Replace(Path.Combine(rootWorkingPath, "skin"), Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))));

            XDocument tgaInfos = XDocument.Load(tgaFile);
            if (tgaInfos.Root != null)
                GenTGAXMLFolder(rootWorkingPath, tgaFile, tgaInfos.Root, JSEngine);
        }
        /// <summary>
        /// Generate a folder of images from an <c>&lt;images&gt;</c> or <c>&lt;folder&gt;</c>
        /// </summary>
        /// <param name="rootWorkingPath"></param>
        /// <param name="XMLFileLocation"></param>
        /// <param name="tgaInfos"></param>
        /// <param name="JSEngine"></param>
        /// <param name="relativeFolder"></param>
        private static void GenTGAXMLFolder(string rootWorkingPath, string XMLFileLocation, XElement tgaInfos, Jint.Engine JSEngine, string relativeFolder = "")
        {

            switch (tgaInfos.Name.LocalName)
            {
                case "image":
                    XAttribute? fileName = tgaInfos.Attributes().ToList().Find((x) => x.Name == "name");
                    string destinationFile = Path.Combine(Path.GetDirectoryName(XMLFileLocation)??"", relativeFolder, fileName == null ? Path.GetFileNameWithoutExtension(XMLFileLocation) : fileName.Value).Replace(Path.Combine(rootWorkingPath, "skin"), Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))));
                    XAttribute? renderCondition = tgaInfos.Attributes().ToList().Find((x) => x.Name == "if");
                    if (renderCondition != null)
                    {
                        if (bool.Parse(JSEngine.Evaluate(renderCondition.Value).ToString()) == true) RenderImage(rootWorkingPath, destinationFile, XMLFileLocation, tgaInfos, JSEngine);
                    }
                    else
                    {
                        RenderImage(rootWorkingPath, destinationFile, XMLFileLocation, tgaInfos, JSEngine);
                    }
                    break;
                case "images":
                    void RenderImgs()
                    {
                        foreach (XElement tgaInfo in tgaInfos.Elements())
                        {
                            GenTGAXMLFolder(rootWorkingPath, XMLFileLocation, tgaInfo, JSEngine, relativeFolder);
                        }
                    }
                    renderCondition = tgaInfos.Attributes().ToList().Find((x) => x.Name == "if");
                    if (renderCondition != null)
                    {
                        if (bool.Parse(JSEngine.Evaluate(renderCondition.Value).ToString()) == true) RenderImgs();
                    }
                    else
                    {
                        RenderImgs();
                    }
                    break;
                case "folder":
                    void RenderDir()
                    {
                        XAttribute? folderName = tgaInfos.Attributes().ToList().Find((x) => x.Name == "name");
                        foreach (XElement tgaInfo in tgaInfos.Elements())
                        {
                            GenTGAXMLFolder(rootWorkingPath, XMLFileLocation, tgaInfo, JSEngine, Path.Combine(relativeFolder, folderName == null ? Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(XMLFileLocation)) : folderName.Value));
                        }
                    }
                    renderCondition = tgaInfos.Attributes().ToList().Find((x) => x.Name == "if");
                    if (renderCondition != null)
                    {
                        if (bool.Parse(JSEngine.Evaluate(renderCondition.Value).ToString()) == true) RenderDir();
                    }
                    else
                    {
                        RenderDir();
                    }
                    break;
            }
        }
        /// <summary>
        /// Generate an image from its <see cref="XElement"/>
        /// </summary>
        /// <param name="rootWorkingPath"></param>
        /// <param name="destinationFile"></param>
        /// <param name="XMLFileLocation"></param>
        /// <param name="tgaInfos"></param>
        /// <param name="JSEngine"></param>
        private static void RenderImage(string rootWorkingPath, string destinationFile, string XMLFileLocation, XElement tgaInfos, Jint.Engine JSEngine)
        {
            string file = XMLFileLocation.Replace(Path.Combine(rootWorkingPath, "skin"), "").Remove(0, 1);
            string destfile = destinationFile.Replace(Path.Combine(rootWorkingPath, "export", string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))), "").Remove(0, 1);
            try
            {
                if (int.TryParse((tgaInfos.Attribute("width")??new XAttribute("","16")).Value, out int imageWidth))
                {
                    if (imageWidth < 1)
                    {
                        ErrorManager.Error("\"width\" Attribute must be positive.", file, destfile);
                        return;
                    }
                    if (int.TryParse((tgaInfos.Attribute("height") ?? new XAttribute("", "16")).Value, out int imageHeight))
                    {
                        if (imageHeight < 1)
                        {
                            ErrorManager.Error("\"height\" Attribute must be positive.", file, destfile);
                            return;
                        }
                        Bitmap resultImage = new Bitmap(imageWidth, imageHeight);
                        Graphics g = Graphics.FromImage(resultImage);

                        // Render each source one by one (<src>)
                        foreach (XElement tgaInfo in tgaInfos.Elements())
                        {
                            void ColorizeImage(Image img, XAttribute? colorOrigin)
                            {

                                // Set the offset
                                XAttribute? offsetAttr = tgaInfo.Attribute("offset");
                                Point offset = new Point(0, 0);
                                if (offsetAttr != null)
                                {
                                    string[] offsetStr = offsetAttr.Value.Split(' ');
                                    int offsetX, offsetY = 0;
                                    if (!(int.TryParse(offsetStr[0], out offsetX) & int.TryParse(offsetStr[1], out offsetY))) ErrorManager.Error($"\"offset\" Attribute must be two numbers", file, destfile);

                                    offset = new Point(offsetX, offsetY);
                                }

                                Image resultImage;

                                if (colorOrigin == null)
                                {
                                    resultImage = img;
                                }
                                else
                                {
                                    object colorTransform = JSEngine.Evaluate(colorOrigin.Value).ToObject();

                                    if (colorTransform.GetType() == typeof(Color))
                                    {
                                        resultImage = new Bitmap(img.Width, img.Height);
                                        ImageAttributes colorImageAttributes = new ImageAttributes();

                                        ColorMatrix colorColorMatrix = new ColorMatrix(getColorMatrix((Color)colorTransform));

                                        colorImageAttributes.SetColorMatrix(colorColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                        Graphics.FromImage(resultImage).DrawImage(img, new Rectangle(new Point(0, 0), img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, colorImageAttributes);
                                    }
                                    else if (colorTransform.GetType() == typeof(Data.JS.Gradient))
                                    {
                                        resultImage = ((Data.JS.Gradient)colorTransform).Apply(img);
                                    }
                                    else
                                    {
                                        resultImage = new Bitmap(img.Width, img.Height);
                                    }
                                }
                                // TODO : Add a better color correction for Steam (See https://github.com/Piripe/Oxygen/issues/1)

                                ImageAttributes imageAttributes = new ImageAttributes();

                                ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
                                            new float[] {1,  0,  0,  0, 0},
                                            new float[] {0,  1,  0,  0, 0},
                                            new float[] {0,  0,  1,  0, 0},
                                            new float[] {0,  0,  0,  1, 0},
                                            new float[] {1.0f/255, 1.0f / 255, 1.0f / 255, 1.0f / 255, 1}});

                                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                g.DrawImage(resultImage, new Rectangle(offset, img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
                            }

                            void RenderSrc()
                            {
                                switch (tgaInfo.Name.LocalName)
                                {
                                    case "src" or "img":
                                        string sourcePath = tgaInfo.Value.StartsWith("~/") ? Path.Combine(rootWorkingPath, "skin", tgaInfo.Value.Remove(0, 2)) : Path.Combine(Path.GetDirectoryName(XMLFileLocation) ?? "", Path.GetFileNameWithoutExtension(XMLFileLocation), tgaInfo.Value);
                                        if (!File.Exists(sourcePath))
                                        {
                                            ErrorManager.Error($"The image \"{sourcePath.Replace(Path.Combine(rootWorkingPath, "skin"), "").Remove(0, 1)}\" must exist.", file, destfile);
                                            return;
                                        }
                                        using (Image img = Bitmap.FromFile(sourcePath))
                                        {
                                            ColorizeImage(img, tgaInfo.Attribute("color"));
                                        }
                                        break;
                                    default:

                                        if (int.TryParse((tgaInfo.Attribute("width") ?? new XAttribute("width", imageWidth.ToString())).Value, out int srcWidth))
                                        {
                                            if (srcWidth < 1)
                                            {
                                                ErrorManager.Error("\"width\" Attribute must be positive.", file, destfile);
                                                return;
                                            }
                                            if (int.TryParse((tgaInfo.Attribute("height") ?? new XAttribute("height", imageWidth.ToString())).Value, out int srcHeight))
                                            {
                                                if (srcHeight < 1)
                                                {
                                                    ErrorManager.Error("\"height\" Attribute must be positive.", file, destfile);
                                                    return;
                                                }
                                                if (int.TryParse((tgaInfo.Attribute("x") ?? new XAttribute("x", "0")).Value, out int srcX))
                                                {
                                                    if (int.TryParse((tgaInfo.Attribute("y") ?? new XAttribute("y", "0")).Value, out int srcY))
                                                    {

                                                        string strokeAlign = (tgaInfo.Attribute("stroke-align") ?? new XAttribute("stroke-align", "center")).Value;

                                                        if (float.TryParse((tgaInfo.Attribute("stroke-width") ?? new XAttribute("stroke-width", "1")).Value, out float strokeWidth))
                                                        {

                                                            if (strokeWidth < 1)
                                                            {
                                                                ErrorManager.Error("\"stroke-width\" Attribute must be positive.", file, destfile);
                                                                return;
                                                            }
                                                            if (int.TryParse((tgaInfo.Attribute("border-radius") ?? new XAttribute("border-radius", "0")).Value, out int borderRadius))
                                                            {
                                                                int strokePathOffset = 0;

                                                                switch (strokeAlign)
                                                                {
                                                                    case "inside":
                                                                        strokePathOffset = (int)-strokeWidth;
                                                                        break;
                                                                    case "outside":
                                                                        strokePathOffset = (int)strokeWidth;
                                                                        break;
                                                                }

                                                                using (Image img = new Bitmap(Math.Max(imageWidth, srcX + srcWidth+ (int)(strokePathOffset +strokeWidth)/2), Math.Max(imageHeight, srcY + srcHeight + (int)(strokePathOffset + strokeWidth)/2)))
                                                                {
                                                                    Graphics img_g = Graphics.FromImage(img);


                                                                    XAttribute? fillColor = tgaInfo.Attribute("fill");
                                                                    XAttribute? strokeColor = tgaInfo.Attribute("stroke");

                                                                    GraphicsPath graphicsPath = new GraphicsPath();
                                                                    GraphicsPath strokeGraphicsPath = new GraphicsPath();

                                                                    switch (tgaInfo.Name.LocalName)
                                                                    {
                                                                        case "rect":

                                                                            if (borderRadius == 0)
                                                                            {
                                                                                graphicsPath.AddRectangle(new Rectangle(srcX, srcY, srcWidth, srcHeight));
                                                                                strokeGraphicsPath.AddRectangle(new Rectangle(srcX - strokePathOffset / 2, srcY - strokePathOffset / 2, srcWidth + strokePathOffset, srcHeight + strokePathOffset));
                                                                            }
                                                                            else
                                                                            {
                                                                                img_g.SmoothingMode = SmoothingMode.HighQuality;
                                                                                graphicsPath.AddRoundedRectangle(new RectangleF(srcX - 0.5f, srcY - 0.5f, srcWidth, srcHeight), borderRadius);
                                                                                strokeGraphicsPath.AddRoundedRectangle(new RectangleF(srcX - strokePathOffset / 2f - 0.5f, srcY - strokePathOffset / 2f - 0.5f, srcWidth + strokePathOffset, srcHeight + strokePathOffset), borderRadius + (borderRadius > 0 ? strokePathOffset : -strokePathOffset) / 2);
                                                                            }
                                                                            break;
                                                                        case "ellipse":
                                                                            img_g.SmoothingMode = SmoothingMode.HighQuality;

                                                                            graphicsPath.AddEllipse(new RectangleF(srcX - 0.5f, srcY - 0.5f, srcWidth, srcHeight));
                                                                            strokeGraphicsPath.AddEllipse(new RectangleF(srcX - strokePathOffset / 2f - 0.5f, srcY - strokePathOffset / 2f - 0.5f, srcWidth + strokePathOffset, srcHeight + strokePathOffset));

                                                                            break;
                                                                        default:
                                                                            img_g.SmoothingMode = SmoothingMode.HighQuality;
                                                                            img_g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                                                            img_g.DrawString(tgaInfo.Name.LocalName, new Font(SystemFonts.DefaultFont.FontFamily, 8), Brushes.White, new Point(srcX, srcY), StringFormat.GenericDefault);

                                                                            break;
                                                                    }

                                                                    if (fillColor != null)
                                                                    {
                                                                        img_g.FillPath(Brushes.White, graphicsPath);

                                                                        ColorizeImage(img, fillColor);
                                                                    }

                                                                    img_g.Clear(Color.Transparent);
                                                                    if (strokeColor != null)
                                                                    {
                                                                        Pen strokePen = new Pen(Brushes.White, strokeWidth);

                                                                        img_g.DrawPath(strokePen, strokeGraphicsPath);

                                                                        ColorizeImage(img, strokeColor);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                            break;
                                }
                            }

                            // Conditionnal rendering : Render the source only if the condition is true
                            XAttribute? renderCondition = tgaInfo.Attributes().ToList().Find((x) => x.Name == "if");
                            if (renderCondition != null)
                            {
                                bool result = bool.Parse(JSEngine.Evaluate(renderCondition.Value).ToString());
                                if (result)
                                {
                                    RenderSrc();
                                }
                            }
                            else
                            {
                                RenderSrc();
                            }

                        }
                        // Create the destination folder
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)??"");
                        if (Path.GetExtension(destinationFile).ToLower() == ".tga")
                        {
                            // Export as TGA
                            TGASharpLib.TGA tga = TGASharpLib.TGA.FromBitmap(resultImage);
                            tga.Save(destinationFile);
                        }
                        else
                        {
                            // Export as any other format supported by GDI+
                            resultImage.Save(destinationFile);
                        }
                    }
                    else
                    {
                        ErrorManager.Error("\"height\" Attribute must be a number.", file, destfile);
                        return;
                    }
                }
                else
                {
                    ErrorManager.Error("\"width\" Attribute must be a number.", file, destfile);
                    return;
                }
            }
            catch (Exception ex)
            {
                    ErrorManager.Error(ex.ToString(), file, destfile);
                    return;
            }
        }

        /// <summary>
        /// Return the color matrix used by <see cref="RenderImage"/>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static float[][] getColorMatrix(Color color)
        {
            float[][] colorMatrixElements = {
                                new float[] {(float)(color.R) / 255,  0,  0,  0, 0},
                                new float[] {0,  (float)(color.G) / 255,  0,  0, 0},
                                new float[] {0,  0,  (float)(color.B) / 255,  0, 0},
                                new float[] {0,  0,  0,  (float)(color.A) / 255, 0},
                                new float[] {0,  0,  0,  0, 2}};
            return colorMatrixElements;
        }

        /// <summary>
        /// Simple function to recursively copy a folder
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void CopyDirectory(string src, string dest)
        {
            var dir = new DirectoryInfo(src);
            if (dir.Exists)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                Directory.CreateDirectory(dest);
                foreach (FileInfo file in dir.GetFiles())
                {
                    string targetFilePath = Path.Combine(dest, file.Name);
                    try
                    {
                        file.CopyTo(targetFilePath);
                    }
                    catch { }
                }
                foreach (DirectoryInfo subDir in dirs)
                {
                    // Exception for .git folder because Git add weird permissions so it's better to ignore this folder
                    if (subDir.Name != ".git")
                    {
                        string newDestinationDir = Path.Combine(dest, subDir.Name);
                        CopyDirectory(subDir.FullName, newDestinationDir);
                    }
                }
            }
        }
    }
}
