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

        public void setActualWeather(WeatherTypes weather, double rainPercent, int trackTemp, int airTemp)
        {
            this.weather_actual.Weather = weather;
            this.weather_actual.RainPercentage = rainPercent;
            this.weather_actual.TrackTemperature = trackTemp;
            this.weather_actual.AirTemperature = airTemp;
            this.weather_actual.OffsetTimeText = "Actual";
        }
    }
}
