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

        public Map()
        {
            InitializeComponent();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarMotionPacket += Connention_CarMotionPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.CarMotionPacket -= Connention_CarMotionPacket;
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
            var player = motion.CarMotionData[motion.Header.Player1CarIndex];

            if (!this.trackCoords.Contains(player.WorldPosition)) this.trackCoords.Add(player.WorldPosition);

            if (this.trackCoords.Count > 0)
            {
                this.path_track.Data = Geometry.Empty;
                var geometry = this.path_track.Data.GetFlattenedPathGeometry();
                var last = Vector3.Zero;

                var dx = Map.ImageMiltiplier(this.trackCoords.Select(x => x.X), this.ActualWidth);
                var dy = Map.ImageMiltiplier(this.trackCoords.Select(x => x.Y), this.ActualHeight);
                var a = this.ActualWidth / 2;

                var m = dx;
                if (m < dy) m = dy;

                foreach (var v in this.trackCoords)
                {
                    if (last != Vector3.Zero)
                    {
                        //var tmp = new LineGeometry(new Point(last.X * m + a, last.Z * m + a), new Point(v.X * m + a, last.Z * m + a));
                        var tmp = new LineGeometry(new Point(last.X * dx + a, last.Z * dy + a), new Point(v.X * dx + a, last.Z * dy + a));
                        if (tmp.CanFreeze) tmp.Freeze();
                        geometry.AddGeometry(tmp);
                    }

                    last = v;
                }

                if (geometry.CanFreeze) geometry.Freeze();
                this.path_track.Data = geometry;
            }

            //this.canvas.Children.Clear();
            //this.canvas.Children.Add(path);
        }

        private static double ImageMiltiplier(IEnumerable<float> x, double forWhat)
        {
            var minX = x.Min();
            var maxX = x.Max();
            return Math.Abs(maxX - minX) / forWhat;
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
