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
        /// Is connection is still alive
        /// </summary>
        public bool IsConnecting { get; private set; }
        public PacketMotionData LastMotionPacket { get; private set; }
        public PacketSessionData LastSessionDataPacket { get; private set; }
        public PacketLapData LastLapDataPacket { get; private set; }
        public PacketParticipantsData LastParticipantsPacket { get; private set; }
        public PacketCarSetupData LastCarSetupPacket { get; private set; }
        public PacketCarTelemetryData LastCartelmetryPacket { get; private set; }
        public PacketCarStatusData LastCarStatusDataPacket { get; private set; }
        public PacketFinalClassificationData LastFinalClassificationPacket { get; private set; }
        public PacketLobbyInfoData LastLobbyInfoPacket { get; private set; }
        public PacketSessionHistoryData LastSessionHistoryPacket { get; private set; }
        internal PacketCarDamageData LastCarDemagePacket { get; private set; }

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

            this._Task = Task.Run(() =>
            {
                this.Connection = new UdpClient(this.EndPoint);
                IPEndPoint groupEP = this.EndPoint;

                try
                {
                    while (this.IsConnecting)
                    {
                        byte[] array = this.Connection.Receive(ref groupEP);
                        var header = new PacketHeader(array);

                        switch (header.PacketID)
                        {
                            case PacketTypes.CarMotion:
                                Task.Run(() => this.CarMotion(array, header));
                                break;
                            case PacketTypes.Session:
                                Task.Run(() => this.Session(array, header));
                                break;
                            case PacketTypes.LapData:
                                Task.Run(() => this.Lapdata(array, header));
                                break;
                            case PacketTypes.Event:
                                Task.Run(() => this.Event(array, header));
                                break;
                            case PacketTypes.Participants:
                                Task.Run(() => this.Participants(array, header));
                                break;
                            case PacketTypes.CarSetups:
                                Task.Run(() => this.CarSetups(array, header));
                                break;
                            case PacketTypes.CarTelemetry:
                                Task.Run(() => this.CarTelemetry(array, header));
                                break;
                            case PacketTypes.CarStatus:
                                Task.Run(() => this.CarStatus(array, header));
                                break;
                            case PacketTypes.FinalClassification:
                                Task.Run(() => this.FinalClassification(array, header));
                                break;
                            case PacketTypes.LobbyInfo:
                                Task.Run(() => this.LobbyInfo(array, header));
                                break;
                            case PacketTypes.CarDamage:
                                Task.Run(() => this.CarDamage(array, header));
                                break;
                            case PacketTypes.SessionHistory:
                                Task.Run(() => this.SessionHistory(array, header));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.OnConnectionError(ex);
                }
                finally
                {
                    this.Connection.Close();
                    this.IsConnecting = false;

                    this.OnClosedConnection(this);

                    if (this.ClosedConnection != null)
                    {
                        var e = new EventArgs();
                        this.ClosedConnection.Invoke(this, e);
                    }
                }
            });

            //this._Thread.Start();
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
            this.ConnectionError?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnDataReadError(Exception sender)
        {
            Debug.WriteLine(sender.Message);
            Debug.WriteLine(sender.StackTrace);
            this.DataReadError?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnClosedConnection(object sender)
        {
            Debug.WriteLine("F1 UDP Connection is closing.");
            this.ClosedConnection?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnCarMotionPacket(PacketMotionData sender)
        {
            this.LastMotionPacket = sender;
            this.CarMotionPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnSessionPacket(PacketSessionData sender)
        {
            this.LastSessionDataPacket = sender;
            this.SessionPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnLapDataPacket(PacketLapData sender)
        {
            this.LastLapDataPacket = sender;
            this.LapDataPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnParticipantsPacket(PacketParticipantsData sender)
        {
            this.LastParticipantsPacket = sender;
            this.ParticipantsPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnCarSetupsPacket(PacketCarSetupData sender)
        {
            this.LastCarSetupPacket = sender;
            this.CarSetupsPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnCarTelemetryPacket(PacketCarTelemetryData sender)
        {
            this.LastCartelmetryPacket = sender;
            this.CarTelemetryPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnCarStatusPacket(PacketCarStatusData sender)
        {
            this.LastCarStatusDataPacket = sender;
            this.CarStatusPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnFinalClassificationPacket(PacketFinalClassificationData sender)
        {
            this.LastFinalClassificationPacket = sender;
            this.FinalClassificationPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnLobbyInfoPacket(PacketLobbyInfoData sender)
        {
            this.LastLobbyInfoPacket = sender;
            this.LobbyInfoPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnSessionHistoryPacket(PacketSessionHistoryData sender)
        {
            this.LastSessionHistoryPacket = sender;
            this.SessionHistoryPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnCarDemagePacket(PacketCarDamageData sender)
        {
            this.LastCarDemagePacket = sender;
            this.DemagePacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacket(PacketEventData sender)
        {
            this.EventPacket?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketFastestLap(FastestLap sender)
        {
            this.EventPacketFastestLap?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketRetirement(Retirement sender)
        {
            this.EventPacketRetirement?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketTeamMateInPits(TeamMateInPits sender)
        {
            this.EventPacketTeamMateInPits?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketRaceWinner(RaceWinner sender)
        {
            this.EventPacketRaceWinner?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketPenalty(Penalty sender)
        {
            this.EventPacketPenalty?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketSpeedTrap(SpeedTrap sender)
        {
            this.EventPacketSpeedTrap?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketStartLights(StartLights sender)
        {
            this.EventPacketStartLights?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventDriveThroughPenaltyServed(DriveThroughPenaltyServed sender)
        {
            this.EventPacketDriveThroughPenaltyServed?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventStopGoPenaltyServed(StopGoPenaltyServed sender)
        {
            this.EventPacketStopGoPenaltyServed?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketFlashback(Flashback sender)
        {
            this.EventPacketFlashback?.Invoke(sender, new EventArgs());
        }

        protected virtual void OnEventPacketButtons(Buttons sender)
        {
            this.EventPacketButtons?.Invoke(sender, new EventArgs());
        }
    }
}
