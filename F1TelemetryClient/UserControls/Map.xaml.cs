using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static F1Telemetry.Helpers.Appendences;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Linq;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.MotionPacket;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.LapDataPacket;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl, IConnectUDP, IDisposable
    {
        internal Tracks TrackID { get; set; } = Tracks.Unknown;
        internal TrackLayout RawTrack { get; private set; }

        private bool disposedValue;
        private bool isSessionRunning;
        private static Point zeroPoint = new Point();
        private static Brush drsColor = new SolidColorBrush(Color.FromArgb(130, 0, 255, 0));
        private bool isParticipantsRunning;
        private bool isCarmotionRunning;
        private double maxWidth;
        private double midX;
        private double midY;
        private double multiply;
        private bool isStatusRunning;
        private bool isLapdataRunning;

        public Map()
        {
            InitializeComponent();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket += Connention_SessionPacket;
                u.Connention.ParticipantsPacket += Connention_ParticipantsPacket;
                u.Connention.CarMotionPacket += Connention_CarMotionPacket;
                u.Connention.CarStatusPacket += Connention_CarStatusPacket;
                u.Connention.LapDataPacket += Connention_LapDataPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket -= Connention_SessionPacket;
                u.Connention.ParticipantsPacket -= Connention_ParticipantsPacket;
                u.Connention.CarMotionPacket -= Connention_CarMotionPacket;
                u.Connention.CarStatusPacket -= Connention_CarStatusPacket;
                u.Connention.LapDataPacket -= Connention_LapDataPacket;
            }
        }

        private void Connention_LapDataPacket(object sender, EventArgs e)
        {
            if (!this.isLapdataRunning && sender != null)
            {
                this.isLapdataRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketLapData;
                    this.UpdateLapdata(data);
                    this.isLapdataRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateLapdata(PacketLapData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data.Lapdata.Length)
            {
                for (int i = 0; i < gridCars.Count; i++)
                {
                    var e = gridCars[i] as Ellipse;
                    var c = data.Lapdata[i];

                    switch (c.DriverStatus)
                    {
                        case DriverSatuses.Unknown:
                        case DriverSatuses.InGarage:
                            e.Visibility = Visibility.Hidden;
                            break;
                        default:
                            switch (c.ResultStatus)
                            {
                                default:
                                    e.Visibility = Visibility.Visible;
                                    break;
                                case ResultSatuses.Disqualified:
                                case ResultSatuses.DidNotFinish:
                                case ResultSatuses.Finished:
                                case ResultSatuses.Retired:
                                case ResultSatuses.NotClassiFied:
                                    e.Visibility = Visibility.Hidden;
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        private void Connention_CarStatusPacket(object sender, EventArgs e)
        {
            if (!this.isStatusRunning && sender != null)
            {
                this.isStatusRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketCarStatusData;
                    this.UpdateCarStatus(data);
                    this.isStatusRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateCarStatus(PacketCarStatusData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data.CarStatusData.Length)
            {
                for (int i = 0; i < gridCars.Count; i++)
                {
                    var e = gridCars[i] as Ellipse;
                    var c = data.CarStatusData[i];
                    var b = u.FlagColors[c.VehicleFIAFlag] as SolidColorBrush;

                    if (b.Color == Brushes.Transparent.Color) b = Map.BorderColor((SolidColorBrush)e.Fill);

                    if (b.CanFreeze) b.Freeze();
                    e.Stroke = b;
                }
            }
        }

        private void Connention_CarMotionPacket(object sender, EventArgs e)
        {
            if (!this.isCarmotionRunning && sender != null)
            {
                this.isCarmotionRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketMotionData;
                    this.UpdateMotion(data);
                    this.isCarmotionRunning = false;
                }
                , DispatcherPriority.Render);
            }
        }

        private void UpdateMotion(PacketMotionData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data.CarMotionData.Length)
            {
                for (int i = 0; i < data.CarMotionData.Length; i++)
                {
                    var item = data.CarMotionData[i];

                    var x = item.WorldPosition.X;
                    var y = item.WorldPosition.Z;
                    var p = this.CalcPoint(new Point(x, y));

                    var e = gridCars[i] as Ellipse;
                    e.Margin = new Thickness
                    {
                        Left = p.X - e.ActualWidth / 2,
                        Top = p.Y - e.ActualHeight / 2,
                    };
                }
            }
        }

        private void Connention_ParticipantsPacket(object sender, EventArgs e)
        {
            if (!this.isParticipantsRunning && sender != null)
            {
                this.isParticipantsRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketParticipantsData;
                    this.UpdateParticipants(data);
                    this.isParticipantsRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateParticipants(PacketParticipantsData data)
        {
            var carsGrid = this.grid_cars.Children;

            if (data.Participants.Length != carsGrid.Count) carsGrid.Clear();

            if (carsGrid.Count == 0)
            {
                Brush b = Brushes.Transparent;

                if (b.CanFreeze) b.Freeze();

                foreach (var p in data.Participants)
                {
                    var teamBrush = u.PickTeamColor(p.TeamID);
                    b = Map.BorderColor((SolidColorBrush)teamBrush);

                    var e = Map.CreateEllipse(teamBrush, b);
                    e.ToolTip = p.Name + " | " + p.RaceNumber + "\r\n" + u.PickTeamName(p.TeamID);
                    carsGrid.Add(e);
                }
            }
        }

        private void Connention_SessionPacket(object sender, EventArgs e)
        {
            if (!this.isSessionRunning && sender != null)
            {
                this.isSessionRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    var data = sender as PacketSessionData;
                    this.UpdateSession(data);
                    this.isSessionRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateSession(PacketSessionData data)
        {
            if (data.TrackID != this.TrackID)
            {
                this.TrackID = data.TrackID;
                this.RawTrack = TrackLayout.FindNearestMap(this.TrackID.ToString(), data.Header.PacketFormat);

                if (RawTrack != null && RawTrack.BaseLine?.Count != 0)
                {
                    this.maxWidth = this.ActualWidth - 30;

                    double ax, ay;
                    var dx = u.ImageMiltiplier(RawTrack.BaseLine.Select(x => x.X), this.maxWidth, out ax);
                    var dy = u.ImageMiltiplier(RawTrack.BaseLine.Select(x => x.Y), this.maxWidth, out ay);

                    var m = dx;
                    if (m > dy) m = dy;

                    this.multiply = m;
                    this.midX = ax;
                    this.midY = ay;

                    this.DrawPath(this.path_baseline, RawTrack.BaseLine);

                    this.DrawPath(this.path_s1, RawTrack.SectorZones[0]);
                    this.DrawPath(this.path_s2, RawTrack.SectorZones[1]);
                    this.DrawPath(this.path_s3, RawTrack.SectorZones[2]);

                    if (RawTrack.MarshalZones.Count > 0)
                    {
                        this.grid_marshal.Children.Clear();

                        foreach (var item in RawTrack.MarshalZones.Values)
                        {
                            var p = Map.CreatePath(null);
                            this.grid_marshal.Children.Add(p);
                            this.DrawPath(p, item);
                        }
                    }

                    if (RawTrack.DRSZones.Count > 0)
                    {
                        this.grid_drs.Children.Clear();

                        foreach (var item in RawTrack.DRSZones.Values)
                        {
                            var p = Map.CreatePath(Map.drsColor);
                            this.grid_drs.Children.Add(p);
                            this.DrawPath(p, item);
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

        private static Ellipse CreateEllipse(Brush fill, Brush stroke)
        {
            return new Ellipse
            {
                Fill = fill,
                Stroke = stroke,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
        }

        private static Path CreatePath(Brush brush)
        {
            return new Path
            {
                Stroke = brush,
                StrokeThickness = 3,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
            };
        }

        private void DrawPath(
            Path p,
            IEnumerable<Point> c
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
                            this.CalcPoint(last),
                            this.CalcPoint(v)
                        );
                    if (tmp.CanFreeze) tmp.Freeze();
                    geometry.AddGeometry(tmp);
                }

                last = v;
            }

            if (geometry.CanFreeze) geometry.Freeze();
            p.Data = geometry;
        }

        private Point CalcPoint(Point v)
        {
            //if (v.X == this.maxx) ;

            var d = maxWidth / 2;
            var x = (v.X + midX * -1) * multiply + d;
            var y = (v.Y + midY * -1) * multiply + d;
            return new Point(10 + x, 10 + y);
        }

        private static SolidColorBrush BorderColor(SolidColorBrush brush)
        {
            SolidColorBrush ret = null;
            const byte alpha = 255;
            const byte val = 40;

            byte r = brush.Color.R;
            byte g = brush.Color.G;
            byte b = brush.Color.B;

            if (brush.Color.IsLightColor())
            {
                if (r - val > 0) r -= val;
                if (g - val > 0) g -= val;
                if (b - val > 0) b -= val;
            }
            else
            {
                if (r + val < 255) r += val;
                if (g + val < 255) g += val;
                if (b + val < 255) b += val;
            }

            ret = new SolidColorBrush(Color.FromArgb(alpha, r, g, b));
            if (ret != null && ret.CanFreeze) ret.Freeze();

            return ret;
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
