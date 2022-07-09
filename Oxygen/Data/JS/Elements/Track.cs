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
    internal class Track : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public int min
        {
            get => attributes.GetOrDefaultInt("min", 0); set
            {
                attributes.SetOrAdd("min", value.ToString());
                altControl.Minimum = value;
            }
        }
        public int max
        {
            get => attributes.GetOrDefaultInt("max", 100); set
            {
                attributes.SetOrAdd("max", value.ToString());
                altControl.Maximum = value;
            }
        }
        public int step
        {
            get => attributes.GetOrDefaultInt("step", 0); set
            {
                attributes.SetOrAdd("step", value.ToString());
                altControl.SmallChange = value;
            }
        }
        public int value
        {
            get => attributes.GetOrDefaultInt("value", 0); set
            {
                attributes.SetOrAdd("value", value.ToString());
                altControl.Value = value;
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
                    ControlHelper.ShiftControlsUnder(parentPanel, control.Top - marginTop, marginTop + control.Height + marginBottom);
                    control.Top = oldTop;
                    altControl.Top = oldAltTop;
                }
                else
                {
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
                altControl.Enabled = !value;
                attributes.SetOrAdd("disabled", value.ToString());
            }
        }

        private System.Windows.Forms.Label control;
        private TrackBar altControl;
        private Panel parentPanel;

        internal Track(XElement element)
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
                Visible = visible,
            };
            altControl = new TrackBar()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Minimum = min,
                Maximum = max,
                SmallChange = step,
                Value = value,
                Name = id,
                Visible = visible,
                Enabled = !disabled,
            };

            ControlHelper.AddGenericEvents(control, attributes, this);
            ControlHelper.AddGenericEvents(altControl, attributes, this);

                altControl.ValueChanged += (object sender, EventArgs e) =>
                {
                    value = altControl.Value;
                    if (attributes.ContainsKey("onchange"))
                    {
                        ControlHelper.ExecuteEvent("onchange", attributes, this);
                    }
                };


            innerText = element.Value;
        }
        public int AddControl(Panel panel,int y)
        {
            control.Location = new Point(24, y+marginTop);

            panel.Controls.Add(control);

            altControl.Location = new Point(control.Size.Width + 48, y+marginTop);
            altControl.Size = new Size(panel.Width - control.Size.Width - 72, 24);

            panel.Controls.Add(altControl);

            panel.Resize += (object sender, EventArgs e) =>
            {
                altControl.Size = new Size(panel.Width - control.Size.Width - 72, 24);
            };

            parentPanel = panel;

            return visible?36+marginTop+marginBottom:0;
        }
    }
}
