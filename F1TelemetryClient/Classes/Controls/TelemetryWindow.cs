using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace F1TelemetryApp.Classes.Controls
{
    class TelemetryWindow : Window
    {
        protected readonly string regPath;

        private const string regRoot = "SOFTWARE\\Csernay\\F1UDP";

        public TelemetryWindow()
        {
            this.regPath = this.CreateRegistryPath();
            this.LoadReg();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Escape) this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.SaveReg();
            base.OnClosing(e);
        }

        private string CreateRegistryPath()
        {
            StringBuilder ret = new StringBuilder(TelemetryWindow.regRoot);
            var p = this.Parent;

            while (p != null)
            {
                if (p is Window)
                {
                    if (ret.ToString() != "") ret.Append("\\");
                    var w = p as Window;
                    ret.Append(w.Title.Replace(" ", "_").Trim());
                }
            }

            return ret.ToString();
        }

        private void LoadReg()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(this.regPath))
            {
                var w = key.GetValue("Width", double.NaN) as double?;
                var l = key.GetValue("Left", double.NaN) as double?;

                if (l.Value + w.Value <= SystemParameters.VirtualScreenWidth)
                {
                    this.Left = l.Value;
                    this.Width = l.Value;
                }
                else if (l.Value <= SystemParameters.VirtualScreenWidth)
                {
                    this.Left = l.Value;
                    this.Width = SystemParameters.VirtualScreenWidth - l.Value;
                }
                else if (w.Value <= SystemParameters.VirtualScreenWidth)
                {
                    this.Left = 0;
                    this.Width = w.Value;
                }

                //key.SetValue("Height", this.ActualHeight);
                //key.SetValue("Left", this.Left);
                //key.SetValue("Top", this.Top);
                //key.SetValue("WindowState", (int)this.WindowState);
            }
        }

        private void SaveReg()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(this.regPath))
            {
                key.SetValue("Width", this.ActualWidth);
                key.SetValue("Height", this.ActualHeight);
                key.SetValue("Left", this.Left);
                key.SetValue("Top", this.Top);
                key.SetValue("WindowState", this.WindowState);
            }
        }
    }
}
