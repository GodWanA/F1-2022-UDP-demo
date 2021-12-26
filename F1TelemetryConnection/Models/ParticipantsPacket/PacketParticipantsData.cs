using F1Telemetry.Helpers;

namespace F1Telemetry.Models.ParticipantsPacket
{
    public class PacketParticipantsData : ProtoModel
    {
        public PacketParticipantsData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; }
        public byte NumberOfActiveCars { get; private set; }
        public ParticipantData[] Participants { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //uint8 m_numActiveCars;  // Number of active cars in the data – should match number of
            //                        // cars on HUD
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfActiveCars = valb;
            //ParticipantData m_participants[22];
            this.Participants = new ParticipantData[22];

            for (int i = 0; i < this.Participants.Length; i++)
            {
                this.Participants[i] = new ParticipantData(index, this.Header.PacketFormat, array);
                index = this.Participants[i].Index;
            }

            this.Index = index;
        }
    }
}
