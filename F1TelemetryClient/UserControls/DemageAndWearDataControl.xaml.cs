using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for DemageAndWearDataControl.xaml
    /// </summary>
    public partial class DemageAndWearDataControl : UserControl, INotifyPropertyChanged, IDisposable
    {
        private string _header;

        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
                this.OnPropertyChanged("Header");
            }
        }

        private double _percent;
        private bool disposedValue;

        public double Percent
        {
            get { return _percent; }
            set
            {
                if (value != this._percent)
                {
                    _percent = value;

                    var red = (byte)Math.Round(255 * (100 - this.Percent) / 100 * 2);
                    var green = (byte)Math.Round(255 * this.Percent / 100 * 2);

                    if (this.Percent >= 50)
                    {
                        this.progressbar_percent.Foreground = new SolidColorBrush(Color.FromRgb(red, 255, 0));
                    }
                    else
                    {
                        this.progressbar_percent.Foreground = new SolidColorBrush(Color.FromRgb(255, green, 0));
                    }

                    this.OnPropertyChanged("Percent");
                }
            }
        }


        public DemageAndWearDataControl()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Percent = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

                    this.RemoveLogicalChild(this.Content);
                    this.Content = null;
                }

                this._header = null;
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
    }
}
