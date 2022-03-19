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
        //private bool isWorking_LapData = false;
        private bool isWorking_ParticipantsData = false;
        private bool isWorking_DemageData = false;
        private bool isWorking_CarStatusData = false;
        private bool isWorking_CarTelemetryData = false;
        private bool isLoadingWear = false;
        private bool canDoUdp = true;
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

            this.cleanTimer.Interval = TimeSpan.FromSeconds(1);
            this.cleanTimer.Tick += CleanTimer_Tick;
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.SessionPacket += this.Upp_SessionPacket;
            //u.Connention.LapDataPacket += this.Udp_LapDataPacket;
            u.Connention.ParticipantsPacket += this.Udp_ParticipantsPacket;
            u.Connention.DemagePacket += this.Udp_DemagePacket;
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
            u.Connention.DemagePacket -= this.Udp_DemagePacket;
            u.Connention.CarStatusPacket -= Udp_CarStatusPacket;
            //u.Connention.CarTelemetryPacket -= Udp_CarTelemetryPacket;
            u.Connention.SessionHistoryPacket -= Udp_SessionHistoryPacket;
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
            this.Dispatcher.BeginInvoke(() =>
            {
                var res = MessageBox.Show(
                    "Error!",
                    ex.Message,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                if (res == MessageBoxResult.OK) Application.Current.Shutdown();
            }, DispatcherPriority.Background);
        }

        private void CleanTimer_Tick(object sender, EventArgs e)
        {
            this.cleanTimer.Stop();

            if (this.canDoUdp)
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
            this.UpdateLayout();
            this.canDoUdp = true;
        }

        private bool ListFilter(object o)
        {
            if (o is PlayerListItemData)
            {
                if ((o as PlayerListItemData).CarPosition > 0) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }
        private void Udp_SessionHistoryPacket(object sender, EventArgs e)
        {
            if (this.canDoUdp)
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

                        //this.CleanUpList();
                    }
                }, DispatcherPriority.Background);
            }
        }

        private void CleanUpList()
        {
            if (!this.isCleaning)
            {
                this.isCleaning = true;

                this.listBox_drivers.Items.Filter = this.ListFilter;
                this.listBox_drivers.Items.Refresh();

                GC.WaitForFullGCApproach();
                GC.WaitForFullGCComplete();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                this.isCleaning = false;
            }
        }

        private void Udp_CarTelemetryPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_CarTelemetryData && this.canDoUdp)
            {
                this.isWorking_CarTelemetryData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    //this.LoadWear();
                    this.isWorking_CarTelemetryData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void Udp_CarStatusPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_CarStatusData && this.canDoUdp)
            {
                this.isWorking_CarStatusData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    //this.LoadWear();
                    var status = sender as PacketCarStatusData;

                    //foreach (PlayerListItemData item in this.participantsList)
                    //{
                    //    var current = status.CarStatusData[item.ArrayIndex];
                    //    item.TyreCompund = current.VisualTyreCompound;
                    //}

                    var flags = status.CarStatusData.Select(x => x.VehicleFIAFlag);
                    this.SetSessionInfoColor(flags);

                    this.isWorking_CarStatusData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void Udp_DemagePacket(object sender, EventArgs e)
        {
            if (!this.isWorking_DemageData && this.canDoUdp)
            {
                this.isWorking_DemageData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    //this.LoadWear();
                    int i = this.listBox_drivers.SelectedIndex;
                    var data = sender as PacketCarDamageData;
                    if (i > -1) this.UpdateDemageInfo(data.CarDamageData[i]);

                    this.isWorking_DemageData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void Udp_ParticipantsPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_ParticipantsData && this.canDoUdp)
            {
                this.isWorking_ParticipantsData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    int index = this.listBox_drivers.SelectedIndex;

                    var participants = sender as PacketParticipantsData;
                    this.PlayerList(participants.Participants.Length);


                    //foreach (PlayerListItemData item in this.participantsList)
                    //{
                    //    var current = participants.Participants[item.ArrayIndex];
                    //    item.DriverName = current.Name;
                    //    item.Nationality = current.Nationality;
                    //    item.TeamID = current.TeamID;
                    //    item.IsMyTeam = current.IsMyTeam;
                    //    item.RaceNumber = current.RaceNumber;
                    //}

                    if (index != -1) this.listBox_drivers.SelectedIndex = index;
                    else if (participants.Header.Player1CarIndex != 255) this.listBox_drivers.SelectedItem = this.participantsList[participants.Header.Player1CarIndex];

                    //this.CalculateInterval();

                    if (this.listBox_drivers.SelectedItem != null)
                    {
                        this.listBox_drivers.ScrollIntoView(this.listBox_drivers.SelectedItem);
                    }

                    this.isWorking_ParticipantsData = false;
                }, DispatcherPriority.Background);
            }
        }

        private void Upp_SessionPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_SessionData && this.canDoUdp)
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

                    this.weatherController.SetActualWeather(sessionData.Weather, sessionData.SessionType, sessionData.TrackTemperature, sessionData.AirTemperature);
                    this.weatherController.SetWeatherForecast(sessionData.WeatherForcastSample);

                    this.isWorking_SessionData = false;

                    var flags = sessionData.MarshalZones.Select(x => x.ZoneFlag);
                    this.SetSessionInfoColor(flags);

                    this.CleanUpList();

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

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.listBox_drivers.SelectedIndex > -1)
                {
                    foreach (var item in this.participantsList)
                    {
                        item.IsSelected = false;
                    }

                    var selected = this.listBox_drivers.SelectedItem as PlayerListItemData;
                    selected.IsSelected = true;

                    int i = selected.ArrayIndex;

                    this.LoadWear();

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

        private void LoadWear()
        {
            if (!this.isLoadingWear)
            {
                this.isLoadingWear = true;
                if (this.listBox_drivers.SelectedIndex != -1)
                {
                    int i = (this.listBox_drivers.SelectedItem as PlayerListItemData).ArrayIndex;
                    var demage = u.Connention.LastCarDemagePacket?.CarDamageData[i];
                    var status = u.Connention.LastCarStatusDataPacket?.CarStatusData[i];
                    var telemetry = u.Connention.LastCarTelmetryPacket?.CarTelemetryData[i];
                    var sessionHistory = u.Connention.LastSessionHistoryPacket[i];
                    var participants = u.Connention.LastParticipantsPacket;
                    var gForce = u.Connention.LastMotionPacket.CarMotionData[i].GForce;
                    var lapdata = u.Connention.LastLapDataPacket;

                    this.tyrecontainer.UpdateDatas(demage, status, telemetry, i);
                    this.drivercontainer.UpdateDatas(status, telemetry, sessionHistory, participants, gForce, lapdata, i);
                    this.UpdateDemageInfo(demage);
                }
                this.isLoadingWear = false;
            }
        }

        private void UpdateDemageInfo(CarDamageData demage)
        {
            if (demage != null)
            {
                this.demage_fwLeft.Percent = 100.0 - demage.FrontLeftWingDemage;
                this.demage_fwRight.Percent = 100.0 - demage.FrontRightWingDemage;
                this.demage_fl.Percent = 100.0 - demage.FloorDemage;
                this.demage_df.Percent = 100.0 - demage.DiffurerDemage;
                this.demage_en.Percent = 100.0 - demage.EngineDemage;
                this.demage_gb.Percent = 100.0 - demage.GearBoxDemage;
                this.demage_rw.Percent = 100.0 - demage.RearWingDemage;
                this.demage_sp.Percent = 100.0 - demage.SidepodDemage;
                this.demage_ex.Percent = 100.0 - demage.ExhasutDemage;

                this.wear_ce.Percent = 100.0 - demage.EngineCEWear;
                this.wear_es.Percent = 100.0 - demage.EngineESWear;
                this.wear_ice.Percent = 100.0 - demage.EngineICEWear;
                this.wear_mguh.Percent = 100.0 - demage.EngineMGUKWear;
                this.wear_mguk.Percent = 100.0 - demage.EngineMGUKWear;
                this.wear_tc.Percent = 100.0 - demage.EngineTCWear;
            }
            else
            {
                this.demage_fwLeft.Percent = double.NaN;
                this.demage_fwRight.Percent = double.NaN;
                this.demage_fl.Percent = double.NaN;
                this.demage_df.Percent = double.NaN;
                this.demage_en.Percent = double.NaN;
                this.demage_gb.Percent = double.NaN;
                this.demage_rw.Percent = double.NaN;
                this.demage_sp.Percent = double.NaN;
                this.demage_ex.Percent = double.NaN;

                this.wear_ce.Percent = double.NaN;
                this.wear_es.Percent = double.NaN;
                this.wear_ice.Percent = double.NaN;
                this.wear_mguh.Percent = double.NaN;
                this.wear_mguk.Percent = double.NaN;
                this.wear_tc.Percent = double.NaN;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.lockTimer.Stop();
            this.canDoUdp = false;

            // New UserControl under construction, so i disable reordering.

            var res = GridSizes.XS;
            var d = this.ActualWidth;

            if (d > 1900) res = GridSizes.XL;
            else if (d > 1100) res = GridSizes.LG;
            else if (d > 800) res = GridSizes.MD;
            else if (d > 500) res = GridSizes.XM;

            Debug.WriteLine(res);

            if (this.PrevRes != res)
            {
                this.PrevRes = res;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            u.Connention.Close();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.lockTimer.Stop();
            this.canDoUdp = false;
            this.lockTimer.Start();
        }

        private void listBox_drivers_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Debug.WriteLine("HELLLLÓÓÓÓÓÓÓ");
        }

        private void menuitem_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void menitem_about_Click(object sender, RoutedEventArgs e)
        {
            var ablak = new AboutWindow();
            ablak.Owner = this;
            ablak.ShowDialog();
        }

        public void ResizeXS()
        {
            //throw new NotImplementedException();
        }

        public void ResizeXM()
        {
            //throw new NotImplementedException();
        }

        public void ResizeMD()
        {
            //throw new NotImplementedException();
        }

        public void ResizeLG()
        {
            //throw new NotImplementedException();
        }

        public void ResizeXL()
        {
            //throw new NotImplementedException();
        }
    }
}
