using F1Telemetry.Models.CarDamagePacket;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.CarTelemetryPacket;
using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls.TyreDisplay
{
    /// <summary>
    /// Interaction logic for TyreDataContainer.xaml
    /// </summary>
    public partial class TyreDataContainer : UserControl, INotifyPropertyChanged, IConnectUDP, IDisposable
    {
        private TyreCompounds actualTyreCpompund;

        public event PropertyChangedEventHandler PropertyChanged;

        public TyreCompounds ActualTyreCpompund
        {
            get { return actualTyreCpompund; }
            set
            {
                if (value != actualTyreCpompund)
                {
                    actualTyreCpompund = value;
                    TyreDataControl.ActualTyreCpompund = value;
                    this.Dispatcher.Invoke(() => this.image_tyre.Source = u.TyreCompoundToImage(this.actualTyreCpompund));
                }
            }
        }

        private int lapAges;

        public int LapAges
        {
            get { return lapAges; }
            set
            {
                if (value != lapAges)
                {
                    lapAges = value;
                    //this.textBlock_tyreAge.Text = this.lapAges.ToString();
                    this.OnPropertyChanged("LapAges");
                }
            }
        }

        private bool canUpdate;
        private int driverIndex;
        private bool disposedValue;

        public TyreDataContainer()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            //if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
        }

        internal void UpdateDatas(CarDamageData demage, CarStatusData status, CarTelemetryData telemetry, int index)
        {
            this.canUpdate = false;

            this.driverIndex = index;

            this.UpdateStatus(ref status);
            this.UpdateTelemetry(ref telemetry);
            this.UpdateDemage(ref demage);

            // this.UpdateLayout();

            this.canUpdate = true;
        }

        private void UpdateDemage(ref CarDamageData demage)
        {
            if (demage != null)
            {
                if (demage.TyreWear != null)
                {
                    var fl = Math.Round(demage.TyreWear["FrontLeft"], 2);
                    var fr = Math.Round(demage.TyreWear["FrontRight"], 2);
                    var rl = Math.Round(demage.TyreWear["RearLeft"], 2);
                    var rr = Math.Round(demage.TyreWear["RearRight"], 2);

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.Wear = fl;
                    this.tyredata_fr.Wear = fr;
                    this.tyredata_rl.Wear = rl;
                    this.tyredata_rr.Wear = rr;
                    //});
                }

                if (demage.TyreDemage != null)
                {
                    var fl = demage.TyreDemage["FrontLeft"];
                    var fr = demage.TyreDemage["FrontRight"];
                    var rl = demage.TyreDemage["RearLeft"];
                    var rr = demage.TyreDemage["RearRight"];

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.Demage = fl;
                    this.tyredata_fr.Demage = fr;
                    this.tyredata_rl.Demage = rl;
                    this.tyredata_rr.Demage = rr;
                    //});
                }

                if (demage.BrakesDemage != null)
                {
                    var fl = demage.BrakesDemage["FrontLeft"];
                    var fr = demage.BrakesDemage["FrontRight"];
                    var rl = demage.BrakesDemage["RearLeft"];
                    var rr = demage.BrakesDemage["RearRight"];

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.BrakeDemage = fl;
                    this.tyredata_fr.BrakeDemage = fr;
                    this.tyredata_rl.BrakeDemage = rl;
                    this.tyredata_rr.BrakeDemage = rr;
                    //});
                }

                // this.UpdateLayout();
            }
        }

        private void UpdateTelemetry(ref CarTelemetryData telemetry)
        {
            if (telemetry != null)
            {
                if (telemetry.TyresInnerTemperature != null)
                {
                    var fl = telemetry.TyresInnerTemperature["FrontLeft"];
                    var fr = telemetry.TyresInnerTemperature["FrontRight"];
                    var rl = telemetry.TyresInnerTemperature["RearLeft"];
                    var rr = telemetry.TyresInnerTemperature["RearRight"];

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.TyreInnerTemperature = fl;
                    this.tyredata_fr.TyreInnerTemperature = fr;
                    this.tyredata_rl.TyreInnerTemperature = rl;
                    this.tyredata_rr.TyreInnerTemperature = rr;
                    //});
                }

                if (telemetry.TyresSurfaceTemperature != null)
                {
                    var fl = telemetry.TyresSurfaceTemperature["FrontLeft"];
                    var fr = telemetry.TyresSurfaceTemperature["FrontRight"];
                    var rl = telemetry.TyresSurfaceTemperature["RearLeft"];
                    var rr = telemetry.TyresSurfaceTemperature["RearRight"];

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.TyreSurfaceTemperature = fl;
                    this.tyredata_fr.TyreSurfaceTemperature = fr;
                    this.tyredata_rl.TyreSurfaceTemperature = rl;
                    this.tyredata_rr.TyreSurfaceTemperature = rr;
                    //});
                }

                if (telemetry.TyresPressure != null)
                {
                    var fl = Math.Round(telemetry.TyresPressure["FrontLeft"], 2);
                    var fr = Math.Round(telemetry.TyresPressure["FrontRight"], 2);
                    var rl = Math.Round(telemetry.TyresPressure["RearLeft"], 2);
                    var rr = Math.Round(telemetry.TyresPressure["RearRight"], 2);

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.Pressure = fl;
                    this.tyredata_fr.Pressure = fr;
                    this.tyredata_rl.Pressure = rl;
                    this.tyredata_rr.Pressure = rr;
                    //});
                }

                if (telemetry.BrakesTemperature != null)
                {
                    var fl = telemetry.BrakesTemperature["FrontLeft"];
                    var fr = telemetry.BrakesTemperature["FrontRight"];
                    var rl = telemetry.BrakesTemperature["RearLeft"];
                    var rr = telemetry.BrakesTemperature["RearRight"];

                    //this.Dispatcher.Invoke(() =>
                    //{
                    this.tyredata_fl.BrakesTemperature = fl;
                    this.tyredata_fr.BrakesTemperature = fr;
                    this.tyredata_rl.BrakesTemperature = rl;
                    this.tyredata_rr.BrakesTemperature = rr;
                    //});
                }
            }
        }

        private void UpdateStatus(ref CarStatusData status)
        {
            if (status != null)
            {
                var atc = status.VisualTyreCompound;
                var tal = (int)status.TyresAgeLaps;

                //this.Dispatcher.Invoke(() =>
                //{
                this.ActualTyreCpompund = atc;
                this.LapAges = tal;
                //});
                // this.UpdateLayout();
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (u.Connention != null)
            {
                this.SubscribeUDPEvents();
            }
        }

        private void Connention_DemagePacket(PacketCarDamageData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateDemage(ref packet.CarDamageData[this.driverIndex]);
            });
        }

        private void Connention_CarTelemetryPacket(PacketCarTelemetryData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateTelemetry(ref packet.CarTelemetryData[this.driverIndex]);
            });
        }

        private void Connention_CarStatusPacket(PacketCarStatusData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateStatus(ref packet.CarStatusData[this.driverIndex]);
            });
        }

        private void OnUpdateEvent(Action method)
        {
            if (this.CanDoEvent())
            {
                this.canUpdate = false;
                if (this.driverIndex > -1) method.Invoke();
                this.canUpdate = true;
            }
        }

        private bool CanDoEvent()
        {
            bool ret = false;
            this.Dispatcher.Invoke(() => ret = this.canUpdate && this.IsLoaded);
            return ret;
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.CarStatusPacket += Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket += Connention_CarTelemetryPacket;
            u.Connention.CarDemagePacket += Connention_DemagePacket;
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.CarStatusPacket -= Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket -= Connention_CarTelemetryPacket;
            u.Connention.CarDemagePacket -= Connention_DemagePacket;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                    this.PropertyChanged = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TyreDataContainer()
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
