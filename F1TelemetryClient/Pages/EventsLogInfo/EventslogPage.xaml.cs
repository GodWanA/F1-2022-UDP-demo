using F1Telemetry.CustomModels.EventPackets;
using F1Telemetry.Helpers;
using F1Telemetry.Models.ParticipantsPacket;
using F1TelemetryApp.Classes;
using F1TelemetryClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Pages.EventsLogInfo
{
    /// <summary>
    /// Interaction logic for EventslogPage.xaml
    /// </summary>
    public partial class EventslogPage : UserControl, IGridResize, IConnectUDP, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DataTable _eventTable;
        public DataTable EventTable
        {
            get { return this._eventTable; }
            set
            {
                if (value != this._eventTable)
                {
                    this._eventTable = value;
                    this.OnPropertyChanged("EventTable");
                }
            }
        }

        public ObservableCollection<Tuple<string, string, DateTime>> EventList { get; set; } = new ObservableCollection<Tuple<string, string, DateTime>>();

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), DispatcherPriority.Background);
            //this.Dispatcher.Invoke(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)), System.Windows.Threading.DispatcherPriority.DataBind);
        }

        public EventslogPage()
        {
            InitializeComponent();

            this.DataContext = this;

            this.EventTable = new DataTable();
            this.AddCol("eventName", "eventName", typeof(string));
            this.AddCol("message", "message", typeof(string));
            this.AddCol("timeStamp", "timeStamp", typeof(DateTime));
            this.AddCol("background", "background", typeof(SolidColorBrush));
            this.AddCol("foreground", "foreground", typeof(SolidColorBrush));

            this.dgv_events.ItemsSource = this.EventTable.DefaultView;

            //var view = this.EventTable.DefaultView;
            //view.rows

            App.Current.Exit += Current_Exit;
        }

        private void AddCol(string name, string caption, Type t)
        {
            var col = new DataColumn(name, t);
            col.ColumnName = caption;
            this.EventTable.Columns.Add(col);
        }

        private void AddRow(string eventName, EventTypes eventType, string message)
        {
            var row = this.EventTable.NewRow();
            row[0] = eventName.Trim();
            row[1] = message.Trim();
            row[2] = DateTime.Now;


            var c = Colors.Black;

            switch (eventType)
            {
                case EventTypes.StartLights:
                    c = Colors.Green;
                    break;
                case EventTypes.RaceWinner:
                case EventTypes.ChequeredFlag:
                    c = Colors.White;
                    break;
                case EventTypes.TeamMateInPits:
                    c = Colors.CornflowerBlue;
                    break;
                case EventTypes.FastestLap:
                    c = Colors.Magenta;
                    break;
                case EventTypes.Warning:
                case EventTypes.DRSdisabled:
                case EventTypes.DRSenabled:
                    c = Colors.Orange;
                    break;
                case EventTypes.PenaltyIssued:
                case EventTypes.StopGoServed:
                case EventTypes.DriveThroughServed:
                    c = Colors.Red;
                    break;
            }

            var b = new SolidColorBrush(c);
            if (b.CanFreeze) b.Freeze();

            row[3] = b;

            if (b.Color.IsLightColor()) row[4] = Brushes.Black;
            else row[4] = Brushes.White;

            this.Dispatcher.Invoke(() =>
            {
                this.EventTable.Rows.Add(row);
            });
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            this.UnsubscribeUDPEvents();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.SubscribeUDPEvents();
        }

        public void CalculateView(IGridResize.GridSizes res = IGridResize.GridSizes.NotSet)
        {
            // throw new NotImplementedException();
        }

        public void ResizeLG()
        {
            throw new NotImplementedException();
        }

        public void ResizeMD()
        {
            throw new NotImplementedException();
        }

        public void ResizeXL()
        {
            throw new NotImplementedException();
        }

        public void ResizeXM()
        {
            throw new NotImplementedException();
        }

        public void ResizeXS()
        {
            throw new NotImplementedException();
        }

        public void SubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                // basic events:
                u.Connention.EventPacket += Connention_EventPacket;
                u.Connention.EventPacketChequeredFlag += Connention_EventPacketChequeredFlag;
                u.Connention.EventPacketDRSDisabled += Connention_EventPacketDRSDisabled;
                u.Connention.EventPacketDRSEnabled += Connention_EventPacketDRSEnabled;
                u.Connention.EventPacketLightsOut += Connention_EventPacketLightsOut;
                u.Connention.EventPacketSessionEnded += Connention_EventPacketSessionEnded;
                u.Connention.EventPacketSessionStart += Connention_EventPacketSessionStart;

                // speciaal named events:
                u.Connention.EventPacketDriveThroughPenaltyServed += Connention_EventPacketDriveThroughPenaltyServed;
                u.Connention.EventPacketFastestLap += Connention_EventPacketFastestLap;
                u.Connention.EventPacketFlashback += Connention_EventPacketFlashback;
                u.Connention.EventPacketPenalty += Connention_EventPacketPenalty;
                u.Connention.EventPacketRetirement += Connention_EventPacketRetirement;
                u.Connention.EventPacketSpeedTrap += Connention_EventPacketSpeedTrap;
                u.Connention.EventPacketStartLights += Connention_EventPacketStartLights;
                u.Connention.EventPacketStopGoPenaltyServed += Connention_EventPacketStopGoPenaltyServed;
                u.Connention.EventPacketTeamMateInPits += Connention_EventPacketTeamMateInPits;

                // costum events:
                u.Connention.EventPacketWarning2 += Connention_EventPacketWarning2;
                u.Connention.EventPacketDriverOnPits2 += Connention_EventPacketDriverOnPits2;
            }
        }

        public void UnsubscribeUDPEvents()
        {
            if (u.Connention != null)
            {
                // basic events:
                u.Connention.EventPacket -= Connention_EventPacket;
                u.Connention.EventPacketChequeredFlag -= Connention_EventPacketChequeredFlag;
                u.Connention.EventPacketDRSDisabled -= Connention_EventPacketDRSDisabled;
                u.Connention.EventPacketDRSEnabled -= Connention_EventPacketDRSEnabled;
                u.Connention.EventPacketLightsOut -= Connention_EventPacketLightsOut;
                u.Connention.EventPacketSessionEnded -= Connention_EventPacketSessionEnded;
                u.Connention.EventPacketSessionStart -= Connention_EventPacketSessionStart;

                // speciaal named events:
                u.Connention.EventPacketDriveThroughPenaltyServed -= Connention_EventPacketDriveThroughPenaltyServed;
                u.Connention.EventPacketFastestLap -= Connention_EventPacketFastestLap;
                u.Connention.EventPacketFlashback -= Connention_EventPacketFlashback;
                u.Connention.EventPacketPenalty -= Connention_EventPacketPenalty;
                u.Connention.EventPacketRetirement -= Connention_EventPacketRetirement;
                u.Connention.EventPacketSpeedTrap -= Connention_EventPacketSpeedTrap;
                u.Connention.EventPacketStartLights -= Connention_EventPacketStartLights;
                u.Connention.EventPacketStopGoPenaltyServed -= Connention_EventPacketStopGoPenaltyServed;
                u.Connention.EventPacketTeamMateInPits -= Connention_EventPacketTeamMateInPits;

                // costum events:
                u.Connention.EventPacketWarning2 -= Connention_EventPacketWarning2;
                u.Connention.EventPacketDriverOnPits2 -= Connention_EventPacketDriverOnPits2;
            }
        }

        private static ParticipantData GetDriver(int index)
        {
            var array = u.Connention?.CurrentParticipantsPacket?.Participants;

            if (index < 0 || index > array.Length) return null;
            else return array[index];
        }

        private void Connention_EventPacketSessionStart(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.Dispatcher.Invoke(() => this.EventTable.Clear());
            this.AddRow(packet.EventName, packet.EventType, "Session started.");
        }

        private void Connention_EventPacketSessionEnded(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "Session ended.");
        }

        private void Connention_EventPacketLightsOut(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "Lights out.");
        }

        private void Connention_EventPacketDRSEnabled(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "DRS enabled.");
        }

        private void Connention_EventPacketDRSDisabled(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "DRS disabled.");
        }

        private void Connention_EventPacketChequeredFlag(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "Chequered flag.");
        }

        private void Connention_EventPacketDriverOnPits2(DriverOnPits2 packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver \"");
            sb.Append(driver.ParticipantName + " (" + driver.RaceNumber + ") ");
            sb.Append(" Entered PIT on lap" + packet.LapNumber);

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketWarning2(Warning2 packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver \"");
            sb.Append(driver.ParticipantName + " (" + driver.RaceNumber + ") ");
            sb.Append("\" recieved a warning. ");
            sb.Append("Total number of driver's warnings: " + packet.TotalWarnings);

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketTeamMateInPits(F1Telemetry.Models.EventPacket.TeamMateInPits packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Team mate ");
            sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
            sb.Append(" in PIT.");

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketStopGoPenaltyServed(F1Telemetry.Models.EventPacket.StopGoPenaltyServed packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver ");
            sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
            sb.Append(" in PIT for serve Stop'n'Go penalty.");

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketStartLights(F1Telemetry.Models.EventPacket.StartLights packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "Number of start light: " + packet.NumberOfLight);
        }

        private void Connention_EventPacketSpeedTrap(F1Telemetry.Models.EventPacket.SpeedTrap packet, EventArgs e)
        {
            if (packet.IsDriverFastestInSession)
            {
                var driver = EventslogPage.GetDriver(packet.VehicleIndex);
                var sb = new StringBuilder();

                sb.Append("Driver ");
                sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
                sb.Append(" in speed trap. His speed: " + packet.Speed + " km/h\r\n");

                this.AddRow(packet.EventName, packet.EventType, sb.ToString());
            }
        }

        private void Connention_EventPacketRetirement(F1Telemetry.Models.EventPacket.Retirement packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver ");
            sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
            sb.Append(" retired for session.");

            //this.AddEventToListView(packet.EventCode, sb.ToString());
            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketPenalty(F1Telemetry.Models.EventPacket.Penalty packet, EventArgs e)
        {
            var driver1 = EventslogPage.GetDriver(packet.VehicleIndex);
            var driver2 = EventslogPage.GetDriver(packet.OtherVehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver ");
            sb.Append("- " + driver1.ParticipantName + " (" + driver1.RaceNumber + ") -");
            sb.Append(" recieved ");

            /*if (packet.Time > TimeSpan.Zero)*/
            sb.Append("+" + packet.Time.TotalSeconds + " time");

            sb.Append("penalty for '" + packet.InfragmentType.GetName() + "'.");

            if (driver2 != null)
            {
                sb.Append(" Other driver was ");
                sb.Append(driver1.ParticipantName + " (" + driver1.RaceNumber + ").");
            }

            if (packet.PlacesGained > 0)
            {
                sb.Append(" Driver 1 places gained: " + packet.PlacesGained);
            }

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketFlashback(F1Telemetry.Models.EventPacket.Flashback packet, EventArgs e)
        {
            this.AddRow(packet.EventName, packet.EventType, "Player flashbacked to " + packet.FlashbackSessionTime.ToString("hh\\:mm\\:ss\\.fff"));
        }

        private void Connention_EventPacketFastestLap(F1Telemetry.Models.EventPacket.FastestLap packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver ");
            sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
            sb.Append(" fastest in session. Laptime: " + packet.LapTime.ToString("hh\\:mm\\:ss\\.fff"));

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacketDriveThroughPenaltyServed(F1Telemetry.Models.EventPacket.DriveThroughPenaltyServed packet, EventArgs e)
        {
            var driver = EventslogPage.GetDriver(packet.VehicleIndex);
            var sb = new StringBuilder();

            sb.Append("Driver ");
            sb.Append("- " + driver.ParticipantName + " (" + driver.RaceNumber + ") -");
            sb.Append(" serving Drive Through Penalty.");

            this.AddRow(packet.EventName, packet.EventType, sb.ToString());
        }

        private void Connention_EventPacket(F1Telemetry.Models.EventPacket.PacketEventData packet, EventArgs e)
        {
            this.AddRow(packet.EventCode, packet.EventType, "Unknown event occured.");
        }
    }
}
