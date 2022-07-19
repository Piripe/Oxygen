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
    internal class Label : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public int fontSize
        {
            get => attributes.GetOrDefaultInt("font-size", 14); set
            {
                attributes.SetOrAdd("font-size", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, value,fontItalic,fontUnderline,fontStrikeout);
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
            get => attributes.GetOrDefaultBool("font-italic", false); set
            {
                attributes.SetOrAdd("font-italic", value.ToString());
                control.Font = FontHelper.getFont(fontWeight, fontSize, value, fontUnderline, fontStrikeout);
            }
        }
        public bool fontUnderline
        {
            get => attributes.GetOrDefaultBool("font-underline", false); set
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
                ControlHelper.ShiftControlsUnder(parentPanel, control.Top+1, value - marginTop);
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
        public string innerText { get
            {
                return control.Text;
            } set
            {
                control.Text = value;
            }
        }

        private System.Windows.Forms.Label control;
        private Panel parentPanel;
        private int oldHeight;

        internal Label(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();

            control = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Name = id,
                Font = FontHelper.getFont(fontWeight, fontSize, fontItalic, fontUnderline, fontStrikeout),
                Visible = visible,
            };

            ControlHelper.AddGenericEvents(control, attributes, this);


            control.Resize += (object sender, EventArgs e) =>
            {
                if (oldHeight != control.Height)
                {
                    ControlHelper.ShiftControlsUnder(parentPanel,control.Top + oldHeight, control.Height - oldHeight);

                    oldHeight = control.Height;
                }
            };

            innerText = element.Value;
        }
        public int AddControl(Panel panel,int y)
        {
            control.MaximumSize = new Size(panel.Width-48, 1000);
            control.Location = new Point(24, y + marginTop);

            panel.Controls.Add(control);

            oldHeight = control.Height;

            panel.Resize += (object sender, EventArgs e) =>
            {
                control.MaximumSize = new Size(panel.Width - 48, 1000);
            };

            parentPanel = panel;

            return visible ? control.Height + marginTop+marginBottom : 0;
        }
    }
}
