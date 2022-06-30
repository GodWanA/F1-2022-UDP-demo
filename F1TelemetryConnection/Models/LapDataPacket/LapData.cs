using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.LapDataPacket
{
    public class LapData : ProtoModel
    {
        private byte lastPitLap;

        /// <summary>
        /// Creates a LapData object from raw byte array.
        /// </summary>
        /// <param name="format">Packet format</param>
        /// <param name="index">Start index of packet</param>
        /// <param name="array">Raw byte array</param>
        public LapData(int format, int index, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        protected override void Reader2018(byte[] array)
        {
            float f;
            byte uint8;
            bool b;
            //float m_lastLapTime;           // Last lap time in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.LastLapTime = TimeSpan.FromSeconds(f);
            //float m_currentLapTime;        // Current time around the lap in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.CurrentLapTime = TimeSpan.FromSeconds(f);
            //float m_bestLapTime;           // Best lap time of the session in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.BestLapTime = TimeSpan.FromSeconds(f);
            //float m_sector1Time;           // Sector 1 time in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.Sector1Time = TimeSpan.FromSeconds(f);
            //float m_sector2Time;           // Sector 2 time in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.Sector2Time = TimeSpan.FromSeconds(f);
            //float m_lapDistance;           // Distance vehicle is around current lap in metres – could
            //                               // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.LapDistance = f;
            //float m_totalDistance;         // Total distance travelled in session in metres – could
            //                               // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.TotalLapDistance = f;
            //float m_safetyCarDelta;        // Delta in seconds for safety car
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SafetyCarDelta = TimeSpan.FromSeconds(f);
            //uint8 m_carPosition;           // Car race position
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.CarPosition = uint8;
            //uint8 m_currentLapNum;         // Current lap number
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.CurrentLapNum = uint8;
            //uint8 m_pitStatus;             // 0 = none, 1 = pitting, 2 = in pit area
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStatus = (PitStatuses)uint8;
            //uint8 m_sector;                // 0 = sector1, 1 = sector2, 2 = sector3
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Sector = (Sectors)uint8;
            //uint8 m_currentLapInvalid;     // Current lap invalid - 0 = valid, 1 = invalid
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsCurrentLapInvalid = b;
            //uint8 m_penalties;             // Accumulated time penalties in seconds to be added
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Penalties = TimeSpan.FromSeconds(uint8);
            //uint8 m_gridPosition;          // Grid position the vehicle started the race in
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.GridPosition = uint8;
            //uint8 m_driverStatus;          // Status of driver - 0 = in garage, 1 = flying lap
            //                               // 2 = in lap, 3 = out lap, 4 = on track
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DriverStatus = (DriverSatuses)uint8;
            //uint8 m_resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                               // 3 = finished, 4 = disqualified, 5 = not classified
            //                               // 6 = retired
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ResultStatus = (ResultSatuses)uint8;
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            float f;
            byte uint8;
            ushort uint16;
            bool b;
            //float m_lastLapTime;               // Last lap time in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.LastLapTime = TimeSpan.FromSeconds(f);
            //float m_currentLapTime;            // Current time around the lap in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.CurrentLapTime = TimeSpan.FromSeconds(f);
            ////UPDATED in Beta 3:
            //uint16 m_sector1TimeInMS;           // Sector 1 time in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.Sector1Time = TimeSpan.FromMilliseconds(uint16);
            //uint16 m_sector2TimeInMS;           // Sector 2 time in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.Sector2Time = TimeSpan.FromMilliseconds(uint16);
            //float m_bestLapTime;               // Best lap time of the session in seconds
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.BestLapTime = TimeSpan.FromSeconds(f);
            //uint8 m_bestLapNum;                // Lap number best time achieved on
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            //this.BestLapNumber = uint8;
            //uint16 m_bestLapSector1TimeInMS;    // Sector 1 time of best lap in the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestLapSector1Time = TimeSpan.FromMilliseconds(uint16);
            //uint16 m_bestLapSector2TimeInMS;    // Sector 2 time of best lap in the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestLapSector2Time = TimeSpan.FromMilliseconds(uint16);
            //uint16 m_bestLapSector3TimeInMS;    // Sector 3 time of best lap in the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestLapSector3Time = TimeSpan.FromMilliseconds(uint16);
            //uint16 m_bestOverallSector1TimeInMS;// Best overall sector 1 time of the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestOverallLapSector1Time = TimeSpan.FromMilliseconds(uint16);
            //uint8 m_bestOverallSector1LapNum;  // Lap number best overall sector 1 time achieved on
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            //this.BestOverallSector1LapNumber = uint8;
            //uint16 m_bestOverallSector2TimeInMS;// Best overall sector 2 time of the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestOverallLapSector2Time = TimeSpan.FromMilliseconds(uint16);
            //uint8 m_bestOverallSector2LapNum;  // Lap number best overall sector 2 time achieved on
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            //this.BestOverallSector2LapNumber = uint8;
            //uint16 m_bestOverallSector3TimeInMS;// Best overall sector 3 time of the session in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            //this.BestOverallLapSector3Time = TimeSpan.FromMilliseconds(uint16);
            //uint8 m_bestOverallSector3LapNum;  // Lap number best overall sector 3 time achieved on
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            //this.BestOverallSector3LapNumber = uint8;
            //float m_lapDistance;               // Distance vehicle is around current lap in metres – could
            //                                   // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.LapDistance = f;
            //float m_totalDistance;             // Total distance travelled in session in metres – could
            //                                   // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.TotalLapDistance = f;
            //float m_safetyCarDelta;            // Delta in seconds for safety car
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.SafetyCarDelta = TimeSpan.FromSeconds(f);
            //uint8 m_carPosition;               // Car race position
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.CarPosition = uint8;
            //uint8 m_currentLapNum;             // Current lap number
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.CurrentLapNum = uint8;
            //uint8 m_pitStatus;                 // 0 = none, 1 = pitting, 2 = in pit area
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PitStatus = (PitStatuses)uint8;
            //uint8 m_sector;                    // 0 = sector1, 1 = sector2, 2 = sector3
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Sector = (Sectors)uint8;
            //uint8 m_currentLapInvalid;         // Current lap invalid - 0 = valid, 1 = invalid
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsCurrentLapInvalid = b;
            //uint8 m_penalties;                 // Accumulated time penalties in seconds to be added
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Penalties = TimeSpan.FromSeconds(uint8);
            //uint8 m_gridPosition;              // Grid position the vehicle started the race in
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.GridPosition = uint8;
            //uint8 m_driverStatus;              // Status of driver - 0 = in garage, 1 = flying lap
            //                                   // 2 = in lap, 3 = out lap, 4 = on track
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DriverStatus = (DriverSatuses)uint8;
            //uint8 m_resultStatus;              // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                                   // 3 = finished, 4 = disqualified, 5 = not classified
            //                                   // 6 = retired
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ResultStatus = (ResultSatuses)uint8;
        }

        protected override void Reader2021(byte[] array)
        {
            uint valui;
            ushort valus;
            float valf;
            byte valb;
            bool valbo;

            //uint32 m_lastLapTimeInMS;            // Last lap time in milliseconds
            this.Index += ByteReader.ToUInt32(array, this.Index, out valui);
            this.LastLapTime = TimeSpan.FromMilliseconds(valui);
            //uint32 m_currentLapTimeInMS;     // Current time around the lap in milliseconds
            this.Index += ByteReader.ToUInt32(array, this.Index, out valui);
            this.CurrentLapTime = TimeSpan.FromMilliseconds(valui);
            //uint16 m_sector1TimeInMS;           // Sector 1 time in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out valus);
            this.Sector1Time = TimeSpan.FromMilliseconds(valus);
            //uint16 m_sector2TimeInMS;           // Sector 2 time in milliseconds
            this.Index += ByteReader.ToUInt16(array, this.Index, out valus);
            this.Sector2Time = TimeSpan.FromMilliseconds(valus);
            //float m_lapDistance;         // Distance vehicle is around current lap in metres – could
            //                             // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.LapDistance = valf;
            //float m_totalDistance;       // Total distance travelled in session in metres – could
            //                             // be negative if line hasn’t been crossed yet
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.TotalLapDistance = valf;
            //float m_safetyCarDelta;            // Delta in seconds for safety car
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.SafetyCarDelta = TimeSpan.FromSeconds(valf);
            //uint8 m_carPosition;             // Car race position
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.CarPosition = valb;
            //uint8 m_currentLapNum;       // Current lap number
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.CurrentLapNum = valb;
            //uint8 m_pitStatus;               // 0 = none, 1 = pitting, 2 = in pit area
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.PitStatus = (PitStatuses)valb;
            //uint8 m_numPitStops;                 // Number of pit stops taken in this race
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.NumberOfPitStops = valb;
            //uint8 m_sector;                  // 0 = sector1, 1 = sector2, 2 = sector3
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Sector = (Sectors)valb;
            //uint8 m_currentLapInvalid;       // Current lap invalid - 0 = valid, 1 = invalid
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsCurrentLapInvalid = valbo;
            //uint8 m_penalties;               // Accumulated time penalties in seconds to be added
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Penalties = TimeSpan.FromSeconds(valb);
            //uint8 m_warnings;                  // Accumulated number of warnings issued
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Warnings = valb;
            //uint8 m_numUnservedDriveThroughPens;  // Num drive through pens left to serve
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.NumberOfUnservedDriveThroughPenalties = valb;
            //uint8 m_numUnservedStopGoPens;        // Num stop go pens left to serve
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.NumberOfUnservedStopGoPenalties = valb;
            //uint8 m_gridPosition;            // Grid position the vehicle started the race in
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.GridPosition = valb;
            //uint8 m_driverStatus;            // Status of driver - 0 = in garage, 1 = flying lap
            //                                 // 2 = in lap, 3 = out lap, 4 = on track
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.DriverStatus = (DriverSatuses)valb;
            //uint8 m_resultStatus;              // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                                   // 3 = finished, 4 = didnotfinish, 5 = disqualified
            //                                   // 6 = not classified, 7 = retired
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.ResultStatus = (ResultSatuses)valb;
            //uint8 m_pitLaneTimerActive;          // Pit lane timing, 0 = inactive, 1 = active
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsPitLaneTimerActive = valbo;
            //uint16 m_pitLaneTimeInLaneInMS;      // If active, the current time spent in the pit lane in ms
            this.Index += ByteReader.ToUInt16(array, this.Index, out valus);
            this.PitLaneTimeInLane = TimeSpan.FromMilliseconds(valus);
            //uint16 m_pitStopTimerInMS;           // Time of the actual pit stop in ms
            this.Index += ByteReader.ToUInt16(array, this.Index, out valus);
            this.PitStopTimer = TimeSpan.FromMilliseconds(valus);
            //uint8 m_pitStopShouldServePen;   	 // Whether the car should serve a penalty at this stop
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsPitStopServePenalty = valbo;
        }

        internal void SetLastPitLap(byte lastPitLap)
        {
            this.lastPitLap = lastPitLap;

            switch (this.PitStatus)
            {
                case PitStatuses.Pitting:
                case PitStatuses.InPitArea:
                    this.lastPitLap = this.CurrentLapNum;
                    break;
            }
        }

        internal byte GetLastPitLap()
        {
            return this.lastPitLap;
        }

        internal void SetLapPercentage(ushort trackLength)
        {
            this.LapPercentege = MathF.Round(this.LapDistance / trackLength * 100f, 2);
        }

        internal void CalculateStatus(SessionTypes session)
        {
            switch (session)
            {
                case SessionTypes.TimeTrial:
                case SessionTypes.Practice1:
                case SessionTypes.Practice2:
                case SessionTypes.Practice3:
                case SessionTypes.Quallifying1:
                case SessionTypes.Quallifying2:
                case SessionTypes.Quallifying3:
                case SessionTypes.OneShotQuallifying:
                case SessionTypes.ShortQuallifying:

                    if (this.GetLastPitLap() == this.CurrentLapNum) this.DriverStatus = DriverSatuses.OutLap;
                    else this.DriverStatus = DriverSatuses.FlyingLap;

                    this.SetTime();

                    break;
            }
        }

        private void SetTime()
        {
            switch (this.PitStatus)
            {
                case PitStatuses.Pitting:
                case PitStatuses.InPitArea:
                    this.CurrentLapTime = TimeSpan.Zero;
                    break;
            }
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
        }

        /// <summary>
        /// Last lap time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan LastLapTime { get; private set; }
        /// <summary>
        /// Current lap time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan CurrentLapTime { get; private set; }
        /// <summary>
        /// Sector 1 time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan Sector1Time { get; private set; }
        /// <summary>
        /// Sector 2 time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan Sector2Time { get; private set; }
        /// <summary>
        /// Penalties.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan Penalties { get; private set; }
        /// <summary>
        /// Pit stop timer.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan PitStopTimer { get; private set; }
        /// <summary>
        /// Pit lane timer.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan PitLaneTimeInLane { get; private set; }
        /// <summary>
        /// Indicates current pit status.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public PitStatuses PitStatus { get; private set; }
        /// <summary>
        /// Indicates current pit status.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public DriverSatuses DriverStatus { get; private set; }
        /// <summary>
        /// Indicates current driver status.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public ResultSatuses ResultStatus { get; private set; }
        /// <summary>
        /// Indicates current driver status.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br
        ///     - 2022<br/>
        /// </summary>
        public float LapDistance { get; private set; }
        /// <summary>
        /// Total distance travelled in session in metres – could be negative if line hasn’t been crossed yet.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float TotalLapDistance { get; private set; }
        /// <summary>
        /// Current prosition.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte CarPosition { get; private set; }
        /// <summary>
        /// Current lap number.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte CurrentLapNum { get; private set; }
        /// <summary>
        /// Number of pit stops.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte NumberOfPitStops { get; private set; }
        /// <summary>
        /// Indicates current lap is valid.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsCurrentLapInvalid { get; private set; }
        /// <summary>
        /// Number of warnings.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte Warnings { get; private set; }
        /// <summary>
        /// Number of unserved drive through penalties.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte NumberOfUnservedDriveThroughPenalties { get; private set; }
        /// <summary>
        /// Number of unserved stop-go penalties.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte NumberOfUnservedStopGoPenalties { get; private set; }
        /// <summary>
        /// Grid position.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte GridPosition { get; private set; }
        /// <summary>
        /// Indicates Pit Lane timer is active.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsPitLaneTimerActive { get; private set; }
        /// <summary>
        /// Indicates driver going to serve penalty at next pit stop.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsPitStopServePenalty { get; private set; }
        /// <summary>
        /// Best lap time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan BestLapTime { get; private set; }
        /// <summary>
        /// Safety Car delta time.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan SafetyCarDelta { get; private set; }
        /// <summary>
        /// Current sector.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br
        ///     - 2022<br/>
        /// </summary>
        public Sectors Sector { get; private set; }
        /// <summary>
        /// Driven distance in track.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float LapPercentege { get; private set; }
    }
}
