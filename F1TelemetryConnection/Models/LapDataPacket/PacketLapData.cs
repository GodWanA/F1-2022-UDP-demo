
using F1Telemetry.Helpers;

namespace F1Telemetry.Models.LapDataPacket
{
    public class PacketLapData : ProtoModel
    {
        /// <summary>
        /// Creates a PacketLapData object from raw byte array.
        /// </summary>
        /// <param name="header">Header of the packet</param>
        /// <param name="array">Raw byte array</param>
        public PacketLapData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Common byte reader method.
        /// </summary>
        /// <param name="array">Raw bytearray</param>
        /// <param name="arraySize">Participants list length</param>
        private void ReaderCommon(byte[] array, int arraySize)
        {
            this.Lapdata = new LapData[arraySize];
            for (int i = 0; i < this.Lapdata.Length; i++)
            {
                this.Lapdata[i] = new LapData(this.Header.PacketFormat, this.Index, array);
                this.Index = this.Lapdata[i].Index;
            }
        }

        protected override void Reader2018(byte[] array)
        {
            this.ReaderCommon(array, 20);
        }

        protected override void Reader2019(byte[] array)
        {
            this.ReaderCommon(array, 20);
        }

        protected override void Reader2020(byte[] array)
        {
            this.ReaderCommon(array, 22);
        }

        protected override void Reader2021(byte[] array)
        {
            this.ReaderCommon(array, 22);
        }

        protected override void Reader2022(byte[] array)
        {
            this.ReaderCommon(array, 22);

            byte uin8;

            //uint8 m_timeTrialPBCarIdx;  // Index of Personal Best car in time trial (255 if invalid)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uin8);

            if (uin8 != 255) this.TT_PersonalBest = this.Lapdata[uin8];
            else this.TT_PersonalBest = null;

            //uint8 m_timeTrialRivalCarIdx; 	// Index of Rival car in time trial (255 if invalid)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uin8);

            if (uin8 != 255) this.TT_Rival = this.Lapdata[uin8];
            else this.TT_Rival = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.Lapdata.Length; i++) this.Lapdata[i].Dispose();
                this.Lapdata = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// All cars' current lapdata.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public LapData[] Lapdata { get; private set; }
        /// <summary>
        /// Personal Best car in time trial (null if invalid)<br/>
        ///     - 2022<br/>
        /// </summary>
        public LapData TT_PersonalBest { get; private set; }
        /// <summary>
        /// Rival car in time trial (null if invalid)<br/>
        ///     - 2022<br/>
        /// </summary>
        public LapData TT_Rival { get; private set; }
    }
}
