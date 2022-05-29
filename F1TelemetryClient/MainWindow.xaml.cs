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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IConnectUDP, IGridResize, IDisposable
    {
        private DispatcherTimer lockTimer = new DispatcherTimer();
        private DispatcherTimer performanceTimer = new DispatcherTimer();
        private string regPath;
        private PerformanceCounter performance = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        private bool isWorking_SessionData = false;
        private bool isWorking_CarStatusData = false;
        //private bool isWorking_CarTelemetryData = false;                
        private Flags lastFlag = Flags.InvalidOrUnknown;
        private bool disposedValue;
        private double maxCPU;
        private double maxRAM;

        public event PropertyChangedEventHandler PropertyChanged;

        public GridSizes PrevRes { get; private set; }

        private string _cpuusage;
        public string CPUusage
        {
            get { return _cpuusage; }
            set
            {
                if (value != _cpuusage)
                {
                    _cpuusage = value;
                    this.OnPropertyChanged("CPUusage");
                }
            }
        }

        private string _ramusage;
        public string RAMusage
        {
            get { return _ramusage; }
            set
            {
                if (value != _ramusage)
                {
                    _ramusage = value;
                    this.OnPropertyChanged("RAMusage");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 10 }
            );

            //this.socket = new ClientWebSocket();
            //this.socket.ConnectAsync(new Uri("ws://localhost:8080"), new System.Threading.CancellationToken());

            //new Recorder();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.regPath = this.CreateRegPath();
            this.LoadWindowPosition(this.regPath);

            this.lockTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.lockTimer.Tick += LockTimer_Tick;

            this.performanceTimer.Interval = TimeSpan.FromSeconds(1);
            this.performanceTimer.Tick += PerformanceTimer_Tick;
        }

        private void PerformanceTimer_Tick(object sender, EventArgs e)
        {
            this.performanceTimer.Stop();

            if (this.IsLoaded)
            {
                var currentCPU = this.performance.NextValue() / 10.0;
                if (this.maxCPU < currentCPU) this.maxCPU = currentCPU;

                this.CPUusage = String.Format("Total CPU usage: {0:0.00}% (Max: {1:0.00}%)", currentCPU, this.maxCPU);

                var currentRAM = Process.GetCurrentProcess().PagedMemorySize64 / 1000000.0;
                if (this.maxRAM < currentRAM) this.maxRAM = currentRAM;

                this.RAMusage = String.Format("Total memory usage: {0:0.00}MB (Max: {1:0.00} MB)", currentRAM, this.maxRAM);
            }

            this.performanceTimer.Start();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.DataReadError += Connention_DataReadError;
                u.Connention.ConnectionError += Connention_ConnectionError;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.DataReadError -= Connention_DataReadError;
                u.Connention.ConnectionError -= Connention_ConnectionError;
            }
        }

        private void Connention_ConnectionError(object sender, Exception ex, EventArgs e)
        {
            this.ShowError(ex);
        }

        private void Connention_DataReadError(object sender, Exception ex, EventArgs e)
        {
            this.ShowError(ex);
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
            }, DispatcherPriority.Render);
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

        //private void SetSessionInfoColor(IEnumerable<Flags> flags)
        //{
        //    var flag = flags.Where(x => x == Flags.Red || x == Flags.Yellow).FirstOrDefault();

        //    if (this.lastFlag != flag)
        //    {
        //        this.lastFlag = flag;

        //        switch (flag)
        //        {
        //            default:
        //                this.border_sessioninfo.Background = Brushes.Black;
        //                this.textBlock_counterHead.Foreground = Brushes.White;
        //                break;
        //            case Flags.Yellow:
        //                this.border_sessioninfo.Background = u.FlagColors[Flags.Yellow];
        //                this.textBlock_counterHead.Foreground = Brushes.Black;
        //                break;
        //            case Flags.Red:
        //                this.border_sessioninfo.Background = u.FlagColors[Flags.Red];
        //                this.textBlock_counterHead.Foreground = Brushes.White;
        //                break;
        //        }
        //    }
        //}

        private void OnPropertyChanged(string propertyName)
        {
            this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            // this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)
            // if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
            this.SubscribeUDPEvents();
            this.CalculateView();
            this.FreezeColors();
            this.performanceTimer.Start();
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
            this.UnsubscribeUDPEvents();
            this.performanceTimer.Stop();
        }

        private void SessionTower_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.personalInfo.LoadWear();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                    this.PropertyChanged = null;

                    this.lockTimer.Stop();
                    this.lockTimer = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MainWindow()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
