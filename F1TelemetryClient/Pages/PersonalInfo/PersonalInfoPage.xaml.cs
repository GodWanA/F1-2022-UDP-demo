using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.Pages.PersonalInfo.InnerDetails;
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
                if (this.livestatusdata.IsLoaded) this.livestatusdata.LoadWear();
                if (this.carsetupdata.IsLoaded) this.carsetupdata.LoadSetup();

                this.isLoadingWear = false;
            }
        }

        public void CalculateView(GridSizes res)
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedContent as IGridResize;
            //    page?.CalculateView(res);
            //}
            this.livestatusdata.CalculateView(res);
            this.carsetupdata.CalculateView(res);
        }

        public void ResizeXS()
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedItem as IGridResize;
            //    page?.ResizeXS();
            //}
            this.livestatusdata.ResizeXS();
            this.carsetupdata.ResizeXS();
        }

        public void ResizeXM()
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedItem as IGridResize;
            //    page?.ResizeXM();
            //}
            this.livestatusdata.ResizeXM();
            this.carsetupdata.ResizeXM();
        }

        public void ResizeMD()
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedItem as IGridResize;
            //    page?.ResizeMD();
            //}
            this.livestatusdata.ResizeMD();
            this.carsetupdata.ResizeMD();
        }

        public void ResizeLG()
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedItem as IGridResize;
            //    page?.ResizeLG();
            //}
            this.livestatusdata.ResizeLG();
            this.carsetupdata.ResizeLG();
        }

        public void ResizeXL()
        {
            //if (this.tabcontrol_content?.SelectedContent is IGridResize)
            //{
            //    var page = this.tabcontrol_content?.SelectedItem as IGridResize;
            //    page?.ResizeXL();
            //}
            this.livestatusdata.ResizeXL();
            this.carsetupdata.ResizeXL();
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
