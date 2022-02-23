using F1Telemetry.Helpers;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.LobbyInfoPacket
{
    public class LobbyInfoData : ProtoModel
    {
        public LobbyInfoData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public bool IsAIControlled { get; private set; }
        public Teams TeamID { get; private set; }
        public Nationalities Natinonality { get; private set; }
        public string Name { get; private set; }
        public byte RaceNumber { get; private set; }
        public ReadyStatuses ReadyStatus { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            bool valbo;
            string s;

            //uint8 m_aiControlled;            // Whether the vehicle is AI (1) or Human (0) controlled
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsAIControlled = valbo;

            //uint8 m_teamId;                  // Team id - see appendix (255 if no team currently selected)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TeamID = (Teams)valb;

            //uint8 m_nationality;             // Nationality of the driver
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Natinonality = (Nationalities)valb;

            //char m_name[48];        // Name of participant in UTF-8 format – null terminated
            //                        // Will be truncated with ... (U+2026) if too long
            index += ByteReader.toStringFromUint8(array, index, 48, out s);
            this.Name = s;

            //uint8 m_carNumber;               // Car number of the player
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RaceNumber = valb;

            //uint8 m_readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
            index += ByteReader.ToUInt8(array, index, out valb);
            this.ReadyStatus = (ReadyStatuses)valb;

            this.Index = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Name = null;
            }

            base.Dispose(disposing);
        }
    }
}
