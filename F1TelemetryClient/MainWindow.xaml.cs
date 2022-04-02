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
        private bool isWorking_LapData = false;
        private bool isWorking_ParticipantsData = false;
        private bool isWorking_DemageData = false;
        private bool isWorking_CarStatusData = false;
        //private bool isWorking_CarTelemetryData = false;
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

            this.cleanTimer.Interval = TimeSpan.FromSeconds(0.5);
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

        private DateTime LastClean;

        private void Udp_LapDataPacket(object sender, EventArgs e)
        {
            if (this.canDoUdp && !this.isWorking_LapData && DateTime.Now - this.LastClean > TimeSpan.FromSeconds(0.5))
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
                    "Error!",
                    ex.Message,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                if (res == MessageBoxResult.OK) Application.Current?.Shutdown();
            }, DispatcherPriority.Render);
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
            // this.UpdateLayout();
            this.canDoUdp = true;
        }

        private bool ListFilter(object o)
        {
            if (o is PlayerListItemData && (o as PlayerListItemData).CarPosition > 0) return true;
            else return false;
        }

        private void Udp_SessionHistoryPacket(object sender, EventArgs e)
        {
            //this.Dispatcher.BeginInvoke(() => this.CleanUpList(), DispatcherPriority.Render);

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
            if (!this.isWorking_CarStatusData && this.canDoUdp)
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
                }, DispatcherPriority.Render);
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

                //this.border_sessioninfo.UpdateLayout();
                //this.textBlock_counterHead.UpdateLayout();
            }
        }

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.listBox_drivers.SelectedIndex > -1)
                {
                    var selected = this.listBox_drivers.SelectedItem as PlayerListItemData;

                    if (selected.CarPosition > 0)
                    {
                        foreach (var item in this.participantsList) item.IsSelected = false;

                        selected.IsSelected = true;
                        this.LoadWear();
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
            this.CalculateView();
        }

        private void CalculateView()
        {
            this.lockTimer.Stop();
            this.canDoUdp = false;

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
                this.drivercontainer.CalculateView(res);
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
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 12);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 3, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 6);
            IGridResize.SetGridSettings(this.demage_fwRight, 0, 1, 6);
            IGridResize.SetGridSettings(this.demage_fl, 0, 2, 6);
            IGridResize.SetGridSettings(this.demage_sp, 0, 3, 6);
            IGridResize.SetGridSettings(this.demage_en, 0, 4, 6);
            IGridResize.SetGridSettings(this.demage_ex, 0, 5, 6);
            IGridResize.SetGridSettings(this.demage_gb, 0, 6, 6);
            IGridResize.SetGridSettings(this.demage_df, 0, 7, 6);
            IGridResize.SetGridSettings(this.demage_rw, 0, 8, 6);
        }

        public void ResizeXM()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 12);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 3, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 3);
            IGridResize.SetGridSettings(this.wear_ice, 3, 0, 3);
            IGridResize.SetGridSettings(this.wear_tc, 0, 1, 3);
            IGridResize.SetGridSettings(this.wear_mguh, 3, 1, 3);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 2, 3);
            IGridResize.SetGridSettings(this.wear_es, 3, 2, 3);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeMD()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 7);
            IGridResize.SetGridSettings(this.groupbox_motor, 7, 2, 5);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 6);
            IGridResize.SetGridSettings(this.demage_ex, 0, 3, 6);
            IGridResize.SetGridSettings(this.demage_gb, 0, 4, 6);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeLG()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 7);
            IGridResize.SetGridSettings(this.groupbox_demage, 7, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 2, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 1);
            IGridResize.SetGridSettings(this.wear_ice, 1, 0, 1);
            IGridResize.SetGridSettings(this.wear_tc, 2, 0, 1);
            IGridResize.SetGridSettings(this.wear_mguh, 3, 0, 1);
            IGridResize.SetGridSettings(this.wear_mguk, 4, 0, 1);
            IGridResize.SetGridSettings(this.wear_es, 5, 0, 1);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeXL()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_demage, 5, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_motor, 10, 1, 2);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.CalculateView();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.CalculateView();
        }

        private void menuitem_trackLayout_Click(object sender, RoutedEventArgs e)
        {
            using (var window = new TrackLayoutRecorderWindow
            {
                Owner = this,
                Width = 600,
                Height = 450,
            })
            {
                if (window.ShowDialog() == true)
                {
                    TrackLayout.SaveTrack(window.Map);
                }
            }
        }
    }
}
