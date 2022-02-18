using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.LapDataPacket
{
    public class LapData : ProtoModel
    {
        public LapData(int format, int index, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public TimeSpan LastLapTime { get; private set; }
        public TimeSpan CurrentLapTime { get; private set; }
        public TimeSpan Sector1Time { get; private set; }
        public TimeSpan Sector2Time { get; private set; }
        public float LapDistance { get; private set; }
        public byte CarPosition { get; private set; }
        public byte CurrentLapNum { get; private set; }
        public PitStatuses PitStatus { get; private set; }
        public byte NumberOfPitStops { get; private set; }
        public bool IsCurrentLapInvalid { get; private set; }
        public TimeSpan Penalties { get; private set; }
        public byte Warnings { get; private set; }
        public byte NumberOfUnservedDriveThroughPenalties { get; private set; }
        public byte NumberOfUnservedStopGoPenalties { get; private set; }
        public byte GridPosition { get; private set; }
        public DriverSatuses DriverStatus { get; private set; }
        public ResultSatuses ResultStatus { get; private set; }
        public bool IsPitLaneTimerActive { get; private set; }
        public TimeSpan PitLaneTimeInLane { get; private set; }
        public TimeSpan PitLaneTimer { get; private set; }
        public bool IsPitStopServePenalty { get; private set; }
        public float TotalLapDistance { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;

            uint valui;
            ushort valus;
            float valf;
            byte valb;
            bool valbo;

            //uint32 m_lastLapTimeInMS;            // Last lap time in milliseconds
            index += ByteReader.ToUInt32(array, index, out valui);
            this.LastLapTime = TimeSpan.FromMilliseconds(valui);

            //uint32 m_currentLapTimeInMS;     // Current time around the lap in milliseconds
            index += ByteReader.ToUInt32(array, index, out valui);
            this.CurrentLapTime = TimeSpan.FromMilliseconds(valui);

            //uint16 m_sector1TimeInMS;           // Sector 1 time in milliseconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Sector1Time = TimeSpan.FromMilliseconds(valus);

            //uint16 m_sector2TimeInMS;           // Sector 2 time in milliseconds
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Sector2Time = TimeSpan.FromMilliseconds(valus);

            //float m_lapDistance;         // Distance vehicle is around current lap in metres – could
            //                             // be negative if line hasn’t been crossed yet
            index += ByteReader.ToFloat(array, index, out valf);
            this.LapDistance = valf;

            //float m_totalDistance;       // Total distance travelled in session in metres – could
            //                             // be negative if line hasn’t been crossed yet
            index += ByteReader.ToFloat(array, index, out valf);
            this.TotalLapDistance = valf;

            //float m_safetyCarDelta;            // Delta in seconds for safety car
            index += ByteReader.ToFloat(array, index, out valf);
            this.Sector1Time = TimeSpan.FromSeconds(valf);

            //uint8 m_carPosition;             // Car race position
            index += ByteReader.ToUInt8(array, index, out valb);
            this.CarPosition = valb;

            //uint8 m_currentLapNum;       // Current lap number
            index += ByteReader.ToUInt8(array, index, out valb);
            this.CurrentLapNum = valb;

            //uint8 m_pitStatus;               // 0 = none, 1 = pitting, 2 = in pit area
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PitStatus = (PitStatuses)valb;

            //uint8 m_numPitStops;                 // Number of pit stops taken in this race
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfPitStops = valb;

            //uint8 m_sector;                  // 0 = sector1, 1 = sector2, 2 = sector3
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfPitStops = valb;

            //uint8 m_currentLapInvalid;       // Current lap invalid - 0 = valid, 1 = invalid
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsCurrentLapInvalid = valbo;

            //uint8 m_penalties;               // Accumulated time penalties in seconds to be added
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Penalties = TimeSpan.FromSeconds(valb);

            //uint8 m_warnings;                  // Accumulated number of warnings issued
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Warnings = valb;

            //uint8 m_numUnservedDriveThroughPens;  // Num drive through pens left to serve
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfUnservedDriveThroughPenalties = valb;

            //uint8 m_numUnservedStopGoPens;        // Num stop go pens left to serve
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfUnservedStopGoPenalties = valb;

            //uint8 m_gridPosition;            // Grid position the vehicle started the race in
            index += ByteReader.ToUInt8(array, index, out valb);
            this.GridPosition = valb;

            //uint8 m_driverStatus;            // Status of driver - 0 = in garage, 1 = flying lap
            //                                 // 2 = in lap, 3 = out lap, 4 = on track
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DriverStatus = (DriverSatuses)valb;

            //uint8 m_resultStatus;              // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                                   // 3 = finished, 4 = didnotfinish, 5 = disqualified
            //                                   // 6 = not classified, 7 = retired
            index += ByteReader.ToUInt8(array, index, out valb);
            this.ResultStatus = (ResultSatuses)valb;

            //uint8 m_pitLaneTimerActive;          // Pit lane timing, 0 = inactive, 1 = active
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsPitLaneTimerActive = valbo;

            //uint16 m_pitLaneTimeInLaneInMS;      // If active, the current time spent in the pit lane in ms
            index += ByteReader.ToUInt16(array, index, out valus);
            this.PitLaneTimeInLane = TimeSpan.FromMilliseconds(valus);

            //uint16 m_pitStopTimerInMS;           // Time of the actual pit stop in ms
            index += ByteReader.ToUInt16(array, index, out valus);
            this.PitLaneTimer = TimeSpan.FromMilliseconds(valus);

            //uint8 m_pitStopShouldServePen;   	 // Whether the car should serve a penalty at this stop
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsPitStopServePenalty = valbo;

            this.Index = index;
        }
    }
}
