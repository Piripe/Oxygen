using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace Oxygen.Modules
{
    internal static class Extensions
    {
        #region Dictionary

        internal static U GetOrDefault<T, U>(this Dictionary<T, U> dict, T key, U defaultValue) where T : notnull
        {
                return dict.ContainsKey(key) ? dict[key] : defaultValue;
            
        }
        internal static int GetOrDefaultInt<T>(this Dictionary<T, string> dict, T key, int defaultValue) where T : notnull
        {
            string value = dict.ContainsKey(key) ? dict[key] : defaultValue.ToString();
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return defaultValue;
        }
        internal static bool GetOrDefaultBool<T>(this Dictionary<T, string> dict, T key, bool defaultValue) where T : notnull
        {
            string value = dict.ContainsKey(key) ? dict[key] : defaultValue.ToString();
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            return defaultValue;
        }
        internal static void SetOrAdd<T, U>(this Dictionary<T, U> dict, T key, U value) where T : notnull
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        #endregion


        #region GraphicsPath

        internal static void AddRoundedRectangle(this GraphicsPath path, RectangleF rect, float radius)
        {
            if (radius > 0)
            {
                radius = Math.Min(radius, rect.Width / 2);
                path.AddArc(rect.Left, rect.Top, radius * 2, radius * 2, 180, 90);
                path.AddLine(rect.Left + radius, rect.Top, rect.Right - radius, rect.Top);
                path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90);
                path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius);
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius, rect.Bottom);
                path.AddArc(rect.Left, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.AddLine(rect.Left, rect.Bottom - radius, rect.Left, rect.Top + radius);
                path.CloseFigure();
            }
            else
            {
                radius = Math.Max(radius, -rect.Width / 2);
                path.AddArc(rect.Left+radius, rect.Top+radius, radius * -2, radius * -2, 90, -90);
                path.AddLine(rect.Left - radius, rect.Top, rect.Right + radius, rect.Top);
                path.AddArc(rect.Right + radius, rect.Top + radius, radius * -2, radius * -2, 180, -90);
                path.AddLine(rect.Right, rect.Top - radius, rect.Right, rect.Bottom + radius);
                path.AddArc(rect.Right + radius, rect.Bottom + radius, radius * -2, radius * -2, 270, -90);
                path.AddLine(rect.Right + radius, rect.Bottom, rect.Left - radius, rect.Bottom);
                path.AddArc(rect.Left + radius, rect.Bottom + radius, radius * -2, radius * -2, 0, -90);
                path.AddLine(rect.Left, rect.Bottom + radius, rect.Left, rect.Top - radius);
                path.CloseFigure();
            }
        }

        #endregion
    }
}
