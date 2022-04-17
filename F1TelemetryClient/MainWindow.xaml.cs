using F1Telemetry;
using F1Telemetry.Models.CarDamagePacket;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.UserControls;
using F1TelemetryApp.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<PlayerListItemData> participantsList;

        public ObservableCollection<PlayerListItemData> ParticipantsList
        {
            get { return participantsList; }
            set
            {
                if (value != participantsList)
                {
                    participantsList = value;
                    this.OnPropertyChanged("ParticipantsList");
                }
            }
        }

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
            this.participantsList = new ObservableCollection<PlayerListItemData>();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            //this.udp = new F1UDP("192.168.0.112", 20777);
            this.SubscribeUDPEvents();

            this.listBox_drivers.Items.SortDescriptions.Add(new SortDescription("CarPosition", ListSortDirection.Ascending));

            this.listBox_drivers.Items.IsLiveSorting = true;
            this.listBox_drivers.Items.IsLiveFiltering = true;

            this.lockTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.lockTimer.Tick += LockTimer_Tick;

            this.cleanTimer.Interval = TimeSpan.FromSeconds(0.5);
            this.cleanTimer.Tick += CleanTimer_Tick;
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.SessionPacket += this.Upp_SessionPacket;
            //u.Connention.LapDataPacket += this.Udp_LapDataPacket;
            u.Connention.ParticipantsPacket += this.Udp_ParticipantsPacket;
            //u.Connention.DemagePacket += this.Udp_DemagePacket;
            u.Connention.CarStatusPacket += Udp_CarStatusPacket;
            //u.Connention.CarTelemetryPacket += Udp_CarTelemetryPacket;
            u.Connention.SessionHistoryPacket += Udp_SessionHistoryPacket;
            u.Connention.DataReadError += Connention_DataReadError;
            u.Connention.ConnectionError += Connention_ConnectionError;

            //this.cleanTimer.Start();
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.SessionPacket -= this.Upp_SessionPacket;
            //u.Connention.LapDataPacket -= this.Udp_LapDataPacket;
            u.Connention.ParticipantsPacket -= this.Udp_ParticipantsPacket;
            //u.Connention.DemagePacket -= this.Udp_DemagePacket;
            u.Connention.CarStatusPacket -= Udp_CarStatusPacket;
            //u.Connention.CarTelemetryPacket -= Udp_CarTelemetryPacket;
            u.Connention.SessionHistoryPacket -= Udp_SessionHistoryPacket;
            u.Connention.DataReadError -= Connention_DataReadError;
            u.Connention.ConnectionError -= Connention_ConnectionError;

            //this.cleanTimer.Stop();
        }

        private DateTime LastClean;

        private void Udp_LapDataPacket(object sender, EventArgs e)
        {
            if (u.CanDoUdp && !this.isWorking_LapData && DateTime.Now - this.LastClean > TimeSpan.FromSeconds(0.5))
            {
                this.isWorking_LapData = true;
                LastClean = DateTime.Now;

                this.Dispatcher.BeginInvoke(() =>
                {
                    this.CleanUpList();
                    this.isWorking_LapData = false;
                }, DispatcherPriority.Render);
            }
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
            this.Dispatcher.BeginInvoke(() =>
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

        private void CleanTimer_Tick(object sender, EventArgs e)
        {
            this.cleanTimer.Stop();

            if (u.CanDoUdp)
            {
                this.CleanUpList();
            }
            //this.listBox_drivers.Items.DeferRefresh();

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

        private bool ListFilter(object o)
        {
            if (o is PlayerListItemData && (o as PlayerListItemData).CarPosition > 0) return true;
            else return false;
        }

        private void Udp_SessionHistoryPacket(object sender, EventArgs e)
        {
            //this.Dispatcher.BeginInvoke(() => this.CleanUpList(), DispatcherPriority.Render);

            if (u.CanDoUdp)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    var curHistory = sender as PacketSessionHistoryData;

                    var arrayHistory = u.Connention.LastSessionHistoryPacket;
                    var lapData = u.Connention.LastLapDataPacket;

                    var itemSource = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();
                    var curItem = itemSource?.Where(x => x.ArrayIndex == curHistory.CarIndex).FirstOrDefault();
                    Brush fontColor = Brushes.White;

                    if (curItem != null)
                    {
                        if (curItem.CarPosition == 1)
                        {
                            curItem.IntervalTime = "interval";
                            curItem.LeaderIntervalTime = "leader";
                            curItem.TextColor = fontColor;
                        }
                        else if (!curItem.IsOut)
                        {
                            curItem.IntervalTime = "";
                            curItem.LeaderIntervalTime = "";
                            var curLapdata = lapData?.Lapdata[curItem.ArrayIndex];

                            var prevItem = itemSource.Where(x => x.CarPosition == curItem.CarPosition - 1).FirstOrDefault();
                            if (prevItem != null)
                            {
                                var prevHistory = arrayHistory.Where(x => x?.CarIndex == prevItem.ArrayIndex).FirstOrDefault();
                                string s = u.CalculateDelta(lapData, curHistory, prevHistory, out fontColor);
                                if (s != null && s != "") curItem.IntervalTime = s;
                                curItem.TextColor = fontColor;
                                prevItem = null;
                                prevItem = null;
                            }

                            var firstItem = itemSource.Where(x => x.CarPosition == 1).FirstOrDefault();
                            if (firstItem != null)
                            {
                                var firstHistory = arrayHistory.Where(x => x?.CarIndex == firstItem.ArrayIndex).FirstOrDefault();
                                string s = u.CalculateDelta(lapData, curHistory, firstHistory, out fontColor);
                                if (s != null && s != "") curItem.LeaderIntervalTime = s;
                                firstItem = null;
                                firstHistory = null;
                            }
                        }
                        else
                        {
                            curItem.IntervalTime = "";
                            curItem.setStateText();
                            curItem.TextColor = Brushes.White;
                        }
                    }

                    //this.CleanUpList();
                }, DispatcherPriority.Render);
            }
        }

        private void CleanUpList()
        {
            if (!this.isCleaning)
            {
                this.isCleaning = true;

                //this.listBox_drivers.Items.Filter = this.ListFilter;
                if (this.participantsList.Count > 0) this.listBox_drivers.Items.Refresh();

                //GC.WaitForFullGCApproach();
                //GC.WaitForFullGCComplete();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();

                this.isCleaning = false;
            }
        }

        private void Udp_CarStatusPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_CarStatusData && u.CanDoUdp)
            {
                this.isWorking_CarStatusData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    var status = sender as PacketCarStatusData;

                    var flags = status.CarStatusData.Select(x => x.VehicleFIAFlag);
                    this.SetSessionInfoColor(flags);

                    //this.CleanUpList();

                    this.isWorking_CarStatusData = false;
                }, DispatcherPriority.Render);
            }
        }

        //private void Udp_DemagePacket(object sender, EventArgs e)
        //{
        //    if (!this.isWorking_DemageData && this.canDoUdp)
        //    {
        //        this.isWorking_DemageData = true;
        //        this.Dispatcher.BeginInvoke(() =>
        //        {
        //            //this.LoadWear();
        //            int i = this.listBox_drivers.SelectedIndex;
        //            var data = sender as PacketCarDamageData;
        //            if (i > -1) this.UpdateDemageInfo(data.CarDamageData[i]);

        //            this.isWorking_DemageData = false;
        //        }, DispatcherPriority.Render);
        //    }
        //}

        private void Udp_ParticipantsPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_ParticipantsData && u.CanDoUdp)
            {
                this.isWorking_ParticipantsData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    int index = this.listBox_drivers.SelectedIndex;

                    var participants = sender as PacketParticipantsData;
                    this.PlayerList(participants.Participants.Length);

                    if (index != -1) this.listBox_drivers.SelectedIndex = index;
                    else if (participants.Header.Player1CarIndex != 255) this.listBox_drivers.SelectedItem = this.participantsList[participants.Header.Player1CarIndex];

                    if (this.listBox_drivers.SelectedItem != null)
                    {
                        this.listBox_drivers.ScrollIntoView(this.listBox_drivers.SelectedItem);
                    }

                    //this.CleanUpList();
                    //this.listBox_drivers.Items.Filter = this.ListFilter;
                    this.isWorking_ParticipantsData = false;
                }, DispatcherPriority.Render);
            }
        }

        private void Upp_SessionPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_SessionData && u.CanDoUdp)
            {
                this.isWorking_SessionData = true;
                this.Dispatcher.BeginInvoke(() =>
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

                }, DispatcherPriority.Render);
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

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            u.SelectedIndex = this.listBox_drivers.SelectedIndex;
            u.SelectedItem = this.listBox_drivers.SelectedItem;

            if (this.IsLoaded)
            {
                if (this.listBox_drivers.SelectedIndex > -1)
                {
                    var selected = this.listBox_drivers.SelectedItem as PlayerListItemData;

                    if (selected.CarPosition > 0)
                    {
                        foreach (var item in this.participantsList) item.IsSelected = false;

                        selected.IsSelected = true;
                        this.personalInfo.LoadWear();
                    }
                    else
                    {
                        this.listBox_drivers.SelectedItem = null;
                    }
                }
            }
        }

        private void PlayerList(int numberOfPlayers)
        {
            if (this.participantsList.Count != numberOfPlayers)
            {
                this.participantsList.Clear();

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    var data = new PlayerListItemData();
                    data.ArrayIndex = i;
                    this.participantsList.Add(data);
                }
            }

            //this.CleanUpList();
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
            using (var window = new PreferencesWindow
            {
                Owner = this,
                Width = 600,
                Height = 450,
            })
            {
                if (window.ShowDialog() == true)
                {

                }
            }
        }

        private void cmdOpenMapTool_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.IsLoaded) e.CanExecute = true;
        }

        private void cmdOpenMapTool_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (var window = new TrackLayoutRecorderWindow
            {
                Owner = this,
                Width = 630,
                Height = 450,
            })
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
    }
}
