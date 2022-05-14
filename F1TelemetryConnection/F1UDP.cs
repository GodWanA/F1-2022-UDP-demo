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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry
{
    public class F1UDP
    {
        /// <summary>
        /// Connection's IP address
        /// </summary>
        public string IPAdress { get; private set; }
        /// <summary>
        /// Connection's Port address
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Enables async process of packet
        /// </summary>
        public bool IsAsyncPacketProcessEnabled { get; set; } = false;

        protected IPEndPoint EndPoint { get; set; }
        protected UdpClient Connection { get; set; }

        private CancellationTokenSource CancelToken;

        /// <summary>
        /// Version of the F1Telmetry dll.
        /// </summary>
        public static Version DLLVersion { get; private set; } = Assembly.GetCallingAssembly().GetName().Version;

        /// <summary>
        /// Maximum number of packet per second. Relevant on packets where intervals settable.<br/>
        /// Important to reduce cpu usage.<br/>
        /// 0 equals no limit.
        /// </summary>
        public double NumberOfPacketPerSecond { get; set; } = 10;
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

        public delegate void ErrorEvent(object sender, Exception ex, EventArgs e);

        /// <summary>
        /// Occours on UDP connection error. Sender is an Exception object.
        /// </summary>
        public event ErrorEvent ConnectionError;
        /// <summary>
        /// Occours on Packet read error. Sender is an Exception object.
        /// </summary>
        public event ErrorEvent DataReadError;
        /// <summary>
        /// Occours after UDP connection closed. Sender is current F1UFP object.
        /// </summary>
        public event EventHandler ClosedConnection;

        // main packet envets:

        public delegate void RawHandler(byte[] rawData, PacketHeader head, EventArgs e);

        public delegate void CarMotionHandler(PacketMotionData packet, EventArgs e);
        public delegate void SessionHandler(PacketSessionData packet, EventArgs e);
        public delegate void LapDataHandler(PacketLapData packet, EventArgs e);
        public delegate void ParticipantsHandler(PacketParticipantsData packet, EventArgs e);
        public delegate void CarSetupsHandler(PacketCarSetupData packet, EventArgs e);
        public delegate void CarTelemetryHandler(PacketCarTelemetryData packet, EventArgs e);
        public delegate void CarStatusHandler(PacketCarStatusData packet, EventArgs e);
        public delegate void FinalClassificationHandler(PacketFinalClassificationData packet, EventArgs e);
        public delegate void LobbyInfoHandler(PacketLobbyInfoData packet, EventArgs e);
        public delegate void SessionHistoryHandler(PacketSessionHistoryData packet, EventArgs e);
        public delegate void CarDemageHandler(PacketCarDamageData packet, EventArgs e);

        public delegate void BasicEventHandler(PacketEventData packet, EventArgs e);
        public delegate void FastestLapEventHandler(FastestLap packet, EventArgs e);
        public delegate void RetirementEventHandler(Retirement packet, EventArgs e);
        public delegate void TeamMateInPitsEventHandler(TeamMateInPits packet, EventArgs e);
        public delegate void RaceWinnerEventHandler(RaceWinner packet, EventArgs e);
        public delegate void PenaltyEventHandler(Penalty packet, EventArgs e);
        public delegate void SpeedTrapEventHandler(SpeedTrap packet, EventArgs e);
        public delegate void StartLightsEventHandler(StartLights packet, EventArgs e);
        public delegate void DriveThroughPenaltyServedEventHandler(DriveThroughPenaltyServed packet, EventArgs e);
        public delegate void StopGoPenaltyServedEventHandler(StopGoPenaltyServed packet, EventArgs e);
        public delegate void FlashbackEventHandler(Flashback packet, EventArgs e);
        public delegate void ButtonsEventHandler(Buttons packet, EventArgs e);

        /// <summary>
        /// Raw recieved byte array.
        /// </summary>
        public event RawHandler RawDataRecieved;

        /// <summary>
        /// Occours on carmotion packet read. Sender is a PacketMotionData object.
        /// </summary>
        public event CarMotionHandler CarMotionPacket;

        /// <summary>
        /// Occours on session packet read. Sender is a PacketSessionData object.
        /// </summary>
        public event SessionHandler SessionPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketLapData object.
        /// </summary>
        public event LapDataHandler LapDataPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketParticipantsData object.
        /// </summary>
        public event ParticipantsHandler ParticipantsPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarSetupData object.
        /// </summary>
        public event CarSetupsHandler CarSetupsPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarTelemetryData object.
        /// </summary>        
        public event CarTelemetryHandler CarTelemetryPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketCarStatusData object.
        /// </summary>
        public event CarStatusHandler CarStatusPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketFinalClassificationData object.
        /// </summary>
        public event FinalClassificationHandler FinalClassificationPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketLobbyInfoData object.
        /// </summary>
        public event LobbyInfoHandler LobbyInfoPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketSessionHistoryData object.
        /// </summary>
        public event SessionHistoryHandler SessionHistoryPacket;

        /// <summary>
        /// Occours on lap data packet read. Sender is a PacketFinalClassificationData object.
        /// </summary>
        public event CarDemageHandler CarDemagePacket;

        // special event packet events:
        /// <summary>
        /// Occours on any undefined event. Sender is a PacketEventData object.
        /// </summary>
        public event BasicEventHandler EventPacket;

        /// <summary>
        /// Occours on any undefined event. Sender is a FastestLap object.
        /// </summary>
        public event FastestLapEventHandler EventPacketFastestLap;

        /// <summary>
        /// Occours on any undefined event. Sender is a Retirement object.
        /// </summary>
        public event RetirementEventHandler EventPacketRetirement;

        /// <summary>
        /// Occours on any undefined event. Sender is a TeamMateInPits object.
        /// </summary>
        public event TeamMateInPitsEventHandler EventPacketTeamMateInPits;

        /// <summary>
        /// Occours on any undefined event. Sender is a RaceWinner object.
        /// </summary>
        public event RaceWinnerEventHandler EventPacketRaceWinner;

        /// <summary>
        /// Occours on any undefined event. Sender is a Penalty object.
        /// </summary>
        public event PenaltyEventHandler EventPacketPenalty;

        /// <summary>
        /// Occours on any undefined event. Sender is a SpeedTrap object.
        /// </summary>
        public event SpeedTrapEventHandler EventPacketSpeedTrap;

        /// <summary>
        /// Occours on any undefined event. Sender is a StartLights object.
        /// </summary>
        public event StartLightsEventHandler EventPacketStartLights;

        /// <summary>
        /// Occours on any undefined event. Sender is a DriveThroughPenaltyServed object.
        /// </summary>
        public event DriveThroughPenaltyServedEventHandler EventPacketDriveThroughPenaltyServed;

        /// <summary>
        /// Occours on any undefined event. Sender is a StopGoPenaltyServed object.
        /// </summary>
        public event StopGoPenaltyServedEventHandler EventPacketStopGoPenaltyServed;

        /// <summary>
        /// Occours on any undefined event. Sender is a Flashback object.
        /// </summary>
        public event FlashbackEventHandler EventPacketFlashback;

        /// <summary>
        /// Occours on any undefined event. Sender is a Buttons object.
        /// </summary>
        public event ButtonsEventHandler EventPacketButtons;

        /// <summary>
        /// Create and connects to Codemaster's F1 games UDP service.
        /// </summary>
        /// <param name="ipAdress">Service IP adress as string</param>
        /// <param name="port">Service Port number as integer</param>
        public F1UDP(string ipAdress, int port)
        {
            this.Connect(ipAdress, port);
        }

        /// <summary>
        /// Opens connection to selected ip and port.
        /// </summary>
        /// <param name="ip">UDP data sender's IP</param>
        /// <param name="port">UDP data sender's port</param>
        public void Connect(string ip, int port)
        {
            try
            {
                this.IPAdress = ip;
                this.Port = port;
                this.IsConnecting = true;
                this.EndPoint = new IPEndPoint(IPAddress.Parse(this.IPAdress), this.Port);
                this.CancelToken = new CancellationTokenSource();

                this.Connection = new UdpClient(this.EndPoint);
                this.Connection.DontFragment = true;
                this.Connection.Client.ReceiveTimeout = int.MaxValue;
                this.Connection.Client.SendTimeout = int.MaxValue;

                this.Connection.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e)
            {
                this.OnConnectionError(e);
            }
        }

        private void recv(IAsyncResult res)
        {
            if (this.IsConnecting)
            {
                IPEndPoint RemoteIpEndPoint = this.EndPoint;
                byte[] received = this.Connection.EndReceive(res, ref RemoteIpEndPoint);

                if (received != null)
                {
                    if (!this.IsAsyncPacketProcessEnabled)
                    {
                        this.ByteArrayProcess(received);
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            this.ByteArrayProcess(received);
                        }, this.CancelToken.Token);
                    }
                }

                //received = null;
                this?.Connection?.BeginReceive(new AsyncCallback(recv), null);
            }
        }

        /// <summary>
        /// Closing UPD connection
        /// </summary>
        public void Close()
        {
            if (this.IsConnecting)
            {
                this.IsConnecting = false;
                this.CancelToken.Cancel();
                //this.CancelToken.Dispose();
                //this.CancelToken = null;
            }

            if (this.ClosedConnection != null) this.ClosedConnection(this, new EventArgs());
            this.Connection.Close();
        }


        /// <summary>
        /// Closing, and reopening connection to UDP service. 
        /// </summary>
        public void Reconnect()
        {
            this.Close();
            this.Connect(this.IPAdress, this.Port);
        }


        /// <summary>
        /// Closing the current UDP service, and opens a new connection to another service.
        /// </summary>
        /// <param name="ip">New UDP data sender's ip</param>
        /// <param name="port">New UDP data sender's port</param>
        public void Reconnect(string ip, int port)
        {
            this.Close();
            this.Connect(ip, port);
        }


        /// <summary>
        /// Selects and delver packet to its process method.
        /// </summary>
        /// <param name="array">Raw byte array</param>
        private void ByteArrayProcess(byte[] array)
        {
            //var start = DateTime.Now;
            var header = new PacketHeader(array);
            this.OnRawDataRecive(array, header);

            switch (header.PacketID)
            {
                case PacketTypes.CarMotion:
                    this.CarMotion(ref array, ref header);
                    break;
                case PacketTypes.Session:
                    this.Session(ref array, ref header);
                    break;
                case PacketTypes.LapData:
                    this.Lapdata(ref array, ref header);
                    break;
                case PacketTypes.Event:
                    this.Event(ref array, ref header);
                    break;
                case PacketTypes.Participants:
                    this.Participants(ref array, ref header);
                    break;
                case PacketTypes.CarSetups:
                    this.CarSetups(ref array, ref header);
                    break;
                case PacketTypes.CarTelemetry:
                    this.CarTelemetry(ref array, ref header);
                    break;
                case PacketTypes.CarStatus:
                    this.CarStatus(ref array, ref header);
                    break;
                case PacketTypes.FinalClassification:
                    this.FinalClassification(ref array, ref header);
                    break;
                case PacketTypes.LobbyInfo:
                    this.LobbyInfo(ref array, ref header);
                    break;
                case PacketTypes.CarDamage:
                    this.CarDamage(ref array, ref header);
                    break;
                case PacketTypes.SessionHistory:
                    this.SessionHistory(ref array, ref header);
                    break;
            }

            //if (DateTime.Now - start > TimeSpan.FromSeconds(0.1))
            //{
            //    this.OnDataReadError(new Exception("LASSÚ!\r\n" + header.PacketID + "\r\n" + DateTime.Now));
            //}
        }

        private void CarMotion(ref byte[] array, ref PacketHeader head)
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

        private void Session(ref byte[] array, ref PacketHeader head)
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

        private void Lapdata(ref byte[] array, ref PacketHeader head)
        {
            try
            {
                var data = new PacketLapData(head, array);
                this.OnLapDataPacket(data);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(TaskCanceledException)) this.OnDataReadError(ex);
            }
        }

        private void Event(ref byte[] array, ref PacketHeader head)
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

        private void Participants(ref byte[] array, ref PacketHeader head)
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

        private void CarSetups(ref byte[] array, ref PacketHeader head)
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

        private void CarTelemetry(ref byte[] array, ref PacketHeader head)
        {
            try
            {
                var data = new PacketCarTelemetryData(head, array);
                this.OnCarTelemetryPacket(data);

                if (head.PacketFormat <= 2020) this.OnEventPacketButtons(data.BuildButtonEvent());
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarStatus(ref byte[] array, ref PacketHeader head)
        {
            try
            {
                var data = new PacketCarStatusData(head, array);
                this.OnCarStatusPacket(data);

                if (head.PacketFormat <= 2020) this.OnCarDemagePacket(data.BuildCarDemagePacket());
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void FinalClassification(ref byte[] array, ref PacketHeader head)
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

        private void LobbyInfo(ref byte[] array, ref PacketHeader head)
        {
            try
            {
                var data = new PacketLobbyInfoData(head, array);

                if (head.PacketFormat == 2020) data.BuildRaceNumbers(this.LastParticipantsPacket.Participants);

                this.OnLobbyInfoPacket(data);
            }
            catch (Exception ex)
            {
                this.OnDataReadError(ex);
            }
        }

        private void CarDamage(ref byte[] array, ref PacketHeader head)
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

        private void SessionHistory(ref byte[] array, ref PacketHeader head)
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

        protected virtual void OnConnectionError(Exception error)
        {
            Debug.WriteLine(error.Message);
            Debug.WriteLine(error.StackTrace);
            if (this.ConnectionError != null) this.ConnectionError(this, error, new EventArgs());
        }

        protected virtual void OnDataReadError(Exception error)
        {
            Debug.WriteLine(error.Message);
            Debug.WriteLine(error.StackTrace);
            if (this.DataReadError != null) this.DataReadError(this, error, new EventArgs());
        }

        protected virtual void OnClosedConnection(object sender)
        {
            Debug.WriteLine("F1 UDP Connection is closing.");
            if (this.ClosedConnection != null) this.ClosedConnection(sender, new EventArgs());
        }

        protected virtual void OnCarMotionPacket(PacketMotionData sender)
        {
            //this.LastMotionPacket?.Dispose();
            //this.LastMotionPacket = null;
            this.LastMotionPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_CarMotion >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarMotionPacket != null) this.CarMotionPacket(sender, new EventArgs());
                this.LastTime_CarMotion = DateTime.Now;
            }
        }

        protected virtual void OnSessionPacket(PacketSessionData sender)
        {
            //this.LastSessionDataPacket?.Dispose();
            //this.LastSessionDataPacket = null;
            this.LastSessionDataPacket = sender;
            if (this.SessionPacket != null) this.SessionPacket(sender, new EventArgs());
        }

        protected virtual void OnLapDataPacket(PacketLapData sender)
        {
            if (sender.Header.PacketFormat < 2021 && this.LastSessionDataPacket != null && this.LastLapDataPacket != null)
            {
                for (int i = 0; i < sender.Lapdata.Length; i++)
                {
                    var c = sender.Lapdata[i];
                    var l = this.LastLapDataPacket.Lapdata[i];

                    c.SetLastPitLap(l.GetLastPitLap());
                    c.CalculateStatus(this.LastSessionDataPacket.SessionType);
                }
            }

            //this.LastLapDataPacket?.Dispose();
            //this.LastLapDataPacket = null;
            this.LastLapDataPacket = sender;
            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_LapData >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.LapDataPacket != null) this.LapDataPacket(sender, new EventArgs());
                this.LastTime_LapData = DateTime.Now;
            }
        }

        protected virtual void OnParticipantsPacket(PacketParticipantsData sender)
        {
            //this.LastParticipantsPacket?.Dispose();
            //this.LastParticipantsPacket = null;
            this.LastParticipantsPacket = sender;
            if (this.ParticipantsPacket != null) this.ParticipantsPacket(sender, new EventArgs());
        }

        protected virtual void OnCarSetupsPacket(PacketCarSetupData sender)
        {
            //this.LastCarSetupPacket?.Dispose();
            //this.LastCarSetupPacket = null;
            this.LastCarSetupPacket = sender;
            if (this.CarSetupsPacket != null) this.CarSetupsPacket(sender, new EventArgs());
        }

        protected virtual void OnCarTelemetryPacket(PacketCarTelemetryData sender)
        {
            //this.LastCarTelmetryPacket?.Dispose();
            //this.LastCarTelmetryPacket = null;
            this.LastCarTelmetryPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_Cartelemetry >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarTelemetryPacket != null) this.CarTelemetryPacket(sender, new EventArgs());
                this.LastTime_Cartelemetry = DateTime.Now;
            }
        }

        protected virtual void OnCarStatusPacket(PacketCarStatusData sender)
        {
            //this.LastCarStatusDataPacket?.Dispose();
            //this.LastCarStatusDataPacket = null;
            this.LastCarStatusDataPacket = sender;

            if (this.NumberOfPacketPerSecond == 0 || DateTime.Now - this.LastTime_CarStatus >= TimeSpan.FromMilliseconds(1000 / this.NumberOfPacketPerSecond))
            {
                if (this.CarStatusPacket != null) this.CarStatusPacket(sender, new EventArgs());
                this.LastTime_CarStatus = DateTime.Now;
            }
        }

        protected virtual void OnFinalClassificationPacket(PacketFinalClassificationData sender)
        {
            //this.LastFinalClassificationPacket?.Dispose();
            //this.LastFinalClassificationPacket = null;
            this.LastFinalClassificationPacket = sender;
            if (this.FinalClassificationPacket != null) this.FinalClassificationPacket(sender, new EventArgs());
        }

        protected virtual void OnLobbyInfoPacket(PacketLobbyInfoData sender)
        {
            this.LastLobbyInfoPacket?.Dispose();
            this.LastLobbyInfoPacket = null;
            this.LastLobbyInfoPacket = sender;
            if (this.LobbyInfoPacket != null) this.LobbyInfoPacket(sender, new EventArgs());
        }

        protected virtual void OnSessionHistoryPacket(PacketSessionHistoryData sender)
        {
            //this.LastSessionHistoryPacket[sender.CarIndex]?.Dispose();
            //this.LastSessionHistoryPacket[sender.CarIndex] = null;
            this.LastSessionHistoryPacket[sender.CarIndex] = sender;
            if (this.SessionHistoryPacket != null) this.SessionHistoryPacket(sender, new EventArgs());
        }

        protected virtual void OnCarDemagePacket(PacketCarDamageData sender)
        {
            //this.LastCarDemagePacket?.Dispose();
            //this.LastCarDemagePacket = null;
            this.LastCarDemagePacket = sender;
            if (this.CarDemagePacket != null) this.CarDemagePacket(sender, new EventArgs());
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

        protected virtual void OnRawDataRecive(byte[] sender, PacketHeader head)
        {
            if (this.RawDataRecieved != null) this.RawDataRecieved(sender, head, new EventArgs());
        }
    }
}
