using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Oxygen.Modules
{
    internal static class CLI
    {
        /// <summary>
        /// Start the Command Line Interface with the specified arguments
        /// </summary>
        /// <param name="args"></param>
        internal static void StartCLI(string[] args)
        {
            Dictionary<string, string> argsDict = new Dictionary<string, string>();

            int firstArg = args.ToList().FindIndex((x) => x.StartsWith("-"));

            if (firstArg != -1)
            {
                foreach (string arg in args.Skip(firstArg))
                {
                    if (arg.StartsWith("--"))
                    {
                        argsDict.Add(arg, "");
                    }
                    else if (arg.StartsWith("-"))
                    {
                        argsDict.Add(arg, "");
                    }
                    else
                    {
                        argsDict[argsDict.Last().Key] = arg;
                    }
                }
            }
            foreach (var arg in argsDict)
            {
                switch (arg.Key)
                {
                    case "--dev":
                    case "-d":
                        Global.DevMode = true;
                        break;
                    case "--skin-path":
                    case "-p":
                        Global.SkinConfig.skinPath = arg.Value;
                        break;
                    case "--output":
                    case "-o":
                        Global.SkinConfig.skinExportPath = arg.Value;
                        break;
                    case "--help":
                    case "-h":
                    case "-?":
                        WriteHelp();
                        return;
                    default:
                        Console.WriteLine($"Unknown argument {arg.Key}\n\nEnter \"oxygen --help\" for help.");
                        return;
                }
            }
            CLI.WriteStart();



            switch (args[0])
            {
                case "build":
                    Global.AutoExport = true;
                    Global.AutoReload = true;

                    Console.WriteLine("Building...");
                    LoadAndExport();
                    break;
                case "watch":
                    Global.AutoExport = true;
                    Global.AutoReload = true;

                    if (Global.SkinConfig.skinPath != null)
                    {
                        Console.WriteLine("Watch Mode");
                        DateTime lastWrite = DateTime.MinValue;
                        while (true)
                        {
                            if (Global.SkinConfig.SkinOrigin != Data.SkinOrigin.Local)
                            {
                                Console.WriteLine("Watch Mode do not support internet skin, stopping.");
                                return;
                            }
                            DateTime getWriteTime(string path)
                            {
                                if (Directory.Exists(path))
                                {
                                    DateTime bestTime = DateTime.MinValue;
                                    foreach (string file in Directory.GetFiles(path))
                                    {
                                        FileInfo fileInfo = new FileInfo(file);
                                        if (bestTime < fileInfo.LastWriteTime)
                                        {
                                            bestTime = fileInfo.LastWriteTime;
                                        }
                                    }
                                    foreach (string dir in Directory.GetDirectories(path))
                                    {
                                        DateTime dirTime = getWriteTime(dir);
                                        if (bestTime < dirTime)
                                        {
                                            bestTime = dirTime;
                                        }
                                    }
                                    return bestTime;
                                }
                                else
                                {
                                    return new FileInfo(path).LastWriteTime;
                                }
                            }

                            DateTime WriteTime = getWriteTime(Global.SkinConfig.skinPath);



                            if (WriteTime != lastWrite)
                            {
                                lastWrite = WriteTime;
                                Console.WriteLine("Building...");
                                LoadAndExport();
                            }
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine($"Missing argument : Require the path of the skin to build. (--skin-path [path])");
                        Console.ForegroundColor = ConsoleColor.White;

                        System.Windows.Forms.Application.Exit();
                    }
                    break;
                default:
                    if (args[0].StartsWith("-")) {
                        if (Global.SkinConfig.skinPath == null)
                        {
                            Program.StartApp(new WelcomeForm());
                        }
                        else
                        {
                            Forms.ProgressBar progressDialog = new Forms.ProgressBar((Forms.ProgressBar progress) =>
                            {
                                if (Global.SkinConfig.skinPath.StartsWith("http"))
                                {
                                    Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Internet;
                                }
                                else
                                {
                                    Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Local;

                                    if (Global.SkinConfig.skinPath.EndsWith(".oxygen.xml"))
                                    {
                                        ProjectManager.Load(progress, Global.SkinConfig.skinPath);
                                    }
                                    else
                                    {
                                        ProjectManager.GenerateProject(progress);
                                    }
                                }
                            });
                            if (progressDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                Global.Editor = new Forms.Editor();
                                Program.StartApp(Global.Editor);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unknown action \"{args[0]}\"\n\nEnter \"oxygen --help\" for help.");
                        return;
                    }

                    break;
            }
        }
        /// <summary>
        /// Simple method to load and export a skin/project
        /// </summary>
        internal static void LoadAndExport()
        {
            if (Global.SkinConfig.skinPath != null)
            {

                Forms.ProgressBar progressDialog = new Forms.ProgressBar((Forms.ProgressBar progress) =>
                {
                    if (Global.SkinConfig.skinPath.StartsWith("http"))
                    {
                        Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Internet;
                    }
                    else
                    {
                        Global.SkinConfig.SkinOrigin = Data.SkinOrigin.Local;

                        if (Global.SkinConfig.skinPath.EndsWith(".oxygen.xml"))
                        {
                            ProjectManager.Load(progress, Global.SkinConfig.skinPath);
                        }
                        else
                        {
                            ProjectManager.GenerateProject(progress);
                        }
                    }
                });
                progressDialog.StartCLI();
                progressDialog = new Forms.ProgressBar((Forms.ProgressBar progress) =>
                {
                    if (Global.SkinConfig.skinExportPath != null)
                    {
                        Exporter.ExportSkin(progress, Global.SkinConfig.skinExportPath);
                    }
                    else
                    {
                        Exporter.ExportSkin(progress, System.IO.Directory.GetCurrentDirectory());
                    }
                });
                progressDialog.StartCLI();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Missing argument : Require the path of the skin to build. (--skin-path [path])");
                Console.ForegroundColor = ConsoleColor.White;

                System.Windows.Forms.Application.Exit();
            }
        }
        internal static void WriteHelp()
        {
            string help = " Usage: \aoxygen [action] [options]\n" +
                "\n" +
                "   Actions:\n" +
                "build                    : \aExport the given skin (CLI only, do not starts the interface)\n" +
                "watch                    : \aExport the given skin everytime a file is changed (CLI only, do not starts the interface)\n" +
                "\n" +
                "   Options:\n"+
                "-d, --dev                : \aStart the interface in Dev Mode\n" +
                "-h, -?, --help           : \aPrint this help message\n" +
                "-o, --output PATH        : \aSet the output path\n" +
                "-p, --skin-path PATH|URL : \aSet the path of the skin (Can be an URL, a file path, or a folder path)";
            foreach(string helpLine in help.Split('\n'))
            {
                string[] helpLineSplit = helpLine.Split('\a');
                if (helpLineSplit.Length==1)
                {
                    Console.WriteLine(helpLine);
                }
                else
                {
                    Console.Write(helpLineSplit[0]);
                    int CursorLeft = Console.CursorLeft;
                    for (int i = 0; i <= helpLineSplit[1].Length / (Console.WindowWidth-CursorLeft); i++)
                    {
                        Console.CursorLeft = CursorLeft;
                        Console.WriteLine(helpLineSplit[1].Substring(i* (Console.WindowWidth - CursorLeft), Math.Min(helpLineSplit[1].Length - i * (Console.WindowWidth - CursorLeft), (Console.WindowWidth - CursorLeft))));
                    }
                }
            }
        }
        internal static void WriteStart()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Oxygen");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($" version {Global.Version}");
            Global.ErrorConsole.AddLine(ErrorManager.LogType.Info, $"Oxygen version {Global.Version}", "Oxygen", "Startup");
        }
    }
}
