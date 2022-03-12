using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for PlayerListItemData.xaml
    /// </summary>
    public partial class PlayerListItemData : UserControl, INotifyPropertyChanged, IComparable<PlayerListItemData>, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string driverName;
        public static bool PosChange { get; set; }

        public string DriverName
        {
            get { return driverName; }
            set
            {
                if (driverName != value)
                {
                    driverName = value;
                    //this.textblock_driver.Text = this.driverName;
                    this.OnPropertyChanged("DriverName");
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
                if (TextColor != value)
                {
                    textColor = value;
                    //this.textblock_interval.Foreground = this.textColor;
                    this.OnPropertyChanged("TextColor");
                }
            }
        }

        private int carPosition;
        public int CarPosition
        {
            get { return carPosition; }
            set
            {
                if (CarPosition != value)
                {
                    carPosition = value;
                    //this.label_carPosition.Content = this.carPosition.ToString();
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
                if (this.ArrayIndex != value)
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
                if (this.IsMyTeam != value)
                {
                    this.isMyTeam = value;
                    if (this.isMyTeam) this.TeamColor = new SolidColorBrush(Color.FromRgb(255, 0, 255));
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
                if (this._nationality != value)
                {
                    this._nationality = value;
                    this.image_nation.Source = u.NationalityImage(this._nationality);
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
                if (this.RaceNumber != value)
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
                if (this.TeamID != value)
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
                    this.image_tyre.Source = u.TyreCompoundToImage(this.tyreCompund);
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
                if (this.isSelected != value)
                {
                    isSelected = value;
                    if (isSelected)
                    {
                        this.grid_header.Background = new SolidColorBrush(Color.FromRgb(160, 160, 160));
                    }
                    else
                    {
                        this.grid_header.Background = Brushes.Black;
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

        private TimeSpan penaltyTime;

        public TimeSpan PenaltyTime
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
                            this.border_position.Opacity = 0.5;
                            this.IsOut = true;
                            break;
                        default:
                            this.border_position.Opacity = 1;
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
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
    }
}
