using F1Telemetry;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.UserControls;
using System;
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
    public partial class MainWindow : Window
    {
        private F1UDP udp;
        private DispatcherTimer lockTimer = new DispatcherTimer();
        private DispatcherTimer cleanTimer = new DispatcherTimer();

        private TyreCompounds actualTyreCpompund;
        public TyreCompounds ActualTyreCpompund
        {
            get { return actualTyreCpompund; }
            set
            {
                if (value != actualTyreCpompund)
                {
                    actualTyreCpompund = value;
                    this.image_tyre.Source = u.TyreCompoundToImage(this.actualTyreCpompund);
                }
            }
        }

        private int lapAges;
        public int LapAges
        {
            get { return lapAges; }
            set
            {
                lapAges = value;
                //this.textBlock_tyreAge.Text = this.lapAges.ToString();
                this.OnPropertyChanged("LapAges");
            }
        }

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

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.participantsList = new ObservableCollection<PlayerListItemData>();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.udp = new F1UDP("127.0.0.1", 20777);
            //this.udp = new F1UDP("192.168.0.112", 20777);
            this.udp.SessionPacket += this.Upp_SessionPacket;
            this.udp.LapDataPacket += this.Udp_LapDataPacket;
            this.udp.ParticipantsPacket += this.Udp_ParticipantsPacket;
            this.udp.DemagePacket += this.Udp_DemagePacket;
            this.udp.CarStatusPacket += Udp_CarStatusPacket;
            this.udp.CarTelemetryPacket += Udp_CarTelemetryPacket;
            this.udp.SessionHistoryPacket += Udp_SessionHistoryPacket;

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

                    var arrayHistory = this.udp.LastSessionHistoryPacket;
                    var sessionData = this.udp.LastSessionDataPacket;
                    var lapData = this.udp.LastLapDataPacket;

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
                        else
                        {
                            curItem.IntervalTime = "";
                            curItem.LeaderIntervalTime = "";
                            var curLapdata = lapData?.Lapdata[curItem.ArrayIndex];

                            var prevItem = itemSource.Where(x => x.CarPosition == curItem.CarPosition - 1).FirstOrDefault();
                            if (prevItem != null)
                            {
                                var prevHistory = arrayHistory.Where(x => x?.CarIndex == prevItem.ArrayIndex).FirstOrDefault();
                                string s = this.CalculateDelta(arrayHistory, lapData, sessionData, curHistory, prevHistory, out fontColor);
                                if (s != null && s != "") curItem.IntervalTime = s;
                                curItem.TextColor = fontColor;
                                prevItem = null;
                                prevItem = null;
                            }

                            var firstItem = itemSource.Where(x => x.CarPosition == 1).FirstOrDefault();
                            if (firstItem != null)
                            {
                                var firstHistory = arrayHistory.Where(x => x?.CarIndex == firstItem.ArrayIndex).FirstOrDefault();
                                string s = this.CalculateDelta(arrayHistory, lapData, sessionData, curHistory, firstHistory, out fontColor);
                                if (s != null && s != "") curItem.LeaderIntervalTime = s;
                                firstItem = null;
                                firstHistory = null;
                            }
                        }
                    }
                }, DispatcherPriority.Background);
            }
        }

        private string CalculateDelta(
            PacketSessionHistoryData[] arrayHistory,
            PacketLapData lapDatas,
            PacketSessionData sessionData,
            PacketSessionHistoryData curHistory,
            PacketSessionHistoryData prevHistory,
            out Brush fontColor
        )
        {
            StringBuilder sb = new StringBuilder();
            fontColor = Brushes.White;

            if (
                arrayHistory != null &&
                lapDatas != null &&
                sessionData != null &&
                curHistory != null &&
                prevHistory != null
            )
            {
                switch (sessionData.SessionType)
                {
                    case SessionTypes.Race:
                    case SessionTypes.Race2:
                        {
                            int lap = curHistory.NumberOfLaps;
                            int sector = 0;

                            if (lap > 0)
                            {
                                if (curHistory.LapHistoryData[lap - 1].Sector1Time != TimeSpan.Zero) sector = 1;
                                else if (curHistory.LapHistoryData[lap - 1].Sector2Time != TimeSpan.Zero) sector = 2;
                                else if (curHistory.LapHistoryData[lap - 1].Sector3Time != TimeSpan.Zero) sector = 3;
                            }

                            var deltaTime = curHistory.GetTimeSum(lap, sector) - prevHistory.GetTimeSum(lap, sector);

                            var deltaLaps = (int)MathF.Round(lapDatas.Lapdata[prevHistory.CarIndex].TotalLapDistance - lapDatas.Lapdata[curHistory.CarIndex].TotalLapDistance) / sessionData.TrackLength;

                            if (deltaTime <= TimeSpan.FromSeconds(1)) fontColor = Brushes.Orange;
                            else if (deltaTime <= TimeSpan.FromSeconds(2)) fontColor = Brushes.Yellow;

                            if (deltaTime >= TimeSpan.Zero) sb.Append("+");
                            else sb.Append("-");

                            if (deltaLaps > 0) sb.Append((int)deltaTime.TotalSeconds + deltaTime.ToString(@"\.f") + "(+" + deltaLaps + " L)");
                            else sb.Append((int)deltaTime.TotalSeconds + deltaTime.ToString(@"\.fff"));
                        }
                        break;
                    default:
                        {
                            if (curHistory.BestLapTimeLapNumber > 0 && prevHistory.BestLapTimeLapNumber > 0)
                            {
                                var curLaptime = curHistory.LapHistoryData[curHistory.BestLapTimeLapNumber - 1].LapTime;
                                var prevLaptime = prevHistory.LapHistoryData[prevHistory.BestLapTimeLapNumber - 1].LapTime;

                                if (curLaptime != TimeSpan.Zero && prevLaptime != TimeSpan.Zero)
                                {
                                    var deltaTime = curLaptime - prevLaptime;

                                    if (deltaTime >= TimeSpan.Zero) sb.Append("+");
                                    else sb.Append("-");

                                    sb.Append((int)deltaTime.TotalSeconds + deltaTime.ToString(@"\.fff"));
                                }
                            }
                        }
                        break;
                }
            }

            return sb.ToString();
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
                   var sessionData = this.udp.LastSessionDataPacket;
                   //int noCarpos = 0;

                   foreach (PlayerListItemData elem in this.participantsList)
                   {
                       var current = lapData.Lapdata[elem.ArrayIndex];
                       float p = current.LapDistance / (float)sessionData.TrackLength * 100.0f;

                       elem.CurrentLapTime = current.CurrentLapTime;
                       elem.TrackLengthPercent = p;
                       elem.CarPosition = current.CarPosition;
                       elem.SetPitStatues(current.PitStatus, current.PitLaneTimer);
                       //if (current.Warnings > 0) Debug.WriteLine("HELLÓÓÓ");
                       elem.WarningNumber = current.Warnings;
                       elem.PenaltyTime = current.Penalties;

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
                    var lapData = this.udp.LastLapDataPacket;
                    var first = lapData?.Lapdata?.Where(x => x.CarPosition == 1).FirstOrDefault();

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("Session: " + Regex.Replace(sessionData.SessionType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim());
                    sb.AppendLine("Laps: " + first?.CurrentLapNum + " / " + sessionData.TotalLaps);
                    sb.Append("TimeLeft: " + sessionData.SessionTimeLeft.ToString());

                    this.textBlock_counterHead.Text = sb.ToString();

                    this.weatherController.SetActualWeather(sessionData.Weather, sessionData.SessionType, sessionData.TrackTemperature, sessionData.AirTemperature);
                    this.weatherController.SetWeatherForecast(sessionData.WeatherForcastSample);

                    this.isWorking_SessionData = false;
                }, DispatcherPriority.Background);
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

        private void listBox_drivers_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void PlayerList(int numberOfPlayers)
        {
            if (this.participantsList.Count == 0)
            {
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    var data = new PlayerListItemData();
                    data.ArrayIndex = i;
                    this.participantsList.Add(data);
                }

                //this.listBox_drivers.Items.IsLiveFiltering = true;
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
                    var demage = this.udp.LastCarDemagePacket?.CarDamageData[i];
                    var status = this.udp.LastCarStatusDataPacket?.CarStatusData[i];
                    var telemtry = this.udp.LastCarTelmetryPacket?.CarTelemetryData[i];

                    if (status != null)
                    {
                        this.ActualTyreCpompund = status.VisualTyreCompound;
                        this.LapAges = (int)status.TyresAgeLaps;
                    }

                    if (telemtry != null)
                    {
                        this.tyredata_fl.TyreInnerTemperature = telemtry.TyresInnerTemperature["FrontLeft"];
                        this.tyredata_fr.TyreInnerTemperature = telemtry.TyresInnerTemperature["FrontRight"];
                        this.tyredata_rl.TyreInnerTemperature = telemtry.TyresInnerTemperature["RearLeft"];
                        this.tyredata_rr.TyreInnerTemperature = telemtry.TyresInnerTemperature["RearRight"];

                        this.tyredata_fl.TyreSurfaceTemperature = telemtry.TyresSurfaceTemperature["FrontLeft"];
                        this.tyredata_fr.TyreSurfaceTemperature = telemtry.TyresInnerTemperature["FrontRight"];
                        this.tyredata_rl.TyreSurfaceTemperature = telemtry.TyresInnerTemperature["RearLeft"];
                        this.tyredata_rr.TyreSurfaceTemperature = telemtry.TyresInnerTemperature["RearRight"];

                        this.tyredata_fl.Pressure = telemtry.TyresPressure["FrontLeft"];
                        this.tyredata_fr.Pressure = telemtry.TyresPressure["FrontRight"];
                        this.tyredata_rl.Pressure = telemtry.TyresPressure["RearLeft"];
                        this.tyredata_rr.Pressure = telemtry.TyresPressure["RearRight"];

                        this.tyredata_fl.BrakesTemperature = telemtry.BrakesTemperature["FrontLeft"];
                        this.tyredata_fr.BrakesTemperature = telemtry.BrakesTemperature["FrontRight"];
                        this.tyredata_rl.BrakesTemperature = telemtry.BrakesTemperature["RearLeft"];
                        this.tyredata_rr.BrakesTemperature = telemtry.BrakesTemperature["RearRight"];
                    }

                    if (demage != null)
                    {
                        this.tyredata_fl.Wear = demage.TyreWear["FrontLeft"];
                        this.tyredata_fr.Wear = demage.TyreWear["FrontRight"];
                        this.tyredata_rl.Wear = demage.TyreWear["RearLeft"];
                        this.tyredata_rr.Wear = demage.TyreWear["RearRight"];

                        this.tyredata_fl.Demage = demage.TyreDemage["FrontLeft"];
                        this.tyredata_fr.Demage = demage.TyreDemage["FrontRight"];
                        this.tyredata_rl.Demage = demage.TyreDemage["RearLeft"];
                        this.tyredata_rr.Demage = demage.TyreDemage["RearRight"];

                        this.demage_fwLeft.Percent = 100.0 - demage.FrontLeftWingDemage;
                        this.demage_fwRight.Percent = 100.0 - demage.FrontRightWingDemage;
                        this.demage_fl.Percent = 100.0 - demage.FloorDemage;
                        this.demage_df.Percent = 100.0 - demage.DiffurerDemage;
                        this.demage_en.Percent = 100.0 - demage.EngineDemage;
                        this.demage_gb.Percent = 100.0 - demage.GearBoxDemage;
                        this.demage_rw.Percent = 100.0 - demage.RearWingDemage;
                        this.demage_sp.Percent = 100.0 - demage.SidepodDemage;

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

            if (this.ActualWidth < 950)
            {
                Grid.SetColumn(this.groupbox_wear, 0);
                Grid.SetColumn(this.groupbox_demage, 0);
                Grid.SetColumn(this.groupbox_motor, 0);

                Grid.SetColumnSpan(this.groupbox_wear, 12);
                Grid.SetColumnSpan(this.groupbox_demage, 12);
                Grid.SetColumnSpan(this.groupbox_motor, 12);

                Grid.SetRow(this.groupbox_wear, 0);
                Grid.SetRow(this.groupbox_demage, 1);
                Grid.SetRow(this.groupbox_motor, 2);

                Grid.SetColumn(this.wear_ce, 0);
                Grid.SetColumn(this.wear_es, 3);
                Grid.SetColumn(this.wear_ice, 0);
                Grid.SetColumn(this.wear_mguh, 3);
                Grid.SetColumn(this.wear_mguk, 0);
                Grid.SetColumn(this.wear_tc, 3);

                Grid.SetColumnSpan(this.wear_ce, 3);
                Grid.SetColumnSpan(this.wear_es, 3);
                Grid.SetColumnSpan(this.wear_ice, 3);
                Grid.SetColumnSpan(this.wear_mguh, 3);
                Grid.SetColumnSpan(this.wear_mguk, 3);
                Grid.SetColumnSpan(this.wear_tc, 3);

                Grid.SetRow(this.wear_ce, 0);
                Grid.SetRow(this.wear_es, 0);
                Grid.SetRow(this.wear_ice, 1);
                Grid.SetRow(this.wear_mguh, 1);
                Grid.SetRow(this.wear_mguk, 2);
                Grid.SetRow(this.wear_tc, 2);
            }
            else if (this.ActualWidth < 1400)
            {
                Grid.SetColumn(this.groupbox_wear, 0);
                Grid.SetColumn(this.groupbox_demage, 8);
                Grid.SetColumn(this.groupbox_motor, 0);

                Grid.SetColumnSpan(this.groupbox_wear, 8);
                Grid.SetColumnSpan(this.groupbox_demage, 4);
                Grid.SetColumnSpan(this.groupbox_motor, 12);

                Grid.SetRow(this.groupbox_wear, 0);
                Grid.SetRow(this.groupbox_demage, 0);
                Grid.SetRow(this.groupbox_motor, 1);

                Grid.SetColumn(this.wear_ce, 0);
                Grid.SetColumn(this.wear_es, 1);
                Grid.SetColumn(this.wear_ice, 2);
                Grid.SetColumn(this.wear_mguh, 3);
                Grid.SetColumn(this.wear_mguk, 4);
                Grid.SetColumn(this.wear_tc, 5);

                Grid.SetColumnSpan(this.wear_ce, 1);
                Grid.SetColumnSpan(this.wear_es, 1);
                Grid.SetColumnSpan(this.wear_ice, 1);
                Grid.SetColumnSpan(this.wear_mguh, 1);
                Grid.SetColumnSpan(this.wear_mguk, 1);
                Grid.SetColumnSpan(this.wear_tc, 1);

                Grid.SetRow(this.wear_ce, 0);
                Grid.SetRow(this.wear_es, 0);
                Grid.SetRow(this.wear_ice, 0);
                Grid.SetRow(this.wear_mguh, 0);
                Grid.SetRow(this.wear_mguk, 0);
                Grid.SetRow(this.wear_tc, 0);
            }
            else if (this.ActualWidth < 1800)
            {
                Grid.SetColumn(this.groupbox_wear, 0);
                Grid.SetColumn(this.groupbox_demage, 6);
                Grid.SetColumn(this.groupbox_motor, 0);

                Grid.SetColumnSpan(this.groupbox_wear, 6);
                Grid.SetColumnSpan(this.groupbox_demage, 6);
                Grid.SetColumnSpan(this.groupbox_motor, 12);

                Grid.SetRow(this.groupbox_wear, 0);
                Grid.SetRow(this.groupbox_demage, 0);
                Grid.SetRow(this.groupbox_motor, 1);

                Grid.SetColumn(this.wear_ce, 0);
                Grid.SetColumn(this.wear_es, 1);
                Grid.SetColumn(this.wear_ice, 2);
                Grid.SetColumn(this.wear_mguh, 3);
                Grid.SetColumn(this.wear_mguk, 4);
                Grid.SetColumn(this.wear_tc, 5);

                Grid.SetColumnSpan(this.wear_ce, 1);
                Grid.SetColumnSpan(this.wear_es, 1);
                Grid.SetColumnSpan(this.wear_ice, 1);
                Grid.SetColumnSpan(this.wear_mguh, 1);
                Grid.SetColumnSpan(this.wear_mguk, 1);
                Grid.SetColumnSpan(this.wear_tc, 1);

                Grid.SetRow(this.wear_ce, 0);
                Grid.SetRow(this.wear_es, 0);
                Grid.SetRow(this.wear_ice, 0);
                Grid.SetRow(this.wear_mguh, 0);
                Grid.SetRow(this.wear_mguk, 0);
                Grid.SetRow(this.wear_tc, 0);
            }
            else
            {
                Grid.SetColumn(this.groupbox_wear, 0);
                Grid.SetColumn(this.groupbox_demage, 5);
                Grid.SetColumn(this.groupbox_motor, 9);

                Grid.SetColumnSpan(this.groupbox_wear, 5);
                Grid.SetColumnSpan(this.groupbox_demage, 4);
                Grid.SetColumnSpan(this.groupbox_motor, 3);

                Grid.SetRow(this.groupbox_wear, 0);
                Grid.SetRow(this.groupbox_demage, 0);
                Grid.SetRow(this.groupbox_motor, 0);

                Grid.SetColumn(this.wear_ce, 0);
                Grid.SetColumn(this.wear_es, 3);
                Grid.SetColumn(this.wear_ice, 0);
                Grid.SetColumn(this.wear_mguh, 3);
                Grid.SetColumn(this.wear_mguk, 0);
                Grid.SetColumn(this.wear_tc, 3);

                Grid.SetColumnSpan(this.wear_ce, 3);
                Grid.SetColumnSpan(this.wear_es, 3);
                Grid.SetColumnSpan(this.wear_ice, 3);
                Grid.SetColumnSpan(this.wear_mguh, 3);
                Grid.SetColumnSpan(this.wear_mguk, 3);
                Grid.SetColumnSpan(this.wear_tc, 3);

                Grid.SetRow(this.wear_ce, 0);
                Grid.SetRow(this.wear_es, 0);
                Grid.SetRow(this.wear_ice, 1);
                Grid.SetRow(this.wear_mguh, 1);
                Grid.SetRow(this.wear_mguk, 2);
                Grid.SetRow(this.wear_tc, 2);
            }

            this.lockTimer.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.udp.Close();
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
    }
}
