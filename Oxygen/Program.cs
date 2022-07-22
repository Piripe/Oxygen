using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Oxygen
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ",args));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Data.Steam.SteamRootDir != null && Global.SkinConfig.skinExportPath == null)
            {
                Global.SkinConfig.skinExportPath = Data.Steam.SkinsDir;
            }

            if (args.Length == 0)
            {
                Modules.CLI.WriteStart();
                StartApp(new WelcomeForm());
            }
            else
            {
                Modules.CLI.StartCLI(args);
            }
        }
        /// <summary>
        /// Starts the app with the specified <see cref="Form"/>
        /// </summary>
        /// <param name="mainForm"></param>
        internal static void StartApp(Form mainForm)
        {
            Application.Run(new LastFormClosingApplicationContext(mainForm));
            Application.ApplicationExit += OnApplicationExit;
        }

        static void OnApplicationExit(object? sender, EventArgs e)
        {
            Directory.Delete(Path.Combine(Path.GetTempPath(),"Oxygen"),true);
        }
        internal class LastFormClosingApplicationContext : ApplicationContext
        {
            public LastFormClosingApplicationContext(Form mainForm) : base(mainForm) { }
            protected override void OnMainFormClosed(object? sender, EventArgs e)
            {
                for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
                {
                    var form = Application.OpenForms[i];
                    if (form != MainForm)
                    {
                        MainForm = form;
                        return;
                    }
                }
                base.OnMainFormClosed(sender, e);
            }
        }
    }
}
