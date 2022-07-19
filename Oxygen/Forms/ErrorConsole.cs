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
    public partial class ErrorConsole : Form
    {
        List<Panel> lines = new List<Panel>();
        Font defaultFont = new Font("Segoe UI Variable Display", 12,GraphicsUnit.Pixel);
        Brush errorLineBrush = new SolidBrush(Color.FromArgb(255, 120, 120));
        Brush errorLocationBrush = new SolidBrush(Color.FromArgb(255, 100, 100));
        Brush errorSeparatorBrush = new SolidBrush(Color.FromArgb(255, 100, 100));
        Brush infoLineBrush = new SolidBrush(Color.FromArgb(126, 216, 233));
        Brush infoLocationBrush = new SolidBrush(Color.FromArgb(116, 194, 206));
        Brush infoSeparatorBrush = new SolidBrush(Color.FromArgb(210,240,255));

        TaskScheduler addLineContext;
        public ErrorConsole()
        {
            InitializeComponent();
            addLineContext = TaskScheduler.FromCurrentSynchronizationContext();
            Clear();
        }
        void Clear()
        {
            Task.Factory.StartNew(() =>
            {
                lines.Clear();
                Controls.Clear();
            }, CancellationToken.None, TaskCreationOptions.None, addLineContext);
        }
        internal void AddLine(Modules.ErrorManager.LogType type, string message, string location, string line)
        {
            Task.Factory.StartNew(() =>
            {
                Panel panel = new Panel()
                {
                    Size = new Size(this.Width, 1),
                    BackColor = type == Modules.ErrorManager.LogType.Error ? Color.FromArgb(246, 185, 185) : Color.FromArgb(250, 254, 255),
                    Location = new Point(0, 0),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                Label messageLabel = new Label()
                {
                    ForeColor = type == Modules.ErrorManager.LogType.Error ? Color.FromArgb(255, 100, 100) : Color.FromArgb(15, 30, 35),
                    Text = message,
                    MaximumSize = new Size(this.Width - 12, 1000),
                    Location = new Point(6, 6),
                    Font = defaultFont,
                    AutoSize = true,
                };
                lines.Add(panel);

                panel.Paint += (object? sender, PaintEventArgs e) => {
                    int left = panel.Width - 26;
                    if (line != "")
                    {
                        left -= (int)e.Graphics.MeasureString(":" + line, defaultFont).Width;
                        e.Graphics.DrawString(":" + line, defaultFont, type == Modules.ErrorManager.LogType.Error ? errorLineBrush : infoLineBrush, new Point(left, 6));
                    }
                    left -= (int)e.Graphics.MeasureString(location, defaultFont).Width - 4;
                    e.Graphics.DrawString(location, defaultFont, type == Modules.ErrorManager.LogType.Error ? errorLocationBrush : infoLocationBrush, new Point(left, 6));
                    messageLabel.Size = new Size(left - 12, 300);
                    messageLabel.MaximumSize = new Size(left - 12, 1000);
                    panel.Height = messageLabel.Height + 12;
                    e.Graphics.FillRectangle(type == Modules.ErrorManager.LogType.Error ? errorSeparatorBrush : infoSeparatorBrush, 0, panel.Height - 1, panel.Width, 1);
                    panel.Location = new Point(0, lines.IndexOf(panel) > 0 ? lines[lines.IndexOf(panel) - 1].Bottom : this.AutoScrollPosition.Y);
                    if (lines.IndexOf(panel) == lines.Count - 1)
                    {
                        this.ScrollControlIntoView(panel);
                    }
                };
                panel.Resize += (object? sender, EventArgs e) => {
                    panel.Invalidate();
                };
                panel.Controls.Add(messageLabel);
                this.Controls.Add(panel);

                if (Global.DevMode & type == Modules.ErrorManager.LogType.Error) Show();
            }, CancellationToken.None, TaskCreationOptions.None, addLineContext);
        }

        private void ErrorConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
