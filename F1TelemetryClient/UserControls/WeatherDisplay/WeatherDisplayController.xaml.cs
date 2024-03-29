﻿using F1Telemetry.Models.SessionPacket;
using System;
using System.Linq;
using System.Windows.Controls;
using static F1Telemetry.Helpers.Appendences;
using static F1Telemetry.Helpers.SessionTypesExtension;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Media;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for WeatherDisplayController.xaml
    /// </summary>
    public partial class WeatherDisplayController : UserControl
    {
        private WeatherForecastSample[] lastWeatherRaw = null;

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
            if (this.lastWeatherRaw != rawData)
            {
                this.lastWeatherRaw = rawData;

                if (stackpanel_nodes.Children.Count == 0)
                {
                    for (int i = 0; i < rawData?.Length; i++)
                    {
                        this.stackpanel_nodes.Children.Add(new WeatherNode
                        {
                            Visibility = Visibility.Collapsed,
                            Margin = new Thickness(0),
                        });
                    }
                }

                this.stackpanel_names.Children.Clear();

                if (rawData != null)
                {
                    var items = this.stackpanel_nodes.Children.Cast<WeatherNode>();

                    for (int i = 0; i < items.Count(); i++)
                    {
                        var item = (WeatherNode)this.stackpanel_nodes.Children[i];
                        if (i == 0) this.weather_actual.RainPercentage = rawData[i].RainPercentage;

                        bool ok = false;
                        if (!this.IsAllSessionVisible) ok = rawData[i].SeassonType == this.weather_actual.SessionType || Regex.IsMatch(rawData[i].SeassonType.ToString(), "quallifying", RegexOptions.IgnoreCase);
                        else ok = rawData[i] != null && rawData[i]?.SeassonType != SessionTypes.Unknown;

                        if (ok)
                        {
                            item.Visibility = Visibility.Visible;
                            item.AirTemperature = rawData[i].AirTemperature;
                            item.TrackTemperature = rawData[i].TrackTemperature;
                            item.RainPercentage = rawData[i].RainPercentage;
                            item.Weather = rawData[i].Weather;
                            item.OffsetTime = rawData[i].TimeOffset;
                            item.SessionType = rawData[i].SeassonType;

                            item.Height = this.weather_actual.ActualHeight;
                            item.Width = this.weather_actual.ActualWidth;

                            if (i != 0 && rawData[i].TimeOffset == TimeSpan.Zero)
                            {
                                item.NewBlockMarker = true;
                                item.Width += 5;
                            }
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

                    var groups = items.Where(x => x.SessionType != SessionTypes.Unknown).GroupBy(x => x.SessionType);
                    Brush fg = Brushes.White;
                    if (fg.CanFreeze) fg.Freeze();

                    foreach (var group in groups)
                    {
                        double d = group.Sum(x => x.ActualWidth);

                        this.stackpanel_names.Children.Add(new TextBlock
                        {
                            Margin = new Thickness(0),
                            Width = d,
                            Height = 16,
                            Foreground = fg,
                            TextAlignment = TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = group.Select(x => x.SessionType).FirstOrDefault().GetSessionType(),
                        });
                    }
                }
            }
        }

        public bool IsAllSessionVisible { get; set; } = true;
    }
}
