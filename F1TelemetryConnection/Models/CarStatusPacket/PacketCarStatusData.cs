
using F1Telemetry.Models.CarDamagePacket;
using System;

namespace F1Telemetry.Models.CarStatusPacket
{
    public class PacketCarStatusData : ProtoModel
    {
        /// <summary>
        /// Creates a PacketCarStatusData object from raw byte array.
        /// </summary>
        /// <param name="header">Header of the packet</param>
        /// <param name="array">Raw byte array</param>
        public PacketCarStatusData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// All cars' current status datas.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public CarStatusData[] CarStatusData { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            this.CommonReader(array, 20);
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            this.CommonReader(array, 22);
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2020(array);
        }

        /// <summary>
        /// Common parts of the readers.
        /// </summary>
        /// <param name="array">Raw byte array.</param>
        /// <param name="arraySize">Size of array.</param>
        private void CommonReader(byte[] array, int arraySize)
        {
            //PacketHeader m_header;     // Header
            //CarStatusData m_carStatusData[22];
            this.CarStatusData = new CarStatusData[arraySize];
            for (int i = 0; i < this.CarStatusData.Length; i++)
            {
                this.CarStatusData[i] = new CarStatusData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarStatusData[i].Index;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.CarStatusData.Length; i++) this.CarStatusData[i].Dispose();
                this.CarStatusData = null;
            }

            base.Dispose(disposing);
        }

        internal PacketCarDamageData BuildCarDemagePacket()
        {
            var ret = new PacketCarDamageData();
            ret.LoadRawDatas((PacketHeader)this.Header.Clone(), (CarStatusData[])this.CarStatusData.Clone());
            return ret;
        }
    }
}
