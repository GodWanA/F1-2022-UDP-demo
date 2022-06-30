using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
                if (value != header)
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
                    this.OnPropertyChanged("Percent");
                }

                if (this.IsLoaded)
                {
                    var p = Math.Round(this.percent / 100.0, 2);
                    var fg = new SolidColorBrush(this.ColorMapBackgound.GradientStops.GetRelativeColor(p));
                    var t = new SolidColorBrush(this.ColorMapText.GradientStops.GetRelativeColor(p));

                    if (fg.CanFreeze) fg.Freeze();
                    if (t.CanFreeze) t.Freeze();

                    this.Dispatcher.Invoke(() =>
                    {
                        this.ProgressbarForeground = fg;
                        this.ProgressbarText = t;
                    });
                }
            }
        }

        private Brush progressbarForeground;
        public Brush ProgressbarForeground
        {
            get { return progressbarForeground; }
            private set
            {
                if (value != progressbarForeground)
                {
                    this.progressbarForeground = value;
                    this.OnPropertyChanged("ProgressbarForeground");
                }
            }
        }

        private Brush progressbarText;

        public Brush ProgressbarText
        {
            get { return progressbarText; }
            private set
            {
                if (value != progressbarText)
                {
                    this.progressbarText = value;
                    this.OnPropertyChanged("ProgressbarText");
                }
            }
        }

        public LinearGradientBrush ColorMapBackgound { get; set; } = null;
        public LinearGradientBrush ColorMapText { get; set; } = null;

        public DemageAndWearDataControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Percent = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            // if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
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
                    this.PropertyChanged = null;
                    this.ProgressbarText = null;
                    this.progressbarForeground = null;
                    this.ColorMapText = null;
                    this.ColorMapBackgound = null;
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
            if (this.ColorMapBackgound == null)
            {
                var colors = new GradientStopCollection();
                colors.Add(new GradientStop(Color.FromRgb(255, 0, 0), 0));
                colors.Add(new GradientStop(Color.FromRgb(255, 187, 51), 0.5));
                colors.Add(new GradientStop(Color.FromRgb(45, 179, 0), 1));

                this.ColorMapBackgound = new LinearGradientBrush(colors);
            }

            if (this.ColorMapText == null)
            {
                var colors = new GradientStopCollection();
                colors.Add(new GradientStop(Color.FromRgb(255, 255, 255), 0));
                colors.Add(new GradientStop(Color.FromRgb(255, 255, 255), 0.5));
                colors.Add(new GradientStop(Color.FromRgb(0, 0, 0), 0.501));
                colors.Add(new GradientStop(Color.FromRgb(0, 0, 0), 1));

                this.ColorMapText = new LinearGradientBrush(colors);
            }

            this.Percent = 100;
        }
    }
}
