using F1Telemetry.Helpers;
using System;
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

        /// <summary>
        /// Whether the vehicle is AI (1) or Human (0) controlled
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsAIControlled { get; private set; }
        /// <summary>
        /// Team id - see appendix
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Teams TeamID { get; private set; }
        /// <summary>
        /// Nationality of the driver
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Nationalities Natinonality { get; private set; }
        /// <summary>
        /// Name of participant in UTF-8 format – null terminated<br/>
        /// Will be truncated with ... (U+2026) if too long
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Car number of the player
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte RaceNumber { get; private set; }
        /// <summary>
        /// Ready status
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public ReadyStatuses ReadyStatus { get; private set; }

        protected override void Reader2020(byte[] array)
        {
            byte uint8;

            this.ReaderCommon(array);
            //uint8 m_readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ReadyStatus = (ReadyStatuses)uint8;
        }

        protected override void Reader2021(byte[] array)
        {
            byte uint8;

            this.ReaderCommon(array);
            //uint8 m_carNumber;               // Car number of the player
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RaceNumber = uint8;
            //uint8 m_readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ReadyStatus = (ReadyStatuses)uint8;
        }

        private void ReaderCommon(byte[] array)
        {
            byte uint8;
            bool b;
            string s;

            //uint8 m_aiControlled;            // Whether the vehicle is AI (1) or Human (0) controlled
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAIControlled = b;
            //uint8 m_teamId;                  // Team id - see appendix (255 if no team currently selected)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TeamID = (Teams)uint8;
            //uint8 m_nationality;             // Nationality of the driver
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Natinonality = (Nationalities)uint8;
            //char m_name[48];        // Name of participant in UTF-8 format – null terminated
            //                        // Will be truncated with ... (U+2026) if too long
            this.Index += ByteReader.toStringFromUint8(array, this.Index, 48, out s);
            this.Name = s;
        }

        internal void EmmulateRaceNumber(byte raceNumber)
        {
            this.RaceNumber = raceNumber;
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
