using F1Telemetry;
using F1TelemetryApp.Classes;
using F1TelemetryApp.Windows;
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
        internal RadioMessageWindow RadioMessageWindow { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            u.Connention = new F1UDP(IPAddress.Any, 20777);
            //u.Connention = new F1UDP("192.168.0.112", 20777);

            App.Current.Exit += Current_Exit;
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            base.OnStartup(e);

            RadioMessageWindow.IsWindowEnabled = true;
            RadioMessageWindow.DemageOption = RadioMessageWindow.RadioOptions.AllDriver;

            this.RadioMessageWindow = new RadioMessageWindow(this.MainWindow);
            this.RadioMessageWindow.Show();

            this.MainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            this.RadioMessageWindow?.Close();
            //this.Shutdown(0);
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            u.Connention?.Close(true);

            throw new System.NotImplementedException();

            //this.Dispatcher.Invoke(() =>
            //{

            //});
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            if (u.Connention?.IsConnecting == true) u.Connention.Close();
        }
    }
}
