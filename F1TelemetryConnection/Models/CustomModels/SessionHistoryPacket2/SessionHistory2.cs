using F1Telemetry.Models.LapDataPacket;
using System;
using System.Linq;

namespace F1Telemetry.Models.CustomModels.SessionHistoryPacket2
{
    public class SessionHistory2 : ProtoModel
    {
        public LapHistoryData2[] DriversHistory { get; private set; }
        public TimeSpan OverallBestSector1 { get; private set; }
        public TimeSpan OverallBestSector2 { get; private set; }
        public TimeSpan OverallBestSector3 { get; private set; }

        public SessionHistory2()
        {
            int n = 22;
            this.DriversHistory = new LapHistoryData2[n];

            for (int i = 0; i < n; i++)
            {
                this.DriversHistory[i] = new LapHistoryData2();
            }
        }

        internal void UpdateDriverHistory(ref LapData current, int i)
        {
            var history = this.DriversHistory[i];

            history.UpdateLapNumber(current.CurrentLapNum);
            history.UpdateLaptime(current.LapPercentege, current.CurrentLapTime);
            history.UpdateLastLapTime(current.LastLapTime, current.CurrentLapNum);
            history.UpdateSectorsTime(current.Sector1Time, current.Sector2Time);

            var bestSector1 = this.DriversHistory.Min(x => x.BestSector1);
            if (bestSector1 > TimeSpan.Zero && this.OverallBestSector1 != bestSector1) this.OverallBestSector1 = bestSector1;

            var bestSector2 = this.DriversHistory.Min(x => x.BestSector1);
            if (bestSector2 > TimeSpan.Zero && this.OverallBestSector2 != bestSector2) this.OverallBestSector2 = bestSector2;

            var bestSector3 = this.DriversHistory.Min(x => x.BestSector3);
            if (bestSector3 > TimeSpan.Zero && this.OverallBestSector3 != bestSector3) this.OverallBestSector3 = bestSector3;

            var bestLap = this.DriversHistory.Min(x => x.BestLapTime);
            if (bestLap > TimeSpan.Zero && this.OverallBestSector1 != bestLap) this.OverallBestSector1 = bestLap;
        }

        public void CalculateInterval(int index1, int index2)
        {

        }
    }
}
