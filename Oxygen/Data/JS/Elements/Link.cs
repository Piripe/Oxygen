using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using Oxygen.Modules;
using System.Windows.Forms;

namespace Oxygen.Data.JS.Elements
{
    internal class Link : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public string? innerText
        {
            get
            {
                return control.Text;
            }
            set
            {
                control.Text = value;
            }
        }
        public int fontSize
        {
            get => attributes.GetOrDefaultInt("font-size", 14); set
            {
                attributes.SetOrAdd("font-size", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, value, fontItalic, fontUnderline, fontStrikeout);
            }
        }
        public int fontWeight
        {
            get => attributes.GetOrDefaultInt("font-weight", 400); set
            {
                attributes.SetOrAdd("font-weight", value.ToString());
                control.Font = FontHelper.getFont(value, fontSize, fontItalic, fontUnderline, fontStrikeout);
            }
        }
        public bool fontItalic
        {
            get => attributes.GetOrDefaultBool("font-italic",false); set
            {
                attributes.SetOrAdd("font-italic", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, fontSize, value, fontUnderline, fontStrikeout);
            }
        }
        public bool fontUnderline
        {
            get => attributes.GetOrDefaultBool("font-underline",false); set
            {
                attributes.SetOrAdd("font-underline", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, fontSize, fontItalic, value, fontStrikeout);
            }
        }
        public bool fontStrikeout
        {
            get => attributes.GetOrDefaultBool("font-strikeout", false); set
            {
                attributes.SetOrAdd("font-strikeout", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, fontSize, fontItalic, fontUnderline, value);
            }
        }
        public string href
        {
            get => attributes.GetOrDefault("href", "");
            set
            {
                attributes.SetOrAdd("href", value);
            }
        }
        public int marginTop
        {
            get => attributes.GetOrDefaultInt("margin-top", 0); set
            {
                if (parentPanel != null)
                    ControlHelper.ShiftControlsUnder(parentPanel, control.Top, value - marginTop);
                attributes.SetOrAdd("margin-top", value.ToString());
            }
        }
        public int marginBottom
        {
            get => attributes.GetOrDefaultInt("margin-bottom", 6); set
            {
                if (parentPanel != null)
                    ControlHelper.ShiftControlsUnder(parentPanel, control.Top + 1, value - marginTop);
                attributes.SetOrAdd("margin-bottom", value.ToString());
            }
        }
        public bool visible
        {
            get => attributes.GetOrDefaultBool("visible", true); set
            {
                if (value)
                {
                    int oldTop = control.Top;
                    if (parentPanel != null)
                        ControlHelper.ShiftControlsUnder(parentPanel, control.Top - marginTop, marginTop + control.Height + marginBottom);
                    control.Top = oldTop;
                }
                else
                {
                    if (parentPanel != null)
                        ControlHelper.ShiftControlsUnder(parentPanel, control.Top + 1, -marginTop - control.Height - marginBottom);
                }
                control.Visible = value;
                attributes.SetOrAdd("visible", value.ToString());
            }
        }
        public bool disabled
        {
            get => attributes.GetOrDefaultBool("disabled", false); set
            {
                control.Enabled = !value;
                attributes.SetOrAdd("disabled", value.ToString());
            }
        }

        private LinkLabel control;
        private Panel? parentPanel;
        private int oldHeight;

        internal Link(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();

            control = new LinkLabel()
            {
                AutoSize = true,
                Name = id,
                Font = FontHelper.getFont(fontWeight, fontSize, fontItalic, fontUnderline, fontStrikeout),
                Visible = visible,
                Enabled = !disabled,
            };

            oldHeight = control.Height;

            ControlHelper.AddGenericEvents(control, attributes, this);


            control.Resize += (object? sender, EventArgs e) =>
            {
                if (oldHeight != control.Height)
                {
                    if (parentPanel != null)
                        ControlHelper.ShiftControlsUnder(parentPanel, control.Top + oldHeight, control.Height - oldHeight);

                    oldHeight = control.Height;
                }
            };
            control.Click += (object? sender, EventArgs e) =>
            {
                if (Uri.IsWellFormedUriString(href, UriKind.Absolute) & (href.StartsWith("http://") | href.StartsWith("https://"))) System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(href) { UseShellExecute = true});
            };
            control.MouseEnter += (object? sender, EventArgs e) =>
            {
                if (Global.Editor != null)
                {
                    Global.Editor.linkPreviewLabel.MaximumSize = new Size(Global.Editor.Width / 2 - 20, 21);
                    Global.Editor.linkPreviewLabel.Text = href;
                    Global.Editor.linkPreviewLabel.Visible = true;
                }
            };
            control.MouseMove += (object? sender, MouseEventArgs e) =>
            {
                if (Global.Editor != null)
                {
                    if (new Rectangle(0, Global.Editor.Height - 60, Global.Editor.linkPreviewLabel.Width, 21).Contains(Global.Editor.PointToClient(Cursor.Position)))
                    {
                        Global.Editor.linkPreviewLabel.Location = new Point(Global.Editor.Width - Global.Editor.linkPreviewLabel.Width - 16, Global.Editor.Height - 57);
                    }
                    else
                    {
                        Global.Editor.linkPreviewLabel.Location = new Point(0, Global.Editor.Height - 57);
                    }
                }
            };
            control.MouseLeave += (object? sender, EventArgs e) =>
            {
                if (Global.Editor != null)
                Global.Editor.linkPreviewLabel.Visible = false;
            };

            innerText = element.Value;
        }
        public int AddControl(Panel panel, int y)
        {
            control.MaximumSize = new Size(panel.Width - 48, 1000);
            control.Location = new Point(24, y+marginTop);

            panel.Controls.Add(control);

            panel.Resize += (object? sender, EventArgs e) =>
            {
                control.MaximumSize = new Size(panel.Width - 48, 1000);
            };

            parentPanel = panel;

            return visible ? control.Height+marginTop+marginBottom : 0;
        }
    }
}
