using F1Telemetry.Models.CarDamagePacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using F1TelemetryApp.UserControls;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryApp.Pages.PersonalInfo.InnerDetails
{
    /// <summary>
    /// Interaction logic for LiveStatusPage.xaml
    /// </summary>
    public partial class LiveStatusPage : UserControl, IConnectUDP, IGridResize, IDisposable
    {
        private bool isWorking_SessionData;
        private bool isWorking_DemageData;
        private bool disposedValue;

        public LiveStatusPage()
        {
            InitializeComponent();
        }

        private void tyrecontainer_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
            this.LoadWear();
        }

        private void tyrecontainer_Loaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                //u.Connention.SessionPacket += Connention_SessionPacket;
                u.Connention.CarDemagePacket += Connention_DemagePacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                //u.Connention.SessionPacket -= Connention_SessionPacket;
                u.Connention.CarDemagePacket -= Connention_DemagePacket;
            }
        }

        private void Connention_DemagePacket(object sender, EventArgs e)
        {
            if (!this.isWorking_DemageData && u.CanDoUdp)
            {
                this.isWorking_DemageData = true;
                this.Dispatcher.Invoke(() =>
                {
                    if (u.SelectedItem != null)
                    {
                        //this.LoadWear();
                        //int i = this.listBox_drivers.SelectedIndex;
                        int i = (u.SelectedItem as PlayerListItemData).ArrayIndex;
                        var data = sender as PacketCarDamageData;
                        if (i > -1) this.UpdateDemageInfo(data.CarDamageData[i]);
                    }

                    this.isWorking_DemageData = false;
                }, DispatcherPriority.Render);
            }
        }

        //private void Connention_SessionPacket(object sender, EventArgs e)
        //{
        //    if (!this.isWorking_SessionData && u.CanDoUdp)
        //    {
        //        this.isWorking_SessionData = true;
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            var sessionData = sender as PacketSessionData;
        //            var map = TrackLayout.FindNearestMap(sessionData.TrackID.ToString(), sessionData.Header.PacketFormat);
        //            //this.tyrecontainer.UpdateTyres(this.map.RawTrack);
        //            TyreDataControl.UpdateTyres(map);

        //            this.isWorking_SessionData = false;

        //        },DispatcherPriority.Render);
        //    }
        //}

        internal void CalculateView(GridSizes res)
        {
            this.drivercontainer.CalculateView(res);
        }

        private void UpdateDemageInfo(CarDamageData demage)
        {
            if (demage != null)
            {
                this.demage_fwLeft.Percent = 100.0 - demage.FrontLeftWingDemage;
                this.demage_fwRight.Percent = 100.0 - demage.FrontRightWingDemage;
                this.demage_fl.Percent = 100.0 - demage.FloorDemage;
                this.demage_df.Percent = 100.0 - demage.DiffurerDemage;
                this.demage_en.Percent = 100.0 - demage.EngineDemage;
                this.demage_gb.Percent = 100.0 - demage.GearBoxDemage;
                this.demage_rw.Percent = 100.0 - demage.RearWingDemage;
                this.demage_sp.Percent = 100.0 - demage.SidepodDemage;
                this.demage_ex.Percent = 100.0 - demage.ExhasutDemage;

                this.wear_ce.Percent = 100.0 - demage.EngineCEWear;
                this.wear_es.Percent = 100.0 - demage.EngineESWear;
                this.wear_ice.Percent = 100.0 - demage.EngineICEWear;
                this.wear_mguh.Percent = 100.0 - demage.EngineMGUKWear;
                this.wear_mguk.Percent = 100.0 - demage.EngineMGUKWear;
                this.wear_tc.Percent = 100.0 - demage.EngineTCWear;
            }
            else
            {
                this.demage_fwLeft.Percent = double.NaN;
                this.demage_fwRight.Percent = double.NaN;
                this.demage_fl.Percent = double.NaN;
                this.demage_df.Percent = double.NaN;
                this.demage_en.Percent = double.NaN;
                this.demage_gb.Percent = double.NaN;
                this.demage_rw.Percent = double.NaN;
                this.demage_sp.Percent = double.NaN;
                this.demage_ex.Percent = double.NaN;

                this.wear_ce.Percent = double.NaN;
                this.wear_es.Percent = double.NaN;
                this.wear_ice.Percent = double.NaN;
                this.wear_mguh.Percent = double.NaN;
                this.wear_mguk.Percent = double.NaN;
                this.wear_tc.Percent = double.NaN;
            }
        }

        internal void LoadWear()
        {
            //if (this.listBox_drivers.SelectedIndex != -1)
            if (u.SelectedPlayer != null && u.Connention != null)
            {
                //int i = (this.listBox_drivers.SelectedItem as PlayerListItemData).ArrayIndex;
                int i = u.SelectedPlayer.ArrayIndex;
                var demage = u.Connention.LastCarDemagePacket?.CarDamageData[i];
                var status = u.Connention.LastCarStatusDataPacket?.CarStatusData[i];
                var telemetry = u.Connention.LastCarTelmetryPacket?.CarTelemetryData[i];
                var sessionHistory = u.Connention?.LastSessionHistoryPacket[i];
                var participants = u.Connention?.LastParticipantsPacket;
                var gForce = u.Connention?.LastMotionPacket?.CarMotionData[i]?.GForce ?? Vector3.Zero;
                var lapdata = u.Connention?.LastLapDataPacket;

                this.tyrecontainer.UpdateDatas(demage, status, telemetry, i);
                this.drivercontainer.UpdateDatas(status, telemetry, sessionHistory, participants, gForce, lapdata, i);
                this.UpdateDemageInfo(demage);
            }
        }

        public void ResizeXS()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 12);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 3, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 6);
            IGridResize.SetGridSettings(this.demage_fwRight, 0, 1, 6);
            IGridResize.SetGridSettings(this.demage_fl, 0, 2, 6);
            IGridResize.SetGridSettings(this.demage_sp, 0, 3, 6);
            IGridResize.SetGridSettings(this.demage_en, 0, 4, 6);
            IGridResize.SetGridSettings(this.demage_ex, 0, 5, 6);
            IGridResize.SetGridSettings(this.demage_gb, 0, 6, 6);
            IGridResize.SetGridSettings(this.demage_df, 0, 7, 6);
            IGridResize.SetGridSettings(this.demage_rw, 0, 8, 6);
        }

        public void ResizeXM()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 12);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 3, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 3);
            IGridResize.SetGridSettings(this.wear_ice, 3, 0, 3);
            IGridResize.SetGridSettings(this.wear_tc, 0, 1, 3);
            IGridResize.SetGridSettings(this.wear_mguh, 3, 1, 3);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 2, 3);
            IGridResize.SetGridSettings(this.wear_es, 3, 2, 3);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeMD()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 12);
            IGridResize.SetGridSettings(this.groupbox_demage, 0, 2, 7);
            IGridResize.SetGridSettings(this.groupbox_motor, 7, 2, 5);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 6);
            IGridResize.SetGridSettings(this.demage_ex, 0, 3, 6);
            IGridResize.SetGridSettings(this.demage_gb, 0, 4, 6);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeLG()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 7);
            IGridResize.SetGridSettings(this.groupbox_demage, 7, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_motor, 0, 2, 12);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 1);
            IGridResize.SetGridSettings(this.wear_ice, 1, 0, 1);
            IGridResize.SetGridSettings(this.wear_tc, 2, 0, 1);
            IGridResize.SetGridSettings(this.wear_mguh, 3, 0, 1);
            IGridResize.SetGridSettings(this.wear_mguk, 4, 0, 1);
            IGridResize.SetGridSettings(this.wear_es, 5, 0, 1);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
        }

        public void ResizeXL()
        {
            // Complete view components:
            IGridResize.SetGridSettings(this.drivercontainer, 0, 0, 12);
            IGridResize.SetGridSettings(this.tyrecontainer, 0, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_demage, 5, 1, 5);
            IGridResize.SetGridSettings(this.groupbox_motor, 10, 1, 2);

            // Engine wear components:
            IGridResize.SetGridSettings(this.wear_ce, 0, 0, 6);
            IGridResize.SetGridSettings(this.wear_ice, 0, 1, 6);
            IGridResize.SetGridSettings(this.wear_tc, 0, 2, 6);
            IGridResize.SetGridSettings(this.wear_mguh, 0, 3, 6);
            IGridResize.SetGridSettings(this.wear_mguk, 0, 4, 6);
            IGridResize.SetGridSettings(this.wear_es, 0, 5, 6);

            // Demage components:
            IGridResize.SetGridSettings(this.demage_fwLeft, 0, 0, 3);
            IGridResize.SetGridSettings(this.demage_fwRight, 3, 0, 3);
            IGridResize.SetGridSettings(this.demage_fl, 0, 1, 3);
            IGridResize.SetGridSettings(this.demage_sp, 3, 1, 3);
            IGridResize.SetGridSettings(this.demage_en, 0, 2, 2);
            IGridResize.SetGridSettings(this.demage_ex, 2, 2, 2);
            IGridResize.SetGridSettings(this.demage_gb, 4, 2, 2);
            IGridResize.SetGridSettings(this.demage_df, 0, 5, 3);
            IGridResize.SetGridSettings(this.demage_rw, 3, 5, 3);
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
        // ~LiveStatusPage()
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
