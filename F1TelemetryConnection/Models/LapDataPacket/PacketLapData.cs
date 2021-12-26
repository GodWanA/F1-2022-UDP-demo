
namespace F1Telemetry.Models.LapDataPacket
{
    public class PacketLapData : ProtoModel
    {
        public PacketHeader Header { get; private set; }
        public LapData[] Lapdata { get; private set; }

        public PacketLapData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;

            this.Lapdata = new LapData[22];
            for (int i = 0; i < this.Lapdata.Length; i++)
            {
                this.Lapdata[i] = new LapData(this.Header.PacketFormat, index, array);
                index = this.Lapdata[i].Index;
            }

            this.Index = index;
        }
    }
}
