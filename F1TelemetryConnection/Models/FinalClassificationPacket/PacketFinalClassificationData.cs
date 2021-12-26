using F1Telemetry.Helpers;

namespace F1Telemetry.Models.FinalClassificationPacket
{
    public class PacketFinalClassificationData : ProtoModel
    {
        public PacketFinalClassificationData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public byte NumberOfCars { get; private set; }
        public FinalClassificationData[] ClassificationData { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //PacketHeader m_header;                      // Header
            //uint8 m_numCars;          // Number of cars in the final classification
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfCars = valb;

            //FinalClassificationData m_classificationData[22];
            this.ClassificationData = new FinalClassificationData[22];
            for (int i = 0; i < this.ClassificationData.Length; i++)
            {
                this.ClassificationData[i] = new FinalClassificationData(index, this.Header.PacketFormat, array);
                index = this.ClassificationData[i].Index;
            }


            this.Index = index;
        }
    }
}
