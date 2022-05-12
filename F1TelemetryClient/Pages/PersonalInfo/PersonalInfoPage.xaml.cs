using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryApp.Pages.PersonalInfo
{
    /// <summary>
    /// Interaction logic for PersonalInfoPage.xaml
    /// </summary>
    public partial class PersonalInfoPage : UserControl, IConnectUDP, IGridResize, IDisposable
    {
        private bool isWorking_SessionData;
        private bool isLoadingWear;
        private bool disposedValue;

        public PersonalInfoPage()
        {
            InitializeComponent();
        }

        private void livestatusdata_Loaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        private void livestatusdata_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket += Connention_SessionPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket -= Connention_SessionPacket;
            }
        }

        private void Connention_SessionPacket(object sender, EventArgs e)
        {
            if (!this.isWorking_SessionData && u.CanDoUdp)
            {
                this.isWorking_SessionData = true;
                this.Dispatcher.Invoke(() =>
                {
                    var sessionData = sender as PacketSessionData;
                    this.weatherController.SetActualWeather(sessionData.Weather, sessionData.SessionType, sessionData.TrackTemperature, sessionData.AirTemperature);
                    this.weatherController.SetWeatherForecast(sessionData.WeatherForcastSample);
                    //this.tyrecontainer.UpdateTyres(this.map.RawTrack);                   

                    this.isWorking_SessionData = false;

                }, DispatcherPriority.Render);
            }
        }

        internal void LoadWear()
        {
            if (!this.isLoadingWear)
            {
                this.isLoadingWear = true;

                //if (this.listBox_drivers.SelectedIndex != -1)
                this.livestatusdata.LoadWear();

                this.isLoadingWear = false;
            }
        }

        internal void CalculateView(GridSizes res)
        {
            this.livestatusdata.CalculateView(res);
        }

        public void ResizeXS()
        {
            this.livestatusdata.ResizeXS();
        }

        public void ResizeXM()
        {
            this.livestatusdata.ResizeXM();
        }

        public void ResizeMD()
        {
            this.livestatusdata.ResizeMD();
        }

        public void ResizeLG()
        {
            this.livestatusdata.ResizeLG();
        }

        public void ResizeXL()
        {
            this.livestatusdata.ResizeXL();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PersonalInfoPage()
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
