using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
