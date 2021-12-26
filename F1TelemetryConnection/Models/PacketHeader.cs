using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models
{
    public class PacketHeader : ProtoModel
    {
        public PacketHeader(byte[] array)
        {
            ushort ui16;

            //    uint16    m_packetFormat;            // 2021
            this.Index += ByteReader.ToUInt16(array, 0, out ui16);
            this.PacketFormat = ui16;

            this.PickReader(this.PacketFormat, array);
        }

        /// <summary>
        /// Reads byte array in 2021 game format.
        /// </summary>
        /// <param name="array">source byte array</param>
        protected override void Reader2021(byte[] array)
        {
            byte ui8;
            uint ui32;
            ulong ui64;
            float f;

            //    uint8     m_gameMajorVersion;        // Game major version - "X.00"
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            //    uint8     m_gameMinorVersion;        // Game minor version - "1.XX"
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            //    uint8     m_packetVersion;           // Version of this packet type, all start from 1
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            //    uint8     m_packetId;                // Identifier for the packet type, see below
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketID = (PacketTypes)ui8;
            //    uint64    m_sessionUID;              // Unique identifier for the session
            this.Index += ByteReader.ToUInt64(array, this.Index, out ui64);
            this.SessionID = ui64;
            //    float     m_sessionTime;             // Session timestamp
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SessionTime = TimeSpan.FromSeconds(f);
            //    uint32    m_frameIdentifier;         // Identifier for the frame the data was retrieved on
            this.Index += ByteReader.ToUInt32(array, this.Index, out ui32);
            this.FrameIdentifier = ui32;
            //    uint8     m_playerCarIndex;          // Index of player's car in the array
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player1CarIndex = ui8;
            //    uint8     m_secondaryPlayerCarIndex; // Index of secondary player's car in the array (splitscreen)
            //                                         // 255 if no second player
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player2CarIndex = ui8;
        }

        /// <summary>
        /// Format of the packet.
        /// </summary>
        public ushort PacketFormat { get; private set; }
        /// <summary>
        /// Identifier for the packet type
        /// </summary>
        public PacketTypes PacketID { get; private set; }
        /// <summary>
        /// Unique identifier for the session
        /// </summary>
        public ulong SessionID { get; private set; }
        /// <summary>
        /// Session timestamp
        /// </summary>
        public TimeSpan SessionTime { get; private set; }
        /// <summary>
        /// Identifier for the frame the data was retrieved on
        /// </summary>
        public uint FrameIdentifier { get; private set; }
        /// <summary>
        /// Index of the player 1 in the data array, 255 if there is no player 1
        /// </summary>
        public byte Player1CarIndex { get; private set; }
        /// <summary>
        /// Index of the player 2 in the data array, 255 if there is no player 2
        /// </summary>
        public byte Player2CarIndex { get; private set; }
    }
}
