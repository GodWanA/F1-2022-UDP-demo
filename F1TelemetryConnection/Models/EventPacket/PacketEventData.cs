using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class PacketEventData : ProtoModel
    {
        /// <summary>
        /// Creates empty PacketEventData from raw byte array.
        /// </summary>
        public PacketEventData() { }

        /// <summary>
        /// Creates a PacketEventData from raw byte array.
        /// </summary>
        /// <param name="header">Header packet of the object.</param>
        /// <param name="array">Raw byte array</param>
        public PacketEventData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        protected override void Reader2018(byte[] array)
        {
            string vals;

            this.Index += ByteReader.toStringFromUint8(array, this.Index, 4, out vals);
            this.EventCode = vals;

            switch (this.EventCode.ToLower())
            {
                case "ssta":
                    this.EventType = EventTypes.SessionStarted;
                    this.EventName = "Session Started";
                    break;
                case "send":
                    this.EventType = EventTypes.SessionEnded;
                    this.EventName = "Session Ended";
                    break;
                case "ftlp":
                    this.EventType = EventTypes.FastestLap;
                    this.EventName = "Fastest Lap";
                    break;
                case "rtmt":
                    this.EventType = EventTypes.Retirement;
                    this.EventName = "Retirement";
                    break;
                case "drse":
                    this.EventType = EventTypes.DRSenabled;
                    this.EventName = "DRS Enabled";
                    break;
                case "drsd":
                    this.EventType = EventTypes.DRSdisabled;
                    this.EventName = "DRS Disabled";
                    break;
                case "tmpt":
                    this.EventType = EventTypes.TeamMateInPits;
                    this.EventName = "Team Mate In Pits";
                    break;
                case "chqf":
                    this.EventType = EventTypes.ChequeredFlag;
                    this.EventName = "Chequered Flag";
                    break;
                case "rcwn":
                    this.EventType = EventTypes.RaceWinner;
                    this.EventName = "Race Winner";
                    break;
                case "pena":
                    this.EventType = EventTypes.PenaltyIssued;
                    this.EventName = "Penalty Issued";
                    break;
                case "sptp":
                    this.EventType = EventTypes.SpeedTrapTriggered;
                    this.EventName = "Speed Trap Triggered";
                    break;
                case "stlg":
                    this.EventType = EventTypes.StartLights;
                    this.EventName = "Start Light";
                    break;
                case "lgot":
                    this.EventType = EventTypes.LightsOut;
                    this.EventName = "Lights Out";
                    break;
                case "dtsv":
                    this.EventType = EventTypes.DriveThroughServed;
                    this.EventName = "Drive Trhough Served";
                    break;
                case "sgsv":
                    this.EventType = EventTypes.StopGoServed;
                    this.EventName = "Stop Go Served";
                    break;
                case "flbk":
                    this.EventType = EventTypes.Flashback;
                    this.EventName = "Flashback";
                    break;
                case "butn":
                    this.EventType = EventTypes.ButtonStatus;
                    this.EventName = "ButtonStatus";
                    break;
            }
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();
                this.EventCode = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; protected set; }
        /// <summary>
        /// Event code as text.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public string EventCode { get; protected set; }
        /// Event code as enum.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public EventTypes EventType { get; protected set; }
        /// Event's full name as string.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public string EventName { get; protected set; }
    }
}
