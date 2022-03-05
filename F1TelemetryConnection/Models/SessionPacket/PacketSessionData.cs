using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class PacketSessionData : ProtoModel
    {
        /// <summary>
        /// Creates a PacketSessionData from raw byte array.
        /// </summary>
        /// <param name="header">Header packet of the object.</param>
        /// <param name="array">Raw byte array</param>
        public PacketSessionData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        protected override void Reader2018(byte[] array)
        {
            byte uint8;
            sbyte int8;
            ushort uint16;
            bool b;
            //    uint8 m_weather;                // Weather - 0 = clear, 1 = light cloud, 2 = overcast
            //                                    // 3 = light rain, 4 = heavy rain, 5 = storm
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Weather = (WeatherTypes)uint8;
            //    int8 m_trackTemperature;        // Track temp. in degrees celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.TrackTemperature = int8;
            //    int8 m_airTemperature;          // Air temp. in degrees celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.AirTemperature = int8;
            //    uint8 m_totalLaps;              // Total number of laps in this race
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TotalLaps = uint8;
            //    uint16 m_trackLength;               // Track length in metres
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.TrackLength = uint16;
            //    uint8 m_sessionType;            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
            //                                    // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
            //                                    // 10 = R, 11 = R2, 12 = Time Trial
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SessionType = (SessionTypes)uint8;
            //    int8 m_trackId;                 // -1 for unknown, 0-21 for tracks, see appendix
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.TrackID = (Tracks)int8;
            //    uint8 m_era;                    // Era, 0 = modern, 1 = classic
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Formula = (Formulas)uint8;
            //    uint16 m_sessionTimeLeft;       // Time left in session in seconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.SessionTimeLeft = TimeSpan.FromSeconds(uint16);
            //    uint16 m_sessionDuration;       // Session duration in seconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.SessionDuration = TimeSpan.FromSeconds(uint16);
            //    uint8 m_pitSpeedLimit;          // Pit speed limit in kilometres per hour
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitSpeedLimit = uint8;
            //    uint8 m_gamePaused;               // Whether the game is paused
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsGamePaused = b;
            //    uint8 m_isSpectating;           // Whether the player is spectating
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsSpectating = b;
            //    uint8 m_spectatorCarthis.Index;      // this.Index of the car being spectated
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SpectatorCarIndex = uint8;
            //    uint8 m_sliProNativeSupport;    // SLI Pro support, 0 = inactive, 1 = active
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsSLIProNativeSupport = b;
            //    uint8 m_numMarshalZones;            // Number of marshal zones to follow
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfMarshalZones = uint8;
            //    MarshalZone m_marshalZones[21];         // List of marshal zones – max 21
            this.MarshalZones = new MarshalZone[21];
            for (int i = 0; i < this.MarshalZones.Length; i++)
            {
                this.MarshalZones[i] = new MarshalZone(this.Index, this.Header.PacketFormat, array, this.TrackLength);
                this.Index = this.MarshalZones[i].Index;
            }
            //    uint8 m_safetyCarStatus;          // 0 = no safety car, 1 = full safety car
            //                                      // 2 = virtual safety car
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SafetyCarStatus = (SafetyCarStatuses)uint8;
            //    uint8 m_networkGame;              // 0 = offline, 1 = online
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsNetworkGame = b;
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            this.Reader2018(array);
            this.ReadWeatherForecast(array, 20);
        }

        protected override void Reader2021(byte[] array)
        {
            bool b;
            byte uint8;
            uint uint32;

            this.Reader2018(array);
            //uint8 m_numWeatherForecastSamples; // Number of weather samples to follow
            //WeatherForecastSample m_weatherForecastSamples[56];   // Array of weather forecast samples
            this.ReadWeatherForecast(array, 56);

            //uint8 m_forecastAccuracy;          // 0 = Perfect, 1 = Approximate
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsForecastApproximate = b;
            //uint8 m_aiDifficulty;              // AI Difficulty rating – 0-110
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.AIDifficulty = uint8;
            //uint32 m_seasonLinkIdentifier;      // Identifier for season - persists across saves
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.SeasonLinkIdentifier = uint32;
            //uint32 m_weekendLinkIdentifier;     // Identifier for weekend - persists across saves
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.WeekendLinkIdentifier = uint32;
            //uint32 m_sessionLinkIdentifier;     // Identifier for session - persists across saves
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.SessionLinkIdentifier = uint32;
            //uint8 m_pitStopWindowIdealLap;     // Ideal lap to pit on for current strategy (player)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStopWindowIdealLap = uint8;
            //uint8 m_pitStopWindowLatestLap;    // Latest lap to pit on for current strategy (player)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStopWindowLastestLap = uint8;
            //uint8 m_pitStopRejoinPosition;     // Predicted position to rejoin at (player)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStopRejoinPosition = uint8;
            //uint8 m_steeringAssist;            // 0 = off, 1 = on
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStopRejoinPosition = uint8;
            //uint8 m_brakingAssist;             // 0 = off, 1 = low, 2 = medium, 3 = high
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.BreakingAssist = (BreakingAssists)uint8;
            //uint8 m_gearboxAssist;             // 1 = manual, 2 = manual & suggested gear, 3 = auto
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.GearboxAssist = (GearboxAssists)uint8;
            //uint8 m_pitAssist;                 // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitAssist = b;
            //uint8 m_pitReleaseAssist;          // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitReleaseAssist = b;
            //uint8 m_ERSAssist;                 // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsERSAssist = b;
            //uint8 m_DRSAssist;                 // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSAssist = b;
            //uint8 m_dynamicRacingLine;         // 0 = off, 1 = corners only, 2 = full
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DynamicRacingLine = (RacingLineSatuses)uint8;
            //uint8 m_dynamicRacingLineType;     // 0 = 2D, 1 = 3D
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DynamicRacingLineType = (RacingLineTypes)uint8;
        }

        private void ReadWeatherForecast(byte[] array, int numberOfArray)
        {
            byte uint8;

            //uint8 m_numWeatherForecastSamples; // Number of weather samples to follow
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfWeatherForcastSamples = uint8;
            //WeatherForecastSample m_weatherForecastSamples[20];   // Array of weather forecast samples
            this.WeatherForcastSample = new WeatherForecastSample[numberOfArray];
            for (int i = 0; i < WeatherForcastSample.Length; i++)
            {
                this.WeatherForcastSample[i] = new WeatherForecastSample(this.Index, this.Header.PacketFormat, array);
                this.Index = this.WeatherForcastSample[i].Index;
            }
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// Actual weathers status type.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public WeatherTypes Weather { get; private set; }
        /// <summary>
        /// Actual track temperature.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public sbyte TrackTemperature { get; private set; }
        /// <summary>
        /// Actual air temperature.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public sbyte AirTemperature { get; private set; }
        /// <summary>
        /// Total laps of session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte TotalLaps { get; private set; }
        /// <summary>
        /// Length of the current track.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort TrackLength { get; private set; }
        /// <summary>
        /// Type of the current session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public SessionTypes SessionType { get; private set; }
        /// <summary>
        /// Current track ID<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Tracks TrackID { get; private set; }
        /// <summary>
        /// The current type of cars.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Formulas Formula { get; private set; }
        /// <summary>
        /// Time left from session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public TimeSpan SessionTimeLeft { get; private set; }
        /// <summary>
        /// Total possible duration of session.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public TimeSpan SessionDuration { get; private set; }
        /// <summary>
        /// Pit speed limit.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte PitSpeedLimit { get; private set; }
        /// <summary>
        /// Indicates game is paused.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsGamePaused { get; private set; }
        /// <summary>
        /// Indicates player is spectating.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsSpectating { get; private set; }
        /// <summary>
        /// Spectated car's this.Index number.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte SpectatorCarIndex { get; private set; }
        /// <summary>
        /// Indicates SLI Pro support.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsSLIProNativeSupport { get; private set; }
        /// <summary>
        /// Number of marshal zones.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte NumberOfMarshalZones { get; private set; }
        /// <summary>
        /// Track's marshal zones.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public MarshalZone[] MarshalZones { get; private set; }
        /// <summary>
        /// Safety car's current status.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public SafetyCarStatuses SafetyCarStatus { get; private set; }
        /// <summary>
        /// Indicates current game is network game.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsNetworkGame { get; private set; }
        /// <summary>
        /// Number of weather forecast smaples.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte NumberOfWeatherForcastSamples { get; private set; }
        /// <summary>
        /// Weather forecast samples array.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public WeatherForecastSample[] WeatherForcastSample { get; private set; }
        /// <summary>
        /// Indicates weather forecast is approximate.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsForecastApproximate { get; private set; }
        /// <summary>
        /// AI difficulty.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte AIDifficulty { get; private set; }
        /// <summary>
        /// Identifier for season - persists across saves.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public uint SeasonLinkIdentifier { get; private set; }
        /// <summary>
        /// Identifier for weekend - persists across saves.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public uint WeekendLinkIdentifier { get; private set; }
        /// <summary>
        /// Identifier for session - persists across saves.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public uint SessionLinkIdentifier { get; private set; }
        /// <summary>
        /// Ideal lap to pit on for current strategy (player).<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte PitStopWindowIdealLap { get; private set; }
        /// <summary>
        /// Last lap to pit on for current strategy (player).<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte PitStopWindowLastestLap { get; private set; }
        /// <summary>
        /// Predicted position to rejoin at (player).<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte PitStopRejoinPosition { get; private set; }
        /// <summary>
        /// Indicates steering assist on/off.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsSteeringAssist { get; private set; }
        /// <summary>
        /// Indicates Pit assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsPitAssist { get; private set; }
        /// <summary>
        /// Indicates Pit release assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsPitReleaseAssist { get; private set; }
        /// <summary>
        /// Indicates Pit release assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsERSAssist { get; private set; }
        /// <summary>
        /// Indicates ERS assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsDRSAssist { get; private set; }
        /// <summary>
        /// Indicates DRS assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public BreakingAssists BreakingAssist { get; private set; }
        /// <summary>
        /// Indicates dynamic racing type assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public RacingLineSatuses DynamicRacingLine { get; private set; }
        /// <summary>
        /// Indicates dynamic racing line type.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public RacingLineTypes DynamicRacingLineType { get; private set; }
        /// <summary>
        /// Indicates gearbox assist current status.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public GearboxAssists GearboxAssist { get; private set; }

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
