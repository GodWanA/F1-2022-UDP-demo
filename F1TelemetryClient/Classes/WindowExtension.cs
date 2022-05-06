using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }

        public static void FreezeColors(this Window window)
        {
            foreach (Control c in FindVisualChildren<Control>(window))
            {
                if (c.BorderBrush != null && c.BorderBrush.CanFreeze) c.BorderBrush.Freeze();
                if (c.Background != null && c.Background.CanFreeze) c.Background.Freeze();
                if (c.Foreground != null && c.Foreground.CanFreeze) c.Foreground.Freeze();
                if (c.OpacityMask != null && c.OpacityMask.CanFreeze) c.OpacityMask.Freeze();
            }
        }
    }
}
