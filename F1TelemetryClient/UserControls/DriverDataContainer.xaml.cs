using F1Telemetry.Helpers;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.CarTelemetryPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.MotionPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;
using static F1TelemetryApp.Classes.IGridResize;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for DriverDataContainer.xaml
    /// </summary>
    public partial class DriverDataContainer : UserControl, INotifyPropertyChanged, IConnectUDP, IDisposable, IGridResize
    {
        public DriverDataContainer()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private int carPosition;

        public int CarPosition
        {
            get { return carPosition; }
            set
            {
                if (value != carPosition)
                {
                    carPosition = value;
                    this.OnPropertyChanged("CarPosition");
                }
            }
        }

        private SolidColorBrush teamColor;

        public SolidColorBrush TeamColor
        {
            get { return teamColor; }
            set
            {
                if (value != teamColor)
                {
                    teamColor = value;
                    this.OnPropertyChanged("TeamColor");
                }
            }
        }

        private string driverName;

        public string DriverName
        {
            get { return driverName; }
            set
            {
                if (value != driverName)
                {
                    driverName = value;
                    this.OnPropertyChanged("DriverName");
                }
            }
        }

        private int raceNumber;

        public int RaceNumber
        {
            get { return raceNumber; }
            set
            {
                if (value != raceNumber)
                {
                    raceNumber = value;
                    this.OnPropertyChanged("RaceNumber");
                }
            }
        }

        private string teamName;

        public string TeamName
        {
            get { return teamName; }
            set
            {
                if (value != teamName)
                {
                    teamName = value;
                    this.OnPropertyChanged("TeamName");
                }
            }
        }

        private TimeSpan currentLapTime;

        public TimeSpan CurrentLapTime
        {
            get { return currentLapTime; }
            set
            {
                if (value != currentLapTime)
                {
                    currentLapTime = value;
                    this.OnPropertyChanged("CurrentLapTime");
                }
            }
        }

        private string currentStatus;

        public string CurrentStatus
        {
            get { return currentStatus; }
            set
            {
                if (value != currentStatus)
                {
                    currentStatus = value;
                    this.OnPropertyChanged("CurrentStatus");
                }
            }
        }

        private TimeSpan bestLapTime;

        public TimeSpan BestLapTime
        {
            get { return bestLapTime; }
            set
            {
                if (value != bestLapTime)
                {
                    bestLapTime = value;
                    this.OnPropertyChanged("BestLapTime");
                }
            }
        }

        private string prevDriver;

        public string PrevDriver
        {
            get { return prevDriver; }
            set
            {
                if (value != prevDriver)
                {
                    prevDriver = value;
                    this.OnPropertyChanged("PrevDriver");
                }
            }
        }

        private string nextDriver;

        public string NextDriver
        {
            get { return nextDriver; }
            set
            {
                if (value != nextDriver)
                {
                    nextDriver = value;
                    this.OnPropertyChanged("NextDriver");
                }
            }
        }

        private string prevDelta;

        public string PrevDelta
        {
            get { return prevDelta; }
            set
            {
                if (value != prevDelta)
                {
                    prevDelta = value;
                    this.OnPropertyChanged("PrevDelta");
                }
            }
        }

        private string nextDelta;

        public string NextDelta
        {
            get { return nextDelta; }
            set
            {
                if (value != nextDelta)
                {
                    nextDelta = value;
                    this.OnPropertyChanged("NextDelta");
                }
            }
        }

        private double lapPercent;

        public double LapPercent
        {
            get { return lapPercent; }
            set
            {
                if (value != lapPercent)
                {
                    lapPercent = value;
                    this.OnPropertyChanged("LapPercent");
                }
            }
        }

        private TimeSpan timePenaltis;

        public TimeSpan TimePenaltis
        {
            get { return timePenaltis; }
            set
            {
                if (value != timePenaltis)
                {
                    timePenaltis = value;
                    this.OnPropertyChanged("TimePenaltis");
                }
            }
        }

        private double driveThrough;

        public double DriveThrough
        {
            get { return driveThrough; }
            set
            {
                if (value != driveThrough)
                {
                    driveThrough = value;
                    this.OnPropertyChanged("DriveThrough");
                }
            }
        }

        private double stopGo;

        public double StopGo
        {
            get { return stopGo; }
            set
            {
                if (value != stopGo)
                {
                    stopGo = value;
                    this.OnPropertyChanged("StopGo");
                }
            }
        }

        private double warning;

        public double Warning
        {
            get { return warning; }
            set
            {
                if (value != warning)
                {
                    warning = value;
                    this.OnPropertyChanged("Warning");
                }
            }
        }

        private double fuelPercent;

        public double FuelPercent
        {
            get { return fuelPercent; }
            set
            {
                if (value != fuelPercent)
                {
                    fuelPercent = value;
                    this.OnPropertyChanged("FuelPercent");
                }
            }
        }

        private double fuelInTank;

        public double FuelInTank
        {
            get { return fuelInTank; }
            set
            {
                if (value != fuelInTank)
                {
                    fuelInTank = value;
                    this.OnPropertyChanged("FuelInTank");
                }
            }
        }

        private double fuelCapacity;

        public double FuelCapacity
        {
            get { return fuelCapacity; }
            set
            {
                if (value != fuelCapacity)
                {
                    fuelCapacity = value;
                    this.OnPropertyChanged("FuelCapacity");
                }
            }
        }

        private double fuelRemaining;

        public double FuelRemaining
        {
            get { return fuelRemaining; }
            set
            {
                if (value != fuelRemaining)
                {
                    fuelRemaining = value;
                    this.OnPropertyChanged("FuelRemaining");
                }
            }
        }

        private Brush remainingColor;

        public Brush RemainingColor
        {
            get { return remainingColor; }
            set
            {
                if (value != remainingColor)
                {
                    remainingColor = value;
                    if (remainingColor.CanFreeze) this.remainingColor.Freeze();
                    this.OnPropertyChanged("RemainingColor");
                }
            }
        }

        private bool isAi;

        public bool IsAi
        {
            get { return isAi; }
            set
            {
                if (value != isAi)
                {
                    isAi = value;
                    BitmapImage img = null;

                    if (this.isAi) img = new BitmapImage(new Uri("pack://application:,,,/Images/DriverInfo/IsRobot.png"));
                    else img = new BitmapImage(new Uri("pack://application:,,,/Images/DriverInfo/NotRobot.png"));

                    if (img.CanFreeze) img.Freeze();
                    this.Dispatcher.Invoke(() => this.image_isAi.Source = img);
                }
            }
        }

        private Brush colorABS;

        public Brush ColorABS
        {
            get { return colorABS; }
            set
            {
                if (value != colorABS)
                {
                    colorABS = value;
                    this.OnPropertyChanged("ColorABS");
                }
            }
        }

        private Brush colorTC;

        public Brush ColorTC
        {
            get { return colorTC; }
            set
            {
                if (value != colorTC)
                {
                    colorTC = value;
                    if (this.remainingColor != null && this.remainingColor.CanFreeze) this.remainingColor.Freeze();
                    this.OnPropertyChanged("ColorTC");
                }
            }
        }

        private FuelMixis fuelMix;

        public FuelMixis FuelMix
        {
            get { return fuelMix; }
            set
            {
                if (value != fuelMix)
                {
                    fuelMix = value;
                    this.OnPropertyChanged("FuelMix");
                }
            }
        }

        private float ersEnergy;

        public float ERSEnergy
        {
            get { return ersEnergy; }
            set
            {
                if (value != ersEnergy)
                {
                    ersEnergy = value;
                    this.OnPropertyChanged("ERSEnergy");
                }
            }
        }

        private float ersDeployable;

        public float ERSDeployable
        {
            get { return ersDeployable; }
            set
            {
                if (value != ersDeployable)
                {
                    ersDeployable = value;
                    this.OnPropertyChanged("ERSDeployable");
                }
            }
        }

        private float ersMGUK;

        public float ERSMGUK
        {
            get { return ersMGUK; }
            set
            {
                if (value != ersMGUK)
                {
                    ersMGUK = value;
                    this.OnPropertyChanged("ERSMGUK");
                }
            }
        }

        private float ersMGUH;

        public float ERSMGUH
        {
            get { return ersMGUH; }
            set
            {
                if (value != ersMGUH)
                {
                    ersMGUH = value;
                    this.OnPropertyChanged("ERSMGUH");
                }
            }
        }

        private ERSModes ersMode;

        public ERSModes ERSMode
        {
            get { return ersMode; }
            set
            {
                if (value != ersMode)
                {
                    ersMode = value;
                    this.OnPropertyChanged("ERSMode");
                }
            }
        }

        private int brakeBias;

        public int BrakeBias
        {
            get { return brakeBias; }
            set
            {
                if (value != brakeBias)
                {
                    brakeBias = value;
                    this.OnPropertyChanged("BrakeBias");
                }
            }
        }

        private float gForce;

        public float GForce
        {
            get { return gForce; }
            set
            {
                if (value != gForce)
                {
                    gForce = value;
                    this.OnPropertyChanged("GForce");
                }
            }
        }

        private float throttle;

        public float Throttle
        {
            get { return throttle; }
            set
            {
                if (value != throttle)
                {
                    throttle = value;
                    this.OnPropertyChanged("Throttle");
                }
            }
        }

        private float brake;

        public float Brake
        {
            get { return brake; }
            set
            {
                if (value != brake)
                {
                    brake = value;
                    this.OnPropertyChanged("Brake");
                }
            }
        }

        private float clutch;

        public float Clutch
        {
            get { return clutch; }
            set
            {
                if (value != clutch)
                {
                    clutch = value;
                    this.OnPropertyChanged("Clutch");
                }
            }
        }

        private float speedKPH;

        public float SpeedKPH
        {
            get { return speedKPH; }
            set
            {
                if (value != speedKPH)
                {
                    speedKPH = value;
                    this.OnPropertyChanged("SpeedKPH");
                }
            }
        }

        private float speedMPH;

        public float SpeedMPH
        {
            get { return speedMPH; }
            set
            {
                if (value != speedMPH)
                {
                    speedMPH = value;
                    this.OnPropertyChanged("SpeedMPH");
                }
            }
        }

        private float steer;

        public float Steer
        {
            get { return steer; }
            set
            {
                if (value != steer)
                {
                    steer = value;
                    this.OnPropertyChanged("Steer");
                }
            }
        }

        private ushort rpm;

        public ushort RPM
        {
            get { return rpm; }
            set
            {
                if (value != rpm)
                {
                    rpm = value;
                    this.OnPropertyChanged("RPM");
                }
            }
        }

        private string gear;

        public string Gear
        {
            get { return gear; }
            set
            {
                if (value != gear)
                {
                    gear = value;
                    this.OnPropertyChanged("Gear");
                }
            }
        }

        private byte numberOfPits;

        public byte NumberOfPits
        {
            get { return numberOfPits; }
            set
            {
                if (value != this.numberOfPits)
                {
                    numberOfPits = value;
                    this.OnPropertyChanged("NumberOfPits");
                }
            }
        }

        private string nextPitWindow;

        public string NextPitWindow
        {
            get { return nextPitWindow; }
            set
            {
                if (value != this.nextPitWindow)
                {
                    nextPitWindow = value;
                    this.OnPropertyChanged("NextPitWindow");
                }
            }
        }

        private string rejoinPos;

        public string RejoinPos
        {
            get { return rejoinPos; }
            set
            {
                if (value != this.rejoinPos)
                {
                    rejoinPos = value;
                    this.OnPropertyChanged("RejoinPos");
                }
            }
        }


        private Flags lastFlag = Flags.InvalidOrUnknown;
        private TractionControlSettings tc;
        private bool isABS;

        private int driverIndex;
        private int prevIndex;
        private bool canUpdate;
        private int nextIndex;
        private bool disposedValue;

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            // if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
        }

        private void UpdateLapdata(ref PacketLapData lapdata)
        {
            if (lapdata != null)
            {
                var player = lapdata.Lapdata[this.driverIndex];
                var a = lapdata.Lapdata;

                //this.Dispatcher.Invoke(() =>
                //{
                this.StopGo = player.NumberOfUnservedStopGoPenalties;
                this.Warning = player.Warnings;
                this.DriveThrough = player.NumberOfUnservedDriveThroughPenalties;
                this.TimePenaltis = player.Penalties;
                this.CarPosition = player.CarPosition;
                this.CurrentLapTime = player.CurrentLapTime;
                this.CurrentStatus = player.ResultStatus.ToString();
                this.NumberOfPits = player.NumberOfPitStops;

                this.LapPercent = player.LapDistance / u.TrackLength * 100f;
                //});

                this.prevIndex = Array.IndexOf(a, a.Where(x => x.CarPosition == player.CarPosition - 1 && x.CarPosition != 0).FirstOrDefault());
                this.nextIndex = Array.IndexOf(a, a.Where(x => x.CarPosition == player.CarPosition + 1 && x.CarPosition != 0).FirstOrDefault());

                // this.UpdateLayout();
            }
        }

        internal void UpdateDatas(
            CarStatusData status,
            CarTelemetryData telemetry,
            PacketSessionHistoryData history,
            PacketParticipantsData participants,
            Vector3 gForce,
            PacketLapData lapdata,
            int playerIndex
            )
        {
            this.canUpdate = false;

            this.driverIndex = playerIndex;

            this.UpdateLapdata(ref lapdata);
            this.UpdateParticipants(ref participants);
            this.UpdateHistory(ref history);
            this.UpdateStatus(ref status);
            this.UpdateGForce(gForce);
            this.UpdateTelemetry(ref telemetry);

            // this.UpdateLayout();

            this.canUpdate = true;
        }

        private void UpdateTelemetry(ref CarTelemetryData telemetry)
        {
            if (telemetry != null)
            {
                var t = MathF.Round(telemetry.Throttle * 100f, 2);
                var b = MathF.Round(telemetry.Brake * 100f, 2);
                var cl = MathF.Round(telemetry.Clutch * 100f, 2);
                var kph = telemetry.Speed;
                var mph = MathF.Round(telemetry.Speed * 0.621371192f);
                var s = MathF.Round(180f * telemetry.Steer, 2);
                var gear = telemetry.GearString;
                var rpm = telemetry.EngineRPM;
                var drs = telemetry.IsDRS;

                var fg = Brushes.Gray;
                var revLight = telemetry.RevLightBitValue;

                this.Dispatcher.Invoke(() =>
                {

                    this.Throttle = t;
                    this.Brake = b;
                    this.Clutch = cl;

                    this.SpeedKPH = kph;
                    this.SpeedMPH = mph;

                    this.Gear = gear;
                    this.RPM = rpm;
                    this.Steer = s;

                    for (int i = 0; i < this.grid_revLight.Children.Count; i++)
                    {
                        var item = this.grid_revLight.Children[i] as Ellipse;
                        var c = Brushes.Gray;

                        if ((revLight & (1 << i)) != 0)
                        {
                            if (i < 5) c = Brushes.LimeGreen;
                            else if (i < 10) c = Brushes.Red;
                            else c = Brushes.Magenta;
                        }

                        if (c.CanFreeze) c.Freeze();
                        item.Fill = c;
                    }

                    if (drs) fg = Brushes.LimeGreen;
                    if (fg.CanFreeze) fg.Freeze();

                    this.textblock_DRS.Foreground = fg;
                    this.image_wheel.RenderTransform = new RotateTransform(this.Steer, this.image_wheel.ActualWidth / 2, this.image_wheel.ActualHeight / 2);
                });

                // this.UpdateLayout();
            }
        }

        private void UpdateGForce(Vector3 gForce)
        {
            float g = MathF.Abs(gForce.X + gForce.Y);

            if (g != this.GForce)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.GForce = g;
                    double r = this.grid_gCanvas.ActualWidth / 2;
                    double o = r - this.ellipse_gpointer.ActualWidth / 2;

                    float a = (float)o / 5f;
                    float fy = gForce.Y * a;

                    if (MathF.Abs(fy) > o) fy = (float)o;

                    float fx = gForce.X * a;
                    if (MathF.Abs(fx) > o) fx = (float)o;

                    this.ellipse_gpointer.Margin = new Thickness
                    {
                        Top = o + fy,
                        Left = o + fx,
                    };

                    if (g > 5f) g = 5f;

                    byte red = (byte)MathF.Round(g * 51);
                    byte others = (byte)(255 - red);
                    var c = new SolidColorBrush(Color.FromRgb(255, others, others));

                    if (c.CanFreeze) c.Freeze();

                    this.textblock_gForce.Foreground = c;
                });

                // this.UpdateLayout();
            }
        }

        private void UpdateStatus(ref CarStatusData status)
        {
            if (status != null)
            {
                if (this.lastFlag != status.VehicleFIAFlag)
                {
                    this.lastFlag = status.VehicleFIAFlag;
                    var flag = u.FlagColors[status.VehicleFIAFlag];
                    this.Dispatcher.Invoke(() => this.rectangle_flag.Fill = flag);
                }

                var fp = Math.Round(status.FuelInTank / status.FuelCapacity * 100, 2);
                var fc = Math.Round(status.FuelCapacity, 2);
                var fit = Math.Round(status.FuelInTank, 2);
                var fr = Math.Round(status.FuelRemainingLaps, 2);
                var mix = status.FuelMix;
                var ers = MathF.Round(status.ERSStoreEnergyPercent, 2);
                var d = MathF.Round(100 - status.ERSDeployedThisLapPercent, 2);
                var mguh = MathF.Round(status.ERSHarvestedThisLapMGUHPercent, 2);
                var mguk = MathF.Round(status.ERSHarvestedThisLapMGUKPercent, 2);
                var ersMode = status.ERSDeployMode;
                var bb = status.FrontBrakeBias;


                this.Dispatcher.Invoke(() =>
                {
                    this.FuelPercent = fp;
                    this.FuelCapacity = fc;
                    this.FuelInTank = fit;
                    this.FuelRemaining = fr;
                    this.FuelMix = mix;

                    this.ERSEnergy = ers;
                    this.ERSDeployable = d;
                    this.ERSMGUH = mguh;
                    this.ERSMGUK = mguk;
                    this.ERSMode = ersMode;

                    this.BrakeBias = bb;

                    if (this.FuelRemaining < 0) this.RemainingColor = Brushes.Red;
                    else if (this.FuelRemaining > 0) this.RemainingColor = Brushes.LimeGreen;
                    else this.RemainingColor = Brushes.LightGray;
                });

                if (this.isABS != status.IsAntiLockBrakes)
                {
                    this.isABS = status.IsAntiLockBrakes;

                    if (this.isABS) this.Dispatcher.Invoke(() => this.ColorABS = Brushes.LimeGreen);
                    else this.Dispatcher.Invoke(() => this.ColorABS = Brushes.Gray);
                }

                if (this.tc != status.TractionControl)
                {
                    this.tc = status.TractionControl;

                    switch (this.tc)
                    {
                        default:
                            this.Dispatcher.Invoke(() => this.ColorTC = Brushes.Gray);
                            break;
                        case TractionControlSettings.Medium:
                            this.Dispatcher.Invoke(() => this.ColorTC = Brushes.Yellow);
                            break;
                        case TractionControlSettings.Full:
                            this.Dispatcher.Invoke(() => this.ColorTC = Brushes.LimeGreen);
                            break;
                    }
                }

                if (status.IsPitLimiter) this.Dispatcher.Invoke(() => this.textblock_PLS.Foreground = Brushes.Cyan);
                else this.Dispatcher.Invoke(() => this.textblock_PLS.Foreground = Brushes.Gray);

                // this.UpdateLayout();
            }
        }

        private void UpdateParticipants(ref PacketParticipantsData participantsData)
        {
            if (participantsData != null)
            {
                if (participantsData.Participants.Length > this.driverIndex)
                {
                    var player = participantsData.Participants[this.driverIndex];
                    var prevDriver = "";
                    var nextDriver = "";
                    var year = participantsData.Header.PacketFormat;

                    if (this.prevIndex != -1) prevDriver = participantsData.Participants[this.prevIndex].ShortName;
                    else prevDriver = "---";

                    if (this.nextIndex != -1) nextDriver = participantsData.Participants[this.nextIndex].ShortName;
                    else nextDriver = "---";

                    this.Dispatcher.Invoke(() =>
                    {
                        this.DriverName = player.ParticipantName;
                        this.RaceNumber = player.RaceNumber;
                        this.TeamName = player.TeamID.GetTeamName(year);
                        this.TeamColor = u.PickTeamColor(player.TeamID);
                        this.image_nationality.Source = u.NationalityImage(player.Nationality);
                        this.IsAi = player.IsAI;
                        this.PrevDriver = prevDriver;
                        this.NextDriver = nextDriver;
                    });
                }
                else
                {
                    this.Dispatcher.Invoke(() => this.Visibility = Visibility.Hidden);
                }

                // this.UpdateLayout();
            }
        }

        private void UpdateHistory(ref PacketSessionHistoryData history)
        {
            if (history != null)
            {
                if (history.BestLapTimeLapNumber > 0 && history.BestLapTimeLapNumber < history.LapHistoryData.Length)
                {
                    var lp = history.LapHistoryData[history.BestLapTimeLapNumber - 1].LapTime;
                    this.Dispatcher.Invoke(() => this.BestLapTime = lp);
                }

                var n = history.NumberOfTyreStints;
                var t = history.TyreStintsHistoryData;

                this.Dispatcher.Invoke(() =>
                {
                    if (n != this.stackpanel_tyrehistory.Children.Count)
                    {
                        this.stackpanel_tyrehistory.Children.Clear();

                        for (int i = 0; i < n; i++)
                        {
                            var tyre = t[i];

                            if (tyre.IsCurrentTyre || tyre.EndLap != 0)
                            {
                                this.stackpanel_tyrehistory.Children.Add(new Image
                                {
                                    Source = u.TyreCompoundToImage(tyre.TyreVisualCompound),
                                    Width = 20,
                                    Height = 20,
                                    Margin = new Thickness(0),
                                });

                            }
                        }
                    }
                });

                var lastHistoryPacket = u.Connention.CurrentSessionHistoryPacket;
                var lastLapData = u.Connention.CurrentLapDataPacket;

                Brush f = Brushes.White;
                if (f.CanFreeze) f.Freeze();

                if (this.prevIndex > -1)
                {
                    var prev = u.CalculateDelta(lastLapData, lastHistoryPacket[prevIndex], history, out f);
                    this.Dispatcher.Invoke(() =>
                    {
                        this.PrevDelta = prev;
                        this.textblock_prev.Foreground = f;
                    });
                }
                else
                {
                    this.PrevDelta = null;
                }

                if (this.nextIndex > -1)
                {
                    var next = u.CalculateDelta(lastLapData, lastHistoryPacket[nextIndex], history, out f);
                    this.Dispatcher.Invoke(() =>
                    {
                        this.NextDelta = next;
                        this.textblock_next.Foreground = f;
                    });
                }
                else
                {
                    this.NextDelta = null;
                }

                f = null;

                // this.UpdateLayout();
            }
        }

        private void UpdateSession(ref PacketSessionData session)
        {
            bool ok = session != null && session.Header.Player1CarIndex == this.driverIndex;
            string npw = null;
            string rp = null;

            if (ok && session.PitStopWindowIdealLap > 0) npw = session.PitStopWindowIdealLap + " - " + session.PitStopWindowLastestLap;
            else npw = "---";

            if (ok && session.PitStopRejoinPosition > 0) rp = session.PitStopRejoinPosition.ToString();
            else rp = "-";

            this.Dispatcher.Invoke(() =>
            {
                this.NextPitWindow = npw;
                this.RejoinPos = rp;
            });

            // this.UpdateLayout();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.tc = TractionControlSettings.Off;
            this.isABS = false;

            this.ColorTC = Brushes.Gray;
            this.ColorABS = Brushes.Gray;

            if (u.Connention != null)
            {
                this.SubscribeUDPEvents();
            }

            var img = u.KepForras(Properties.Resources.natinalityMask);
            if (img.CanFreeze) img.Freeze();

            this.grid_nationality.OpacityMask = new ImageBrush
            {
                ImageSource = img,
                TileMode = TileMode.None,
                Stretch = Stretch.Fill,
            };
        }

        private void Connention_LapDataPacket(PacketLapData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateLapdata(ref packet);
            });
        }

        private void Connention_CarMotionPacket(PacketMotionData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                if (packet != null) this.UpdateGForce(packet.CarMotionData[this.driverIndex].GForce);
            });
        }

        private void Connention_CarStatusPacket(PacketCarStatusData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateStatus(ref packet.CarStatusData[this.driverIndex]);
            });
        }

        private void Connention_CarTelemetryPacket(PacketCarTelemetryData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateTelemetry(ref packet.CarTelemetryData[this.driverIndex]);
            });
        }

        private void Connention_SessionHistoryPacket(PacketSessionHistoryData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                if (packet.CarIndex == this.driverIndex) this.UpdateHistory(ref packet);
            });
        }

        private void Connention_ParticipantsPacket(PacketParticipantsData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateParticipants(ref packet);
            });
        }

        private void Connention_SessionPacket(PacketSessionData packet, EventArgs e)
        {
            this.OnUpdateEvent(() =>
            {
                this.UpdateSession(ref packet);
            });
        }

        private void OnUpdateEvent(Action method)
        {
            //this.Dispatcher.Invoke(() =>
            //{
            if (this.CanDoEvents())
            {
                this.canUpdate = false;
                if (this.driverIndex > -1) method();
                this.canUpdate = true;
            }
            //}, DispatcherPriority.Render);
        }

        private bool CanDoEvents()
        {
            var ret = false;
            this.Dispatcher.Invoke(() => ret = this.canUpdate && this.IsLoaded);
            return ret;
        }

        public void SubscribeUDPEvents()
        {
            u.Connention.ParticipantsPacket += Connention_ParticipantsPacket;
            u.Connention.SessionHistoryPacket += Connention_SessionHistoryPacket;
            u.Connention.CarStatusPacket += Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket += Connention_CarTelemetryPacket;
            u.Connention.CarMotionPacket += Connention_CarMotionPacket;
            u.Connention.LapDataPacket += Connention_LapDataPacket;
            u.Connention.SessionPacket += Connention_SessionPacket;
        }

        public void UnsubscribeUDPEvents()
        {
            u.Connention.ParticipantsPacket -= Connention_ParticipantsPacket;
            u.Connention.SessionHistoryPacket -= Connention_SessionHistoryPacket;
            u.Connention.CarStatusPacket -= Connention_CarStatusPacket;
            u.Connention.CarTelemetryPacket -= Connention_CarTelemetryPacket;
            u.Connention.CarMotionPacket -= Connention_CarMotionPacket;
            u.Connention.LapDataPacket -= Connention_LapDataPacket;
            u.Connention.SessionPacket -= Connention_SessionPacket;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.Background = null;
                    this.ColorABS = null;
                    this.ColorTC = null;
                    this.DriverName = null;
                    this.image_isAi.Source = null;
                    this.image_nationality.Source = null;
                    this.NextDelta = null;
                    this.PrevDelta = null;
                    this.RemainingColor = null;
                    this.TeamColor = null;
                    this.TeamName = null;

                    this.UnsubscribeUDPEvents();
                    this.PropertyChanged = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DriverDataContainer()
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

        public void ResizeXS()
        {
            IGridResize.SetGridSettings(this.border_driverInfo, 0, 0, 12);
            IGridResize.SetGridSettings(this.border_speedOMeter, 0, 1, 12);
            IGridResize.SetGridSettings(this.border_fuel, 0, 2, 12);
            IGridResize.SetGridSettings(this.border_gForce, 0, 3, 12);
        }

        public void ResizeXM()
        {
            IGridResize.SetGridSettings(this.border_driverInfo, 0, 0, 12);
            IGridResize.SetGridSettings(this.border_speedOMeter, 0, 1, 12);
            IGridResize.SetGridSettings(this.border_fuel, 0, 2, 7);
            IGridResize.SetGridSettings(this.border_gForce, 7, 2, 5);
        }

        public void ResizeMD()
        {
            IGridResize.SetGridSettings(this.border_driverInfo, 0, 0, 8);
            IGridResize.SetGridSettings(this.border_gForce, 8, 0, 4);
            IGridResize.SetGridSettings(this.border_speedOMeter, 0, 1, 7);
            IGridResize.SetGridSettings(this.border_fuel, 7, 1, 5);
        }

        public void ResizeLG()
        {
            IGridResize.SetGridSettings(this.border_driverInfo, 0, 0, 4);
            IGridResize.SetGridSettings(this.border_fuel, 4, 0, 3);
            IGridResize.SetGridSettings(this.border_speedOMeter, 7, 0, 3);
            IGridResize.SetGridSettings(this.border_gForce, 10, 0, 2);
        }

        public void ResizeXL()
        {
            IGridResize.SetGridSettings(this.border_driverInfo, 0, 0, 5);
            IGridResize.SetGridSettings(this.border_fuel, 5, 0, 2);
            IGridResize.SetGridSettings(this.border_speedOMeter, 7, 0, 3);
            IGridResize.SetGridSettings(this.border_gForce, 10, 0, 2);
        }

        public void CalculateView(GridSizes gridSizes)
        {
            switch (gridSizes)
            {
                case GridSizes.XS:
                    this.ResizeXS();
                    break;
                case GridSizes.XM:
                    this.ResizeXM();
                    break;
                case GridSizes.MD:
                    this.ResizeMD();
                    break;
                case GridSizes.LG:
                    this.ResizeLG();
                    break;
                case GridSizes.XL:
                    this.ResizeXL();
                    break;
            }

            // this.UpdateLayout();
        }
    }
}
