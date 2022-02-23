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

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            uint valui;
            double vald;
            TyreCompounds[] valtc;

            //uint8 m_position;              // Finishing position
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Position = valb;

            //uint8 m_numLaps;               // Number of laps completed
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfLaps = valb;

            //uint8 m_gridPosition;          // Grid position of the car
            index += ByteReader.ToUInt8(array, index, out valb);
            this.GridPosition = valb;

            //uint8 m_points;                // Number of points scored
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Points = valb;

            //uint8 m_numPitStops;           // Number of pit stops made
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfPitStops = valb;

            //uint8 m_resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
            //                               // 3 = finished, 4 = didnotfinish, 5 = disqualified
            //                               // 6 = not classified, 7 = retired
            index += ByteReader.ToUInt8(array, index, out valb);
            this.ResultStatus = (ResultSatuses)valb;

            //uint32 m_bestLapTimeInMS;       // Best lap time of the session in milliseconds
            index += ByteReader.ToUInt32(array, index, out valui);
            this.BestLapTime = TimeSpan.FromMilliseconds(valui);

            //double m_totalRaceTime;         // Total race time in seconds without penalties
            index += ByteReader.ToDouble(array, index, out vald);
            this.TotalRaceTime = TimeSpan.FromSeconds(vald);

            //uint8 m_penaltiesTime;         // Total penalties accumulated in seconds
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PenaltiesTime = TimeSpan.FromSeconds(valb);

            //uint8 m_numPenalties;          // Number of penalties applied to this driver
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfPenalties = valb;

            //uint8 m_numTyreStints;         // Number of tyres stints up to maximum
            index += ByteReader.ToUInt8(array, index, out valb);
            this.NumberOfTyreStints = valb;

            //uint8 m_tyreStintsActual[8];   // Actual tyres used by this driver
            index += ByteReader.TyreArray(array, index, out valtc, 8);
            this.TyreStintsActual = valtc;

            //uint8 m_tyreStintsVisual[8];   // Visual tyres used by this driver
            index += ByteReader.TyreArray(array, index, out valtc, 8);
            this.TyreStintsVisual = valtc;

            this.Index = index;
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
