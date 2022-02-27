using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models
{
    /// <summary>
    /// Header of the packets.
    /// </summary>
    public class PacketHeader : ProtoModel
    {
        /// <summary>
        /// Creates PacketHeader object from raw byte array.
        /// </summary>
        /// <param name="array">raw byte array</param>
        public PacketHeader(byte[] array)
        {
            ushort ui16;

            //    uint16    m_packetFormat;            // 2021
            this.Index += ByteReader.ToUInt16(array, 0, out ui16);
            this.PacketFormat = ui16;

            this.PickReader(this.PacketFormat, array);
        }

        /// <summary>
        /// Reads byte array in 2018 game format.
        /// </summary>
        /// <param name="array">source byte array</param>
        protected override void Reader2018(byte[] array)
        {
            byte ui8;
            uint ui32;
            ulong ui64;
            float f;

            //uint8 m_packetVersion;        // Version of this packet type, all start from 1
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketVersion = ui8;
            //uint8 m_packetId;             // Identifier for the packet type, see below
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketID = (PacketTypes)ui8;
            //uint64 m_sessionUID;           // Unique identifier for the session
            this.Index += ByteReader.ToUInt64(array, this.Index, out ui64);
            this.SessionID = ui64;
            //float m_sessionTime;          // Session timestamp
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SessionTime = TimeSpan.FromMilliseconds(f);
            //uint m_frameIdentifier;      // Identifier for the frame the data was retrieved on
            this.Index += ByteReader.ToUInt32(array, this.Index, out ui32);
            this.FrameIdentifier = ui32;
            //uint8 m_playerCarIndex;       // Index of player's car in the array
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player1CarIndex = ui8;
        }

        /// <summary>
        /// Reads byte array in 2019 game format.
        /// </summary>
        /// <param name="array">source byte array</param>
        protected override void Reader2019(byte[] array)
        {
            byte ui8;
            uint ui32;
            ulong ui64;
            float f;

            this.CreateGameVersion(array);
            //uint8 m_packetVersion;        // Version of this packet type, all start from 1
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketVersion = ui8;
            //uint8 m_packetId;             // Identifier for the packet type, see below
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketID = (PacketTypes)ui8;
            //uint64 m_sessionUID;           // Unique identifier for the session
            this.Index += ByteReader.ToUInt64(array, this.Index, out ui64);
            this.SessionID = ui64;
            //float m_sessionTime;          // Session timestamp
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SessionTime = TimeSpan.FromMilliseconds(f);
            //uint m_frameIdentifier;      // Identifier for the frame the data was retrieved on
            this.Index += ByteReader.ToUInt32(array, this.Index, out ui32);
            this.FrameIdentifier = ui32;
            //uint8 m_playerCarIndex;       // Index of player's car in the array
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player1CarIndex = ui8;
        }

        /// <summary>
        /// Reads byte array in 2020 game format.
        /// </summary>
        /// <param name="array">source byte array</param>
        protected override void Reader2020(byte[] array)
        {
            byte ui8;
            uint ui32;
            ulong ui64;
            float f;

            //uint8 m_gameMajorVersion;         // Game major version - "X.00"
            //uint8 m_gameMinorVersion;         // Game minor version - "1.XX"
            this.CreateGameVersion(array);
            //uint8 m_packetVersion;            // Version of this packet type, all start from 1
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketVersion = ui8;
            //uint8 m_packetId;                 // Identifier for the packet type, see below
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketID = (PacketTypes)ui8;
            //uint64 m_sessionUID;               // Unique identifier for the session
            this.Index += ByteReader.ToUInt64(array, this.Index, out ui64);
            this.SessionID = ui64;
            //float m_sessionTime;              // Session timestamp
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SessionTime = TimeSpan.FromMilliseconds(f);
            //uint32 m_frameIdentifier;          // Identifier for the frame the data was retrieved on
            this.Index += ByteReader.ToUInt32(array, this.Index, out ui32);
            this.FrameIdentifier = ui32;
            //uint8 m_playerCarIndex;           // Index of player's car in the array
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player1CarIndex = ui8;

            //// ADDED IN BETA 2: 
            //uint8 m_secondaryPlayerCarIndex;  // Index of secondary player's car in the array (splitscreen)
            //                                  // 255 if no second player
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.Player2CarIndex = ui8;
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

            this.CreateGameVersion(array);
            //    uint8     m_packetVersion;           // Version of this packet type, all start from 1
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.PacketVersion = ui8;
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
        /// Reads game version data, and fills game version properties.
        /// </summary>
        /// <param name="array"></param>
        private void CreateGameVersion(byte[] array)
        {
            byte ui8;
            //uint8 m_gameMajorVersion;     // Game major version - "X.00"
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.GameMajorVersion = ui8;
            //uint8 m_gameMinorVersion;     // Game minor version - "1.XX"
            this.Index += ByteReader.ToUInt8(array, this.Index, out ui8);
            this.GameMinorVersion = ui8;

            this.GameVersion = this.GameMajorVersion + (float)(this.GameMinorVersion / 100.0f);
        }

        /// <summary>
        /// Format of the packet.<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort PacketFormat { get; private set; }
        /// <summary>
        /// Identifier for the packet type<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public PacketTypes PacketID { get; private set; }
        /// <summary>
        /// Unique identifier for the session<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ulong SessionID { get; private set; }
        /// <summary>
        /// Session timestamp<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public TimeSpan SessionTime { get; private set; }
        /// <summary>
        /// Identifier for the frame the data was retrieved on<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public uint FrameIdentifier { get; private set; }
        /// <summary>
        /// Index of the player 1 in the data array, 255 if there is no player 1<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte Player1CarIndex { get; private set; }
        /// <summary>
        /// Index of the player 2 in the data array, 255 if there is no player 2<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte Player2CarIndex { get; private set; }
        /// <summary>
        /// Game major version - "X.00"<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte GameMajorVersion { get; private set; }
        /// <summary>
        /// Game minor version - "1.XX"<br/>
        /// Supported:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte GameMinorVersion { get; private set; }
        /// <summary>
        /// Full game version - "X.XX"<br/>
        /// Supported:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float GameVersion { get; private set; }
        /// <summary>
        /// Version of this packet type, all start from 1<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte PacketVersion { get; private set; }
    }
}
