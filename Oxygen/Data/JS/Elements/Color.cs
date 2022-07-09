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
    internal class Color : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public System.Drawing.Color value
        {
            get
            {
                try
                {
                    return ColorTranslator.FromHtml(attributes.GetOrDefault("value", "#000"));
                }
                catch
                {
                    return System.Drawing.Color.Black;
                }
                
            } set
            {
                attributes.SetOrAdd("value", ColorTranslator.ToHtml(value));
                altControl.BackColor = value;
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
                    int oldAltTop = altControl.Top;
                    ControlHelper.ShiftControlsUnder(parentPanel, altControl.Top - marginTop, marginTop + control.Height + marginBottom);
                    control.Top = oldTop;
                    altControl.Top = oldAltTop;
                }
                else
                {
                    ControlHelper.ShiftControlsUnder(parentPanel, altControl.Top + 1, -marginTop - control.Height - marginBottom);
                }
                control.Visible = value;
                altControl.Visible = value;
                attributes.SetOrAdd("visible", value.ToString());
            }
        }
        public bool disabled
        {
            get => attributes.GetOrDefaultBool("disabled", false); set
            {
                altControl.Enabled = !value;
                attributes.SetOrAdd("disabled", value.ToString());
            }
        }

        private System.Windows.Forms.Label control;
        private Panel parentPanel;
        private Button altControl;

        internal Color(XElement element)
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
            };
            altControl = new Button()
            {
                AutoSize = false,
                TabStop = false,
                BackColor = value,
                Size = new Size(24, 24),
                FlatStyle = FlatStyle.Flat,
                Name = id,
                Enabled = !disabled,
            };

            ControlHelper.AddGenericEvents(control, attributes, this);
            ControlHelper.AddGenericEvents(altControl, attributes, this);

            altControl.Click += (object sender, EventArgs e) =>
            {
                ColorDialog dialog = new ColorDialog();
                dialog.Color = value;
                dialog.FullOpen = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    value = dialog.Color;
                    if (attributes.ContainsKey("onchange"))
                    {
                        ControlHelper.ExecuteEvent("onchange", attributes, this);
                    }
                }
            };


            innerText = element.Value;
        }
        public int AddControl(Panel panel,int y)
        {
            control.Location = new Point(24, y+2+marginTop);

            panel.Controls.Add(control);

            altControl.Location = new Point(control.Size.Width + 48, y+marginTop);

            panel.Controls.Add(altControl);

            parentPanel = panel;

            return visible ? 36+marginTop+marginBottom : 0;
        }
    }
}
