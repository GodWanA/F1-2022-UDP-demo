using F1Telemetry.Models;
using F1Telemetry.Models.CarDamagePacket;
using F1Telemetry.Models.CarSetupsPacket;
using F1Telemetry.Models.CarStatusPacket;
using F1Telemetry.Models.CarTelemetryPacket;
using F1Telemetry.Models.EventPacket;
using F1Telemetry.Models.FinalClassificationPacket;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.LobbyInfoPacket;
using F1Telemetry.Models.MotionPacket;
using F1Telemetry.Models.ParticipantsPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry
{
    public class F1UDP
    {
        /// <summary>
        /// The connections working paralel task
        /// </summary>
        public Task _Task { get; private set; }
        /// <summary>
        /// Connection's IP address
        /// </summary>
        public string IPAdress { get; private set; }
        /// <summary>
        /// Connection's Port address
        /// </summary>
        public int Port { get; private set; }

        protected IPEndPoint EndPoint { get; set; }
        protected UdpClient Connection { get; set; }
        /// <summary>
        /// Maximum number of packet per second. Relevant on packets where intervals settable.<br/>
        /// Important to reduce cpu usage.<br/>
        /// 0 equals no limit.
        /// </summary>
        public double NumberOfPacketPerSecond { get; set; } = 8;
        /// <summary>
        /// Is connection is still alive
        /// </summary>
        public bool IsConnecting { get; private set; }
        /// <summary>
        /// Last recived motion packet.
        /// </summary>
        public PacketMotionData LastMotionPacket { get; private set; }
        /// <summary>
        /// Last recived session data packet.
        /// </summary>
        public PacketSessionData LastSessionDataPacket { get; private set; }
        /// <summary>
        /// Last recived lap data packet.
        /// </summary>
        public PacketLapData LastLapDataPacket { get; private set; }
        /// <summary>
        /// Last recived participants packet.
        /// </summary>
        public PacketParticipantsData LastParticipantsPacket { get; private set; }
        /// <summary>
        /// Last recived car setup packet.
        /// </summary>
        public PacketCarSetupData LastCarSetupPacket { get; private set; }
        /// <summary>
        /// Last recieved car telemetry packet.
        /// </summary>
        public PacketCarTelemetryData LastCarTelmetryPacket { get; private set; }
        /// <summary>
        /// Last recieved car status data packet.
        /// </summary>
        public PacketCarStatusData LastCarStatusDataPacket { get; private set; }
        /// <summary>
        /// Last recieved car final classification packet.
        /// </summary>
        public PacketFinalClassificationData LastFinalClassificationPacket { get; private set; }
        /// <summary>
        /// Last recieved lobby info packet.
        /// </summary>
        public PacketLobbyInfoData LastLobbyInfoPacket { get; private set; }
        /// <summary>
        /// Collects and contain all cars last aviable session history packets.
        /// </summary>
        public PacketSessionHistoryData[] LastSessionHistoryPacket { get; private set; } = new PacketSessionHistoryData[22];
        /// <summary>
        /// Last recieved car demage packet.
        /// </summary>
        public PacketCarDamageData LastCarDemagePacket { get; private set; }
        /// <summary>
        /// last time when car motion event delegated.
        /// </summary>
        public DateTime LastTime_CarMotion { get; private set; }
        /// <summary>
        /// last time when lap data event delegated.
        /// </summary>
        public DateTime LastTime_LapData { get; private set; }
        /// <summary>
        /// last time when car telemetry event delegated.
        /// </summary>
        public DateTime LastTime_Cartelemetry { get; private set; }
        /// <summary>
        /// last time when car status event delegated.
        /// </summary>
        public DateTime LastTime_CarStatus { get; private set; }

        // udp events:
        /// <summary>
        /// Occours on UDP connection error. Sender is an Exception object.
        /// </summary>
        public event EventHandler ConnectionError;
        /// <summary>
        /// Occours on Packet read error. Sender is an Exception object.
        /// </summary>
        public event EventHandler DataReadError;
        /// <summary>
        /// Occours after UDP connection closed. Sender is current F1UFP object.
        /// </summary>
        public event EventHandler ClosedConnection;

        // main packet envets:
        /// <summary>
        /// Occours on carmotion packet read. Sender is a PacketMotionData object.
        /// </summary>
        public event EventHandler CarMotionPacket;
        /// <summary>
        /// Occours on session packet read. Sender is a PacketSessionData object.
        /// </summary>
        public event EventHandler SessionPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketLapData object.
        /// </summary>
        public event EventHandler LapDataPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketParticipantsData object.
        /// </summary>
        public event EventHandler ParticipantsPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarSetupData object.
        /// </summary>
        public event EventHandler CarSetupsPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarTelemetryData object.
        /// </summary>        
        public event EventHandler CarTelemetryPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarStatusData object.
        /// </summary>
        public event EventHandler CarStatusPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketFinalClassificationData object.
        /// </summary>
        public event EventHandler FinalClassificationPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketLobbyInfoData object.
        /// </summary>
        public event EventHandler LobbyInfoPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketSessionHistoryData object.
        /// </summary>
        public event EventHandler SessionHistoryPacket;
        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketFinalClassificationData object.
        /// </summary>
        public event EventHandler DemagePacket;

        // special event packet events:
        /// <summary>
        /// Occours on any undefined event. Sender is a PacketEventData object.
        /// </summary>
        public event EventHandler EventPacket;
        /// <summary>
        /// Occours on any undefined event. Sender is a FastestLap object.
        /// </summary>
        public event EventHandler EventPacketFastestLap;
        /// <summary>
        /// Occours on any undefined event. Sender is a Retirement object.
        /// </summary>
        public event EventHandler EventPacketRetirement;
        /// <summary>
        /// Occours on any undefined event. Sender is a TeamMateInPits object.
        /// </summary>
        public event EventHandler EventPacketTeamMateInPits;
        /// <summary>
        /// Occours on any undefined event. Sender is a RaceWinner object.
        /// </summary>
        public event EventHandler EventPacketRaceWinner;
        /// <summary>
        /// Occours on any undefined event. Sender is a Penalty object.
        /// </summary>
        public event EventHandler EventPacketPenalty;
        /// <summary>
        /// Occours on any undefined event. Sender is a SpeedTrap object.
        /// </summary>
        public event EventHandler EventPacketSpeedTrap;
        /// <summary>
        /// Occours on any undefined event. Sender is a StartLights object.
        /// </summary>
        public event EventHandler EventPacketStartLights;
        /// <summary>
        /// Occours on any undefined event. Sender is a DriveThroughPenaltyServed object.
        /// </summary>
        public event EventHandler EventPacketDriveThroughPenaltyServed;
        /// <summary>
        /// Occours on any undefined event. Sender is a StopGoPenaltyServed object.
        /// </summary>
        public event EventHandler EventPacketStopGoPenaltyServed;
        /// <summary>
        /// Occours on any undefined event. Sender is a Flashback object.
        /// </summary>
        public event EventHandler EventPacketFlashback;
        /// <summary>
        /// Occours on any undefined event. Sender is a Buttons object.
        /// </summary>
        public event EventHandler EventPacketButtons;

        /// <summary>
        /// Create and connects to Codemaster's F1 games UDP service.
        /// </summary>
        /// <param name="ipAdress">Service IP adress as string</param>
        /// <param name="port">Service Port number as integer</param>
        public F1UDP(string ipAdress, int port)
        {
            this.IPAdress = ipAdress;
            this.Port = port;
            this.IsConnecting = true;

            this.EndPoint = new IPEndPoint(IPAddress.Parse(this.IPAdress), this.Port);

            this.Connection = new UdpClient(this.EndPoint);

            Task.Run(() =>
            {
                try
                {
                    while (IsConnecting)
                    {
                        var remoteEndPoint = new IPEndPoint(this.EndPoint.Address, this.EndPoint.Port);
                        var receivedResults = this.Connection.Receive(ref remoteEndPoint);

                        Task.Run(() =>
                        {
                            this.ByteArrayProcess((byte[])(receivedResults.Clone()));
                        });
                    }
                }
                catch (Exception ex)
                {
                    this.OnConnectionError(ex);
                }
                finally
                {
                    this.Connection.Close();
                    this.OnClosedConnection(this);
                }
            });
        }

        /// <summary>
        /// Selects and delver packet to its process method.
        /// </summary>
        /// <param name="array">Raw byte array</param>
        private void ByteArrayProcess(byte[] array)
        {
            var header = new PacketHeader(array);

            switch (header.PacketID)
            {
                case PacketTypes.CarMotion:
                    this.CarMotion(array, header);
                    break;
                case PacketTypes.Session:
                    this.Session(array, header);
                    break;
                case PacketTypes.LapData:
                    this.Lapdata(array, header);
                    break;
                case PacketTypes.Event:
                    this.Event(array, header);
                    break;
                case PacketTypes.Participants:
                    this.Participants(array, header);
                    break;
                case PacketTypes.CarSetups:
                    this.CarSetups(array, header);
                    break;
                case PacketTypes.CarTelemetry:
                    this.CarTelemetry(array, header);
                    break;
                case PacketTypes.CarStatus:
                    this.CarStatus(array, header);
                    break;
                case PacketTypes.FinalClassification:
                    this.FinalClassification(array, header);
                    break;
                case PacketTypes.LobbyInfo:
                    this.LobbyInfo(array, header);
                    break;
                case PacketTypes.CarDamage:
                    this.CarDamage(array, header);
                    break;
                case PacketTypes.SessionHistory:
                    this.SessionHistory(array, header);
                    break;
            }
        }

        /// <summary>
        /// Closing UPD connection
        /// </summary>
        public void Close()
        {
            this.IsConnecting = false;
            //this.Connection.Close();
            //this._Thread.Abort();
        }

        private void CarMotion(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketMotionData(head, array);
                this.OnCarMotionPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void Session(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketSessionData(head, array);
                this.OnSessionPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void Lapdata(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketLapData(head, array);
                this.OnLapDataPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void Event(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketEventData(head, array);
                switch (data.EventType)
                {
                    default:
                        this.OnEventPacket(data);
                        break;
                    case EventTypes.FastestLap:
                        data = new FastestLap(data, array);
                        this.OnEventPacketFastestLap((FastestLap)data);
                        break;
                    case EventTypes.Retirement:
                        data = new Retirement(data, array);
                        this.OnEventPacketRetirement((Retirement)data);
                        break;
                    case EventTypes.TeamMateInPits:
                        data = new TeamMateInPits(data, array);
                        this.OnEventPacketTeamMateInPits((TeamMateInPits)data);
                        break;
                    case EventTypes.RaceWinner:
                        data = new RaceWinner(data, array);
                        this.OnEventPacketRaceWinner((RaceWinner)data);
                        break;
                    case EventTypes.PenaltyIssued:
                        data = new Penalty(data, array);
                        this.OnEventPacketPenalty((Penalty)data);
                        break;
                    case EventTypes.SpeedTrapTriggered:
                        data = new SpeedTrap(data, array);
                        this.OnEventPacketSpeedTrap((SpeedTrap)data);
                        break;
                    case EventTypes.StartLights:
                        data = new StartLights(data, array);
                        this.OnEventPacketStartLights((StartLights)data);
                        break;
                    case EventTypes.DriveThroughServed:
                        data = new DriveThroughPenaltyServed(data, array);
                        this.OnEventDriveThroughPenaltyServed((DriveThroughPenaltyServed)data);
                        break;
                    case EventTypes.StopGoServed:
                        data = new StopGoPenaltyServed(data, array);
                        this.OnEventStopGoPenaltyServed((StopGoPenaltyServed)data);
                        break;
                    case EventTypes.Flashback:
                        data = new Flashback(data, array);
                        this.OnEventPacketFlashback((Flashback)data);
                        break;
                    case EventTypes.ButtonStatus:
                        data = new Buttons(data, array);
                        this.OnEventPacketButtons((Buttons)data);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void Participants(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketParticipantsData(head, array);
                this.OnParticipantsPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarSetups(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketCarSetupData(head, array);
                this.OnCarSetupsPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarTelemetry(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketCarTelemetryData(head, array);
                this.OnCarTelemetryPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarStatus(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketCarStatusData(head, array);
                this.OnCarStatusPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void FinalClassification(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketFinalClassificationData(head, array);
                this.OnFinalClassificationPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void LobbyInfo(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketLobbyInfoData(head, array);
                this.OnLobbyInfoPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarDamage(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketCarDamageData(head, array);
                this.OnCarDemagePacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void SessionHistory(byte[] array, PacketHeader head)
        {
            try
            {
                var data = new PacketSessionHistoryData(head, array);
                this.OnSessionHistoryPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        protected virtual void OnConnectionError(Exception sender)
        {
            Debug.WriteLine(sender.Message);
            Debug.WriteLine(sender.StackTrace);
            if (this.ConnectionError != null) this.ConnectionError(sender, new EventArgs());
        }

        protected virtual void OnDataReadError(Exception sender)
        {
            Debug.WriteLine(sender.Message);
            Debug.WriteLine(sender.StackTrace);
            if (this.DataReadError != null) this.DataReadError(sender, new EventArgs());
        }

        protected virtual void OnClosedConnection(object sender)
        {
            Debug.WriteLine("F1 UDP Connection is closing.");
            if (this.ClosedConnection != null) this.ClosedConnection(sender, new EventArgs());
        }

        protected virtual void OnCarMotionPacket(PacketMotionData sender)
        {
            this.LastMotionPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_CarMotion >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarMotionPacket != null) this.CarMotionPacket(sender, new EventArgs());
                this.LastTime_CarMotion = DateTime.Now;
            }
        }

        protected virtual void OnSessionPacket(PacketSessionData sender)
        {
            this.LastSessionDataPacket = sender;
            if (this.SessionPacket != null) this.SessionPacket(sender, new EventArgs());
        }

        protected virtual void OnLapDataPacket(PacketLapData sender)
        {
            this.LastLapDataPacket = sender;
            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_LapData >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.LapDataPacket != null) this.LapDataPacket(sender, new EventArgs());
                this.LastTime_LapData = DateTime.Now;
            }
        }

        protected virtual void OnParticipantsPacket(PacketParticipantsData sender)
        {
            this.LastParticipantsPacket = sender;
            if (this.ParticipantsPacket != null) this.ParticipantsPacket(sender, new EventArgs());
        }

        protected virtual void OnCarSetupsPacket(PacketCarSetupData sender)
        {
            this.LastCarSetupPacket = sender;
            if (this.CarSetupsPacket != null) this.CarSetupsPacket(sender, new EventArgs());
        }

        protected virtual void OnCarTelemetryPacket(PacketCarTelemetryData sender)
        {
            this.LastCarTelmetryPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_Cartelemetry >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarTelemetryPacket != null) this.CarTelemetryPacket(sender, new EventArgs());
                this.LastTime_Cartelemetry = DateTime.Now;
            }
        }

        protected virtual void OnCarStatusPacket(PacketCarStatusData sender)
        {
            this.LastCarStatusDataPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_CarStatus >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarStatusPacket != null) this.CarStatusPacket(sender, new EventArgs());
                this.LastTime_CarStatus = DateTime.Now;
            }
        }

        protected virtual void OnFinalClassificationPacket(PacketFinalClassificationData sender)
        {
            this.LastFinalClassificationPacket = sender;
            if (this.FinalClassificationPacket != null) this.FinalClassificationPacket(sender, new EventArgs());
        }

        protected virtual void OnLobbyInfoPacket(PacketLobbyInfoData sender)
        {
            this.LastLobbyInfoPacket = sender;
            if (this.LobbyInfoPacket != null) this.LobbyInfoPacket(sender, new EventArgs());
        }

        protected virtual void OnSessionHistoryPacket(PacketSessionHistoryData sender)
        {
            this.LastSessionHistoryPacket[sender.CarIndex] = sender;
            if (this.SessionHistoryPacket != null) this.SessionHistoryPacket(sender, new EventArgs());
        }

        protected virtual void OnCarDemagePacket(PacketCarDamageData sender)
        {
            this.LastCarDemagePacket = sender;
            if (this.DemagePacket != null) this.DemagePacket(sender, new EventArgs());
        }

        protected virtual void OnEventPacket(PacketEventData sender)
        {
            if (this.EventPacket != null) this.EventPacket(sender, new EventArgs());
        }

        protected virtual void OnEventPacketFastestLap(FastestLap sender)
        {
            if (this.EventPacketFastestLap != null) this.EventPacketFastestLap(sender, new EventArgs());
        }

        protected virtual void OnEventPacketRetirement(Retirement sender)
        {
            if (this.EventPacketRetirement != null) this.EventPacketRetirement(sender, new EventArgs());
        }

        protected virtual void OnEventPacketTeamMateInPits(TeamMateInPits sender)
        {
            if (this.EventPacketTeamMateInPits != null) this.EventPacketTeamMateInPits(sender, new EventArgs());
        }

        protected virtual void OnEventPacketRaceWinner(RaceWinner sender)
        {
            if (this.EventPacketRaceWinner != null) this.EventPacketRaceWinner(sender, new EventArgs());
        }

        protected virtual void OnEventPacketPenalty(Penalty sender)
        {
            if (this.EventPacketPenalty != null) this.EventPacketPenalty(sender, new EventArgs());
        }

        protected virtual void OnEventPacketSpeedTrap(SpeedTrap sender)
        {
            if (this.EventPacketSpeedTrap != null) this.EventPacketSpeedTrap(sender, new EventArgs());
        }

        protected virtual void OnEventPacketStartLights(StartLights sender)
        {
            if (this.EventPacketStartLights != null) this.EventPacketStartLights(sender, new EventArgs());
        }

        protected virtual void OnEventDriveThroughPenaltyServed(DriveThroughPenaltyServed sender)
        {
            if (this.EventPacketDriveThroughPenaltyServed != null) this.EventPacketDriveThroughPenaltyServed(sender, new EventArgs());
        }

        protected virtual void OnEventStopGoPenaltyServed(StopGoPenaltyServed sender)
        {
            if (this.EventPacketStopGoPenaltyServed != null) this.EventPacketStopGoPenaltyServed(sender, new EventArgs());
        }

        protected virtual void OnEventPacketFlashback(Flashback sender)
        {
            if (this.EventPacketFlashback != null) this.EventPacketFlashback(sender, new EventArgs());
        }

        protected virtual void OnEventPacketButtons(Buttons sender)
        {
            if (this.EventPacketButtons != null) this.EventPacketButtons(sender, new EventArgs());
        }
    }
}
