using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.CarTelemetryPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using F1TelemetryApp.Classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.UserControls
{
    /// <summary>
    /// Interaction logic for DriverDataContainer.xaml
    /// </summary>
    public partial class DriverDataContainer : UserControl, INotifyPropertyChanged
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
                if (carPosition != value)
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
                if (teamColor != value)
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
                if (driverName != value)
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
                if (raceNumber != value)
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
                if (teamName != value)
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
                if (currentLapTime != value)
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
                if (currentStatus != value)
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
                if (bestLapTime != value)
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
                if (prevDriver != value)
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
                if (nextDriver != value)
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
                if (prevDelta != value)
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
                if (nextDelta != value)
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
                if (lapPercent != value)
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
                if (timePenaltis != value)
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
                if (driveThrough != value)
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
                if (stopGo != value)
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
                if (warning != value)
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
                if (fuelPercent != value)
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
                if (fuelInTank != value)
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
                if (fuelCapacity != value)
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
                if (fuelRemaining != value)
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
                if (remainingColor != value)
                {
                    remainingColor = value;
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
                if (isAi != value)
                {
                    isAi = value;
                    if (this.isAi) this.image_isAi.Source = DriverDataContainer.IsRobot;
                    else this.image_isAi.Source = DriverDataContainer.NotRobot;
                }
            }
        }

        private Brush colorABS;

        public Brush ColorABS
        {
            get { return colorABS; }
            set
            {
                if (colorABS != value)
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
                if (colorTC != value)
                {
                    colorTC = value;
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
                if (fuelMix != value)
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
                if (ersEnergy != value)
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
                if (ersDeployable != value)
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
                if (ersMGUK != value)
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
                if (ersMGUH != value)
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
                if (ersMode != value)
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
                if (brakeBias != value)
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
                if (gForce != value)
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
                if (throttle != value)
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
                if (brake != value)
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
                if (clutch != value)
                {
                    clutch = value;
                    this.OnPropertyChanged("Clutch");
                }
            }
        }

        private Flags lastFlag = Flags.InvalidOrUnknown;
        private TractionControlSettings tc;
        private bool isABS;

        private static BitmapImage IsRobot = new BitmapImage(new Uri("pack://application:,,,/Images/DriverInfo/IsRobot.png"));
        private static BitmapImage NotRobot = new BitmapImage(new Uri("pack://application:,,,/Images/DriverInfo/NotRobot.png"));

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void UpdateLapdata(LapData lapdata, float trackLength)
        {
            if (lapdata != null)
            {
                this.StopGo = lapdata.NumberOfUnservedStopGoPenalties;
                this.Warning = lapdata.Warnings;
                this.DriveThrough = lapdata.NumberOfUnservedDriveThroughPenalties;
                this.TimePenaltis = lapdata.Penalties;
                this.CarPosition = lapdata.CarPosition;
                this.CurrentLapTime = lapdata.CurrentLapTime;
                this.CurrentStatus = lapdata.ResultStatus.ToString();

                this.LapPercent = lapdata.LapDistance / trackLength * 100f;
            }

            this.UpdateLayout();
        }

        private static int FindPlayer(int position, int arraySize)
        {
            int ret = -1;
            var lastLapData = u.Connention.LastLapDataPacket;

            if (lastLapData != null)
            {
                int i = Array.IndexOf(lastLapData.Lapdata, lastLapData.Lapdata.Where(x => x.CarPosition == position).FirstOrDefault());
                if (i < arraySize && i > -1) ret = i;
            }

            return ret;
        }

        internal void UpdateDatas(
            CarStatusData status,
            CarTelemetryData telemetry,
            PacketSessionHistoryData history,
            PacketParticipantsData participants,
            Vector3 gForce,
            int playerIndex
        )
        {
            int prevIndex = DriverDataContainer.FindPlayer(this.carPosition - 1, participants.Participants.Length);
            int nextIndex = DriverDataContainer.FindPlayer(this.carPosition + 1, participants.Participants.Length);

            if (participants != null)
            {
                var player = participants.Participants[playerIndex];
                this.DriverName = player.Name;
                this.RaceNumber = player.RaceNumber;
                this.TeamName = u.PickTeamName(player.TeamID);
                this.TeamColor = u.PickTeamColor(player.TeamID);
                this.image_nationality.Source = u.NationalityImage(player.Nationality);
                this.IsAi = player.IsAI;

                if (prevIndex != -1) this.PrevDriver = participants.Participants[prevIndex].ShortName;
                else this.PrevDriver = "---";

                if (nextIndex != -1) this.NextDriver = participants.Participants[nextIndex].ShortName;
                else this.NextDriver = "---";
            }

            if (history != null)
            {
                if (history.BestLapTimeLapNumber > 0 && history.BestLapTimeLapNumber < history.LapHistoryData.Length)
                {
                    this.BestLapTime = history.LapHistoryData[history.BestLapTimeLapNumber - 1].LapTime;
                }

                var lastHistoryPacket = u.Connention.LastSessionHistoryPacket;
                var lastLapData = u.Connention.LastLapDataPacket;

                Brush f = Brushes.White;
                if (prevIndex > -1)
                {
                    this.PrevDelta = u.CalculateDelta(lastLapData, lastHistoryPacket[prevIndex], history, out f);
                    this.textblock_prev.Foreground = f;
                }
                if (nextIndex > -1)
                {
                    this.NextDelta = u.CalculateDelta(lastLapData, lastHistoryPacket[nextIndex], history, out f);
                    this.textblock_next.Foreground = f;
                }
                f = null;
            }

            if (status != null)
            {
                if (this.lastFlag != status.VehicleFIAFlag)
                {
                    this.lastFlag = status.VehicleFIAFlag;
                    this.rectangle_flag.Fill = u.FlagColors[status.VehicleFIAFlag];
                }

                this.FuelPercent = status.FuelInTank / status.FuelCapacity * 100;
                this.FuelCapacity = status.FuelCapacity;
                this.FuelInTank = status.FuelInTank;
                this.FuelRemaining = status.FuelRemainingLaps;
                this.FuelMix = status.FuelMix;

                this.ERSEnergy = status.ERSStoreEnergyPercent;
                this.ERSDeployable = 100 - status.ERSDeployedThisLapPercent;
                this.ERSMGUH = status.ERSHarvestedThisLapMGUHPercent;
                this.ERSMGUK = status.ERSHarvestedThisLapMGUKPercent;
                this.ERSMode = status.ERSDeployMode;

                this.BrakeBias = status.FrontBrakeBias;

                if (this.isABS != status.IsAntiLockBrakes)
                {
                    this.isABS = status.IsAntiLockBrakes;

                    if (this.isABS) this.ColorABS = Brushes.LimeGreen;
                    else this.ColorABS = Brushes.Gray;
                }

                if (this.tc != status.TractionControl)
                {
                    this.tc = status.TractionControl;

                    switch (this.tc)
                    {
                        default:
                            this.ColorTC = Brushes.Gray;
                            break;
                        case TractionControlSettings.Medium:
                            this.ColorTC = Brushes.Yellow;
                            break;
                        case TractionControlSettings.Full:
                            this.ColorTC = Brushes.LimeGreen;
                            break;
                    }
                }

                if (this.FuelRemaining < 0) this.RemainingColor = Brushes.Red;
                else if (this.FuelRemaining > 0) this.RemainingColor = Brushes.LimeGreen;
                else this.RemainingColor = Brushes.LightGray;
            }

            float g = MathF.Abs(gForce.X + gForce.Y);
            if (g != this.GForce)
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
                this.textblock_gForce.Foreground = new SolidColorBrush(Color.FromRgb(255, others, others));
            }

            if (telemetry != null)
            {
                this.Throttle = telemetry.Throttle * 100f;
                this.Brake = telemetry.Brake * 100f;
                this.Clutch = telemetry.Clutch * 100f;
            }

            this.UpdateLayout();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.tc = TractionControlSettings.Off;
            this.isABS = false;

            this.ColorTC = Brushes.Gray;
            this.ColorABS = Brushes.Gray;
        }
    }
}
