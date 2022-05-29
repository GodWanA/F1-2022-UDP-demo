using F1TelemetryApp.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls.Session
{
    /// <summary>
    /// Interaction logic for SessionHead.xaml
    /// </summary>
    public partial class SessionHead : UserControl, IDisposable, IConnectUDP, INotifyPropertyChanged
    {
        private bool disposedValue;
        private bool isWorkingSession;
        private bool isWorkingLapdata;
        private TimeSpan _sessionTimeLeft;
        private TrackLayout track;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _trackName;

        public string TrackName
        {
            get { return this._trackName; }
            set
            {
                if (value != this._trackName)
                {
                    this._trackName = value;
                    this.OnPropertyChanged("TrackName");
                }
            }
        }

        private string _sessionName;

        public string SessionName
        {
            get { return this._sessionName; }
            set
            {
                if (value != this._sessionName)
                {
                    this._sessionName = value;
                    this.OnPropertyChanged("SessionName");
                }
            }
        }

        private int _currentLap;

        public int CurrentLap
        {
            get { return this._currentLap; }
            set
            {
                if (value != this._currentLap)
                {
                    this._currentLap = value;
                    this.OnPropertyChanged("CurrentLap");
                }
            }
        }

        private int _totalLaps;

        public int TotalLaps
        {
            get { return this._totalLaps; }
            set
            {
                if (value != this._totalLaps)
                {
                    this._totalLaps = value;
                    this.OnPropertyChanged("TotalLaps");
                }
            }
        }

        private double _timeLeft;

        public double TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                if (value != this._timeLeft)
                {
                    this._timeLeft = value;
                    this.OnPropertyChanged("TimeLeft");
                }
            }
        }

        private SessionTypes session;
        private SolidColorBrush _flagColor;

        public SolidColorBrush FlagColor
        {
            get { return _flagColor; }
            set
            {
                if (value != this._flagColor)
                {
                    this._flagColor = value;
                    this.OnPropertyChanged("FlagColor");
                }
            }
        }

        private string _flagName;

        public string FlagName
        {
            get { return _flagName; }
            set
            {
                if (value != this._flagName)
                {
                    this._flagName = value;
                    this.OnPropertyChanged("FlagName");
                }
            }
        }

        private string _flagLocation;
        private bool isRaceOver;

        public string FlagLocation
        {
            get { return _flagLocation; }
            set
            {
                if (value != this._flagLocation)
                {
                    this._flagLocation = value;
                    this.OnPropertyChanged("FlagLocation");
                }
            }
        }


        public TimeSpan SessionTimeLeft
        {
            get { return _sessionTimeLeft; }
            set
            {
                if (value != this._sessionTimeLeft)
                {
                    this._sessionTimeLeft = value;
                    this.OnPropertyChanged("SessionTimeLeft");
                }
            }
        }

        private string _nonLapInfo;

        public string NonLapInfo
        {
            get { return _nonLapInfo; }
            set
            {
                if (value != this._nonLapInfo)
                {
                    this._nonLapInfo = value;
                    this.OnPropertyChanged("NonLapInfo");
                }
            }
        }


        public SessionHead()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket += Connention_SessionPacket;
                u.Connention.LapDataPacket += Connention_LapDataPacket;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                u.Connention.SessionPacket -= Connention_SessionPacket;
                u.Connention.LapDataPacket -= Connention_LapDataPacket;
            }
        }

        private void Connention_LapDataPacket(F1Telemetry.Models.LapDataPacket.PacketLapData packet, EventArgs e)
        {
            if (!this.isWorkingLapdata && u.CanDoUdp)
            {
                this.isWorkingLapdata = true;
                var data = packet.Lapdata.Where(x => x.CarPosition == 1).Select(x => x.CurrentLapNum).FirstOrDefault();

                //this.Dispatcher.Invoke(() =>
                //{
                switch (this.session)
                {
                    default:

                        if (this.SessionTimeLeft == TimeSpan.Zero) this.isRaceOver = true;
                        else if (data != 0) this.isRaceOver = false;

                        break;
                    case SessionTypes.Race:
                    case SessionTypes.Race2:
                    case SessionTypes.Race3:

                        if (data > this.TotalLaps)
                        {
                            this.isRaceOver = true;
                        }
                        else
                        {
                            this.isRaceOver = false;
                            //this.Dispatcher.Invoke(() => this.CurrentLap = data);
                            this.CurrentLap = data;
                        }

                        break;
                }

                this.isWorkingLapdata = false;
                //}, DispatcherPriority.Render);
            }
        }

        private void Connention_SessionPacket(F1Telemetry.Models.SessionPacket.PacketSessionData packet, EventArgs e)
        {
            if (!this.isWorkingSession && u.CanDoUdp)
            {
                this.isWorkingSession = true;

                this.TrackName = packet.TrackID.ToString();
                this.SessionName = packet.GetSessionType(true);
                this.TimeLeft = Math.Round(packet.SessionTimeLeft.TotalSeconds / (this.isRaceOver ? 160 : packet.SessionDuration.TotalSeconds) * 100.0, 2);
                this.session = packet.SessionType;

                //this.Dispatcher.Invoke(() =>
                //{
                this.TotalLaps = packet.TotalLaps;
                this.SessionTimeLeft = packet.SessionTimeLeft;
                //});

                if (!this.isRaceOver)
                {
                    switch (packet.SessionType)
                    {
                        default:

                            this.Dispatcher.Invoke(() =>
                            {
                                this.grid_timeData.Visibility = Visibility.Visible;
                                this.stackpanel_lapData.Visibility = Visibility.Hidden;
                                this.grid_formationLap.Visibility = Visibility.Hidden;
                            });

                            break;
                        case SessionTypes.TimeTrial:
                        case SessionTypes.Race:
                        case SessionTypes.Race2:
                        case SessionTypes.Race3:

                            this.Dispatcher.Invoke(() =>
                            {
                                this.grid_timeData.Visibility = Visibility.Hidden;
                                this.stackpanel_lapData.Visibility = Visibility.Visible;
                                this.grid_formationLap.Visibility = Visibility.Hidden;
                            });

                            break;
                    }
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        this.grid_timeData.Visibility = Visibility.Hidden;
                        this.stackpanel_lapData.Visibility = Visibility.Hidden;
                        this.grid_formationLap.Visibility = Visibility.Visible;
                    });

                    this.NonLapInfo = "Finnished";
                }

                var flags = packet.MarshalZones.Select(x => x.ZoneFlag).Distinct().Where(x => x == Flags.Yellow || x == Flags.Red || x == Flags.Green);
                var f = this.SetFlags(flags);

                if (packet.MarshalZones.Where(x => x.ZoneFlag == Flags.Yellow).Count() == packet.NumberOfMarshalZones)
                {
                    //this.Dispatcher.Invoke(() => this.FlagLocation = "Full course");
                    this.FlagLocation = "Full course";
                }
                else
                {
                    List<Sectors> sectors = new List<Sectors>();
                    string s = packet.TrackID.ToString();

                    if (this.track == null || this.track.TrackName != s || this.track.Year != packet.Header.PacketFormat) this.track = TrackLayout.FindNearestMap(s, packet.Header.PacketFormat);

                    if (this.track != null)
                    {
                        for (int i = 0; i < packet.NumberOfMarshalZones; i++)
                        {
                            if (packet.MarshalZones[i].ZoneFlag == f)
                            {
                                foreach (var item in this.track.MarshalZoneInSector(this.track.MarshalZones[i]))
                                {
                                    if (!sectors.Contains(item)) sectors.Add(item);
                                }
                            }
                        }

                        var sb = new StringBuilder();

                        foreach (var item in sectors)
                        {
                            if (sb.Length > 0) sb.Append(" & ");
                            sb.Append(item);
                        }

                        //this.Dispatcher.Invoke(() => this.FlagLocation = sb.ToString());
                        this.FlagLocation = sb.ToString();
                    }
                }

                switch (packet.SafetyCarStatus)
                {
                    case SafetyCarStatuses.NoSafetyCar:
                        break;
                    case SafetyCarStatuses.FullSafetyCar:
                        // this.Dispatcher.Invoke(() => this.FlagName += " - SC");
                        this.FlagName += " - SC";
                        break;
                    case SafetyCarStatuses.VirtualSafetyCar:
                        this.FlagName += " - VSC";
                        break;
                    case SafetyCarStatuses.FormationLap:
                        this.Dispatcher.Invoke(() =>
                        {
                            this.grid_timeData.Visibility = Visibility.Visible;
                            this.stackpanel_lapData.Visibility = Visibility.Hidden;
                            this.grid_formationLap.Visibility = Visibility.Visible;
                        });

                        this.NonLapInfo = "Formation Lap";
                        break;
                }

                this.isWorkingSession = false;
            }
        }

        private Flags SetFlags(IEnumerable<Flags> flags)
        {
            var flag = Flags.None;

            if (flags == null || flags.Count() == 0)
            {
                this.Dispatcher.Invoke(() => this.grid_flag.Visibility = Visibility.Collapsed);
                //this.grid_flag.Height = 0;
            }
            else
            {
                flag = Flags.None;
                this.Dispatcher.Invoke(() => this.grid_flag.Visibility = Visibility.Visible);
                //this.grid_flag.Height = 48;

                if (flags.Contains(Flags.Red))
                {
                    flag = Flags.Red;
                    // this.Dispatcher.Invoke(() => this.FlagName = "RED Flag");
                    this.FlagName = "RED Flag";
                }
                else if (flags.Contains(Flags.Yellow))
                {
                    flag = Flags.Yellow;
                    // this.Dispatcher.Invoke(() => this.FlagName = "YELLOW Flag");
                    this.FlagName = "YELLOW Flag";
                }
                else if (flags.Contains(Flags.Green))
                {
                    flag = Flags.Green;
                    // this.Dispatcher.Invoke(() => this.FlagName = "GREEN Flag");
                    this.FlagName = "GREEN Flag";
                }

                var c = u.FlagColors[flag];
                if (c.CanFreeze) c.Freeze();
                this.FlagColor = (SolidColorBrush)c;
            }

            return flag;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.UnsubscribeUDPEvents();
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

                    this.TrackName = null;
                    this.FlagColor = null;
                    this.FlagLocation = null;
                    this.FlagName = null;
                    this.SessionName = null;
                }

                this._trackName = null;
                this._flagColor = null;
                this._sessionName = null;
                this._flagLocation = null;
                this._flagName = null;


                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SessionHead()
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

        private void OnPropertyChanged(string propertyName)
        {
            // this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        private void UserControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var ses = u.Connention.LastSessionDataPacket;
            var sb = new StringBuilder();

            sb.AppendLine("Track: " + this.TrackName);
            sb.AppendLine("Session: " + ses?.GetSessionType(false));
            sb.AppendLine("Laps: " + this.CurrentLap + "/" + this.TotalLaps);
            sb.AppendLine("Session duration: " + ses?.SessionDuration);
            sb.AppendLine("Time left: " + this.SessionTimeLeft);

            this.ToolTip = sb.ToString().Trim();
        }
    }
}
