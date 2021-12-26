using F1Telemetry.Helpers;
using System;

namespace F1Telemetry.Models.EventPacket
{
    public class Flashback : PacketEventData
    {
        public Flashback(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public uint FlashbackFraneIdentifier { get; private set; }
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
            this.FlashbackSessionTime = TimeSpan.FromMilliseconds(valf);

            this.Index = index;
        }
    }
}
