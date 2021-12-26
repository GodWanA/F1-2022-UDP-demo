using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class StopGoPenaltyServed : PacketEventData
    {
        public StopGoPenaltyServed(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public byte VehicleIndex { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            float valf;

            //uint8 vehicleIdx;                 // Vehicle index of the vehicle serving stop go
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleIndex = valb;

            this.Index = index;
        }
    }
}
