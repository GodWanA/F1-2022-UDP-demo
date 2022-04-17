using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls.TyreDisplay
{
    /// <summary>
    /// Interaction logic for TyreIcon.xaml
    /// </summary>
    public partial class TyreIcon : UserControl, IDisposable, INotifyPropertyChanged
    {
        private bool disposedValue;

        private TyreCompounds tyre;
        internal TyreCompounds Tyre
        {
            get
            {
                return this.tyre;
            }
            set
            {
                //if (value != this.tyre)
                //{
                this.tyre = value;
                this.image_tyre.Source = u.TyreCompoundToImage(value);
                this.textblock_name.Text = value.ToString();
                //this.OnPropertyChanged("Tyre");
                //}
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TyreIcon(TyreCompounds tyre)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Tyre = tyre;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.PropertyChanged = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TyreIcon()
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
