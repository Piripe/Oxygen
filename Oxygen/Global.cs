using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;

namespace Oxygen
{
    internal static class Global
    {
        public const string Version = "alpha 0.1";
        internal static Data.SkinConfig SkinConfig = new Data.SkinConfig();
        internal static Data.SkinData SkinData;
        internal static Engine JSEngine;
        internal static Data.JS.Document Document;
        internal static Forms.Editor Editor;

        internal static bool DevMode = false;
        internal static bool AutoReload = false;
        internal static bool AutoExport = false;
        internal static DateTime AutoReloadLastWrite = DateTime.MinValue;
    }
}
