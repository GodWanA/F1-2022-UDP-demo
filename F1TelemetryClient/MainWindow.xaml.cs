using F1Telemetry;
using F1TelemetryApp.UserControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace F1TelemetryClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private F1UDP udp;
        private DispatcherTimer cleaner;

        private string actualTyreCpompund;

        public string ActualTyreCpompund
        {
            get { return actualTyreCpompund; }
            set
            {
                actualTyreCpompund = value;
                this.OnPropertyChanged("ActualTyreCpompund");
            }
        }

        private int lapAges;
        public int LapAges
        {
            get { return lapAges; }
            set
            {
                lapAges = value;
                this.OnPropertyChanged("LapAges");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            this.cleaner = new DispatcherTimer();
            this.cleaner.Interval = TimeSpan.FromSeconds(30);
            this.cleaner.Tick += this.Cleaner_Tick;
            this.cleaner.Start();

            this.udp = new F1UDP("127.0.0.1", 20777);
            this.udp.SessionPacket += this.Upp_SessionPacket;
            this.udp.LapDataPacket += this.Udp_LapDataPacket;
            this.udp.ParticipantsPacket += this.Udp_ParticipantsPacket;
            this.udp.DemagePacket += Udp_DemagePacket;
            this.udp.CarStatusPacket += Udp_CarStatusPacket;
            this.udp.CarTelemetryPacket += Udp_CarTelemetryPacket;

            this.listBox_drivers.Items.IsLiveSorting = true;
            this.listBox_drivers.Items.SortDescriptions.Add(new SortDescription("CarPosition", ListSortDirection.Ascending));
        }

        private void Udp_CarTelemetryPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LoadWear();
            });
        }

        private void Udp_CarStatusPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LoadWear();

                var items = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();
                if (items != null)
                {
                    var status = this.udp.LastCarStatusDataPacket;

                    for (int i = 0; i < status.CarStatusData.Length; i++)
                    {
                        var current = status.CarStatusData[i];
                        var elem = items.Where(x => x.ArrayIndex == i).FirstOrDefault();

                        if (elem != null)
                        {
                            elem.TyreCompund = current.ActualTyreCompound;
                            elem.VisualTyreCompund = current.ViszalTyreCompound;
                        }
                    }
                }
            });
        }

        private void Udp_DemagePacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.LoadWear();
            });
        }

        private void Udp_ParticipantsPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var participants = this.udp.LastParticipantsPacket;
                this.PlyerList(participants.Participants.Length);
                var items = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();

                if (items != null)
                {

                    for (int i = 0; i < participants.Participants.Length; i++)
                    {
                        var current = participants.Participants[i];
                        var elem = items.Where(x => x.ArrayIndex == i).FirstOrDefault();
                        if (elem != null)
                        {
                            elem.DriverID = current.DriverID;
                            elem.Name = current.Name;
                            elem.Nationality = current.Nationality;
                            elem.TeamID = current.TeamID;
                            elem.IsAI = current.IsAIControlled;
                            elem.IsMyTeam = current.IsMyTeam;
                            elem.IsHuman = !current.IsAIControlled;
                            elem.RaceNumber = current.RaceNumber;
                        }
                    }
                }
            });
        }

        private void Udp_LapDataPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var lapData = this.udp.LastLapDataPacket;
                var sessionData = this.udp.LastSessionDataPacket;
                var items = this.listBox_drivers.ItemsSource?.Cast<PlayerListItemData>();

                if (items != null)
                {
                    for (int i = 0; i < lapData.Lapdata.Length; i++)
                    {
                        var current = lapData.Lapdata[i];
                        var elem = items.Where(x => x.ArrayIndex == i).FirstOrDefault();
                        if (elem != null)
                        {
                            elem.CurrentLapTime = current.CurrentLapTime;
                            elem.TrackLengthPercent = current.LapDistance / sessionData.TrackLength * 100.0;
                            elem.CarPosition = current.CarPosition;

                            if (elem.CarPosition == 0) elem.Visibility = Visibility.Collapsed;
                            else elem.Visibility = Visibility.Visible;
                        }
                    }
                }
            });
        }

        private void Cleaner_Tick(object sender, EventArgs e)
        {
            this.cleaner.Stop();

            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCApproach();
            GC.WaitForFullGCComplete();
            GC.Collect();

            this.cleaner.Start();
        }

        private void Upp_SessionPacket(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var sessionData = this.udp.LastSessionDataPacket;
                var lapData = this.udp.LastLapDataPacket;
                var first = lapData?.Lapdata?.Where(x => x.CarPosition == 1).FirstOrDefault();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Session: " + Regex.Replace(sessionData.SessionType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim());
                sb.AppendLine("Laps: " + first?.CurrentLapNum + " / " + sessionData.TotalLaps);
                sb.Append("TimeLeft: " + sessionData.SessionTimeLeft.ToString());

                this.textBlock_counterHead.Text = sb.ToString();
            });
        }

        private void listBox_drivers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.listBox_drivers.SelectedIndex > -1)
                {
                    this.LoadWear();
                }
            }
        }

        private void listBox_drivers_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void PlyerList(int numberOfPlayers)
        {
            if (this.listBox_drivers.Items.Count == 0)
            {
                var tmp = new ObservableCollection<PlayerListItemData>();

                for (int i = 0; i < numberOfPlayers; i++)
                {
                    var data = new PlayerListItemData();
                    data.ArrayIndex = i;
                    tmp.Add(data);
                }

                this.listBox_drivers.ItemsSource = tmp;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadWear()
        {
            if (this.listBox_drivers.SelectedIndex != -1)
            {
                int i = (this.listBox_drivers.SelectedItem as PlayerListItemData).ArrayIndex;
                var demage = this.udp.LastCarDemagePacket.CarDamageData[i];
                var status = this.udp.LastCarStatusDataPacket.CarStatusData[i];
                var telemtry = this.udp.LastCartelmetryPacket.CarTelemetryData[i];

                if (status != null)
                {
                    this.ActualTyreCpompund = status.ViszalTyreCompound.ToString();
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

                    this.wear_ce.Percent = 100.0 - demage.EngineICEWear;
                    this.wear_es.Percent = 100.0 - demage.EngineESWear;
                    this.wear_ice.Percent = 100.0 - demage.EngineICEWear;
                    this.wear_mguh.Percent = 100.0 - demage.EngineMGUKWear;
                    this.wear_mguk.Percent = 100.0 - demage.EngineMGUKWear;
                    this.wear_tc.Percent = 100.0 - demage.EngineTCWear;
                }
            }
        }
    }
}
