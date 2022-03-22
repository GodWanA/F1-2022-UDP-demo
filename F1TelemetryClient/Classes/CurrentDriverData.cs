using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Classes
{
    public class CurrentDriverData : INotifyPropertyChanged, IComparable<CurrentDriverData>, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _carPosition;
        public int CarPosition
        {
            get { return _carPosition; }
            set
            {
                _carPosition = value;
                this.OnPropertyChanged("CarPosition");
            }
        }

        private DriverSatuses _driverStatus;
        public DriverSatuses DriverStatus
        {
            get { return _driverStatus; }
            set
            {
                _driverStatus = value;
                this.OnPropertyChanged("DriverStatus");
            }
        }

        private TimeSpan _currentLapTime;
        public TimeSpan CurrentLapTime
        {
            get { return _currentLapTime; }
            set
            {
                _currentLapTime = value;
                this.FormattedTime = _currentLapTime.ToString(@"mm\:ss\.fff");
                this.OnPropertyChanged("CurrentLapTime");
            }
        }

        private string _formattedTime;
        public string FormattedTime
        {
            get { return _formattedTime; }
            set
            {
                _formattedTime = value;
                this.OnPropertyChanged("FormattedTime");
            }
        }

        private Drivers _driverID;
        public Drivers DriverID
        {
            get
            {
                return this._driverID;
            }
            set
            {
                this._driverID = value;
                this.DriverName = Regex.Replace(this.DriverID.ToString(), "[A-Z]", m => " " + m.ToString());
                this.OnPropertyChanged("DriverID");
            }
        }

        private string _driverName;
        public string DriverName
        {
            get
            {
                return this._driverName;
            }
            set
            {
                this._driverName = value;
                this.OnPropertyChanged("DriverName");
            }
        }

        private bool _isAI;
        public bool IsAI
        {
            get
            {
                return this._isAI;
            }
            set
            {
                this._isAI = value;
                this.OnPropertyChanged("IsAI");
            }
        }

        private bool _isHuman;
        public bool IsHuman
        {
            get
            {
                return this._isHuman;
            }
            set
            {
                this._isHuman = value;
                this.OnPropertyChanged("IsHuman");
            }
        }


        private bool _isMyTeam;
        public bool IsMyTeam
        {
            get
            {
                return this._isMyTeam;
            }
            set
            {
                this._isMyTeam = value;
                if (this._isMyTeam) this._teamColor = new SolidColorBrush(Color.FromRgb(255, 0, 255));
                this.OnPropertyChanged("IsMyTeam");
            }
        }

        private string _pilotName;
        public string PilotName
        {
            get
            {
                return this._pilotName;
            }
            set
            {
                this._pilotName = value;
                this.OnPropertyChanged("PilotName");
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
                this._nationality = value;

                var img = new BitmapImage(new Uri("Images/Flags/" + this._nationality + ".png", UriKind.Relative));
                if (img.CanFreeze) img.Freeze();
                this.ImageSource = img;

                this.OnPropertyChanged("Nationality");
            }
        }

        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get
            {
                return this._imageSource;
            }
            set
            {
                this._imageSource = value;
                this.OnPropertyChanged("ImageSource");
            }
        }

        private int _networkID;
        public int NetworkID
        {
            get
            {
                return this._networkID;
            }
            set
            {
                this._networkID = value;
                this.OnPropertyChanged("NetworkID");
            }
        }

        private int _raceNumber;
        public int RaceNumber
        {
            get
            {
                return this._raceNumber;
            }
            set
            {
                this._raceNumber = value;
                this.OnPropertyChanged("RaceNumber");
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
                this._teamID = value;
                switch (this._teamID)
                {
                    default:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(255, 0, 255));
                        break;
                    case Teams.Mercedes:
                    case Teams.Mercedes2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(0, 210, 90));
                        break;
                    case Teams.Ferrari:
                    case Teams.Ferrari2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(220, 0, 0));
                        break;
                    case Teams.RedBullRacing:
                    case Teams.RedBull2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(6, 0, 239));
                        break;
                    case Teams.Alpine:
                    case Teams.Renault2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(0, 144, 255));
                        break;
                    case Teams.Haas:
                    case Teams.Haas2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                        break;
                    case Teams.AstonMartin:
                    case Teams.RacingPoint2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(0, 111, 98));
                        break;
                    case Teams.AlphaTauri:
                    case Teams.AlphaTauri2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(43, 69, 98));
                        break;
                    case Teams.McLaren:
                    case Teams.McLaren2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(255, 135, 0));
                        break;
                    case Teams.AlfaRomeo:
                    case Teams.AlfaRomeo2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(144, 0, 0));
                        break;
                    case Teams.Williams:
                    case Teams.Williams2020:
                        this.TeamColor = new SolidColorBrush(Color.FromRgb(0, 90, 255));
                        break;
                }
                this.OnPropertyChanged("TeamID");
            }
        }

        private double _trackLengthPercent;
        public double TrackLengthPercent
        {
            get
            {
                return this._trackLengthPercent;
            }
            set
            {
                this._trackLengthPercent = value;
                this.OnPropertyChanged("TrackLengthPercent");
            }
        }

        public SolidColorBrush _teamColor;
        private bool disposedValue;

        public SolidColorBrush TeamColor
        {
            get
            {
                return this._teamColor;
            }
            set
            {
                this._teamColor = value;
                this.OnPropertyChanged("TeamColor");
            }
        }

        public int CurrentLap { get; internal set; }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(CurrentDriverData other)
        {
            if (this.CarPosition == 0 && other.CarPosition == 0) return 0;
            if (this.CarPosition == 0) return 1;
            if (other.CarPosition == 0) return -1;

            if (this.CarPosition < other.CarPosition) return -1;
            if (this.CarPosition > other.CarPosition) return 1;

            return 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.PropertyChanged = null;
                    this.DriverName = null;
                    this.FormattedTime = null;
                    this.ImageSource = null;
                    this.PilotName = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CurrentDriverData()
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
