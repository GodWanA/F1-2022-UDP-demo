using F1Telemetry.Helpers;

namespace F1Telemetry.Models.LobbyInfoPacket
{
    public class PacketLobbyInfoData : ProtoModel
    {
        public PacketLobbyInfoData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public byte NumberOfPlayers { get; private set; }
        public LobbyInfoData[] LobbyPlayers { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //PacketHeader m_header;                       // Header
            //// Packet specific data
            //uint8 m_numPlayers;               // Number of players in the lobby data
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfPlayers = valb;
            //LobbyInfoData m_lobbyPlayers[22];
            this.LobbyPlayers = new LobbyInfoData[22];
            for (int i = 0; i < this.LobbyPlayers.Length; i++)
            {
                this.LobbyPlayers[i] = new LobbyInfoData(index, this.Header.PacketFormat, array);
                index = this.LobbyPlayers[i].Index;
            }

            this.Index = index;
        }
    }
}
