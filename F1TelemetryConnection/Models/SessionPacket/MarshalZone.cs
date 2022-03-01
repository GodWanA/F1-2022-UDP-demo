using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class MarshalZone : ProtoModel
    {
        private int tl;

        /// <summary>
        /// Creates a MarshalZone object.
        /// </summary>
        /// <param name="index">Start index of object.</param>
        /// <param name="format">Packet format</param>
        /// <param name="array">raw byte array</param>
        /// <param name="tracklength">track length</param>
        public MarshalZone(int index, int format, byte[] array, int tracklength)
        {
            this.Index = index;
            this.tl = tracklength;
            this.PickReader(format, array);
        }

        protected override void Reader2018(byte[] array)
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

            this.ZoneStartMeter = this.tl * this.ZoneStart;

            this.Index = index;
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

        /// <summary>
        /// Fraction (0..1) of way through the lap the marshal zone starts.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float ZoneStart { get; private set; }
        /// <summary>
        /// Current flag in marhal zone.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Flags ZoneFlag { get; private set; }
        /// <summary>
        /// Fraction (in meters) of way through the lap the marshal zone starts.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float ZoneStartMeter { get; private set; }


    }
}
