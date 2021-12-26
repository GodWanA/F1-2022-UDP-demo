using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionHistoryPacket
{
    public class TyreStintHistoryData : ProtoModel
    {
        public TyreStintHistoryData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public byte EndLap { get; private set; }
        public bool IsCurrentTyre { get; private set; }
        public TyreCompounds TyreActualCompound { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //uint8 m_endLap;                // Lap the tyre usage ends on (255 of current tyre)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EndLap = valb;

            if (this.EndLap == 255) this.IsCurrentTyre = true;
            else this.IsCurrentTyre = false;

            //uint8 m_tyreActualCompound;    // Actual tyres used by this driver
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TyreActualCompound = (TyreCompounds)valb;

            //uint8 m_tyreVisualCompound;    // Visual tyres used by this driver
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TyreActualCompound = (TyreCompounds)valb;

            this.Index = index;
        }
    }
}
