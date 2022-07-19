using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oxygen.Data.JS;
using System.Drawing;

namespace Oxygen.Modules
{
    internal static class ControlHelper
    {
        /// <summary>
        /// Execute the JS function of an event
        /// </summary>
        /// <param name="key"></param>
        /// <param name="attributes"></param>
        /// <param name="target"></param>
        internal static void ExecuteEvent(string key, Dictionary<string, string> attributes, object target)
        {
            if (Global.JSEngine != null)
            {
                try
                {
                    Global.JSEngine.Invoke(attributes[key], new Event(target));
                }
                catch
                {
                    try
                    {
                        Global.JSEngine.Execute(attributes[key]);
                    }
                    catch
                    {

                    }
                }
            }
        }
        /// <summary>
        /// Add all generic events for an <see cref="IElement"/>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="attributes"></param>
        /// <param name="target"></param>
        internal static void AddGenericEvents(Control control, Dictionary<string,string> attributes, object target)
        {
            if (attributes.ContainsKey("onclick"))
            {
                control.Click += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onclick", attributes,target);
                };
            }
            if (attributes.ContainsKey("ondblclick"))
            {
                control.DoubleClick += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("ondblclick", attributes, target);
                };
            }
            if (attributes.ContainsKey("onmousedown"))
            {
                control.MouseDown += (object? sender, MouseEventArgs e) =>
                {
                    ExecuteEvent("onmousedown", attributes, target);
                };
            }
            if (attributes.ContainsKey("onmouseenter"))
            {
                control.MouseEnter += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onmouseenter", attributes, target);
                };
            }
            if (attributes.ContainsKey("onmouseleave"))
            {
                control.MouseLeave += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onmouseleave", attributes, target);
                };
            }
            if (attributes.ContainsKey("onmousemove"))
            {
                control.MouseMove += (object? sender, MouseEventArgs e) =>
                {
                    ExecuteEvent("onmousemove", attributes, target);
                };
            }
            if (attributes.ContainsKey("onmouseup"))
            {
                control.MouseUp += (object? sender, MouseEventArgs e) =>
                {
                    ExecuteEvent("onmouseup", attributes, target);
                };
            }
            if (attributes.ContainsKey("onkeydown"))
            {
                control.KeyDown += (object? sender, KeyEventArgs e) =>
                {
                    ExecuteEvent("onkeydown", attributes, target);
                };
            }
            if (attributes.ContainsKey("onkeypress"))
            {
                control.KeyPress += (object? sender, KeyPressEventArgs e) =>
                {
                    ExecuteEvent("onkeypress", attributes, target);
                };
            }
            if (attributes.ContainsKey("onkeyup"))
            {
                control.KeyUp += (object? sender, KeyEventArgs e) =>
                {
                    ExecuteEvent("onkeyup", attributes, target);
                };
            }
            if (attributes.ContainsKey("onfocus"))
            {
                control.GotFocus += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onfocus", attributes, target);
                };
            }
            if (attributes.ContainsKey("onfocusin"))
            {
                control.GotFocus += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onfocusin", attributes, target);
                };
            }
            if (attributes.ContainsKey("onfocusout"))
            {
                control.LostFocus += (object? sender, EventArgs e) =>
                {
                    ExecuteEvent("onfocusout", attributes, target);
                };
            }
            if (attributes.ContainsKey("onwheel"))
            {
                control.MouseWheel += (object? sender, MouseEventArgs e) =>
                {
                    ExecuteEvent("onwheel", attributes, target);
                };
            }
        }
        /// <summary>
        /// Shift all <see cref="Control"/> under the <paramref name="underY"/> of <paramref name="shiftY"/> pixels
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="underY"></param>
        /// <param name="shiftY"></param>
        internal static void ShiftControlsUnder(Control panel,int underY, int shiftY)
        {
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                {
                    if (control.Location.Y >= underY)
                    {
                        control.Location = new Point(control.Left, control.Top + shiftY);
                    }
                }
            }
        }
    }
}
