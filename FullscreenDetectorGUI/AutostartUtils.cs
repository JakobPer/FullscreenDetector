using System.IO;
using Microsoft.Win32;

namespace FullscreenDetectorGUI
{
    public class AutostartUtils
    {
        private const string run = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static void addAutoStartRegistry(string appName, string appPath)
        {
            string AppName = appName;
            string AppPath = appPath;
            if (Path.HasExtension(appPath))
            {
                RegistryKey startKey = Registry.CurrentUser.OpenSubKey(run, true);
                startKey.SetValue(AppName, AppPath);
            }
        }

        public static bool isInAutostartRegistry(string appName)
        {
            RegistryKey startKey = Registry.CurrentUser.OpenSubKey(run, true);
            return startKey.GetValue(appName) != null;
        }

        public static void removeAutostartRegistry(string appName)
        {
            RegistryKey startKey = Registry.CurrentUser.OpenSubKey(run, true);
            if (isInAutostartRegistry(appName))
            {
                startKey.DeleteValue(appName);
            }
        }
    }
}