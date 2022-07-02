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
using F1Telemetry.Models;
using F1Telemetry.Helpers;

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
        private double maxHight;
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

        private bool CanUpdateEvent()
        {
            bool ret = false;

            this.Dispatcher.Invoke(() => ret = this.IsLoaded && this.IsEnabled);

            return ret;
        }

        private void Connention_LapDataPacket(PacketLapData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isLapdataRunning && packet != null)
            {
                this.isLapdataRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    this.UpdateLapdata(ref packet);
                    this.isLapdataRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateLapdata(ref PacketLapData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data.Lapdata.Length)
            {
                for (int i = 0; i < gridCars.Count; i++)
                {
                    var e = gridCars[i] as Border;
                    var c = data.Lapdata[i];

                    Panel.SetZIndex(e, 100 - c.CarPosition);

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

        private void Connention_CarStatusPacket(PacketCarStatusData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isStatusRunning && packet != null)
            {
                this.isStatusRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    this.UpdateCarStatus(ref packet);
                    this.isStatusRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateCarStatus(ref PacketCarStatusData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data.CarStatusData.Length)
            {
                for (int i = 0; i < gridCars.Count; i++)
                {
                    var c = data.CarStatusData[i];
                    var b = u.FlagColors[c.VehicleFIAFlag] as SolidColorBrush;

                    this.Dispatcher.Invoke(() =>
                    {
                        var e = gridCars[i] as Border;

                        if (b.Color == Brushes.Transparent.Color) b = Map.BorderColor((SolidColorBrush)e.Background);
                        if (b.CanFreeze) b.Freeze();

                        e.BorderBrush = b;
                    });
                }
            }
        }

        private void Connention_CarMotionPacket(PacketMotionData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isCarmotionRunning && packet != null)
            {
                this.isCarmotionRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    this.UpdateMotion(ref packet);
                    this.isCarmotionRunning = false;
                }
                , DispatcherPriority.Render);
            }
        }

        private void UpdateMotion(ref PacketMotionData data)
        {
            var gridCars = this.grid_cars.Children;
            if (gridCars.Count == data?.CarMotionData?.Length)
            {
                for (int i = 0; i < data?.CarMotionData?.Length; i++)
                {
                    var item = data.CarMotionData[i];

                    var x = item.WorldPosition.X;
                    var y = item.WorldPosition.Z;
                    var p = this.CalcPoint(new Point(x, y));

                    var e = gridCars[i] as Border;
                    e.Margin = new Thickness
                    {
                        Left = p.X - e.ActualWidth / 2,
                        Top = p.Y - e.ActualHeight / 2,
                    };
                }
            }
        }

        private void Connention_ParticipantsPacket(PacketParticipantsData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isParticipantsRunning && packet != null)
            {
                this.isParticipantsRunning = true;
                this.Dispatcher.Invoke(() =>
                {
                    this.UpdateParticipants(ref packet);
                    this.isParticipantsRunning = false;
                }, DispatcherPriority.Render);
            }
        }

        private void UpdateParticipants(ref PacketParticipantsData data)
        {
            var carsGrid = this.grid_cars.Children;

            if (data.Participants.Length != carsGrid.Count) carsGrid.Clear();

            if (carsGrid.Count == 0)
            {
                foreach (var p in data.Participants)
                {
                    var teamBrush = u.PickTeamColor(p.TeamID);
                    var b = Map.BorderColor(teamBrush);


                    var e = Map.CreateEllipse(teamBrush, b, p.RaceNumber);
                    e.ToolTip = p.ParticipantName + " | " + p.RaceNumber + "\r\n" + p.TeamID.GetTeamName(data.Header.PacketFormat);
                    carsGrid.Add(e);
                }
            }
            else
            {
                for (int i = 0; i < carsGrid.Count; i++)
                {
                    var teamBrush = u.PickTeamColor(data.Participants[i].TeamID);
                    var b = Map.BorderColor(teamBrush);
                    if (b.CanFreeze) b.Freeze();

                    string s = data.Participants[i].ParticipantName + " | " + data.Participants[i].RaceNumber + "\r\n" + data.Participants[i].TeamID.GetTeamName(data.Header.PacketFormat);
                    this.Dispatcher.Invoke(() => ((Border)carsGrid[i]).ToolTip = s);
                }
            }
        }

        private void Connention_SessionPacket(PacketSessionData packet, EventArgs e)
        {
            if (this.CanUpdateEvent() && !this.isSessionRunning && packet != null)
            {
                this.isSessionRunning = true;
                //this.Dispatcher.Invoke(() =>
                //{
                this.UpdateSession(ref packet);
                this.isSessionRunning = false;
                //}, DispatcherPriority.Render);
            }
        }

        private void UpdateSession(ref PacketSessionData data, bool forced = false)
        {
            if (data?.TrackID != this.TrackID || forced)
            {
                this.TrackID = data?.TrackID ?? Tracks.Unknown;
                this.RawTrack = TrackLayout.FindNearestMap(this.TrackID.ToString(), data?.Header?.PacketFormat ?? 0);

                if (RawTrack != null && RawTrack.BaseLine?.Count != 0)
                {
                    this.CalcMaxPoints();

                    double ax, ay;
                    var dx = u.ImageMiltiplier(RawTrack.BaseLine.Select(x => x.X), this.maxWidth, out ax);
                    var dy = u.ImageMiltiplier(RawTrack.BaseLine.Select(x => x.Y), this.maxHight, out ay);

                    var m = dx;
                    if (m > dy) m = dy;

                    this.multiply = m;
                    this.midX = ax;
                    this.midY = ay;

                    this.Dispatcher.Invoke(() =>
                    {
                        this.DrawPath(this.path_baseline, RawTrack.BaseLine);

                        this.DrawPath(this.path_s1, RawTrack.SectorZones[0]);
                        this.DrawPath(this.path_s2, RawTrack.SectorZones[1]);
                        this.DrawPath(this.path_s3, RawTrack.SectorZones[2]);
                    });

                    if (RawTrack.MarshalZones.Count > 0)
                    {
                        this.Dispatcher.Invoke(() => this.grid_marshal.Children.Clear());

                        foreach (var item in RawTrack.MarshalZones.Values)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                var p = Map.CreatePath(null);
                                this.grid_marshal.Children.Add(p);
                                this.DrawPath(p, item);
                            });
                        }
                    }

                    if (RawTrack.DRSZones.Count > 0)
                    {
                        this.Dispatcher.Invoke(() => this.grid_drs.Children.Clear());

                        foreach (var item in RawTrack.DRSZones.Values)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                var p = Map.CreatePath(Map.drsColor);
                                this.grid_drs.Children.Add(p);
                                this.DrawPath(p, item);
                            });
                        }
                    }
                }
            }

            var d = data?.MarshalZones;
            this.Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < this.grid_marshal.Children.Count; i++)
                {
                    var p = this.grid_marshal.Children[i] as Path;
                    p.Stroke = u.FlagColors[d[i].ZoneFlag];
                }
            });
        }

        private void CalcMaxPoints()
        {
            this.maxWidth = this.ActualWidth - 30;
            this.maxHight = this.ActualHeight - 30;
        }

        private static Border CreateEllipse(Brush fill, Brush stroke, int raceNumber)
        {
            SolidColorBrush text = null;

            if (fill is SolidColorBrush)
            {
                if ((fill as SolidColorBrush).Color.IsLightColor()) text = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                else text = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }

            return new Border
            {
                Background = fill,
                BorderBrush = stroke,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(10),
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Child = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = raceNumber.ToString(),
                    Foreground = text,
                    TextWrapping = TextWrapping.NoWrap,
                    FontSize = 7,
                },
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

            var dx = this.maxWidth / 2;
            var dy = this.maxHight / 2;

            var x = (v.X + midX * -1) * multiply + dx;
            var y = (v.Y + midY * -1) * multiply + dy;

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

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (this.ActualHeight < 60)
                {
                    if (this.IsEnabled)
                    {
                        this.IsEnabled = false;
                        this.grid_content.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    if (!this.IsEnabled)
                    {
                        this.IsEnabled = true;
                        this.grid_content.Visibility = Visibility.Visible;
                    }
                }

                if (this.IsEnabled)
                {
                    this.isSessionRunning = true;
                    this.isCarmotionRunning = true;

                    var ses = u.Connention?.CurrentSessionDataPacket;
                    var mot = u.Connention?.CurrentMotionPacket;

                    this.UpdateSession(ref ses, true);
                    this.UpdateMotion(ref mot);

                    this.isSessionRunning = false;
                    this.isCarmotionRunning = false;
                }
            }
        }
    }
}
