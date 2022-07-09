using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oxygen.Forms
{
    public partial class ProgressBar : Form
    {
        public bool Cancelled = false;
        Action<ProgressBar> action;

        int CursorTop = 0;
        TaskCompletionSource<bool> tsc = null;

        private DialogResult CLIResult_;
        public DialogResult CLIResult
        {
            get
            {
                return CLIResult_;
            }
            set
            {
                CLIResult_ = value;
                tsc?.SetResult(true);
            }
        }

        public ProgressBar(Action<ProgressBar> action)
        {
            InitializeComponent();
            this.action = action;
            try
            {
                CursorTop = Console.CursorTop;
                Console.CursorTop += 1;
            }
            catch
            {

            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Cancelled = true;
        }

        private void ProgressBar_Shown(object sender, EventArgs e)
        {
            action(this);
        }
        internal DialogResult StartCLI()
        {
            tsc = new TaskCompletionSource<bool>();
            action(this);
            tsc.Task.Wait();
            return CLIResult;
        }
        internal void drawCLIProgressbar(string text, int percent)
        {
            try { 
                int oldCursorTop = Console.CursorTop;
                int oldCursorLeft = Console.CursorLeft;

                Console.CursorTop = CursorTop;
                Console.CursorLeft = 0;

                Console.Write(new string(' ', Console.WindowWidth));
                Console.CursorTop = CursorTop;
                Console.CursorLeft = 0;

                Console.Write(text.Length > Console.WindowWidth - 43 ? text.Remove(Console.WindowWidth - 46) + "..." : text);

                Console.CursorTop = CursorTop;
                Console.CursorLeft = Console.WindowWidth - 40;

                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine(new string(' ', (int)Math.Floor(percent / 2.5)));
                Console.BackgroundColor = ConsoleColor.Black;

                Console.CursorTop = oldCursorTop;
                Console.CursorLeft = oldCursorLeft;
            }
            catch { }
        }
    }
}
