﻿using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for DemageAndWearDataControl.xaml
    /// </summary>
    public partial class DemageAndWearDataControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private string header;

        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                if (header != value)
                {
                    header = value;
                    //this.textblock_Header.Text = this.header;
                }
                this.OnPropertyChanged("Header");
            }
        }

        private bool disposedValue;

        private double percent;
        public double Percent
        {
            get { return percent; }
            set
            {
                if (value != this.percent)
                {
                    percent = value;

                    //this.progressbar_percent.Value = percent;
                    //this.textblock_percent.Text = percent.ToString("0.##");
                    this.ProgressbarColor = new SolidColorBrush(this.ColorMap.GradientStops.GetRelativeColor(this.percent / 100.0));

                    this.OnPropertyChanged("Percent");
                }
            }
        }

        private Brush progressbarColor;

        public Brush ProgressbarColor
        {
            get { return progressbarColor; }
            private set
            {
                if (value != this.progressbarColor)
                {
                    progressbarColor = value;
                    this.OnPropertyChanged("ProgressbarColor");
                }
            }
        }


        public LinearGradientBrush ColorMap { get; set; } = null;

        public DemageAndWearDataControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Percent = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.PropertyChanged = null;
                    this.Header = null;
                }

                this.header = null;
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DemageAndWearDataControl()
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

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (this.ColorMap == null)
            {
                var colors = new GradientStopCollection();
                colors.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0));
                colors.Add(new GradientStop(Color.FromRgb(255, 187, 51), 0.5));
                colors.Add(new GradientStop(Color.FromRgb(45, 179, 0), 1));

                this.ColorMap = new LinearGradientBrush(colors);
            }
        }
    }
}