using F1TelemetryApp.Classes;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace F1TelemetryApp.Windows
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window, IDisposable
    {
        private bool disposedValue;
        private readonly string regPath;

        public PreferencesWindow(Window owner)
        {
            InitializeComponent();

            this.DataContext = this;
            this.Owner = owner;

            this.regPath = this.CreateRegPath();
            this.LoadWindowPosition(this.regPath);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.udpsettings.LoadData();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    //this.regPath = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PreferencesWindow()
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SaveWindowPosition(this.regPath);
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.udpsettings.SaveData();
            this.Close();
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
