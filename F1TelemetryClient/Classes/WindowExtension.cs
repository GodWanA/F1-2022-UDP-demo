using Microsoft.Win32;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace F1TelemetryApp.Classes
{
    internal static class WindowExtension
    {
        private const string regRoot = "SOFTWARE\\Csernay\\F1UDP";

        internal static string CreateRegPath(this Window window)
        {
            var sb = new StringBuilder(WindowExtension.regRoot);
            var o = window.Owner;

            while (o != null)
            {
                if (sb.Length != 0) sb.Append("\\");
                sb.Append(WindowExtension.CleanTitle(o.Title));
                o = o.Owner;
            }

            if (sb.Length != 0) sb.Append("\\");
            sb.Append(WindowExtension.CleanTitle(window.Title));

            return sb.ToString();
        }

        private static string CleanTitle(string title)
        {
            title = Regex.Replace(title, "jeff\\+", "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, "\\|", "", RegexOptions.IgnoreCase);
            return title.Trim().Replace(" ", "_");
        }

        internal static void SaveWindowPosition(this Window window, string regPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath))
            {
                //if (window.WindowStartupLocation == WindowStartupLocation.Manual)
                //{
                key.SetValue("width", window.ActualWidth, RegistryValueKind.String);
                key.SetValue("height", window.ActualHeight, RegistryValueKind.String);
                key.SetValue("left", window.Left, RegistryValueKind.String);
                key.SetValue("top", window.Top, RegistryValueKind.String);
                key.SetValue("state", (int)window.WindowState, RegistryValueKind.String);
                //}
            }
        }

        internal static void LoadWindowPosition(this Window window, string regPath)
        {
            if (window != null && regPath != null)
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath))
                {
                    if (key != null)
                    {
                        var maxW = SystemParameters.VirtualScreenWidth;
                        var maxH = SystemParameters.VirtualScreenHeight;
                        var w = CastDouble(key.GetValue("width"));
                        var h = CastDouble(key.GetValue("height"));
                        var l = CastDouble(key.GetValue("left"));
                        var t = CastDouble(key.GetValue("top"));
                        var s = CastWindowState(key.GetValue("state"));

                        if (w > maxW) w = maxW;
                        if (h > maxH) h = maxH;

                        if (window.WindowStartupLocation == WindowStartupLocation.Manual)
                        {
                            if (l < 0) l = 0;
                            if (l + w > maxW) l = maxW - w;

                            if (t < 0) t = 0;
                            if (t + h > maxH) t = maxH - h;

                            window.Top = t;
                            window.Left = l;
                        }

                        window.Height = h;
                        window.Width = w;
                        window.WindowState = s;
                    }
                }
            }
        }

        private static double CastDouble(object? reg)
        {
            string s = reg as string;

            if (s == null || s == "") return double.NaN;
            else return double.Parse(s, NumberStyles.Any);
        }

        private static WindowState CastWindowState(object? reg)
        {
            string s = reg as string;

            if (s == null || s == "") return WindowState.Normal;
            else return (WindowState)int.Parse(s, NumberStyles.Any);
        }
    }
}
