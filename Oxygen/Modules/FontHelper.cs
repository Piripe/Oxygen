using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Oxygen.Modules
{
    internal static class FontHelper
    {
        /// <summary>
        /// Return the GDI+ font name from the <paramref name="weight"/>
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        internal static string getFontName(int weight)
        {
            if (weight <= 600)
            {
                if (weight <= 400)
                {
                    if (weight <= 350)
                    {
                        if (weight <= 300)
                        {
                            return "Segoe UI Variable Display Light";
                        }
                        return "Segoe UI Variable Display Semilight";
                    }
                    return "Segoe UI Variable Display";
                }
                return "Segoe UI Variable Display Semibold";
            }
            return "Segoe UI Variable Display";
        }
        /// <summary>
        /// Return the <see cref="Font"/> from all of its attributes
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="size"></param>
        /// <param name="italic"></param>
        /// <param name="underline"></param>
        /// <param name="strikeout"></param>
        /// <returns></returns>
        internal static Font getFont(int weight, int size, bool italic, bool underline, bool strikeout)
        {
            return new Font(getFontName(weight), size, (weight >= 700 ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0) | (underline ? FontStyle.Underline : 0) | (strikeout ? FontStyle.Strikeout : 0));
        }
    }
}
