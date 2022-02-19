using F1Telemetry.Models.SessionPacket;
using System;
using System.Linq;
using System.Windows.Controls;
using static F1Telemetry.Helpers.Appendences;

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

        public void SetActualWeather(WeatherTypes weather, double rainPercent, int trackTemp, int airTemp)
        {
            this.weather_actual.Weather = weather;
            this.weather_actual.RainPercentage = rainPercent;
            this.weather_actual.TrackTemperature = trackTemp;
            this.weather_actual.AirTemperature = airTemp;
            this.weather_actual.OffsetTimeText = "Actual";
        }

        public void SetWeatherForecast(TimeSpan duration, WeatherForecastSample[] rawData)
        {
            var nodes = rawData.Where(x => x.AirTemperature > 0 && x.TrackTemperature > 0);
            if (nodes != null)
            {
                this.stackpanel_nodes.Children.Clear();
                double d = this.stackpanel_nodes.ActualWidth / nodes.Count();
                int i = 1;

                foreach (var item in nodes)
                {
                    var newNode = new WheatherNode
                    {
                        AirTemperature = item.AirTemperature,
                        TrackTemperature = item.TrackTemperature,
                        RainPercentage = item.TrackTemperature,
                        Weather = item.Weather,
                        OffsetTimeText = item.TimeOffset.ToString(@"hh\:mm\:ss"),
                        Width = d * i++,
                    };

                    this.stackpanel_nodes.Children.Add(newNode);
                }
            }
        }
    }
}
