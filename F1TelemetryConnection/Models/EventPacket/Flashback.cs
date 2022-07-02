using F1Telemetry.Helpers;
using System;

namespace F1Telemetry.Models.EventPacket
{
    public class Flashback : PacketEventData
    {
        /// <summary>
        /// Creates a Flashback object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public Flashback(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Frame identifier flashed back to.<br/>
        /// Supported:<br/>
        ///     - 2021<br/>
        /// </summary>
        public uint FlashbackFraneIdentifier { get; private set; }
        /// <summary>
        /// Session time flashed back to.<br/>
        /// Supported:<br/>
        ///     - 2021<br/>
        /// </summary>
        public TimeSpan FlashbackSessionTime { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            uint valui;
            float valf;

            //uint32 flashbackFrameIdentifier;  // Frame identifier flashed back to
            index += ByteReader.ToUInt32(array, index, out valui);
            this.FlashbackFraneIdentifier = valui;

            //float flashbackSessionTime;       // Session time flashed back to
            index += ByteReader.ToFloat(array, index, out valf);
            this.FlashbackSessionTime = TimeSpan.FromSeconds(valf);

            this.Index = index;
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
        }
    }
}
