using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;

namespace F1Telemetry.Models.SessionHistoryPacket
{
    public class LapHistoryData : ProtoModel
    {
        public LapHistoryData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public TimeSpan LapTime { get; private set; }
        public TimeSpan Sector1Time { get; private set; }
        public TimeSpan Sector2Time { get; private set; }
        public TimeSpan Sector3Time { get; private set; }
        public List<Appendences.LapValid> LapValids { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            uint valui;
            ushort valus;
            byte valb;

            //uint32 m_lapTimeInMS;           // Lap time in milliseconds
            index += ByteReader.ToUInt32(array, index, out valui);
            this.LapTime = TimeSpan.FromMilliseconds(valui);

            //uint16 m_sector1TimeInMS;       // Sector 1 time in milliseconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Sector1Time = TimeSpan.FromMilliseconds(valus);

            //uint16 m_sector2TimeInMS;       // Sector 2 time in milliseconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Sector2Time = TimeSpan.FromMilliseconds(valus);

            //uint16 m_sector3TimeInMS;       // Sector 3 time in milliseconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Sector3Time = TimeSpan.FromMilliseconds(valus);

            //uint8 m_lapValidBitFlags;      // 0x01 bit set-lap valid,      0x02 bit set-sector 1 valid
            //                               // 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
            index += ByteReader.ToUInt8(array, index, out valb);
            this.LapValids = Appendences.LapValidChecker(valb);


            this.Index = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.LapValids.Clear();
                this.LapValids = null;
            }

            base.Dispose(disposing);
        }
    }
}
