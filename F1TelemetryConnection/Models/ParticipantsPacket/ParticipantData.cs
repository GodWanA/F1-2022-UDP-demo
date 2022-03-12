using F1Telemetry.Helpers;
using System;
using System.Text.RegularExpressions;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.ParticipantsPacket
{
    public class ParticipantData : ProtoModel
    {
        /// <summary>
        /// Creates ParticipantData object.
        /// </summary>
        /// <param name="index">Start index of packet.</param>
        /// <param name="format">Format of the packet.</param>
        /// <param name="array">Raw byte array.</param>
        public ParticipantData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        /// <summary>
        /// Whether the vehicle is AI or Human controlled.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsAI { get; private set; }
        /// <summary>
        /// Driver as enum.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Drivers DriverID { get; private set; }
        /// <summary>
        /// Network id – unique identifier for network players.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte NetworkID { get; private set; }
        /// <summary>
        /// Team as enum.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Teams TeamID { get; private set; }
        /// <summary>
        /// My team flag.<br/>
        /// Supports:<br/>
        ///     - 2018 (emulated)<br/>
        ///     - 2019 (emulated)<br/>
        ///     - 2020 (emulated)<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsMyTeam { get; private set; }
        /// <summary>
        /// Race number of the car.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte RaceNumber { get; private set; }
        /// <summary>
        /// Nationality of the driver.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Nationalities Nationality { get; private set; }
        /// <summary>
        /// Name of participant in UTF-8 format – null terminated<br/>
        /// Will be truncated with … (U+2026) if too long<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The player's UDP setting, (restricted|public).<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public TelemetrySettings YouTelemetry { get; private set; }
        /// <summary>
        /// Short name of participant in UTF-8 format – null terminated<br/>
        /// Will be truncated with … (U+2026) if too long<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public string ShortName { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            bool b;
            byte uint8;
            string s;

            //uint8 m_aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAI = b;
            //uint8 m_driverId;               // Driver id - see appendix
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DriverID = (Drivers)uint8;
            //uint8 m_teamId;                 // Team id - see appendix
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetTeam(uint8);

            if (this.TeamID == Teams.Unknown) this.IsMyTeam = true;
            else this.IsMyTeam = false;

            //uint8 m_raceNumber;             // Race number of the car
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RaceNumber = uint8;
            //uint8 m_nationality;            // Nationality of the driver
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetNationality(uint8);
            //char m_name[48];               // Name of participant in UTF-8 format – null terminated
            //                               // Will be truncated with … (U+2026) if too long
            this.Index += ByteReader.toStringFromUint8(array, this.Index, 48, out s);
            this.Name = s;
            this.CreateShortName(this.DriverID);
        }

        protected override void Reader2019(byte[] array)
        {
            byte uint8;

            this.Reader2018(array);
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.YouTelemetry = (TelemetrySettings)uint8;
        }

        protected override void Reader2020(byte[] array)
        {
            this.Reader2019(array);
        }

        protected override void Reader2021(byte[] array)
        {
            byte uint8;
            bool b;
            string s;

            //uint8 m_aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAI = b;
            //uint8 m_driverId;       // Driver id - see appendix, 255 if network human
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetDriver(uint8);
            //uint8 m_networkId;      // Network id – unique identifier for network players
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NetworkID = uint8;
            //uint8 m_teamId;                 // Team id - see appendix
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetTeam(uint8);
            //uint8 m_myTeam;                 // My team flag – 1 = My Team, 0 = otherwise
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsMyTeam = b;
            //uint8 m_raceNumber;             // Race number of the car
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RaceNumber = uint8;
            //uint8 m_nationality;            // Nationality of the driver
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetNationality(uint8);
            //char m_name[48];               // Name of participant in UTF-8 format – null terminated
            //                               // Will be truncated with … (U+2026) if too long
            this.Index += ByteReader.toStringFromUint8(array, this.Index, 48, out s);
            this.Name = s;
            //uint8 m_yourTelemetry;          // The player's UDP setting, 0 = restricted, 1 = public
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.YouTelemetry = (TelemetrySettings)uint8;

            this.CreateShortName(this.DriverID);
        }

        private void SetNationality(byte uint8)
        {
            if (uint8 < Enum.GetValues(Nationalities.Unknown.GetType()).Length) this.Nationality = (Nationalities)uint8;
            else this.Nationality = Nationalities.Unknown;
        }

        private void SetDriver(byte uint8)
        {
            if (uint8 <= Enum.GetValues(Nationalities.Unknown.GetType()).Length - 1) this.DriverID = (Drivers)uint8;
            else this.DriverID = Drivers.Unknown;
        }

        private void SetTeam(byte uint8)
        {
            if (uint8 <= Enum.GetValues(Teams.Unknown.GetType()).Length - 2) this.TeamID = (Teams)uint8;
            else if (uint8 == (int)Teams.MyTeam) this.TeamID = Teams.MyTeam;
            else this.TeamID = Teams.Unknown;
        }

        private void CreateShortName(Drivers drivers)
        {
            string name = drivers.ToString();

            switch (name)
            {
                default:
                    var elements = Regex.Split(name, "(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                    this.ShortName = elements[elements.Length - 1].Substring(0, 3).ToUpper();
                    break;

            }
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
