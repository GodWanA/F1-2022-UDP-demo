using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;
using F1TelemetryApp.Classes;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for WheatherNode.xaml
    /// </summary>
    public partial class WeatherNode : UserControl, INotifyPropertyChanged, IDisposable
    {
        public WeatherNode()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Weather = WeatherTypes.Unknown;
            this.NewBlockMarker = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private TimeSpan? offsetTime;

        public TimeSpan? OffsetTime
        {
            get { return offsetTime; }
            set
            {
                if (this.offsetTime != value)
                {
                    offsetTime = value;
                    //if (this.offsetTime != null) this.textblock_offsetTime.Text = this.offsetTime.Value.ToString(@"\+h\:mm");
                    //else this.textblock_offsetTime.Text = "Actual";
                    this.OnPropertyChanged("OffsetTime");
                }
            }
        }

        private int trackTemperature;

        public int TrackTemperature
        {
            get { return trackTemperature; }
            set
            {
                if (this.trackTemperature != value)
                {
                    trackTemperature = value;
                    //this.textblock_trackTemperature.Text = value.ToString();
                    this.OnPropertyChanged("TrackTemperature");
                }
            }
        }

        private int airTemperature;

        public int AirTemperature
        {
            get { return airTemperature; }
            set
            {
                if (this.airTemperature != value)
                {
                    airTemperature = value;
                    //this.textblock_airTemperature.Text = this.airTemperature.ToString();
                    this.OnPropertyChanged("AirTemperature");
                }
            }
        }

        private double rainPercentage;

        public double RainPercentage
        {
            get { return rainPercentage; }
            set
            {
                if (this.rainPercentage != value)
                {
                    rainPercentage = value;
                    //this.progressbar_rainRercent.Value = this.rainPercentage;
                    //this.textblock_rainPercentage.Text = this.rainPercentage.ToString() + " %";
                    this.OnPropertyChanged("RainPercentage");
                }
            }
        }

        private WeatherTypes weather;

        public WeatherTypes Weather
        {
            get { return weather; }
            set
            {
                if (this.weather != value)
                {
                    weather = value;
                    BitmapImage source;

                    switch (this.weather)
                    {
                        default:
                            source = null;
                            break;
                        case WeatherTypes.Clear:
                            source = new BitmapImage(new Uri(forras + "Clear.png"));
                            break;
                        case WeatherTypes.LitghtCloud:
                            source = new BitmapImage(new Uri(forras + "LitghtCloud.png"));
                            break;
                        case WeatherTypes.Overcast:
                            source = new BitmapImage(new Uri(forras + "Overcast.png"));
                            break;
                        case WeatherTypes.LightRain:
                            source = new BitmapImage(new Uri(forras + "LightRain.png"));
                            break;
                        case WeatherTypes.HeavyRain:
                            source = new BitmapImage(new Uri(forras + "HeavyRain.png"));
                            break;
                        case WeatherTypes.Storm:
                            source = new BitmapImage(new Uri(forras + "Storm.png"));
                            break;
                    }

                    if (source != null && source.CanFreeze) source.Freeze();
                    this.image_wheather.Source = source;
                    //this.OnPropertyChanged("Weather");
                }
            }
        }

        private bool newBlockMarker;

        public bool NewBlockMarker
        {
            get { return newBlockMarker; }
            set
            {
                if (this.newBlockMarker != value)
                {
                    newBlockMarker = value;
                    if (newBlockMarker) this.rectangle_marker.Visibility = Visibility.Visible;
                    else this.rectangle_marker.Visibility = Visibility.Collapsed;
                }
            }
        }

        private SessionTypes sessionType;

        public SessionTypes SessionType
        {
            get { return sessionType; }
            set
            {
                sessionType = value;
                //this.OnPropertyChanged("SessionType");
            }
        }

        private bool isCurrentSession;

        public bool IsCurrentSession
        {
            get { return isCurrentSession; }
            set
            {
                if (this.isCurrentSession != value)
                {
                    isCurrentSession = value;

                    if (isCurrentSession) this.rectangle_background.Visibility = Visibility.Visible;
                    else this.rectangle_background.Visibility = Visibility.Hidden;

                    //this.OnPropertyChanged("IsCurrentSession");
                }
            }
        }


        private const string forras = "pack://application:,,,/Images/WeatherIcons/";
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.OffsetTime = null;
                    this.PropertyChanged = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this.offsetTime = null;
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WheatherNode()
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
