﻿using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.CarTelemetryPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for PlayerListItemData.xaml
    /// </summary>
    public partial class PlayerListItemData : UserControl, INotifyPropertyChanged, IComparable<PlayerListItemData>, IDisposable, IConnectUDP
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static bool PosChange { get; set; }

        private string driverName;

        public string DriverName
        {
            get { return driverName; }
            set
            {
                if (value != driverName)
                {
                    driverName = value;
                    //this.textblock_driver.Text = this.driverName;
                    this.OnPropertyChanged("DriverName");
                }
            }
        }

        private string driverCode;

        public string DriverCode
        {
            get { return driverCode; }
            set
            {
                if (value != driverCode)
                {
                    driverCode = value;
                    //this.textblock_driver.Text = this.driverName;
                    this.OnPropertyChanged("DriverCode");
                }
            }
        }

        private TimeSpan? currentLapTime;

        public TimeSpan? CurrentLapTime
        {
            get { return currentLapTime; }
            set
            {
                if (value != currentLapTime)
                {
                    currentLapTime = value;
                    //this.textblock_laptime.Text = this.currentLapTime?.ToString(@"m\:ss\.fff");
                    this.OnPropertyChanged("CurrentLapTime");
                }
            }
        }

        private string leaderIntervalTime;

        public string LeaderIntervalTime
        {
            get { return leaderIntervalTime; }
            set
            {
                if (value != leaderIntervalTime)
                {
                    leaderIntervalTime = value;
                    //this.textblock_leaderInterval.Text = this.leaderIntervalTime;
                    this.OnPropertyChanged("LeaderIntervalTime");
                }
            }
        }

        private double tyreWearAvarege;

        public double TyreWearAvarege
        {
            get { return tyreWearAvarege; }
            set
            {
                if (value != tyreWearAvarege)
                {
                    tyreWearAvarege = value;
                    //this.textblock_leaderInterval.Text = this.leaderIntervalTime;
                    this.OnPropertyChanged("TyreWearAvarege");
                }
            }
        }

        private string intervalTime;

        public string IntervalTime
        {
            get { return intervalTime; }
            set
            {
                if (value != intervalTime)
                {
                    intervalTime = value;
                    //this.textblock_interval.Text = this.intervalTime;
                    this.OnPropertyChanged("IntervalTime");
                }
            }
        }

        private Brush textColor;
        public Brush TextColor
        {
            get { return textColor; }
            set
            {
                if (value != TextColor)
                {
                    textColor = value;
                    //this.textblock_interval.Foreground = this.textColor;
                    this.OnPropertyChanged("TextColor");
                }
            }
        }

        private int carPosition = -1;
        public int CarPosition
        {
            get { return carPosition; }
            set
            {
                if (value != carPosition)
                {

                    if (value == 0)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.Visibility = System.Windows.Visibility.Collapsed;
                            this.IsEnabled = false;
                        });
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.Visibility = System.Windows.Visibility.Visible;
                            this.IsEnabled = true;

                            if (this.IsLoaded && this.carPosition > 0)
                            {
                                Storyboard sb;
                                if (carPosition > value) sb = this.Resources["PosDec"] as Storyboard;
                                else sb = this.Resources["PosInc"] as Storyboard;
                                if (sb != null) sb.Begin();
                            }
                        });
                    }

                    carPosition = value;

                    PlayerListItemData.PosChange = true;
                    this.OnPropertyChanged("CarPosition");
                }
            }
        }

        private int arrayIndex;
        public int ArrayIndex
        {
            get { return arrayIndex; }
            set
            {
                if (value != this.ArrayIndex)
                {
                    arrayIndex = value;
                    //this.OnPropertyChanged("CarPosition");
                }
            }
        }

        private bool isMyTeam;
        public bool IsMyTeam
        {
            get
            {
                return this.isMyTeam;
            }
            set
            {
                if (value != this.IsMyTeam)
                {
                    this.isMyTeam = value;
                    if (this.isMyTeam)
                    {
                        var c = new SolidColorBrush(Color.FromRgb(255, 0, 255));
                        if (c.CanFreeze) c.Freeze();
                        this.TeamColor = c;
                    }
                    //this.OnPropertyChanged("IsMyTeam");
                }
            }
        }

        private Nationalities _nationality;
        public Nationalities Nationality
        {
            get
            {
                return this._nationality;
            }
            set
            {
                if (value != this._nationality)
                {
                    this._nationality = value;
                    this.Dispatcher.Invoke(() => this.image_nation.Source = u.NationalityImage(this._nationality));
                    //this.OnPropertyChanged("Nationality");
                }
            }
        }

        private int raceNumber;
        public int RaceNumber
        {
            get
            {
                return this.raceNumber;
            }
            set
            {
                if (value != this.RaceNumber)
                {
                    this.raceNumber = value;
                    //this.label_raceNumber.Content = this.raceNumber;
                    this.OnPropertyChanged("RaceNumber");
                }
            }
        }

        private Teams _teamID;
        public Teams TeamID
        {
            get
            {
                return this._teamID;
            }
            set
            {
                if (value != this.TeamID)
                {
                    this._teamID = value;
                    this.TeamColor = u.PickTeamColor(this.TeamID);

                    //this.OnPropertyChanged("TeamID");
                }
            }
        }

        private double trackLengthPercent;
        public double TrackLengthPercent
        {
            get
            {
                return this.trackLengthPercent;
            }
            set
            {
                if (value != TrackLengthPercent)
                {
                    this.trackLengthPercent = value;
                    //this.progressBar_Lap.Value = this.trackLengthPercent;
                    this.OnPropertyChanged("TrackLengthPercent");
                }
            }
        }

        public SolidColorBrush teamColor;
        public SolidColorBrush TeamColor
        {
            get
            {
                return this.teamColor;
            }
            set
            {
                if (value != this.TeamColor)
                {
                    this.teamColor = value;
                    //this.label_raceNumber.Foreground = this.teamColor;
                    //this.rectangle_teamColor.Fill = this.teamColor;
                    this.OnPropertyChanged("TeamColor");
                }
            }
        }

        public int CurrentLap { get; internal set; }

        private TyreCompounds tyreCompund;
        private bool disposedValue;

        public TyreCompounds TyreCompund
        {
            get
            {
                return this.tyreCompund;
            }
            set
            {
                if (value != this.TyreCompund)
                {
                    this.tyreCompund = value;
                    this.Dispatcher.Invoke(() => this.image_tyre.Source = u.TyreCompoundToImage(this.tyreCompund));
                    //this.OnPropertyChanged("TyreCompund");
                }
            }
        }

        private Brush timerForeground;

        public Brush TimerForeground
        {
            get { return timerForeground; }
            set
            {
                if (value != this.timerForeground)
                {
                    timerForeground = value;
                    this.OnPropertyChanged("TimerForeground");
                }
            }
        }

        private Brush trackPercentForeground;

        public Brush TrackPercentForeground
        {
            get { return trackPercentForeground; }
            set
            {
                if (value != this.trackPercentForeground)
                {
                    trackPercentForeground = value;
                    this.OnPropertyChanged("TrackPercentForeground");
                }
            }
        }

        private string pitText;

        public string PitText
        {
            get { return pitText; }
            set
            {
                if (value != this.pitText)
                {
                    pitText = value;
                    this.OnPropertyChanged("PitText");
                }
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != this.isSelected)
                {
                    isSelected = value;
                    if (isSelected)
                    {
                        // this.grid_header.Background = new SolidColorBrush(Color.FromRgb(130, 130, 130));
                        this.Dispatcher.Invoke(() => this.grid_header.Background = new SolidColorBrush(Color.FromRgb(130, 130, 130)));
                    }
                    else
                    {
                        // this.grid_header.Background = Brushes.Black;
                        this.Dispatcher.Invoke(() => this.grid_header.Background = Brushes.Black);
                    }
                }
            }
        }

        private int warningNumber;

        public int WarningNumber
        {
            get { return warningNumber; }
            set
            {
                if (value != this.warningNumber)
                {
                    warningNumber = value;
                    this.OnPropertyChanged("WarningNumber");
                }
            }
        }

        private TimeSpan? penaltyTime;

        public TimeSpan? PenaltyTime
        {
            get { return penaltyTime; }
            set
            {
                if (value != this.penaltyTime)
                {
                    penaltyTime = value;
                    this.OnPropertyChanged("PenaltyTime");
                }
            }
        }

        private ResultSatuses resultSatuses;
        private bool isLapdata;
        private bool isParticipants;
        private bool isStatus;
        private bool isTelemetry;
        private bool isCarDemage;

        public ResultSatuses ResultStatus
        {
            get { return resultSatuses; }
            set
            {
                if (value != this.resultSatuses)
                {
                    resultSatuses = value;

                    switch (resultSatuses)
                    {
                        case ResultSatuses.Disqualified:
                        case ResultSatuses.NotClassiFied:
                        case ResultSatuses.DidNotFinish:
                        case ResultSatuses.Retired:
                            this.Dispatcher.Invoke(() => this.border_position.Opacity = 0.5);
                            // this.border_position.Opacity = 0.5;
                            this.IsOut = true;
                            break;
                        default:
                            this.Dispatcher.Invoke(() => this.border_position.Opacity = 1);
                            // this.border_position.Opacity = 1;
                            this.IsOut = false;
                            break;
                    }
                    //this.OnPropertyChanged("ResultStatus");
                }
            }
        }

        internal bool IsOut { get; private set; }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
            // if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(PlayerListItemData other)
        {
            if (this.CarPosition == 0 && other.CarPosition == 0) return 0;
            if (this.CarPosition == 0) return 1;
            if (other.CarPosition == 0) return -1;

            if (this.CarPosition < other.CarPosition) return -1;
            if (this.CarPosition > other.CarPosition) return 1;

            return 0;
        }

        public PlayerListItemData()
        {
            InitializeComponent();
            DataContext = this;
            this.TeamID = Teams.MyTeam;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.CurrentLapTime = null;
                    this.DriverName = null;
                    this.TeamColor = null;
                    this.TextColor = null;
                    this.TimerForeground = null;
                    this.IntervalTime = null;
                    this.LeaderIntervalTime = null;
                    this.PropertyChanged = null;
                    this.TrackPercentForeground = null;
                    this.TimerForeground = null;
                    this.UnsubscribeUDPEvents();
                }

                this.currentLapTime = null;
                this.driverName = null;
                this.teamColor = null;
                this.textColor = null;
                this.timerForeground = null;
                this.intervalTime = null;
                this.leaderIntervalTime = null;
                this.trackPercentForeground = null;
                this.timerForeground = null;

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PlayerListItemData()
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

        internal void SetPitStatues(PitStatuses pitStatus, TimeSpan pitLaneTimer)
        {
            if (pitStatus == PitStatuses.InPitArea || pitStatus == PitStatuses.Pitting)
            {
                this.textblock_pittime.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.textblock_pittime.Visibility = System.Windows.Visibility.Hidden;
            }

            if (pitLaneTimer != TimeSpan.Zero) this.PitText = pitLaneTimer.ToString(@"s\:ff");
            else this.PitText = "PIT";
        }

        internal void setStateText()
        {
            switch (this.resultSatuses)
            {
                case ResultSatuses.Disqualified:
                    this.LeaderIntervalTime = "DSQ";
                    break;
                case ResultSatuses.DidNotFinish:
                    this.LeaderIntervalTime = "DNF";
                    break;
                case ResultSatuses.NotClassiFied:
                    this.LeaderIntervalTime = "NCF";
                    break;
                case ResultSatuses.Retired:
                    this.LeaderIntervalTime = "Out";
                    break;
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        private void Connention_CarStatusPacket(PacketCarStatusData packet, EventArgs e)
        {
            if (packet != null && !this.isStatus)
            {
                this.isStatus = true;
                var current = packet.CarStatusData[this.ArrayIndex];
                //this.Dispatcher.Invoke(() => this.TyreCompund = current.VisualTyreCompound);
                this.TyreCompund = current.VisualTyreCompound;

                //// this.UpdateLayout();
                this.isStatus = false;
            }
        }

        private void Connention_ParticipantsPacket(PacketParticipantsData packet, EventArgs e)
        {
            if (packet != null && !this.isParticipants)
            {
                this.isParticipants = true;

                //this.Dispatcher.Invoke(() =>
                //{
                if (this.ArrayIndex > -1 && this.ArrayIndex < packet.Participants.Length)
                {
                    var current = packet.Participants[this.ArrayIndex];
                    this.DriverName = current.ParticipantName;
                    this.DriverCode = current.ShortName;
                    this.Nationality = current.Nationality;
                    this.TeamID = current.TeamID;
                    this.IsMyTeam = current.IsMyTeam;
                    this.RaceNumber = current.RaceNumber;
                }

                //// this.UpdateLayout();
                this.isParticipants = false;

                //if (this.CarPosition == 0) elem.Visibility = Visibility.Collapsed;
                //else this.Visibility = Visibility.Visible;
                //}, DispatcherPriority.Render);
            }
        }

        private void Connention_LapDataPacket(PacketLapData packet, EventArgs e)
        {
            if (packet != null && !this.isLapdata)
            {
                this.isLapdata = true;

                if (this.arrayIndex < packet.Lapdata.Length)
                {
                    var current = packet.Lapdata[this.ArrayIndex];
                    this.Dispatcher.Invoke(() => this.CarPosition = current.CarPosition);

                    if (current.CarPosition > 0)
                    {
                        float p = (float)Math.Round(current.LapDistance / u.TrackLength * 100.0, 2);

                        this.Dispatcher.Invoke(() =>
                        {
                            this.CurrentLapTime = current.CurrentLapTime;
                            this.TrackLengthPercent = p;
                            this.WarningNumber = current.Warnings;

                            if (current.Penalties > TimeSpan.Zero) this.PenaltyTime = current.Penalties;
                            else this.PenaltyTime = null;

                            this.ResultStatus = current.ResultStatus;
                            this.SetPitStatues(current.PitStatus, current.PitStopTimer);
                        });

                        if (current.IsCurrentLapInvalid)
                        {
                            //this.Dispatcher.Invoke(() =>
                            //{
                            this.TimerForeground = Brushes.Red;
                            this.TrackPercentForeground = Brushes.Red;
                            //});
                        }
                        else
                        {
                            //this.Dispatcher.Invoke(() =>
                            //{
                            this.TimerForeground = Brushes.White;
                            this.TrackPercentForeground = Brushes.LimeGreen;
                            //});
                        }
                    }
                }
                //// this.UpdateLayout();
                this.isLapdata = false;
            }
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.LapDataPacket += Connention_LapDataPacket;
            u.Connention.ParticipantsPacket += Connention_ParticipantsPacket;
            u.Connention.CarStatusPacket += Connention_CarStatusPacket;
            u.Connention.CarDemagePacket += Connention_CarDemagePacket;
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.LapDataPacket -= Connention_LapDataPacket;
            u.Connention.ParticipantsPacket -= Connention_ParticipantsPacket;
            u.Connention.CarStatusPacket -= Connention_CarStatusPacket;
            u.Connention.CarDemagePacket -= Connention_CarDemagePacket;
        }

        private void Connention_CarDemagePacket(F1Telemetry.Models.CarDamagePacket.PacketCarDamageData packet, EventArgs e)
        {
            if (packet != null && !this.isCarDemage)
            {
                this.isCarDemage = true;

                var driverTyreWear = packet.CarDamageData[this.arrayIndex].TyreWear;
                var sum = 0.0;

                foreach (var item in driverTyreWear) sum += 100 - item.Value;
                this.TyreWearAvarege = sum / 4.0;
                this.isCarDemage = false;
            }
        }

        private void UserControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var sb = new StringBuilder();

            var lap = u.Connention?.CurrentLapDataPacket?.Lapdata[this.arrayIndex];

            sb.AppendLine("Driver name: " + this.DriverName + " | " + this.DriverCode);
            sb.AppendLine("Racenumber: " + this.RaceNumber);
            sb.AppendLine("Nationality: " + this.Nationality);
            sb.AppendLine("Team: " + this.TeamID);
            sb.AppendLine("Tyre: " + this.TyreCompund);
            sb.AppendLine("Lap number: " + lap?.CurrentLapNum);
            sb.AppendLine("Current status: " + lap?.DriverStatus);
            sb.AppendLine("Result status: " + lap?.ResultStatus);
            sb.AppendLine("Pit status: " + lap?.PitStatus);
            sb.AppendLine("Current position: " + this.CarPosition);
            sb.AppendLine("Grid Position: " + lap?.GridPosition);
            sb.AppendLine("Warnings: " + this.WarningNumber);
            sb.AppendLine("Penalty time: " + this.PenaltyTime + " sec");
            sb.AppendLine("Driver through: " + lap?.NumberOfUnservedDriveThroughPenalties);
            sb.AppendLine("Stop'n'Go :" + lap?.NumberOfUnservedStopGoPenalties);

            this.ToolTip = sb.ToString();
        }
    }
}
