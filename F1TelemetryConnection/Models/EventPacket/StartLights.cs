using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class StartLights : PacketEventData
    {
        public StartLights(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public byte NumberOfLight { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //uint8 vehicleIdx;                 // Vehicle index of the vehicle serving drive through
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfLight = valb;

            this.Index = index;
        }
    }
}
