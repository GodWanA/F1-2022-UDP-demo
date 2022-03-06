using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.FinalClassificationPacket
{
    public class FinalClassificationData : ProtoModel
    {
        public FinalClassificationData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public byte Position { get; private set; }
        public byte NumberOfLaps { get; private set; }
        public byte GridPosition { get; private set; }
        public byte Points { get; private set; }
        public byte NumberOfPitStops { get; private set; }
        public ResultSatuses ResultStatus { get; private set; }
        public TimeSpan BestLapTime { get; private set; }
        public TimeSpan TotalRaceTime { get; private set; }
        public TimeSpan PenaltiesTime { get; private set; }
        public byte NumberOfPenalties { get; private set; }
        public byte NumberOfTyreStints { get; private set; }
        public TyreCompounds[] TyreStintsActual { get; private set; }
        public TyreCompounds[] TyreStintsVisual { get; private set; }

        protected override void Reader2020(byte[] array)
        {
            byte uint8;
            uint uint32;
            double d;
            TyreCompounds[] tc;

            //uint8 m_position;              // Finishing position
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Position = uint8;
            //uint8 m_numLaps;               // Number of laps completed
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfLaps = uint8;
            //uint8 m_gridPosition;          // Grid position of the car
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.GridPosition = uint8;
            //uint8 m_points;                // Number of points scored
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Points = uint8;
            //uint8 m_numPitStops;           // Number of pit stops made
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfPitStops = uint8;
            //uint8 m_resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                               // 3 = finished, 4 = didnotfinish, 5 = disqualified
            //                               // 6 = not classified, 7 = retired
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ResultStatus = (ResultSatuses)uint8;
            //uint32 m_bestLapTimeInMS;       // Best lap time of the session in milliseconds
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.BestLapTime = TimeSpan.FromMilliseconds(uint32);
            //double m_totalRaceTime;         // Total race time in seconds without penalties
            this.Index += ByteReader.ToDouble(array, this.Index, out d);
            this.TotalRaceTime = TimeSpan.FromSeconds(d);
            //uint8 m_penaltiesTime;         // Total penalties accumulated in seconds
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.PenaltiesTime = TimeSpan.FromSeconds(uint8);
            //uint8 m_numPenalties;          // Number of penalties applied to this driver
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfPenalties = uint8;
            //uint8 m_numTyreStints;         // Number of tyres stints up to maximum
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.NumberOfTyreStints = uint8;
            //uint8 m_tyreStintsActual[8];   // Actual tyres used by this driver
            this.Index += ByteReader.TyreArray(array, this.Index, out tc, 8);
            this.TyreStintsActual = tc;
            //uint8 m_tyreStintsVisual[8];   // Visual tyres used by this driver
            this.Index += ByteReader.TyreArray(array, this.Index, out tc, 8);
            this.TyreStintsVisual = tc;
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TyreStintsActual = null;
                this.TyreStintsVisual = null;
            }

            base.Dispose(disposing);
        }
    }
}
