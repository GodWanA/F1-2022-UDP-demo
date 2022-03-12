using F1Telemetry;
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

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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


        public event PropertyChangedEventHandler PropertyChanged;

        private bool isWorking_SessionData = false;
        private bool isWorking_LapData = false;
        private bool isWorking_ParticipantsData = false;
        private bool isWorking_DemageData = false;
        private bool isWorking_CarStatusData = false;
        private bool isWorking_CarTelemetryData = false;
        private bool isLoadingWear = false;
        private bool canDoUdp = true;
        private Flags lastFlag = Flags.InvalidOrUnknown;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.participantsList = new ObservableCollection<PlayerListItemData>();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            u.Connention = new F1UDP("127.0.0.1", 20777);
            //this.udp = new F1UDP("192.168.0.112", 20777);
            u.Connention.SessionPacket += this.Upp_SessionPacket;
            u.Connention.LapDataPacket += this.Udp_LapDataPacket;
            u.Connention.ParticipantsPacket += this.Udp_ParticipantsPacket;
            u.Connention.DemagePacket += this.Udp_DemagePacket;
            u.Connention.CarStatusPacket += Udp_CarStatusPacket;
            u.Connention.CarTelemetryPacket += Udp_CarTelemetryPacket;
            u.Connention.SessionHistoryPacket += Udp_SessionHistoryPacket;

            this.listBox_drivers.Items.SortDescriptions.Add(new SortDescription("CarPosition", ListSortDirection.Ascending));

            this.listBox_drivers.Items.IsLiveSorting = true;
            this.listBox_drivers.Items.IsLiveFiltering = true;

            this.lockTimer.Interval = TimeSpan.FromMilliseconds(500);
            this.lockTimer.Tick += LockTimer_Tick;

            //this.cleanTimer.Interval = TimeSpan.FromSeconds(1);
            //this.cleanTimer.Tick += CleanTimer_Tick;
        }

        private void CleanTimer_Tick(object sender, EventArgs e)
        {
            this.cleanTimer.Stop();

            //this.listBox_drivers.Items.Refresh();
            this.listBox_drivers.Items.DeferRefresh();

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
                    }
                }, DispatcherPriority.Background);
            }
        }

        private void Udp_CarTelemetryPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_CarTelemetryData && this.canDoUdp)
            {
                this.isWorking_CarTelemetryData = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.LoadWear();
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
                    this.LoadWear();
                    var status = sender as PacketCarStatusData;

                    foreach (PlayerListItemData item in this.participantsList)
                    {
                        var current = status.CarStatusData[item.ArrayIndex];
                        item.TyreCompund = current.VisualTyreCompound;
                    }

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
                    this.LoadWear();
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
                    var items = this.listBox_drivers.Items?.Cast<PlayerListItemData>();

                    foreach (PlayerListItemData item in this.participantsList)
                    {
                        var current = participants.Participants[item.ArrayIndex];
                        item.DriverName = current.Name;
                        item.Nationality = current.Nationality;
                        item.TeamID = current.TeamID;
                        item.IsMyTeam = current.IsMyTeam;
                        item.RaceNumber = current.RaceNumber;
                    }

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

        private void Udp_LapDataPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_LapData && this.canDoUdp)
            {
                this.isWorking_LapData = true;
                this.Dispatcher.BeginInvoke(() =>
               {
                   var lapData = sender as PacketLapData;
                   var sessionData = u.Connention.LastSessionDataPacket;
                   //int noCarpos = 0;

                   foreach (PlayerListItemData elem in this.participantsList)
                   {
                       var current = lapData.Lapdata[elem.ArrayIndex];
                       float p = current.LapDistance / (float)sessionData.TrackLength * 100.0f;

                       elem.CurrentLapTime = current.CurrentLapTime;
                       elem.TrackLengthPercent = p;
                       elem.CarPosition = current.CarPosition;
                       //if (current.Warnings > 0) Debug.WriteLine("HELLÓÓÓ");
                       elem.WarningNumber = current.Warnings;
                       elem.PenaltyTime = current.Penalties;
                       elem.ResultStatus = current.ResultStatus;
                       elem.SetPitStatues(current.PitStatus, current.PitStopTimer);

                       if (current.IsCurrentLapInvalid)
                       {
                           elem.TimerForeground = Brushes.Red;
                           elem.TrackPercentForeground = Brushes.Red;
                       }
                       else
                       {
                           elem.TimerForeground = Brushes.White;
                           elem.TrackPercentForeground = Brushes.LimeGreen;
                       }

                       if (elem.CarPosition == 0) elem.Visibility = Visibility.Collapsed;
                       else elem.Visibility = Visibility.Visible;
                   }

                   if (this.listBox_drivers.SelectedItem != null)
                   {
                       var selected = this.listBox_drivers.SelectedItem as PlayerListItemData;
                       this.drivercontainer.UpdateLapdata(lapData.Lapdata[selected.ArrayIndex], sessionData.TrackLength);
                   }

                   this.listBox_drivers.Items.Filter = this.ListFilter;
                   this.listBox_drivers.Items.Refresh();

                   this.isWorking_LapData = false;
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

                    (this.listBox_drivers.SelectedItem as PlayerListItemData).IsSelected = true;
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
                    var history = u.Connention.LastSessionHistoryPacket[i];
                    var lapdata = u.Connention.LastLapDataPacket.Lapdata[i];
                    var participants = u.Connention.LastParticipantsPacket;
                    var trackLength = u.Connention.LastSessionDataPacket.TrackLength;
                    var gforce = u.Connention.LastMotionPacket.CarMotionData[i].GForce;

                    this.tyrecontainer.UpdateDatas(demage, status, telemetry);
                    this.drivercontainer.UpdateLapdata(lapdata, trackLength);
                    this.drivercontainer.UpdateDatas(status, telemetry, history, participants, gforce, i);

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
                }
                this.isLoadingWear = false;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.lockTimer.Stop();
            this.canDoUdp = false;

            // New UserControl under construction, so i disable reordering.

            //if (this.ActualWidth < 950)
            //{
            //    Grid.SetColumn(this.tyrecontainer, 0);
            //    Grid.SetColumn(this.groupbox_demage, 0);
            //    Grid.SetColumn(this.groupbox_motor, 0);

            //    Grid.SetColumnSpan(this.tyrecontainer, 12);
            //    Grid.SetColumnSpan(this.groupbox_demage, 12);
            //    Grid.SetColumnSpan(this.groupbox_motor, 12);

            //    Grid.SetRow(this.tyrecontainer, 0);
            //    Grid.SetRow(this.groupbox_demage, 1);
            //    Grid.SetRow(this.groupbox_motor, 2);

            //    Grid.SetColumn(this.wear_ce, 0);
            //    Grid.SetColumn(this.wear_es, 3);
            //    Grid.SetColumn(this.wear_ice, 0);
            //    Grid.SetColumn(this.wear_mguh, 3);
            //    Grid.SetColumn(this.wear_mguk, 0);
            //    Grid.SetColumn(this.wear_tc, 3);

            //    Grid.SetColumnSpan(this.wear_ce, 3);
            //    Grid.SetColumnSpan(this.wear_es, 3);
            //    Grid.SetColumnSpan(this.wear_ice, 3);
            //    Grid.SetColumnSpan(this.wear_mguh, 3);
            //    Grid.SetColumnSpan(this.wear_mguk, 3);
            //    Grid.SetColumnSpan(this.wear_tc, 3);

            //    Grid.SetRow(this.wear_ce, 0);
            //    Grid.SetRow(this.wear_es, 0);
            //    Grid.SetRow(this.wear_ice, 1);
            //    Grid.SetRow(this.wear_mguh, 1);
            //    Grid.SetRow(this.wear_mguk, 2);
            //    Grid.SetRow(this.wear_tc, 2);
            //}
            //else if (this.ActualWidth < 1400)
            //{
            //    Grid.SetColumn(this.tyrecontainer, 0);
            //    Grid.SetColumn(this.groupbox_demage, 8);
            //    Grid.SetColumn(this.groupbox_motor, 0);

            //    Grid.SetColumnSpan(this.tyrecontainer, 8);
            //    Grid.SetColumnSpan(this.groupbox_demage, 4);
            //    Grid.SetColumnSpan(this.groupbox_motor, 12);

            //    Grid.SetRow(this.tyrecontainer, 0);
            //    Grid.SetRow(this.groupbox_demage, 0);
            //    Grid.SetRow(this.groupbox_motor, 1);

            //    Grid.SetColumn(this.wear_ce, 0);
            //    Grid.SetColumn(this.wear_es, 1);
            //    Grid.SetColumn(this.wear_ice, 2);
            //    Grid.SetColumn(this.wear_mguh, 3);
            //    Grid.SetColumn(this.wear_mguk, 4);
            //    Grid.SetColumn(this.wear_tc, 5);

            //    Grid.SetColumnSpan(this.wear_ce, 1);
            //    Grid.SetColumnSpan(this.wear_es, 1);
            //    Grid.SetColumnSpan(this.wear_ice, 1);
            //    Grid.SetColumnSpan(this.wear_mguh, 1);
            //    Grid.SetColumnSpan(this.wear_mguk, 1);
            //    Grid.SetColumnSpan(this.wear_tc, 1);

            //    Grid.SetRow(this.wear_ce, 0);
            //    Grid.SetRow(this.wear_es, 0);
            //    Grid.SetRow(this.wear_ice, 0);
            //    Grid.SetRow(this.wear_mguh, 0);
            //    Grid.SetRow(this.wear_mguk, 0);
            //    Grid.SetRow(this.wear_tc, 0);
            //}
            //else if (this.ActualWidth < 1800)
            //{
            //    Grid.SetColumn(this.tyrecontainer, 0);
            //    Grid.SetColumn(this.groupbox_demage, 6);
            //    Grid.SetColumn(this.groupbox_motor, 0);

            //    Grid.SetColumnSpan(this.tyrecontainer, 6);
            //    Grid.SetColumnSpan(this.groupbox_demage, 6);
            //    Grid.SetColumnSpan(this.groupbox_motor, 12);

            //    Grid.SetRow(this.tyrecontainer, 0);
            //    Grid.SetRow(this.groupbox_demage, 0);
            //    Grid.SetRow(this.groupbox_motor, 1);

            //    Grid.SetColumn(this.wear_ce, 0);
            //    Grid.SetColumn(this.wear_es, 1);
            //    Grid.SetColumn(this.wear_ice, 2);
            //    Grid.SetColumn(this.wear_mguh, 3);
            //    Grid.SetColumn(this.wear_mguk, 4);
            //    Grid.SetColumn(this.wear_tc, 5);

            //    Grid.SetColumnSpan(this.wear_ce, 1);
            //    Grid.SetColumnSpan(this.wear_es, 1);
            //    Grid.SetColumnSpan(this.wear_ice, 1);
            //    Grid.SetColumnSpan(this.wear_mguh, 1);
            //    Grid.SetColumnSpan(this.wear_mguk, 1);
            //    Grid.SetColumnSpan(this.wear_tc, 1);

            //    Grid.SetRow(this.wear_ce, 0);
            //    Grid.SetRow(this.wear_es, 0);
            //    Grid.SetRow(this.wear_ice, 0);
            //    Grid.SetRow(this.wear_mguh, 0);
            //    Grid.SetRow(this.wear_mguk, 0);
            //    Grid.SetRow(this.wear_tc, 0);
            //}
            //else
            //{
            //    Grid.SetColumn(this.tyrecontainer, 0);
            //    Grid.SetColumn(this.groupbox_demage, 5);
            //    Grid.SetColumn(this.groupbox_motor, 9);

            //    Grid.SetColumnSpan(this.tyrecontainer, 5);
            //    Grid.SetColumnSpan(this.groupbox_demage, 4);
            //    Grid.SetColumnSpan(this.groupbox_motor, 3);

            //    Grid.SetRow(this.tyrecontainer, 0);
            //    Grid.SetRow(this.groupbox_demage, 0);
            //    Grid.SetRow(this.groupbox_motor, 0);

            //    Grid.SetColumn(this.wear_ce, 0);
            //    Grid.SetColumn(this.wear_es, 3);
            //    Grid.SetColumn(this.wear_ice, 0);
            //    Grid.SetColumn(this.wear_mguh, 3);
            //    Grid.SetColumn(this.wear_mguk, 0);
            //    Grid.SetColumn(this.wear_tc, 3);

            //    Grid.SetColumnSpan(this.wear_ce, 3);
            //    Grid.SetColumnSpan(this.wear_es, 3);
            //    Grid.SetColumnSpan(this.wear_ice, 3);
            //    Grid.SetColumnSpan(this.wear_mguh, 3);
            //    Grid.SetColumnSpan(this.wear_mguk, 3);
            //    Grid.SetColumnSpan(this.wear_tc, 3);

            //    Grid.SetRow(this.wear_ce, 0);
            //    Grid.SetRow(this.wear_es, 0);
            //    Grid.SetRow(this.wear_ice, 1);
            //    Grid.SetRow(this.wear_mguh, 1);
            //    Grid.SetRow(this.wear_mguk, 2);
            //    Grid.SetRow(this.wear_tc, 2);
            //}

            this.lockTimer.Start();
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
    }
}
