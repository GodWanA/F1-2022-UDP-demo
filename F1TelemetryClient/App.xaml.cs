using F1Telemetry;
using F1TelemetryApp.Classes;
using System.Net;
using System.Windows;
using System.Windows.Threading;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            u.Connention = new F1UDP(IPAddress.Any.ToString(), 20777);
            //u.Connention = new F1UDP("192.168.0.112", 20777);

            App.Current.Exit += Current_Exit;
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            base.OnStartup(e);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            throw new System.NotImplementedException();
            this.Dispatcher.Invoke(() =>
            {

            });
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            if (u.Connention?.IsConnecting == true) u.Connention.Close();
        }
    }
}
