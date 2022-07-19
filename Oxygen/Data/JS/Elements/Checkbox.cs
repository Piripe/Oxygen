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
    internal class Checkbox : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id",""); set => attributes.SetOrAdd("id",value); }
        public string? innerText { get
            {
                return control.Text;
            } set
            {
                control.Text = value;
            }
        }
        public bool value
        {
            get => attributes.GetOrDefaultBool("value", false); set
            {
                attributes.SetOrAdd("value", value.ToString());
                control.Checked = value;
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

        private CheckBox control;
        private Panel? parentPanel;

        internal Checkbox(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();

            control = new CheckBox()
            {
                AutoSize = true,
                Name = id,
                Checked = value,
                Visible = visible,
                Enabled = !disabled,
            };

            ControlHelper.AddGenericEvents(control, attributes, this);

            control.CheckedChanged += (object? sender, EventArgs e) =>
            {
                value = control.Checked;
                if (attributes.ContainsKey("onchange"))
                {
                    ControlHelper.ExecuteEvent("onchange", attributes, this);
                }
            };

            innerText = element.Value;
        }
        public int AddControl(Panel panel,int y)
        {
            panel.Controls.Add(control);

            control.Location = new Point(24, y+marginTop);

            parentPanel = panel;

            return visible ? control.Height + marginTop + marginBottom : 0;
        }
    }
}
