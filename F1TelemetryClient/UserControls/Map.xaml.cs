using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.MotionPacket;
using F1TelemetryApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, IConnectUDP, IDisposable
    {
        private bool disposedValue;
        private bool isCarMotion;
        private List<Vector3> trackCoords = new List<Vector3>();
        private bool isLapdata;
        private bool canDraw;
        private int lapNumber;
        private bool canRecord = true;

        private double maxY = 0;
        private double maxX = 0;
        private double minX = 0;
        private double minY = 0;

        public Map()
        {
            InitializeComponent();
        }

        public void SubscribeUDPEvents()
        {
            //if (u.Connention != null)
            //{
            //    u.Connention.CarMotionPacket += Connention_CarMotionPacket;
            //    u.Connention.LapDataPacket += Connention_LapDataPacket;
            //}
        }

        public void UnsubscribeUDPEvents()
        {
            //if (u.Connention != null)
            //{
            //    u.Connention.CarMotionPacket -= Connention_CarMotionPacket;
            //    u.Connention.LapDataPacket -= Connention_LapDataPacket;
            //}
        }

        private void Connention_LapDataPacket(object sender, EventArgs e)
        {
            if (!this.isLapdata && sender != null)
            {
                this.isLapdata = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    var data = sender as PacketLapData;
                    this.UpdateLapdata(data);
                    this.isLapdata = false;
                }, DispatcherPriority.Background);
            }
        }

        private void UpdateLapdata(PacketLapData data)
        {
            if (this.canRecord)
            {
                var player = data.Lapdata[data.Header.Player1CarIndex];
                bool ok = player.DriverStatus == F1Telemetry.Helpers.Appendences.DriverSatuses.FlyingLap
                          || player.DriverStatus == F1Telemetry.Helpers.Appendences.DriverSatuses.OnTrack;

                if (ok && this.lapNumber == 0)
                {
                    this.canDraw = true;
                    this.lapNumber = player.CurrentLapNum;
                }

                if (!ok || this.lapNumber + 2 < player.CurrentLapNum)
                {
                    this.canDraw = false;
                    this.canRecord = false;
                    this.lapNumber = 0;

                    this.maxX = 0;
                    this.maxY = 0;
                    this.minX = 0;
                    this.minY = 0;
                }
            }
            else
            {
                this.canDraw = false;
            }
        }

        private void Connention_CarMotionPacket(object sender, EventArgs e)
        {
            if (!this.isCarMotion && sender != null)
            {
                this.isCarMotion = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    var data = sender as PacketMotionData;
                    this.UpdateCarMotion(data);
                    this.isCarMotion = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateCarMotion(PacketMotionData motion)
        {
            if (this.canDraw)
            {
                var player = motion.CarMotionData[motion.Header.Player1CarIndex];

                if (!this.trackCoords.Contains(player.WorldPosition))
                {
                    this.trackCoords.Add(player.WorldPosition);
                    //this.minX = 0;
                    //this.maxX = 0;
                    //this.minY = 0;
                    //this.maxX = 0;
                }
            }

            if (this.trackCoords.Count > 0)
            {
                this.path_track.Data = Geometry.Empty;
                var geometry = this.path_track.Data.GetFlattenedPathGeometry();
                var last = Vector3.Zero;

                double ax, ay;

                var x = this.trackCoords.Select(x => x.X);
                var y = this.trackCoords.Select(x => x.Z);

                var dx = Map.ImageMiltiplier(x, this.ActualWidth - 20, out ax);
                var dy = Map.ImageMiltiplier(y, this.ActualHeight - 20, out ay);

                var m = dx;
                if (m > dy) m = dy;

                foreach (var v in this.trackCoords)
                {
                    if (last != Vector3.Zero)
                    {
                        var tmp = new LineGeometry(
                                this.CalcPoint(last, ax, ay, m),
                                this.CalcPoint(v, ax, ay, m)
                            );
                        if (tmp.CanFreeze) tmp.Freeze();
                        geometry.AddGeometry(tmp);
                    }

                    last = v;
                }

                if (geometry.CanFreeze) geometry.Freeze();
                this.path_track.Data = geometry;

                if (this.trackCoords.Count > 1)
                {
                    this.canvas.Children.Clear();
                    foreach (var c in motion.CarMotionData)
                    {
                        var p = this.CalcPoint(c.WorldPosition, ax, ay, m, false);
                        this.canvas.Children.Add(new Ellipse
                        {
                            Margin = new Thickness(p.X - 5, p.Y - 5, 0, 0),
                            Width = 10,
                            Height = 10,
                            Fill = Brushes.Red,
                        });
                    }
                }
            }


        }

        private Point CalcPoint(Vector3 v, double midX, double midY, double multiply, bool canListenMinMax = true)
        {
            var x = 5 + (v.X - midX) * multiply;
            var y = 5 + (v.Z - midY) * multiply;

            if (canListenMinMax)
            {
                if (x > this.maxX) this.maxX = x;
                if (x < this.minY) this.minY = x;
                if (y > this.maxY) this.maxY = y;
                if (y < this.minY) this.minY = y;
            }

            var ax = (this.minX + this.maxX) / 2;
            var ay = (this.minY + this.maxY) / 2;

            return new Point(x + (this.ActualWidth - 20) / 2 - ax, y + (this.ActualHeight - 20) / 2 - ay);
        }

        private static double ImageMiltiplier(IEnumerable<float> x, double forWhat, out double a)
        {
            var min = x.Min();
            var max = x.Max();

            a = min;

            return forWhat / Math.Abs(max - min);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.UnsubscribeUDPEvents();
                    this.trackCoords = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Map()
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
