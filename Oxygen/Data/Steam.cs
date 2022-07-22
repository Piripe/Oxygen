using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace Oxygen.Data
{
    public class Steam
    {
        public static string SkinsDir => Path.Join(SteamDir, "skins");

        [SupportedOSPlatform("windows")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeSmell", "ERP022:Unobserved exception in generic exception handler", Justification = "ok if null")]
        private static object? GetRegistryData(string key, string valueName)
        {
                using RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey(key);
                object? value = null;
                object? regValue = registryKey?.GetValue(valueName);
                if (regValue != null)
                {
                    value = regValue;
                }

                return value;
        }

        public static string? SteamRootDir
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SteamDirectory))
                {
                    return Properties.Settings.Default.SteamDirectory;
                }

                if (OperatingSystem.IsWindows())
                {
                    return SteamDir;
                }

                else if (OperatingSystem.IsLinux())
                {
                    return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".steam");
                }

                else if (OperatingSystem.IsMacOS())
                {
                    return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Steam");
                }

                return null;
            }
        }

        public static string? SteamDir
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.SteamDirectory))
                {
                    return Properties.Settings.Default.SteamDirectory;
                }

                if (OperatingSystem.IsWindows())
                {
                    return GetRegistryData(@"SOFTWARE\Valve\Steam", "SteamPath")?.ToString()?.Replace(@"/", @"\");
                }

                if (OperatingSystem.IsLinux())
                {
                    return Path.Join(SteamRootDir, "steam");
                }

                // OSX
                return Path.Join(SteamRootDir, "Steam.AppBundle", "Steam", "Contents", "MacOS");
            }
        }


    }
}
