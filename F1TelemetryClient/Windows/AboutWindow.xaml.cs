using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace F1TelemetryApp.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.textblock_gameVersion.Text = Assembly.GetCallingAssembly().GetName().Version.ToString();
            this.textblock_dllVersion.Text = F1Telemetry.F1UDP.DLLVersion.ToString();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo { FileName = link.NavigateUri.AbsoluteUri });
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var url = e.Uri.ToString();
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
    }
}
