using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for WheatherNode.xaml
    /// </summary>
    public partial class WheatherNode : UserControl, INotifyPropertyChanged, IDisposable
    {
        public WheatherNode()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            while (this.LogicalChildren.Current != null)
            {
                var child = this.LogicalChildren.Current;
                this.RemoveLogicalChild(child);
                this.LogicalChildren.Reset();
                GC.SuppressFinalize(child);
            }

            GC.SuppressFinalize(this);
        }

        private string offsetTimeText;

        public string OffsetTimeText
        {
            get { return offsetTimeText; }
            set
            {
                if (this.offsetTimeText != value)
                {
                    offsetTimeText = value;
                    this.OnPropertyChanged("OffsetTimeText");
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
                    this.OnPropertyChanged("AirTemperature");
                }
            }
        }

        private double rainPercentage;

        public double RainPercentage
        {
            get { return airTemperature; }
            set
            {
                if (this.rainPercentage != value)
                {
                    rainPercentage = value;
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

                    switch (this.weather)
                    {
                        default:
                            this.image_wheather.Source = null;
                            break;
                        case WeatherTypes.Clear:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "Clear.png"));
                            break;
                        case WeatherTypes.LitghtCloud:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "LitghtCloud.png"));
                            break;
                        case WeatherTypes.Overcast:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "Overcast.png"));
                            break;
                        case WeatherTypes.LightRain:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "LightRain.png"));
                            break;
                        case WeatherTypes.HeavyRain:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "HeavyRain.png"));
                            break;
                        case WeatherTypes.Storm:
                            this.image_wheather.Source = new BitmapImage(new Uri(forras + "Storm.png"));
                            break;
                    }

                    this.OnPropertyChanged("Weather");
                }
            }
        }

        private static string forras = "pack://application:,,,/Images/WeatherIcons/";
    }
}
