using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class MarshalZone : ProtoModel
    {
        public float ZoneStart { get; private set; }
        public Flags ZoneFlag { get; private set; }
        public float ZoneStartMeter { get; private set; }

        private int tl;

        public MarshalZone(int index, int format, byte[] array, int tracklength)
        {
            this.Index = index;
            this.tl = tracklength;
            this.PickReader(format, array);
        }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            float valf;
            sbyte valsb;

            //float m_zoneStart;   // Fraction (0..1) of way through the lap the marshal zone starts
            index += ByteReader.ToFloat(array, index, out valf);
            this.ZoneStart = valf;

            //int8 m_zoneFlag;    // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red
            index += ByteReader.ToInt8(array, index, out valsb);
            this.ZoneFlag = (Flags)valsb;

            this.ZoneStartMeter = this.tl / this.ZoneStart;

            this.Index = index;
        }
    }
}
