using F1Telemetry.Models.SessionPacket;
using System;
using System.Linq;
using System.Windows.Controls;
using static F1Telemetry.Helpers.Appendences;
using System.Windows;
using System.Text.RegularExpressions;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for WeatherDisplayController.xaml
    /// </summary>
    public partial class WeatherDisplayController : UserControl
    {
        public WeatherDisplayController()
        {
            InitializeComponent();
        }

        public void SetActualWeather(WeatherTypes weather, SessionTypes type, int trackTemp, int airTemp)
        {
            this.weather_actual.Weather = weather;
            this.weather_actual.TrackTemperature = trackTemp;
            this.weather_actual.AirTemperature = airTemp;
            this.weather_actual.SessionType = type;
            this.weather_actual.OffsetTime = null;
        }

        public void SetWeatherForecast(WeatherForecastSample[] rawData)
        {
            if (stackpanel_nodes.Children.Count == 0)
            {
                for (int i = 0; i < rawData.Length; i++)
                {
                    this.stackpanel_nodes.Children.Add(new WheatherNode
                    {
                        Visibility = Visibility.Collapsed,
                    });
                }
            }

            var items = this.stackpanel_nodes.Children.Cast<WheatherNode>();

            for (int i = 0; i < items.Count(); i++)
            {
                var item = (WheatherNode)this.stackpanel_nodes.Children[i];
                if (i == 0) this.weather_actual.RainPercentage = rawData[i].RainPercentage;

                bool ok = false;
                if (!this.IsAllSessionVisible) ok = rawData[i].SeassonType == this.weather_actual.SessionType || Regex.IsMatch(rawData[i].SeassonType.ToString(), "quallifying", RegexOptions.IgnoreCase);
                else ok = rawData[i].SeassonType != SessionTypes.Unknown;

                if (ok)
                {
                    item.Visibility = Visibility.Visible;
                    item.AirTemperature = rawData[i].AirTemperature;
                    item.TrackTemperature = rawData[i].TrackTemperature;
                    item.RainPercentage = rawData[i].RainPercentage;
                    item.Weather = rawData[i].Weather;
                    item.OffsetTime = rawData[i].TimeOffset;
                    item.SessionType = rawData[i].SeassonType;

                    if (i != 0 && rawData[i].TimeOffset == TimeSpan.Zero) item.NewBlockMarker = true;
                    else item.NewBlockMarker = false;

                    if (rawData[i].SeassonType == this.weather_actual.SessionType) item.IsCurrentSession = true;
                    else item.IsCurrentSession = false;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                    item.SessionType = SessionTypes.Unknown;
                }
            }
            //this.rectangle_marker.Margin = new Thickness(d, 1, 1, 0);
        }

        public bool IsAllSessionVisible { get; set; } = true;
    }
}
