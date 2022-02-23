using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class PacketSessionData : ProtoModel
    {
        public PacketHeader Header { get; private set; }
        public WeatherTypes Weather { get; private set; }
        public sbyte TrackTemperature { get; private set; }
        public sbyte AirTemperature { get; private set; }
        public byte TotalLaps { get; private set; }
        public ushort TrackLength { get; private set; }
        public SessionTypes SessionType { get; private set; }
        public Tracks TrackID { get; private set; }
        public Formulas Formula { get; private set; }
        public TimeSpan SessionTimeLeft { get; private set; }
        public TimeSpan SessionDuration { get; private set; }
        public byte PitSpeedLimit { get; private set; }
        public bool IsGamePaused { get; private set; }
        public bool IsSpectating { get; private set; }
        public byte SpectatorCarIndex { get; private set; }
        public bool IsSLIProNativeSupport { get; private set; }
        public byte NumberOfMarshalZones { get; private set; }
        public MarshalZone[] MarshalZones { get; private set; }
        public SafetyCarStatuses SafetyCarStatus { get; private set; }
        public bool IsNetworkGame { get; private set; }
        public byte NumberOfWheaterForcastSamples { get; private set; }
        public WeatherForecastSample[] WeatherForcastSample { get; private set; }
        public bool IsForecastApproximate { get; private set; }
        public byte AIDifficulty { get; private set; }
        public uint SeasonLinkIdentifier { get; private set; }
        public uint WeekendLinkIdentifier { get; private set; }
        public uint SessionLinkIdentifier { get; private set; }
        public byte PitStopWindowIdealLap { get; private set; }
        public byte PitStopWindowLastestLap { get; private set; }
        public byte PitStopRejoinPosition { get; private set; }
        public bool IsSteeringAssist { get; private set; }
        public bool IsPitAssist { get; private set; }
        public bool IsPitReleaseAssist { get; private set; }
        public bool IsERSAssist { get; private set; }
        public bool IsDRSAssist { get; private set; }
        public BreakingAssistSettings BreakingAssist { get; private set; }
        public RacingLineSatuses DynamicRacingLine { get; private set; }
        public RacingLineTypes DynamicRacingLineTyoe { get; private set; }
        public GearboxAssists GearboxAssist { get; private set; }

        public PacketSessionData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            sbyte valsb;
            ushort valus;
            bool valbo;
            uint valui;

            //uint8 m_weather;              	// Weather - 0 = clear, 1 = light cloud, 2 = overcast
            //                                  // 3 = light rain, 4 = heavy rain, 5 = storm
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Weather = (WeatherTypes)valb;

            //int8 m_trackTemperature;        // Track temp. in degrees celsius
            index += ByteReader.ToInt8(array, index, out valsb);
            this.TrackTemperature = valsb;

            //int8 m_airTemperature;          // Air temp. in degrees celsius
            index += ByteReader.ToInt8(array, index, out valsb);
            this.AirTemperature = valsb;

            //uint8 m_totalLaps;              // Total number of laps in this race
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TotalLaps = valb;

            //uint16 m_trackLength;               // Track length in metres
            index += ByteReader.ToUInt16(array, index, out valus);
            this.TrackLength = valus;

            //uint8 m_sessionType;            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
            //                                // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
            //                                // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SessionType = (SessionTypes)valb;

            //int8 m_trackId;                 // -1 for unknown, 0-21 for tracks, see appendix
            index += ByteReader.ToInt8(array, index, out valsb);
            this.TrackID = (Tracks)valsb;

            //uint8 m_formula;                    // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
            //                                    // 3 = F1 Generic
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Formula = (Formulas)valb;

            //uint16 m_sessionTimeLeft;       // Time left in session in seconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.SessionTimeLeft = TimeSpan.FromSeconds(valus);

            //uint16 m_sessionDuration;       // Session duration in seconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.SessionDuration = TimeSpan.FromSeconds(valus);

            //uint8 m_pitSpeedLimit;          // Pit speed limit in kilometres per hour
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PitSpeedLimit = valb;

            //uint8 m_gamePaused;                // Whether the game is paused
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsGamePaused = valbo;

            //uint8 m_isSpectating;           // Whether the player is spectating
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsSpectating = valbo;

            //uint8 m_spectatorCarIndex;      // Index of the car being spectated
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SpectatorCarIndex = valb;

            //uint8 m_sliProNativeSupport;    // SLI Pro support, 0 = inactive, 1 = active
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsSLIProNativeSupport = valbo;

            //uint8 m_numMarshalZones;            // Number of marshal zones to follow
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfMarshalZones = valb;

            //MarshalZone m_marshalZones[21];             // List of marshal zones – max 21
            this.MarshalZones = new MarshalZone[21];
            for (int i = 0; i < this.MarshalZones.Length; i++)
            {
                this.MarshalZones[i] = new MarshalZone(index, this.Header.PacketFormat, array, this.TrackLength);
                index = this.MarshalZones[i].Index;
            }

            //uint8 m_safetyCarStatus;           // 0 = no safety car, 1 = full
            //                                   // 2 = virtual, 3 = formation lap
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SafetyCarStatus = (SafetyCarStatuses)valb;

            //uint8 m_networkGame;               // 0 = offline, 1 = online
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsNetworkGame = valbo;

            //uint8 m_numWeatherForecastSamples; // Number of weather samples to follow
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfWheaterForcastSamples = valb;

            //WeatherForecastSample m_weatherForecastSamples[56];   // Array of weather forecast samples
            this.WeatherForcastSample = new WeatherForecastSample[56];
            for (int i = 0; i < this.WeatherForcastSample.Length; i++)
            {
                this.WeatherForcastSample[i] = new WeatherForecastSample(index, this.Header.PacketFormat, array);
                index = this.WeatherForcastSample[i].Index;
            }

            //uint8 m_forecastAccuracy;          // 0 = Perfect, 1 = Approximate
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsForecastApproximate = valbo;

            //uint8 m_aiDifficulty;              // AI Difficulty rating – 0-110
            index += ByteReader.ToUInt8(array, index, out valb);
            this.AIDifficulty = valb;

            //uint32 m_seasonLinkIdentifier;      // Identifier for season - persists across saves
            index += ByteReader.ToUInt32(array, index, out valui);
            this.SeasonLinkIdentifier = valui;

            //uint32 m_weekendLinkIdentifier;     // Identifier for weekend - persists across saves
            index += ByteReader.ToUInt32(array, index, out valui);
            this.WeekendLinkIdentifier = valui;

            //uint32 m_sessionLinkIdentifier;     // Identifier for session - persists across saves
            index += ByteReader.ToUInt32(array, index, out valui);
            this.SessionLinkIdentifier = valui;

            //uint8 m_pitStopWindowIdealLap;     // Ideal lap to pit on for current strategy (player)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PitStopWindowIdealLap = valb;

            //uint8 m_pitStopWindowLatestLap;    // Latest lap to pit on for current strategy (player)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PitStopWindowLastestLap = valb;

            //uint8 m_pitStopRejoinPosition;     // Predicted position to rejoin at (player)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PitStopRejoinPosition = valb;

            //uint8 m_steeringAssist;            // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsSteeringAssist = valbo;

            //uint8 m_brakingAssist;             // 0 = off, 1 = low, 2 = medium, 3 = high
            index += ByteReader.ToUInt8(array, index, out valb);
            this.BreakingAssist = (BreakingAssistSettings)valb;

            //uint8 m_gearboxAssist;             // 1 = manual, 2 = manual & suggested gear, 3 = auto
            index += ByteReader.ToUInt8(array, index, out valb);
            this.GearboxAssist = (GearboxAssists)valb;

            //uint8 m_pitAssist;                 // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsPitAssist = valbo;

            //uint8 m_pitReleaseAssist;          // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsPitReleaseAssist = valbo;

            //uint8 m_ERSAssist;                 // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsERSAssist = valbo;

            //uint8 m_DRSAssist;                 // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsDRSAssist = valbo;

            //uint8 m_dynamicRacingLine;         // 0 = off, 1 = corners only, 2 = full
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DynamicRacingLine = (RacingLineSatuses)valb;

            //uint8 m_dynamicRacingLineType;     // 0 = 2D, 1 = 3D
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DynamicRacingLineTyoe = (RacingLineTypes)valb;

            this.Index = index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.MarshalZones.Length; i++) this.MarshalZones[i].Dispose();
                this.MarshalZones = null;

                for (int i = 0; i < this.WeatherForcastSample.Length; i++) this.WeatherForcastSample[i].Dispose();
                this.WeatherForcastSample = null;
            }

            base.Dispose(disposing);
        }
    }
}
