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
using System.Threading;
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
        public double NumberOfPacketPerSecond { get; set; } = 8;
        /// <summary>
        /// Is connection is still alive
        /// </summary>
        public bool IsConnecting { get; private set; }
        public PacketMotionData LastMotionPacket { get; private set; }
        public PacketSessionData LastSessionDataPacket { get; private set; }
        public PacketLapData LastLapDataPacket { get; private set; }
        public PacketParticipantsData LastParticipantsPacket { get; private set; }
        public PacketCarSetupData LastCarSetupPacket { get; private set; }
        public PacketCarTelemetryData LastCarTelmetryPacket { get; private set; }
        public PacketCarStatusData LastCarStatusDataPacket { get; private set; }
        public PacketFinalClassificationData LastFinalClassificationPacket { get; private set; }
        public PacketLobbyInfoData LastLobbyInfoPacket { get; private set; }
        public PacketSessionHistoryData[] LastSessionHistoryPacket { get; private set; } = new PacketSessionHistoryData[22];
        public PacketCarDamageData LastCarDemagePacket { get; private set; }
        public DateTime LastTime_CarMotion { get; private set; }
        public DateTime LastTime_LapData { get; private set; }
        public DateTime LastTime_Cartelemetry { get; private set; }
        public DateTime LastTime_CarStatus { get; private set; }

        public event EventHandler ConnectionError;
        public event EventHandler DataReadError;
        public event EventHandler ClosedConnection;
        public event EventHandler CarMotionPacket;
        public event EventHandler SessionPacket;
        public event EventHandler LapDataPacket;
        public event EventHandler ParticipantsPacket;
        public event EventHandler CarSetupsPacket;
        public event EventHandler CarTelemetryPacket;
        public event EventHandler CarStatusPacket;
        public event EventHandler FinalClassificationPacket;
        public event EventHandler LobbyInfoPacket;
        public event EventHandler SessionHistoryPacket;
        public event EventHandler DemagePacket;

        public event EventHandler EventPacket;
        public event EventHandler EventPacketFastestLap;
        public event EventHandler EventPacketRetirement;
        public event EventHandler EventPacketTeamMateInPits;
        public event EventHandler EventPacketRaceWinner;
        public event EventHandler EventPacketPenalty;
        public event EventHandler EventPacketSpeedTrap;
        public event EventHandler EventPacketStartLights;
        public event EventHandler EventPacketDriveThroughPenaltyServed;
        public event EventHandler EventPacketStopGoPenaltyServed;
        public event EventHandler EventPacketFlashback;
        public event EventHandler EventPacketButtons;

        //private DateTime lastClean;

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
            //this.lastClean = DateTime.Now;
            //this.Connection.Client.ReceiveTimeout = 3;
            //this.Connection.Client.SendTimeout = 3;

            Task.Run(() =>
            {
                try
                {
                    while (IsConnecting)
                    {
                        //var receivedResults = await this.Connection.ReceiveAsync();
                        //Task.Run(() => this.ByteArrayProcess((byte[])(receivedResults.Buffer.Clone())));

                        var remoteEndPoint = new IPEndPoint(this.EndPoint.Address, this.EndPoint.Port);
                        var receivedResults = this.Connection.Receive(ref remoteEndPoint);

                        Task.Run(() =>
                        {
                            this.ByteArrayProcess((byte[])(receivedResults.Clone()));
                        });

                        //this.ByteArrayProcess((byte[])(receivedResults.Clone()));


                        //if (DateTime.Now - this.lastClean > TimeSpan.FromSeconds(0.3))
                        //{
                        //    this.Connection.Close();
                        //    //this.Connection.Dispose();
                        //    this.Connection = new UdpClient(this.EndPoint);
                        //    this.lastClean = DateTime.Now;
                        //}
                    }
                }
                catch (Exception ex)
                {
                    this.OnConnectionError(ex);
                    this.Connection.Close();
                }
            });

            //var t = new Thread(delegate ()
            //{
            //    try
            //    {
            //        while (IsConnecting)
            //        {
            //            //var receivedResults = await this.Connection.ReceiveAsync();
            //            //Task.Run(() => this.ByteArrayProcess((byte[])(receivedResults.Buffer.Clone())));

            //            var remoteEndPoint = new IPEndPoint(this.EndPoint.Address, this.EndPoint.Port);
            //            var receivedResults = this.Connection.Receive(ref remoteEndPoint);
            //            Task.Run(() => this.ByteArrayProcess((byte[])(receivedResults.Clone())));
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        this.OnConnectionError(ex);
            //        this.Connection.Close();
            //    }
            //});
            //t.Priority = ThreadPriority.BelowNormal;
            //t.Start();

            //try
            //{
            //    this.Connection.BeginReceive(new AsyncCallback(recv), null);
            //}
            //catch (Exception ex)
            //{
            //    this.OnConnectionError(ex);
            //    this.Connection.Close();
            //}
        }

        private void recv(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = this.EndPoint;
            try
            {
                byte[] array = this.Connection.EndReceive(res, ref RemoteIpEndPoint);

                //Process codes
                Task.Run(() => this.ByteArrayProcess((byte[])array.Clone()));
                //this.ByteArrayProcess((byte[])array.Clone());
            }
            catch (Exception ex)
            {
                this.OnConnectionError(ex);
                this.IsConnecting = false;
            }

            if (this.IsConnecting)
            {
                //int d = (int)DateTime.Now.Ticks % 10;
                //if (d != 1) Thread.Sleep(10 - d);
                this.Connection.BeginReceive(new AsyncCallback(recv), null);
            }
            else
            {
                this.OnClosedConnection(this);
                this.Connection.Close();
            }
        }

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
