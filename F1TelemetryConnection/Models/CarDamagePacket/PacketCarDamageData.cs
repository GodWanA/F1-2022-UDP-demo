
using F1Telemetry.Models.CarStatusPacket;

namespace F1Telemetry.Models.CarDamagePacket
{
    public class PacketCarDamageData : ProtoModel
    {
        public PacketCarDamageData() { }

        public PacketCarDamageData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Packet header
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// All cardemage data
        /// </summary>
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

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
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

        /// <summary>
        /// Fills up car demage data pack from carstatus pack (for older games)
        /// </summary>
        /// <param name="head">Packet header</param>
        /// <param name="source">source car status datas</param>
        internal void LoadRawDatas(PacketHeader head, CarStatusData[] source)
        {
            this.Header = head;
            this.CarDamageData = new CarDamageData[source.Length];
            for (int i = 0; i < this.CarDamageData.Length; i++)
            {
                CarDamageData[i] = source[i].BuildCardemageData();
            }
        }
    }
}
