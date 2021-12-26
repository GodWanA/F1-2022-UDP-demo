using F1Telemetry.Helpers;
using System;

namespace F1Telemetry.Models.EventPacket
{
    public class FastestLap : PacketEventData
    {
        public FastestLap(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public byte VehicleIndex { get; private set; }
        public TimeSpan LapTime { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            float valf;

            //uint8 vehicleIdx; // Vehicle index of car achieving fastest lap
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleIndex = valb;
            //float lapTime;    // Lap time is in seconds
            index += ByteReader.ToFloat(array, index, out valf);
            this.LapTime = TimeSpan.FromSeconds(valf);

            this.Index = index;
        }
    }
}
