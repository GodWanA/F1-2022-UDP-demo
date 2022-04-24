using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IConnectUDP, IGridResize
    {
        private DispatcherTimer lockTimer = new DispatcherTimer();
        private DispatcherTimer cleanTimer = new DispatcherTimer();
        private string regPath;

        public GridSizes PrevRes { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isWorking_SessionData = false;
        private bool isWorking_LapData = false;
        private bool isWorking_ParticipantsData = false;
        private bool isWorking_CarStatusData = false;
        //private bool isWorking_CarTelemetryData = false;                
        private Flags lastFlag = Flags.InvalidOrUnknown;
        private bool isCleaning;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.regPath = this.CreateRegPath();
            this.LoadWindowPosition(this.regPath);
            this.SubscribeUDPEvents();

            this.lockTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.lockTimer.Tick += LockTimer_Tick;

            this.cleanTimer.Interval = TimeSpan.FromSeconds(30);
            //this.cleanTimer.Tick += CleanTimer_Tick;
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.SessionPacket += this.Upp_SessionPacket;
            u.Connention.CarStatusPacket += Udp_CarStatusPacket;
            u.Connention.DataReadError += Connention_DataReadError;
            u.Connention.ConnectionError += Connention_ConnectionError;

            //this.cleanTimer.Start();
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.SessionPacket -= this.Upp_SessionPacket;
            u.Connention.CarStatusPacket -= Udp_CarStatusPacket;
            u.Connention.DataReadError -= Connention_DataReadError;
            u.Connention.ConnectionError -= Connention_ConnectionError;

            //this.cleanTimer.Stop();
        }

        private void Connention_ConnectionError(object sender, EventArgs e)
        {
            this.ShowError(sender as Exception);
        }

        private void Connention_DataReadError(object sender, EventArgs e)
        {
            this.ShowError(sender as Exception);
        }

        private void ShowError(Exception ex)
        {
            this.Dispatcher.Invoke(() =>
            {
                var res = MessageBox.Show(
                    ex.Message,
                    "Error!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                if (res == MessageBoxResult.OK) Application.Current?.Shutdown();
            }, DispatcherPriority.Background);
        }

        private void CleanTimer_Tick(object sender, EventArgs e)
        {
            this.cleanTimer.Stop();

            //if (u.CanDoUdp)
            //{
            //    this.CleanUpList();
            //}
            //this.listBox_drivers.Items.DeferRefresh();
            GC.WaitForFullGCApproach();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            this.cleanTimer.Start();
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            this.lockTimer.Stop();
            GC.WaitForFullGCApproach();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            // this.UpdateLayout();
            u.CanDoUdp = true;
        }

        private void Udp_CarStatusPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_CarStatusData && u.CanDoUdp)
            {
                this.isWorking_CarStatusData = true;
                this.Dispatcher.Invoke(() =>
                {
                    var status = sender as PacketCarStatusData;

                    var flags = status.CarStatusData.Select(x => x.VehicleFIAFlag);
                    this.SetSessionInfoColor(flags);

                    //this.CleanUpList();

                    this.isWorking_CarStatusData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void Upp_SessionPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_SessionData && u.CanDoUdp)
            {
                this.isWorking_SessionData = true;
                this.Dispatcher.Invoke(() =>
                {
                    var sessionData = sender as PacketSessionData;
                    var lapData = u.Connention.LastLapDataPacket;
                    var first = lapData?.Lapdata?.Where(x => x.CarPosition == 1).FirstOrDefault();

                    u.TrackLength = sessionData.TrackLength;
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("Session: " + Regex.Replace(sessionData.SessionType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim());
                    sb.AppendLine("Laps: " + first?.CurrentLapNum + " / " + sessionData.TotalLaps);
                    sb.Append("TimeLeft: " + sessionData.SessionTimeLeft.ToString());

                    this.textBlock_counterHead.Text = sb.ToString();

                    //this.weatherController.SetActualWeather(sessionData.Weather, sessionData.SessionType, sessionData.TrackTemperature, sessionData.AirTemperature);
                    //this.weatherController.SetWeatherForecast(sessionData.WeatherForcastSample);
                    //this.tyrecontainer.UpdateTyres(this.map.RawTrack);

                    var flags = sessionData.MarshalZones.Select(x => x.ZoneFlag);
                    this.SetSessionInfoColor(flags);

                    this.isWorking_SessionData = false;
                    //this.CleanUpList();

                }, DispatcherPriority.Background);
            }
        }

        private void SetSessionInfoColor(IEnumerable<Flags> flags)
        {
            var flag = flags.Where(x => x == Flags.Red || x == Flags.Yellow).FirstOrDefault();

            if (this.lastFlag != flag)
            {
                this.lastFlag = flag;

                switch (flag)
                {
                    default:
                        this.border_sessioninfo.Background = Brushes.Black;
                        this.textBlock_counterHead.Foreground = Brushes.White;
                        break;
                    case Flags.Yellow:
                        this.border_sessioninfo.Background = u.FlagColors[Flags.Yellow];
                        this.textBlock_counterHead.Foreground = Brushes.Black;
                        break;
                    case Flags.Red:
                        this.border_sessioninfo.Background = u.FlagColors[Flags.Red];
                        this.textBlock_counterHead.Foreground = Brushes.White;
                        break;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CalculateView();
        }

        private void CalculateView()
        {
            this.lockTimer.Stop();
            u.CanDoUdp = false;

            // New UserControl under construction, so i disable reordering.

            var res = GridSizes.XS;
            var d = this.ActualWidth;

            if (d > 1600) res = GridSizes.XL;
            else if (d > 1200) res = GridSizes.LG;
            else if (d > 900) res = GridSizes.MD;
            else if (d > 700) res = GridSizes.XM;

            Debug.WriteLine(res);

            if (this.PrevRes != res)
            {
                this.PrevRes = res;
                //this.drivercontainer.CalculateView(res);
                this.personalInfo.CalculateView(res);
                this.Render();
            }

            this.lockTimer.Start();
        }

        private void Render()
        {
            switch (this.PrevRes)
            {
                case GridSizes.XS:
                    this.ResizeXS();
                    break;
                case GridSizes.XM:
                    this.ResizeXM();
                    break;
                case GridSizes.MD:
                    this.ResizeMD();
                    break;
                case GridSizes.LG:
                    this.ResizeLG();
                    break;
                case GridSizes.XL:
                    this.ResizeXL();
                    break;
            }
        }

        public void ResizeXS()
        {
            this.personalInfo.ResizeXS();
        }

        public void ResizeXM()
        {
            this.personalInfo.ResizeXM();
        }

        public void ResizeMD()
        {
            this.personalInfo.ResizeMD();
        }

        public void ResizeLG()
        {
            this.personalInfo.ResizeLG();
        }

        public void ResizeXL()
        {
            this.personalInfo.ResizeXL();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            u.Connention.Close();
            this.SaveWindowPosition(this.regPath);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.lockTimer.Stop();
            u.CanDoUdp = false;
            this.lockTimer.Start();
        }

        private void listBox_drivers_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Debug.WriteLine("HELLLLÓÓÓÓÓÓÓ");
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.CalculateView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CalculateView();
            this.cleanTimer.Start();
        }

        private void cmdExit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void cmdExit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsLoaded) e.CanExecute = true;
        }

        private void cmdOpenPreferences_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsLoaded) e.CanExecute = true;
        }

        private void cmdOpenPreferences_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var window = new PreferencesWindow(this))
            {
                if (window.ShowDialog() == true)
                {
                    MessageBox.Show("Müxik");
                }
            }
        }

        private void cmdOpenMapTool_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsLoaded) e.CanExecute = true;
        }

        private void cmdOpenMapTool_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var window = new TrackLayoutRecorderWindow(this))
            {
                if (window.ShowDialog() == true)
                {
                    TrackLayout.SaveTrack(window.Map);
                    this.map.TrackID = Tracks.Unknown;
                }
            }
        }

        private void cmdOpenAbout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsLoaded) e.CanExecute = true;
        }

        private void cmdOpenAbout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var window = new AboutWindow { Owner = this })
            {
                window.ShowDialog();
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.cleanTimer.Stop();
        }

        private void SessionTower_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.personalInfo.LoadWear();
        }
    }
}
