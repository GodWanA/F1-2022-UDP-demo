
namespace F1Telemetry.Models.CarSetupsPacket
{
    public class PacketCarSetupData : ProtoModel
    {
        public PacketCarSetupData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public CarSetupData[] CarSetups { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;

            //CarSetupData m_carSetups[22];
            this.CarSetups = new CarSetupData[22];
            for (int i = 0; i < this.CarSetups.Length; i++)
            {
                this.CarSetups[i] = new CarSetupData(index, this.Header.PacketFormat, array);
                index = this.CarSetups[i].Index;
            }

            this.Index = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.CarSetups.Length; i++) this.CarSetups[i].Dispose();
                this.CarSetups = null;
            }

            base.Dispose(disposing);
        }
    }
}
