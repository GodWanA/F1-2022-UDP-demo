﻿using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for TyreDataControl.xaml
    /// </summary>
    public partial class TyreDataControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private double wear;
        private double demage;
        private double breakTemperature;
        private double condition;
        private double tyreInnerTemperature;
        private double tyreSurfaceTemperature;
        private double pressure;

        private LinearGradientBrush heatMapInnerSoft = null;
        private LinearGradientBrush heatMapSurfaceSoft = null;
        private LinearGradientBrush heatMapInnerMedium = null;
        private LinearGradientBrush heatMapSurfaceMedium = null;
        private LinearGradientBrush heatMapInnerHard = null;
        private LinearGradientBrush heatMapSurfaceHard = null;
        private LinearGradientBrush heatMapInnerInter = null;
        private LinearGradientBrush heatMapSurfaceInter = null;
        private LinearGradientBrush heatMapInnerWet = null;
        private LinearGradientBrush heatMapSurfaceWet = null;

        public TyreDataControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private Brush tyreConditionForeground;

        public Brush TyreConditionForeground
        {
            get { return tyreConditionForeground; }
            private set
            {
                if (value != tyreConditionForeground)
                {
                    this.tyreConditionForeground = value;
                    this.OnPropertyChanged("TyreConditionForeground");
                }
            }
        }

        private CornerRadius cornerRadius;

        public CornerRadius CornerRadius
        {
            get { return this.cornerRadius; }
            set
            {
                if (value != this.cornerRadius)
                {
                    this.cornerRadius = value;
                    this.OnPropertyChanged("CornerRadius");
                }
            }
        }

        private Brush tyreConditionText;

        public Brush TyreConditionText
        {
            get { return tyreConditionText; }
            private set
            {
                if (value != tyreConditionText)
                {
                    this.tyreConditionText = value;
                    this.OnPropertyChanged("TyreConditionText");
                }
            }
        }

        public double Wear
        {
            get
            {
                return this.wear;
            }
            set
            {
                if (value != this.wear)
                {
                    this.wear = value;
                    this.OnPropertyChanged("Wear");
                }

                if (this.IsLoaded)
                {
                    this.Condition = 100.0 - this.Wear;
                    var fg = new SolidColorBrush(this.ColorMapTyreConditionBackground.GradientStops.GetRelativeColor(this.Condition / 100.0));
                    var t = new SolidColorBrush(this.ColorMapTyreConditionText.GradientStops.GetRelativeColor(this.Condition / 100.0));

                    if (fg.CanFreeze) fg.Freeze();
                    if (t.CanFreeze) t.Freeze();

                    this.TyreConditionForeground = fg;
                    this.TyreConditionText = t;
                }
            }
        }

        public double Demage
        {
            get
            {
                return this.demage;
            }
            set
            {
                if (value != this.demage)
                {
                    this.demage = value;
                    this.OnPropertyChanged("Demage");
                }
            }
        }

        public double BrakeDemage
        {
            get
            {
                return this.brakeDemage;
            }
            set
            {
                if (value != this.brakeDemage)
                {
                    this.brakeDemage = value;
                    this.OnPropertyChanged("BrakeDemage");
                }
            }
        }

        public double Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                if (value != this.condition)
                {
                    this.condition = value;
                    this.OnPropertyChanged("Condition");
                }
            }
        }

        public double BrakesTemperature
        {
            get
            {
                return this.breakTemperature;
            }
            set
            {
                if (value != this.breakTemperature)
                {
                    this.breakTemperature = value;
                    //this.textBlock_brake.Text = this.breakTemperature.ToString();
                    this.OnPropertyChanged("BrakesTemperature");
                }
            }
        }
        public double TyreInnerTemperature
        {
            get
            {
                return this.tyreInnerTemperature;
            }
            set
            {
                if (value != this.tyreInnerTemperature)
                {
                    this.tyreInnerTemperature = value;
                    //this.textBlock_inner.Text = this.tyreInnerTemperature.ToString();
                    this.ColorInnerHeat(value);
                    this.OnPropertyChanged("TyreInnerTemperature");
                }
            }
        }

        public double TyreSurfaceTemperature
        {
            get
            {
                return this.tyreSurfaceTemperature;
            }
            set
            {
                if (value != this.tyreSurfaceTemperature)
                {
                    this.tyreSurfaceTemperature = value;
                    this.ColorOuterHeat(value);
                    //this.textBlock_surface.Text = this.tyreSurfaceTemperature.ToString();
                    this.OnPropertyChanged("TyreSurfaceTemperature");
                }
            }
        }
        public double Pressure
        {
            get
            {
                return this.pressure;
            }
            set
            {
                if (value != this.pressure)
                {
                    this.pressure = value;
                    //this.textBlock_pressure.Text = this.pressure.ToString("0.##");
                    this.OnPropertyChanged("Pressure");
                }
            }
        }

        private Side progressbarSide;
        private bool disposedValue;
        private double brakeDemage;

        public Side ProgressbarSide
        {
            get { return this.progressbarSide; }
            set
            {
                if (value != this.progressbarSide)
                {
                    this.progressbarSide = value;
                }
            }
        }

        public LinearGradientBrush ColorMapTyreConditionBackground { get; set; } = null;
        public LinearGradientBrush ColorMapTyreConditionText { get; set; } = null;
        public static TyreCompounds TyreSoft { get; internal set; } = TyreCompounds.Unknown;
        public static TyreCompounds TyreMedium { get; internal set; } = TyreCompounds.Unknown;
        public static TyreCompounds TyreHard { get; internal set; } = TyreCompounds.Unknown;
        public static TyreCompounds ActualTyreCpompund { get; internal set; } = TyreCompounds.Unknown;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (this.ColorMapTyreConditionBackground == null)
            {
                var colors = new GradientStopCollection();
                // red range
                colors.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0));
                colors.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0.33));
                // yellow range
                colors.Add(new GradientStop(Color.FromRgb(255, 187, 51), 0.6));
                colors.Add(new GradientStop(Color.FromRgb(255, 187, 51), 0.66));
                // green range
                colors.Add(new GradientStop(Colors.LimeGreen, 0.9));
                colors.Add(new GradientStop(Colors.LimeGreen, 1));

                var c = new LinearGradientBrush(colors);
                if (c.CanFreeze) c.Freeze();
                this.ColorMapTyreConditionBackground = c;
            }

            if (this.ColorMapTyreConditionText == null)
            {
                var colors = new GradientStopCollection();
                // red range
                colors.Add(new GradientStop(Colors.White, 0.0));
                colors.Add(new GradientStop(Colors.White, 0.45));
                // yellow range
                colors.Add(new GradientStop(Colors.Black, 0.5));
                colors.Add(new GradientStop(Colors.Black, 1.0));
                // green range
                //colors.Add(new GradientStop(Colors.White, 0.9));
                //colors.Add(new GradientStop(Colors.White, 1));

                var b = new LinearGradientBrush(colors);
                if (b.CanFreeze) b.Freeze();
                this.ColorMapTyreConditionText = b;
            }

            if (this.heatMapInnerSoft == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0.8));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.85));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.03));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.03));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.06));
                // red range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 1.08));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapInnerSoft = b;
            }

            if (this.heatMapSurfaceSoft == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0.65));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.8));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.03));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.25));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.35));
                // red range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 1.40));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapSurfaceSoft = b;
            }

            if (this.heatMapInnerMedium == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0.83));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.87));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.05));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.06));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.10));
                // red range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 1.11));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapInnerMedium = b;
            }

            if (this.heatMapSurfaceMedium == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0.65));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.75));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.25));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.25));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.35));
                // red range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 1.40));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapSurfaceMedium = b;
            }

            if (this.heatMapInnerHard == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0.83));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.85));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.05));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.07));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 1.10));
                // red range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 1.11));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapInnerHard = b;
            }

            if (this.heatMapSurfaceHard == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0.65));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.75));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.25));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.25));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.35));
                // red range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 1.40));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapSurfaceHard = b;
            }

            if (this.heatMapInnerInter == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0.6));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.63));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.88));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 0.89));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 0.92));
                // red range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 0.96));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapInnerInter = b;
            }

            if (this.heatMapSurfaceInter == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0.45));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.5));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.03));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.04));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.10));
                // red range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 1.20));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapSurfaceInter = b;
            }

            if (this.heatMapInnerWet == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(127, 0, 153, 255), 0.5));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.55));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.78));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 0.8));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 217, 0), 0.85));
                // red range
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 0.86));
                c.Add(new GradientStop(Color.FromArgb(127, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapInnerWet = b;
            }

            if (this.heatMapSurfaceWet == null)
            {
                var c = new GradientStopCollection();

                // blue range
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0));
                c.Add(new GradientStop(Color.FromArgb(191, 0, 153, 255), 0.35));
                // green range
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.45));
                c.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.68));
                // yellow range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 0.70));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 217, 0), 1.05));
                // red range
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 1.10));
                c.Add(new GradientStop(Color.FromArgb(191, 255, 0, 0), 2));

                var b = new LinearGradientBrush(c);
                if (b.CanFreeze) b.Freeze();
                this.heatMapSurfaceWet = b;
            }

            this.Wear = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (this.ProgressbarSide)
            {
                case Side.Left:
                    this.grid_container.ColumnDefinitions[0].Width = new System.Windows.GridLength(55, System.Windows.GridUnitType.Pixel);
                    this.grid_container.ColumnDefinitions[1].Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    Grid.SetColumn(this.grid_progress, 0);
                    Grid.SetColumn(this.grid_data, 1);
                    break;
                case Side.Right:
                    this.grid_container.ColumnDefinitions[0].Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                    this.grid_container.ColumnDefinitions[1].Width = new System.Windows.GridLength(55, System.Windows.GridUnitType.Pixel);
                    Grid.SetColumn(this.grid_progress, 1);
                    Grid.SetColumn(this.grid_data, 0);
                    break;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.TyreConditionForeground = null;
                    this.TyreConditionText = null;
                    this.ColorMapTyreConditionBackground = null;
                    this.ColorMapTyreConditionText = null;
                    this.PropertyChanged = null;
                }

                this.tyreConditionForeground = null;
                this.tyreConditionText = null;
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TyreDataControl()
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

        private void ColorInnerHeat(double value)
        {
            Color c = Colors.Transparent;
            value = value / 100;

            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreSoft) c = this.heatMapInnerSoft.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreMedium) c = this.heatMapInnerMedium.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreHard) c = this.heatMapInnerHard.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreCompounds.Inter) c = this.heatMapInnerInter.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreCompounds.Wet) c = this.heatMapInnerWet.GradientStops.GetRelativeColor(value);

            var b = new SolidColorBrush(c);
            if (b.CanFreeze) b.Freeze();
            this.border_heatMap.Background = b;
        }

        private void ColorOuterHeat(double value)
        {
            Color c = Colors.Transparent;
            value = value / 100;

            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreSoft) c = this.heatMapSurfaceSoft.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreMedium) c = this.heatMapSurfaceMedium.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreDataControl.TyreHard) c = this.heatMapSurfaceHard.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreCompounds.Inter) c = this.heatMapSurfaceInter.GradientStops.GetRelativeColor(value);
            if (TyreDataControl.ActualTyreCpompund == TyreCompounds.Wet) c = this.heatMapSurfaceWet.GradientStops.GetRelativeColor(value);

            var b = new SolidColorBrush(c);
            if (b.CanFreeze) b.Freeze();
            this.border_heatMap.BorderBrush = b;
        }
    }

    public enum Side
    {
        Left,
        Right
    }
}
