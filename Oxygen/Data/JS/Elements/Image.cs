using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Drawing;
using Oxygen.Modules;
using System.IO;
using System.Windows.Forms;

namespace Oxygen.Data.JS.Elements
{
    internal class Image : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id",""); set => attributes.SetOrAdd("id",value); }
        public string innerText { get
            {
                return control.Text;
            } set
            {
                control.Text = value;
            }
        }
        public string src
        {
            get => attributes.GetOrDefault("src", "false"); set
            {
                attributes.SetOrAdd("src", value);
                control.ImageLocation = Path.Combine(Path.GetTempPath(), "Oxygen", "skin", value);
            }
        }
        public int marginTop
        {
            get => attributes.GetOrDefaultInt("margin-top", 0); set
            {
                ControlHelper.ShiftControlsUnder(parentPanel, control.Top, value - marginTop);
                attributes.SetOrAdd("margin-top", value.ToString());
            }
        }
        public int marginBottom
        {
            get => attributes.GetOrDefaultInt("margin-bottom", 6); set
            {
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
                    ControlHelper.ShiftControlsUnder(parentPanel, control.Top - marginTop, marginTop + control.Height + marginBottom);
                    control.Top = oldTop;
                }
                else
                {
                    ControlHelper.ShiftControlsUnder(parentPanel, control.Top + 1, -marginTop - control.Height - marginBottom);
                }
                control.Visible = value;
                attributes.SetOrAdd("visible", value.ToString());
            }
        }

        private PictureBox control;
        private Panel parentPanel;
        private int oldHeight;

        internal Image(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();

            control = new PictureBox()
            {
                Name = id,
                SizeMode = PictureBoxSizeMode.AutoSize,
                ImageLocation = Path.Combine(Path.GetTempPath(), "Oxygen", "skin", src),
                Visible = visible,
            };
            oldHeight = control.Height;

            ControlHelper.AddGenericEvents(control, attributes, this);
            control.Invalidated += (object sender, InvalidateEventArgs e) =>
            {
                if (oldHeight != control.Height)
                {
                    foreach (Control control in parentPanel.Controls)
                    {
                        if (control.Location.Y > this.control.Location.Y)
                        {
                            control.Location = new Point(control.Left, control.Top + (this.control.Height - oldHeight));
                        }
                    }
                    control.Location = new Point((parentPanel.Width - control.Width) / 2, control.Top);

                    oldHeight = control.Height;
                }
            };

            innerText = element.Value;
        }
        public int AddControl(Panel panel,int y)
        {
            control.Location = new Point((panel.Width - control.Width) / 2, y+marginTop);
            panel.Resize += (object sender, EventArgs e) => {
                control.Location = new Point((panel.Width - control.Width) / 2, y+marginTop - panel.VerticalScroll.Value);
            };

            panel.Controls.Add(control);

            parentPanel = panel;

            return visible ? control.Height+marginTop+marginBottom : 0;
        }
    }
}
