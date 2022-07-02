using F1Telemetry.Helpers;
using System;

namespace F1Telemetry.Models.EventPacket
{
    public class FastestLap : PacketEventData
    {
        /// <summary>
        /// Creates a FastestLap object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public FastestLap(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Car index in other pacekts.<br/>
        /// Supported:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }
        /// <summary>
        /// Fastest lap time.<br/>
        /// Supported:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan LapTime { get; private set; }

        protected override void Reader2019(byte[] array)
        {
            byte valb;
            float valf;

            //uint8 vehicleIdx; // Vehicle index of car achieving fastest lap
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.VehicleIndex = valb;
            //float lapTime;    // Lap time is in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.LapTime = TimeSpan.FromSeconds(valf);
        }

        protected override void Reader2020(byte[] array)
        {
            this.Reader2019(array);
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2019(array);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2019(array);
        }
    }
}
