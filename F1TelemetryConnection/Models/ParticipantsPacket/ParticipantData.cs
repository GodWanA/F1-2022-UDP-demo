using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.ParticipantsPacket
{
    public class ParticipantData : ProtoModel
    {
        public ParticipantData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public bool IsAIControlled { get; private set; }
        public Drivers DriverID { get; private set; }
        public byte NetworkID { get; private set; }
        public Teams TeamID { get; private set; }
        public bool IsMyTeam { get; private set; }
        public byte RaceNumber { get; private set; }
        public Nationalities Nationality { get; private set; }
        public string Name { get; private set; }
        public TelemetrySettings YouTelemetry { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            bool valbo;
            string s;

            //uint8 m_aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsAIControlled = valbo;

            //uint8 m_driverId;       // Driver id - see appendix, 255 if network human
            index += ByteReader.ToUInt8(array, index, out valb);
            if (valb <= Enum.GetValues(Nationalities.Unknown.GetType()).Length - 1) this.DriverID = (Drivers)valb;
            else this.DriverID = Drivers.Unknown;

            //uint8 m_networkId;      // Network id – unique identifier for network players
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NetworkID = valb;

            //uint8 m_teamId;                 // Team id - see appendix
            index += ByteReader.ToUInt8(array, index, out valb);
            if (valb <= Enum.GetValues(Teams.Unknown.GetType()).Length - 2) this.TeamID = (Teams)valb;
            else if (valb == (int)Teams.MyTeam) this.TeamID = Teams.MyTeam;
            else this.TeamID = Teams.Unknown;

            //uint8 m_myTeam;                 // My team flag – 1 = My Team, 0 = otherwise
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsMyTeam = valbo;

            //uint8 m_raceNumber;             // Race number of the car
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RaceNumber = valb;

            //uint8 m_nationality;            // Nationality of the driver
            index += ByteReader.ToUInt8(array, index, out valb);
            if (valb < Enum.GetValues(Nationalities.Unknown.GetType()).Length) this.Nationality = (Nationalities)valb;
            else this.Nationality = Nationalities.Unknown;

            //char m_name[48];               // Name of participant in UTF-8 format – null terminated
            //                               // Will be truncated with … (U+2026) if too long
            index += ByteReader.toStringFromUint8(array, index, 48, out s);
            this.Name = s;

            //uint8 m_yourTelemetry;          // The player's UDP setting, 0 = restricted, 1 = public
            index += ByteReader.ToUInt8(array, index, out valb);
            this.YouTelemetry = (TelemetrySettings)valb;

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
