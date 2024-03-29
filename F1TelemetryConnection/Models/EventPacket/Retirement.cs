﻿using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class Retirement : PacketEventData
    {
        /// <summary>
        /// Creates a Retirement object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public Retirement(PacketEventData e, byte[] array)
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

        protected override void Reader2019(byte[] array)
        {
            byte valb;

            //uint8 vehicleIdx; // Vehicle index of car retiring
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.VehicleIndex = valb;
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
