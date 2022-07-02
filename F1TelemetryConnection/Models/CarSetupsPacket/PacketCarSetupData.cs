
namespace F1Telemetry.Models.CarSetupsPacket
{
    public class PacketCarSetupData : ProtoModel
    {
        /// <summary>
        /// Creates PacketCarSetupData object.
        /// </summary>
        /// <param name="header">Haader of the packet.</param>
        /// <param name="array">Raw byte array.</param>
        public PacketCarSetupData(PacketHeader header, byte[] array)
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
        /// Array of the car setups in session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public CarSetupData[] CarSetups { get; private set; }

        /// <summary>
        /// Reads data from byte array.
        /// </summary>
        /// <param name="array">Raw byte array.</param>
        /// <param name="arraySize">Size of participants array.</param>
        private void ReaderCommon(byte[] array, int arraySize)
        {
            this.CarSetups = new CarSetupData[arraySize];
            for (int i = 0; i < this.CarSetups.Length; i++)
            {
                this.CarSetups[i] = new CarSetupData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarSetups[i].Index;
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
