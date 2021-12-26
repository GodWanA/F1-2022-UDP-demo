using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class SpeedTrap : PacketEventData
    {
        public SpeedTrap(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public byte VehicleIndex { get; private set; }
        public float Speed { get; private set; }
        public bool OverallFastestInSession { get; private set; }
        public bool DriverFastestInSession { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            float valf;
            bool valbo;

            //uint8 vehicleIdx;       // Vehicle index of the vehicle triggering speed trap
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleIndex = valb;
            //float speed;            // Top speed achieved in kilometres per hour
            index += ByteReader.ToFloat(array, index, out valf);
            this.Speed = valf;

            //uint8 overallFastestInSession;   // Overall fastest speed in session = 1, otherwise 0
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.OverallFastestInSession = valbo;

            //uint8 driverFastestInSession;    // Fastest speed for driver in session = 1, otherwise 0
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.DriverFastestInSession = valbo;

            this.Index = index;
        }
    }
}
