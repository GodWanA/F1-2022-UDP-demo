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
                    this.image_tyre.Source = u.TyreCompoundToImage(this.actualTyreCpompund);
                }
            }
        }

        private int lapAges;

        public int LapAges
        {
            get { return lapAges; }
            set
            {
                lapAges = value;
                //this.textBlock_tyreAge.Text = this.lapAges.ToString();
                this.OnPropertyChanged("LapAges");
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
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void UpdateDatas(CarDamageData demage, CarStatusData status, CarTelemetryData telemetry, int index)
        {
            this.canUpdate = false;

            this.driverIndex = index;

            this.UpdateStatus(status);
            this.UpdateTelemetry(telemetry);
            this.UpdateDemage(demage);

            this.UpdateLayout();

            this.canUpdate = true;
        }

        private void UpdateDemage(CarDamageData demage)
        {
            if (demage != null)
            {
                this.tyredata_fl.Wear = demage.TyreWear["FrontLeft"];
                this.tyredata_fr.Wear = demage.TyreWear["FrontRight"];
                this.tyredata_rl.Wear = demage.TyreWear["RearLeft"];
                this.tyredata_rr.Wear = demage.TyreWear["RearRight"];

                this.tyredata_fl.Demage = demage.TyreDemage["FrontLeft"];
                this.tyredata_fr.Demage = demage.TyreDemage["FrontRight"];
                this.tyredata_rl.Demage = demage.TyreDemage["RearLeft"];
                this.tyredata_rr.Demage = demage.TyreDemage["RearRight"];

                this.tyredata_fl.BrakeDemage = demage.BrakesDemage["FrontLeft"];
                this.tyredata_fr.BrakeDemage = demage.BrakesDemage["FrontRight"];
                this.tyredata_rl.BrakeDemage = demage.BrakesDemage["RearLeft"];
                this.tyredata_rr.BrakeDemage = demage.BrakesDemage["RearRight"];

                this.UpdateLayout();
            }
        }

        private void UpdateTelemetry(CarTelemetryData telemetry)
        {
            if (telemetry != null)
            {
                this.tyredata_fl.TyreInnerTemperature = telemetry.TyresInnerTemperature["FrontLeft"];
                this.tyredata_fr.TyreInnerTemperature = telemetry.TyresInnerTemperature["FrontRight"];
                this.tyredata_rl.TyreInnerTemperature = telemetry.TyresInnerTemperature["RearLeft"];
                this.tyredata_rr.TyreInnerTemperature = telemetry.TyresInnerTemperature["RearRight"];

                this.tyredata_fl.TyreSurfaceTemperature = telemetry.TyresSurfaceTemperature["FrontLeft"];
                this.tyredata_fr.TyreSurfaceTemperature = telemetry.TyresSurfaceTemperature["FrontRight"];
                this.tyredata_rl.TyreSurfaceTemperature = telemetry.TyresSurfaceTemperature["RearLeft"];
                this.tyredata_rr.TyreSurfaceTemperature = telemetry.TyresSurfaceTemperature["RearRight"];

                this.tyredata_fl.Pressure = telemetry.TyresPressure["FrontLeft"];
                this.tyredata_fr.Pressure = telemetry.TyresPressure["FrontRight"];
                this.tyredata_rl.Pressure = telemetry.TyresPressure["RearLeft"];
                this.tyredata_rr.Pressure = telemetry.TyresPressure["RearRight"];

                this.tyredata_fl.BrakesTemperature = telemetry.BrakesTemperature["FrontLeft"];
                this.tyredata_fr.BrakesTemperature = telemetry.BrakesTemperature["FrontRight"];
                this.tyredata_rl.BrakesTemperature = telemetry.BrakesTemperature["RearLeft"];
                this.tyredata_rr.BrakesTemperature = telemetry.BrakesTemperature["RearRight"];

                this.UpdateLayout();
            }
        }

        private void UpdateStatus(CarStatusData status)
        {
            if (status != null)
            {
                this.ActualTyreCpompund = status.VisualTyreCompound;
                this.LapAges = (int)status.TyresAgeLaps;

                this.UpdateLayout();
            }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            if (u.Connention != null)
            {
                this.SubscribeUDPEvents();
            }
        }

        private void Connention_DemagePacket(object sender, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                var data = sender as PacketCarDamageData;
                this.UpdateDemage(data.CarDamageData[this.driverIndex]);
            });
        }

        private void Connention_CarTelemetryPacket(object sender, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                var data = sender as PacketCarTelemetryData;
                this.UpdateTelemetry(data.CarTelemetryData[this.driverIndex]);
            });
        }

        private void Connention_CarStatusPacket(object sender, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                var data = sender as PacketCarStatusData;
                this.UpdateStatus(data.CarStatusData[this.driverIndex]);
            });
        }

        private void OnUpdateEvent(Action method)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.canUpdate && this.IsLoaded)
                {
                    this.canUpdate = false;
                    if (this.driverIndex > -1) method.Invoke();
                    this.canUpdate = true;
                }
            }, DispatcherPriority.Background);
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.CarStatusPacket += Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket += Connention_CarTelemetryPacket;
            u.Connention.DemagePacket += Connention_DemagePacket;
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.CarStatusPacket -= Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket -= Connention_CarTelemetryPacket;
            u.Connention.DemagePacket -= Connention_DemagePacket;
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
