using System;
using System.ComponentModel;
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

        private int _carPosition;
        public int CarPosition
        {
            get { return _carPosition; }
            set
            {
                if (CarPosition != value)
                {
                    _carPosition = value;
                    this.OnPropertyChanged("CarPosition");
                }
            }
        }

        private int _arrayIndex;
        public int ArrayIndex
        {
            get { return _arrayIndex; }
            set
            {
                if (this.ArrayIndex != value)
                {
                    _arrayIndex = value;
                    this.OnPropertyChanged("CarPosition");
                }
            }
        }

        private DriverSatuses _driverStatus;
        public DriverSatuses DriverStatus
        {
            get { return _driverStatus; }
            set
            {
                if (DriverStatus != value)
                {
                    _driverStatus = value;

                    switch (this._driverStatus)
                    {
                        default:
                            this.Opacity = 1.0;
                            break;
                        case DriverSatuses.InGarage:
                        case DriverSatuses.Unknown:
                            this.Opacity = 0.5;
                            break;
                    }

                    this.OnPropertyChanged("DriverStatus");
                }
            }
        }

        private ResultSatuses _resultStatus;
        public ResultSatuses ResultStatus
        {
            get { return _resultStatus; }
            set
            {
                if (ResultStatus != value)
                {
                    _resultStatus = value;

                    switch (this._resultStatus)
                    {
                        default:
                            this.Opacity = 1.0;
                            break;
                        case ResultSatuses.DidNotFinish:
                        case ResultSatuses.Disqualified:
                        case ResultSatuses.NotClassiFied:
                        case ResultSatuses.Retired:
                            this.Opacity = 0.5;
                            break;
                    }

                    this.OnPropertyChanged("ResultStatus");
                }
            }
        }

        private TimeSpan _currentLapTime;
        public TimeSpan CurrentLapTime
        {
            get { return _currentLapTime; }
            set
            {
                if (CurrentLapTime != value)
                {
                    _currentLapTime = value;
                    this.FormattedTime = _currentLapTime.ToString(@"mm\:ss\.fff");
                    this.OnPropertyChanged("CurrentLapTime");
                }
            }
        }

        private string _formattedTime;
        public string FormattedTime
        {
            get { return _formattedTime; }
            set
            {
                if (this.FormattedTime != value)
                {
                    _formattedTime = value;
                    this.OnPropertyChanged("FormattedTime");
                }
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
                if (value != this.DriverID)
                {
                    this._driverID = value;
                    this.DriverName = Regex.Replace(this.DriverID.ToString(), "[A-Z]", m => " " + m.ToString());
                    this.OnPropertyChanged("DriverID");
                }
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
                if (this.DriverName != value)
                {
                    this._driverName = value;
                    this.OnPropertyChanged("DriverName");
                }
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
                if (this.IsAI != value)
                {
                    this._isAI = value;
                    this.OnPropertyChanged("IsAI");
                }
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
                if (this.IsHuman != value)
                {
                    this._isHuman = value;
                    this.OnPropertyChanged("IsHuman");
                }
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
                if (this.IsMyTeam != value)
                {
                    this._isMyTeam = value;
                    if (this._isMyTeam) this.TeamColor = new SolidColorBrush(Color.FromRgb(255, 0, 255));
                    this.OnPropertyChanged("IsMyTeam");
                }
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
                if (this.PilotName != value)
                {
                    this._pilotName = value;
                    this.OnPropertyChanged("PilotName");
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

                    this.OnPropertyChanged("Nationality");

                    if (this._nationality == (Nationalities)255) this._nationality = 0;

                    var uj = "pack://application:,,,/Images/Flags/" + this._nationality + ".png";
                    var bitmap = new BitmapImage(new Uri(uj, UriKind.Absolute));
                    bitmap.Freeze();
                    this.image_nation.Source = bitmap;

                    //this.ImageSource = u.KepForras(Properties.Resources["asd"])

                    //var rs = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

                    //using (MemoryStream ms = new MemoryStream((byte[])rs.GetObject(this._nationality.ToString())))
                    //using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ms))
                    //{
                    //    if (bitmap != null) this.image_nation.Source = u.KepForras((System.Drawing.Bitmap)bitmap);
                    //}
                }
            }
        }

        //private BitmapImage _imageSource;
        //public BitmapImage ImageSource
        //{
        //    get
        //    {
        //        return this._imageSource;
        //    }
        //    set
        //    {
        //        if (this._imageSource != value)
        //        {
        //            this._imageSource = value;
        //            this.OnPropertyChanged("ImageSource");
        //        }
        //    }
        //}

        private int _networkID;
        public int NetworkID
        {
            get
            {
                return this._networkID;
            }
            set
            {
                if (this.NetworkID != value)
                {
                    this._networkID = value;
                    this.OnPropertyChanged("NetworkID");
                }
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
                if (this.RaceNumber != value)
                {
                    this._raceNumber = value;
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
                if (value != TrackLengthPercent)
                {
                    this._trackLengthPercent = value;
                    this.OnPropertyChanged("TrackLengthPercent");
                }
            }
        }

        public SolidColorBrush _teamColor;
        public SolidColorBrush TeamColor
        {
            get
            {
                return this._teamColor;
            }
            set
            {
                if (value != this.TeamColor)
                {
                    this._teamColor = value;
                    this.OnPropertyChanged("TeamColor");
                }
            }
        }

        public int CurrentLap { get; internal set; }

        private TyreCompounds _visualTyreCompund;
        public TyreCompounds VisualTyreCompund
        {
            get
            {
                return this._visualTyreCompund;
            }
            set
            {
                if (value != this.VisualTyreCompund)
                {
                    this._visualTyreCompund = value;
                    this.OnPropertyChanged("VisualTyreCompund");
                }
            }
        }

        private TyreCompounds _tyreCompund;
        private bool disposedValue;

        public TyreCompounds TyreCompund
        {
            get
            {
                return this._tyreCompund;
            }
            set
            {
                if (value != this.TyreCompund)
                {
                    this._tyreCompund = value;
                    this.OnPropertyChanged("TyreCompund");
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    GC.SuppressFinalize(this.PilotName);
                    GC.SuppressFinalize(this.CarPosition);
                    GC.SuppressFinalize(this.CurrentLap);
                    GC.SuppressFinalize(this.CurrentLapTime);
                    GC.SuppressFinalize(this.DriverID);
                    GC.SuppressFinalize(this.DriverName);
                    GC.SuppressFinalize(this.DriverStatus);
                    //GC.SuppressFinalize(this.ImageSource);
                    GC.SuppressFinalize(this.IsAI);
                    GC.SuppressFinalize(this.IsHuman);
                    GC.SuppressFinalize(this.Nationality);
                    GC.SuppressFinalize(this.NetworkID);
                    GC.SuppressFinalize(this.PilotName);

                    this.RemoveLogicalChild(this.Content);
                    GC.SuppressFinalize(this.Content);
                }

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


    }
}
