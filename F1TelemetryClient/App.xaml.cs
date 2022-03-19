using F1Telemetry;
using F1TelemetryApp.Classes;
using System.Windows;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            u.Connention = new F1UDP("127.0.0.1", 20777);
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (u.Connention?.IsConnecting == true) u.Connention.Close();
        }
    }
}
