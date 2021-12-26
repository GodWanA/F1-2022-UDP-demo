
namespace F1Telemetry.Models.CarStatusPacket
{
    public class PacketCarStatusData : ProtoModel
    {
        public PacketCarStatusData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public CarStatusData[] CarStatusData { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;

            //PacketHeader m_header;     // Header
            //CarStatusData m_carStatusData[22];
            this.CarStatusData = new CarStatusData[22];
            for (int i = 0; i < this.CarStatusData.Length; i++)
            {
                this.CarStatusData[i] = new CarStatusData(index, this.Header.PacketFormat, array);
                index = this.CarStatusData[i].Index;
            }

            this.Index = index;
        }
    }
}
