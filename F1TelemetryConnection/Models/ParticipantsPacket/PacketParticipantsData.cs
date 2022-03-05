using F1Telemetry.Helpers;

namespace F1Telemetry.Models.ParticipantsPacket
{
    public class PacketParticipantsData : ProtoModel
    {
        /// <summary>
        /// Creates PacketParticipantsData object.
        /// </summary>
        /// <param name="header">Haader of the packet.</param>
        /// <param name="array">Raw byte array.</param>
        public PacketParticipantsData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; }
        /// <summary>
        /// Number of active cars in the data.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte NumberOfActiveCars { get; private set; }
        /// <summary>
        /// Array of the participants in session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ParticipantData[] Participants { get; private set; }

        /// <summary>
        /// Reads data from byte array.
        /// </summary>
        /// <param name="array">Raw byte array.</param>
        /// <param name="arraySize">Size of participants array.</param>
        private void ReaderCommon(byte[] array, int arraySize)
        {
            byte valb;

            //uint8 m_numActiveCars;  // Number of active cars in the data – should match number of
            //                        // cars on HUD
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.NumberOfActiveCars = valb;
            //ParticipantData m_participants[22];
            this.Participants = new ParticipantData[arraySize];

            for (int i = 0; i < this.Participants.Length; i++)
            {
                this.Participants[i] = new ParticipantData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.Participants[i].Index;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.Participants.Length; i++) this.Participants[i].Dispose();
                this.Participants = null;
            }

            base.Dispose(disposing);
        }
    }
}
