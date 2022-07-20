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
                string destinationFile = vguiFile.Replace(Path.Combine(rootWorkingPath, "skin"), Path.Combine(Path.Combine(rootWorkingPath, "export"), string.Join("_", (Global.SkinData??new Data.SkinData()).Title.Split(Path.GetInvalidFileNameChars()))));

                Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)??"");
                File.WriteAllText(destinationFile, Preprocessor.PreProcess(File.ReadAllText(vguiFile), vguiFile.Replace(Path.Combine(rootWorkingPath, "skin"),"").Remove(0,1), JSEngine));
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
            string[] vgui_exts = new string[] { ".styles" , ".layout" , ".res", ".menu" };
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
                            void RenderSrc()
                            {
                                string sourcePath = tgaInfo.Value.StartsWith("~/") ? Path.Combine(rootWorkingPath, "skin", tgaInfo.Value.Remove(0, 2)) : Path.Combine(Path.GetDirectoryName(XMLFileLocation)??"", Path.GetFileNameWithoutExtension(XMLFileLocation), tgaInfo.Value);
                                if (!File.Exists(sourcePath))
                                {
                                    ErrorManager.Error($"The image \"{sourcePath.Replace(Path.Combine(rootWorkingPath, "skin"), "").Remove(0, 1)}\" must exist.", file, destfile);
                                    return;
                                }
                                using (Image img = Bitmap.FromFile(sourcePath))
                                {
                                    List<XAttribute> attrs = tgaInfo.Attributes().ToList();
                                    XAttribute? colorOrigin = attrs.Find((x) => x.Name == "color");

                                    // Set the offset
                                    XAttribute? offsetAttr = attrs.Find((x) => x.Name == "offset");
                                    Point offset = new Point(0, 0);
                                    if (offsetAttr != null)
                                    {
                                        string[] offsetStr = offsetAttr.Value.Split(' ');
                                        int offsetX = 0;
                                        int offsetY = 0;
                                        if (!(int.TryParse(offsetStr[0], out offsetX) & int.TryParse(offsetStr[1], out offsetY))) ErrorManager.Error($"\"offset\" Attribute must be two numbers", file, destfile);

                                        offset = new Point(offsetX, offsetY);
                                    }

                                    Image resultImage;

                                    if (colorOrigin == null)
                                    {
                                        resultImage = img;
                                    }
                                    else{
                                        object colorTransform = JSEngine.Evaluate(colorOrigin.Value).ToObject();

                                        if (colorTransform.GetType() == typeof(Color))
                                        {
                                            resultImage = new Bitmap(img.Width, img.Height);
                                            ImageAttributes colorImageAttributes = new ImageAttributes();

                                            ColorMatrix colorColorMatrix = new ColorMatrix(getColorMatrix((Color)colorTransform));

                                            colorImageAttributes.SetColorMatrix(colorColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                            Graphics.FromImage(resultImage).DrawImage(img, new Rectangle(new Point(0,0), img.Size), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, colorImageAttributes);
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
                    ErrorManager.Error(ex.Message, file, destfile);
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
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(dest);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(dest, file.Name);
                try
                {
                    file.CopyTo(targetFilePath);
                } catch { }
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
