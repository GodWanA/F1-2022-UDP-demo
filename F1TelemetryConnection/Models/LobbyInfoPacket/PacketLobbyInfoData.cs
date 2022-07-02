using F1Telemetry.Helpers;
using F1Telemetry.Models.ParticipantsPacket;
using System;

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

        /// <summary>
        /// Packet header
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// Number of players
        /// </summary>
        public byte NumberOfPlayers { get; private set; }
        /// <summary>
        /// All players in lobby
        /// </summary>
        public LobbyInfoData[] LobbyPlayers { get; private set; }

        protected override void Reader2020(byte[] array)
        {
            byte uint8;

            //PacketHeader m_header;                       // Header
            //// Packet specific data
            //uint8 m_numPlayers;               // Number of players in the lobby data
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfPlayers = uint8;
            //LobbyInfoData m_lobbyPlayers[22];
            this.LobbyPlayers = new LobbyInfoData[22];
            for (int i = 0; i < this.LobbyPlayers.Length; i++)
            {
                this.LobbyPlayers[i] = new LobbyInfoData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.LobbyPlayers[i].Index;
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

                for (int i = 0; i < this.LobbyPlayers.Length; i++) this.LobbyPlayers[i].Dispose();
                this.LobbyPlayers = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Fills up racenumbers from participants data
        /// </summary>
        /// <param name="participants">source participants</param>
        internal void BuildRaceNumbers(ParticipantData[] participants)
        {
            if (this.LobbyPlayers != null && participants != null)
            {
                for (int i = 0; i < this.LobbyPlayers.Length; i++)
                {
                    this.LobbyPlayers[i].EmmulateRaceNumber(participants[i].RaceNumber);
                }
            }
        }
    }
}
