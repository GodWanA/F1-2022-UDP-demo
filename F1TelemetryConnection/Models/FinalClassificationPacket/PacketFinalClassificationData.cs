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

        protected override void Reader2020(byte[] array)
        {
            byte uint8;

            //PacketHeader m_header;                      // Header
            //uint8 m_numCars;          // Number of cars in the final classification
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfCars = uint8;
            //FinalClassificationData m_classificationData[22];
            this.ClassificationData = new FinalClassificationData[22];
            for (int i = 0; i < this.ClassificationData.Length; i++)
            {
                this.ClassificationData[i] = new FinalClassificationData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.ClassificationData[i].Index;
            }
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.ClassificationData.Length; i++) this.ClassificationData[i].Dispose();
                this.ClassificationData = null;
            }

            base.Dispose(disposing);
        }
    }
}
