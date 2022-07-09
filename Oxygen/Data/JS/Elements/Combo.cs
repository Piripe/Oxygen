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
    internal class Combo : IElement
    {
        public List<IElement> children { get; }
        public Dictionary<string, string> attributes { get; }
        public string id { get => attributes.GetOrDefault("id", ""); set => attributes.SetOrAdd("id", value); }
        public string value
        {
            get
            {
                return attributes.GetOrDefault("value", "0");
            }
            set
            {
                attributes.SetOrAdd("value", value.ToString());
                altControl.SelectedIndex = children.FindIndex((x) => ((Option)x).value == value);
            }
        }
        public string[] values
        {
            get
            {
                return altControl.Items.Cast<string>().ToArray();
            }
            set
            {
                altControl.Items.Clear();
                children.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    altControl.Items.Add(value[i].ToString());
                    children.Add(new Option(i.ToString(), value[i]));

                    altControl.SelectedIndex = Math.Min(altControl.Items.Count - 1, lastSelect);
                    lastSelect = altControl.SelectedIndex;
                }

            }
        }
        public bool simple
        {
            get => attributes.GetOrDefaultBool("simple", false); set
            {
                attributes.SetOrAdd("simple", value.ToString());
                altControl.DropDownStyle = value ? ComboBoxStyle.Simple : ComboBoxStyle.DropDownList;
            }
        }
        public bool editable
        {
            get => attributes.GetOrDefaultBool("editable", false); set
            {
                attributes.SetOrAdd("editable", value.ToString());
            }
        }
        public int height
        {
            get => attributes.GetOrDefaultInt("height", simple ? 196 : 28); set
            {
                attributes.SetOrAdd("height", value.ToString());
                altControl.Size = new Size(altControl.Width,value - (editable ? 38 : 0) );
            }
        }
        public string defaultValue
        {
            get => attributes.GetOrDefault("default-value", "");
            set
            {
                attributes.SetOrAdd("default-value", value.ToString());
                altControl.SelectedIndex = children.FindIndex((x) => ((Option)x).value == value);
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
                    int oldAddTop = addControl.Top;
                    int oldDelTop = delControl.Top;
                    int oldUpTop = upControl.Top;
                    int oldDownTop = downControl.Top;
                    ControlHelper.ShiftControlsUnder(parentPanel, altControl.Top - marginTop, marginTop + (editable ? height : 29) + marginBottom);
                    control.Top = oldTop;
                    altControl.Top = oldAltTop;
                    addControl.Top = oldAddTop;
                    delControl.Top = oldDelTop;
                    upControl.Top = oldUpTop;
                    downControl.Top = oldDownTop;
                }
                else
                {
                    int oldTop = control.Top;
                    int oldAddTop = addControl.Top;
                    int oldDelTop = delControl.Top;
                    int oldUpTop = upControl.Top;
                    int oldDownTop = downControl.Top;
                    ControlHelper.ShiftControlsUnder(parentPanel, altControl.Top + altControl.Height, -marginTop - (editable ? height : 29) - marginBottom);
                    control.Top = oldTop;
                    addControl.Top = oldAddTop;
                    delControl.Top = oldDelTop;
                    upControl.Top = oldUpTop;
                    downControl.Top = oldDownTop;
                }
                control.Visible = value;
                altControl.Visible = value;
                addControl.Visible = value & editable;
                delControl.Visible = value & editable;
                upControl.Visible = value & editable;
                downControl.Visible = value & editable;
                attributes.SetOrAdd("visible", value.ToString());
            }
        }
        public bool disabled
        {
            get => attributes.GetOrDefaultBool("disabled", false); set
            {
                altControl.Enabled = !value;
                addControl.Enabled = !value;
                delControl.Enabled = !value;
                upControl.Enabled = !value;
                downControl.Enabled = !value;
                attributes.SetOrAdd("disabled", value.ToString());
            }
        }

        private System.Windows.Forms.Label control;
        private ComboBox altControl;
        private Button addControl;
        private Button delControl;
        private Button upControl;
        private Button downControl;
        private Panel parentPanel;
        private int oldHeight;
        private int lastSelect = -1;

        internal Combo(XElement element)
        {
            attributes = new Dictionary<string, string>();
            foreach (XAttribute attr in element.Attributes())
            {
                attributes.Add(attr.Name.ToString(), attr.Value);
            }
            children = new List<IElement>();
            foreach (XElement childElement in element.Elements())
            {
                switch (childElement.Name.ToString())
                {
                    case "option":
                        children.Add(new Option(childElement));
                        break;
                }
            }

            control = new System.Windows.Forms.Label()
            {
                AutoSize = true,
                Name = id,
                Visible = visible,
            };
            altControl = new ComboBox()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                DropDownStyle = simple ? ComboBoxStyle.Simple : ComboBoxStyle.DropDownList,
                Name = id,
                Visible = visible,
                Enabled = !disabled,
            };
            addControl = new Button()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "Add",
                Size = new Size(48, 32),
                Name = id,
                Visible = visible & editable,
                Enabled = !disabled,
            };
            delControl = new Button()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "Delete",
                Size = new Size(64, 32),
                Name = id,
                Visible = visible & editable,
                Enabled = !disabled,
            };
            upControl = new Button()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "↑",
                Size = new Size(32, 32),
                Name = id,
                Visible = visible & editable,
                Enabled = !disabled,
            };
            downControl = new Button()
            {
                AutoSize = false,
                TabStop = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Text = "↓",
                Size = new Size(32, 32),
                Name = id,
                Visible = visible & editable,
                Enabled = !disabled,
            };

            foreach (Option option in children)
            {
                altControl.Items.Add(option.ToString());
                option.innerTextChanged += (object sender, EventArgs e) =>
                {
                    altControl.Items[children.IndexOf(option)] = sender;
                };
            }
            altControl.SelectedIndex = children.FindIndex((x) => ((Option)x).value == value);


            ControlHelper.AddGenericEvents(control, attributes, this);
            ControlHelper.AddGenericEvents(altControl, attributes, this);

            altControl.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                if (altControl.SelectedIndex >= 0)
                {
                    value = ((Option)children[altControl.SelectedIndex]).value;
                    if (attributes.ContainsKey("onchange"))
                    {
                        ControlHelper.ExecuteEvent("onchange", attributes, this);
                    }
                    if (altControl.SelectedIndex != lastSelect)
                    {
                        lastSelect = altControl.SelectedIndex;
                    }
                }
            };

            altControl.TextChanged += (object sender, EventArgs e) =>
            {
                if (editable)
                {
                    if (altControl.SelectedIndex == -1 & lastSelect != -1)
                    {
                        int lastSelection = altControl.SelectionStart;
                        altControl.Items[lastSelect] = altControl.Text;
                        altControl.SelectedIndex = lastSelect;
                        altControl.SelectionStart = lastSelection;
                        altControl.SelectionLength = 0;
                    }
                }
                else
                {
                    if (altControl.SelectedIndex == -1 & lastSelect != -1)
                    {
                        altControl.Text = altControl.Items[lastSelect].ToString();
                        altControl.SelectionStart = altControl.Text.Length;
                        altControl.SelectionLength = 0;
                    }
                }
            };

            control.Resize += (object sender, EventArgs e) =>
            {
                    if (parentPanel != null)
                {
                    altControl.Location = new Point(control.Size.Width + 48, altControl.Top);
                    altControl.Size = new Size(parentPanel.Width - control.Size.Width - 72, oldHeight);
                }
            };
            altControl.Resize += (object sender, EventArgs e) =>
            {
                if (oldHeight != altControl.Height)
                {
                    if (parentPanel != null)
                    {
                        if (simple) ControlHelper.ShiftControlsUnder(parentPanel, altControl.Location.Y + oldHeight+100, altControl.Height - oldHeight);
                    }

                    oldHeight = altControl.Height;
                }
            };

            addControl.Click += (object sender, EventArgs e) =>
            {
                children.Add(new Option(children.Count.ToString(), defaultValue));
                altControl.Items.Add(children.Last().innerText);
            };
            delControl.Click += (object sender, EventArgs e) =>
            {
                children.RemoveAt(altControl.SelectedIndex);
                altControl.Items.RemoveAt(altControl.SelectedIndex);
                altControl.SelectedIndex = Math.Min(altControl.Items.Count - 1, lastSelect);
                lastSelect = altControl.SelectedIndex;
            };
            upControl.Click += (object sender, EventArgs e) =>
            {
                if (altControl.SelectedIndex > 0)
                {
                    IElement upOption = children[altControl.SelectedIndex - 1];
                    object upItem = altControl.Items[altControl.SelectedIndex - 1];
                    children[altControl.SelectedIndex - 1] = children[altControl.SelectedIndex];
                    ((Option)children[altControl.SelectedIndex - 1]).value = (altControl.SelectedIndex - 1).ToString();
                    children[altControl.SelectedIndex] = upOption;
                    ((Option)children[altControl.SelectedIndex]).value = (altControl.SelectedIndex).ToString();
                    altControl.Items[altControl.SelectedIndex - 1] = altControl.Items[altControl.SelectedIndex];
                    altControl.Items[altControl.SelectedIndex] = upItem;
                    altControl.SelectedIndex = Math.Min(altControl.Items.Count - 1, lastSelect - 1);
                    lastSelect = altControl.SelectedIndex;
                }
            };
            downControl.Click += (object sender, EventArgs e) =>
            {
                if (altControl.SelectedIndex < altControl.Items.Count - 1)
                {
                    IElement upOption = children[altControl.SelectedIndex + 1];
                    object upItem = altControl.Items[altControl.SelectedIndex + 1];
                    children[altControl.SelectedIndex + 1] = children[altControl.SelectedIndex];
                    ((Option)children[altControl.SelectedIndex + 1]).value = (altControl.SelectedIndex + 1).ToString();
                    children[altControl.SelectedIndex] = upOption;
                    ((Option)children[altControl.SelectedIndex]).value = (altControl.SelectedIndex).ToString();
                    altControl.Items[altControl.SelectedIndex + 1] = altControl.Items[altControl.SelectedIndex];
                    altControl.Items[altControl.SelectedIndex] = upItem;
                    altControl.SelectedIndex = Math.Min(altControl.Items.Count - 1, lastSelect + 1);
                    lastSelect = altControl.SelectedIndex;
                }
            };



            innerText = (children.Count > 0 ? element.Value.Remove(element.Value.IndexOf(children[0].innerText)) : element.Value).Trim();
        }
        public int AddControl(Panel panel,int y)
        {
            control.Location = new Point(24, y+2+marginTop);
            control.MaximumSize = new Size((int)Math.Min(panel.Width * 0.75, panel.Width - (editable ? 242 : 100)), height);

            panel.Controls.Add(control);

            altControl.Location = new Point(Math.Min(panel.Width - Math.Max(194, panel.Width - control.Size.Width - 72) - 24, control.Size.Width + 48), y+marginTop);

            panel.Controls.Add(altControl);

            if (editable)
            {
                altControl.Size = new Size(Math.Max(194, panel.Width - control.Size.Width - 72), height - 38);
                addControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 218, y + marginTop + height - 32);
                panel.Controls.Add(addControl);
                addControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 218, y + marginTop + height - 32);
                delControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 164, y + marginTop + height - 32);
                panel.Controls.Add(delControl);
                upControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 94, y + marginTop + height - 32);
                panel.Controls.Add(upControl);
                downControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 56, y + marginTop + height - 32);
                panel.Controls.Add(downControl);
            }
            else
            {
                altControl.Size = new Size(Math.Max(194, panel.Width - control.Size.Width - 72), height);
            }
            oldHeight = altControl.Height;

            panel.Resize += (object sender, EventArgs e) =>
            {
                control.MaximumSize = new Size((int)Math.Min(panel.Width * 0.75, panel.Width - 242), height);
                altControl.Location = new Point(Math.Min(panel.Width - Math.Max(194, panel.Width - control.Size.Width - 72) - 24, control.Size.Width + 48), altControl.Top);
                if (editable)
                {
                    altControl.Size = new Size(Math.Max(194, panel.Width - control.Size.Width - 72), height - 38);
                    addControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 218, addControl.Top);
                    delControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 164, delControl.Top);
                    upControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 94, upControl.Top);
                    downControl.Location = new Point(control.Size.Width + panel.Width - control.Size.Width - 56, downControl.Top);
                }
                else
                {
                    altControl.Size = new Size(Math.Max(194, panel.Width - control.Size.Width - 72), oldHeight);
                }
            };

            parentPanel = panel;
            
            return visible ? (editable ? height : 29) + marginTop+marginBottom : 0;
        }
    }
}
