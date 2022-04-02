using F1Telemetry.Helpers;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.MotionPacket;
using F1Telemetry.Models.SessionPacket;
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
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, IConnectUDP, IDisposable
    {
        private bool disposedValue;
        private bool isSessionRunning;
        private Tracks trackID = Tracks.Unknown;
        private static Point zeroPoint = new Point();
        private static Brush drsColor = new SolidColorBrush(Color.FromArgb(60, 30, 200, 20));

        public Map()
        {
            InitializeComponent();
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
            if (!this.isSessionRunning && sender != null)
            {
                this.isSessionRunning = true;
                this.Dispatcher.BeginInvoke(() =>
                {
                    var data = sender as PacketSessionData;
                    this.UpdateSession(data);
                    this.isSessionRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateSession(PacketSessionData data)
        {
            if (data.TrackID != this.trackID)
            {
                this.trackID = data.TrackID;
                var t = TrackLayout.FindNearestMap(this.trackID.ToString(), data.Header.PacketFormat);

                if (t != null)
                {
                    var minX = t.BaseLine.Min(x => x.X);
                    var maxX = t.BaseLine.Max(x => x.X);
                    var minY = t.BaseLine.Min(x => x.Y);
                    var maxY = t.BaseLine.Max(x => x.Y);

                    double ax, ay;
                    var dx = u.ImageMiltiplier(t.BaseLine.Select(x => x.X), this.path_baseline.ActualWidth, out ax);
                    var dy = u.ImageMiltiplier(t.BaseLine.Select(x => x.Y), this.path_baseline.ActualHeight, out ay);

                    var m = dx;
                    if (m > dy) m = dy;

                    Map.DrawPath(this.path_baseline, ax, ay, m, minX, maxX, minY, maxY, t.BaseLine, 235);

                    Map.DrawPath(this.path_s1, ax, ay, m, minX, maxX, minY, maxY, t.SectorZones[0], 235);
                    Map.DrawPath(this.path_s2, ax, ay, m, minX, maxX, minY, maxY, t.SectorZones[1], 235);
                    Map.DrawPath(this.path_s3, ax, ay, m, minX, maxX, minY, maxY, t.SectorZones[2], 235);

                    if (t.MarshalZones.Count > 0)
                    {
                        this.grid_drs.Children.Clear();

                        foreach (var item in t.DRSZones.Values)
                        {
                            var p = Map.CreatePath(null);
                            this.grid_drs.Children.Add(p);
                            Map.DrawPath(p, ax, ay, m, minX, maxX, minY, maxY, item, 235);
                        }
                    }

                    if (t.DRSZones.Count > 0)
                    {
                        this.grid_drs.Children.Clear();

                        foreach (var item in t.DRSZones.Values)
                        {
                            var p = Map.CreatePath(Map.drsColor);
                            this.grid_drs.Children.Add(p);
                            Map.DrawPath(p, ax, ay, m, minX, maxX, minY, maxY, item, 235);
                        }
                    }
                }
            }

            for (int i = 0; i < this.grid_marshal.Children.Count; i++)
            {
                var p = this.grid_marshal.Children[i] as Path;
                p.Stroke = u.FlagColors[data.MarshalZones[i].ZoneFlag];
            }
        }

        private static Path CreatePath(Brush brush)
        {
            return new Path
            {
                Stroke = brush,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
            };
        }

        private static void DrawPath(
            Path p,
            double ax,
            double ay,
            double m,
            double minX,
            double maxX,
            double minY,
            double maxY,
            IEnumerable<Point> c,
            double maxWidth
        )
        {
            p.Data = Geometry.Empty;
            var geometry = p.Data.GetFlattenedPathGeometry();
            Point last = new Point();

            foreach (var v in c)
            {
                if (last != zeroPoint)
                {
                    var tmp = new LineGeometry(
                            Map.CalcPoint(last, ax, ay, m, minX, maxX, minY, maxY, maxWidth),
                            Map.CalcPoint(v, ax, ay, m, minX, maxX, minY, maxY, maxWidth)
                        );
                    if (tmp.CanFreeze) tmp.Freeze();
                    geometry.AddGeometry(tmp);
                }

                last = v;
            }

            if (geometry.CanFreeze) geometry.Freeze();
            p.Data = geometry;
        }

        private static Point CalcPoint(
            Point v,
            double midX,
            double midY,
            double multiply,
            double minX,
            double maxX,
            double minY,
            double maxY,
            double maxWidth
        )
        {
            var x = 5 + (v.X - midX) * multiply;
            var y = 5 + (v.Y - midY) * multiply;

            var ax = (minX + maxX) / 2;
            var ay = (minY + maxY) / 2;

            return new Point(x + maxWidth / 2 - ax, y + maxWidth / 2 - ay);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.SubscribeUDPEvents();
            if (Map.drsColor.CanFreeze) Map.drsColor.Freeze();
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
