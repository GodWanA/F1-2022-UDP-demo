using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class PacketEventData : ProtoModel
    {
        public PacketEventData() { }
        public PacketEventData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; protected set; }
        public string EventCode { get; protected set; }
        public EventTypes EventType { get; protected set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            string vals;

            index += ByteReader.toStringFromUint8(array, index, 4, out vals);
            this.EventCode = vals;

            switch (this.EventCode.ToLower())
            {
                case "ssta":
                    this.EventType = EventTypes.SessionStarted;
                    break;
                case "send":
                    this.EventType = EventTypes.SessionEnded;
                    break;
                case "ftlp":
                    this.EventType = EventTypes.FastestLap;
                    break;
                case "rtmt":
                    this.EventType = EventTypes.Retirement;
                    break;
                case "drse":
                    this.EventType = EventTypes.DRSenabled;
                    break;
                case "drsd":
                    this.EventType = EventTypes.DRSdisabled;
                    break;
                case "tmpt":
                    this.EventType = EventTypes.TeamMateInPits;
                    break;
                case "chqf":
                    this.EventType = EventTypes.ChequeredFlag;
                    break;
                case "rcwn":
                    this.EventType = EventTypes.RaceWinner;
                    break;
                case "pena":
                    this.EventType = EventTypes.PenaltyIssued;
                    break;
                case "sptp":
                    this.EventType = EventTypes.SpeedTrapTriggered;
                    break;
                case "stlg":
                    this.EventType = EventTypes.StartLights;
                    break;
                case "lgot":
                    this.EventType = EventTypes.LightsOut;
                    break;
                case "dtsv":
                    this.EventType = EventTypes.DriveThroughServed;
                    break;
                case "sgsv":
                    this.EventType = EventTypes.StopGoServed;
                    break;
                case "flbk":
                    this.EventType = EventTypes.Flashback;
                    break;
                case "butn":
                    this.EventType = EventTypes.ButtonStatus;
                    break;
            }

            this.Index = index;
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
    }
}
