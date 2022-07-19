using System;
using System.Windows.Forms;

namespace Oxygen.Forms
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            versionLabel.Text = "Version " + Global.Version;
        }

        private void openLink(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(((Control)sender).Tag.ToString() ?? "https://google.com") { UseShellExecute = true });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
