
namespace F1Telemetry.Models.CarDamagePacket
{
    public class PacketCarDamageData : ProtoModel
    {
        public PacketCarDamageData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public CarDamageData[] CarDamageData { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;

            //PacketHeader m_header;               // Header
            //CarDamageData m_carDamageData[22];
            this.CarDamageData = new CarDamageData[22];
            for (int i = 0; i < this.CarDamageData.Length; i++)
            {
                this.CarDamageData[i] = new CarDamageData(index, this.Header.PacketFormat, array);
                index = this.CarDamageData[i].Index;
            }

            this.Index = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.CarDamageData.Length; i++) this.CarDamageData[i].Dispose();
                this.CarDamageData = null;
            }

            base.Dispose(disposing);
        }
    }
}
