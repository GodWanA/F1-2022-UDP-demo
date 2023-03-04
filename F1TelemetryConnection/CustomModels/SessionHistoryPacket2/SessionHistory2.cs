using F1Telemetry.Helpers;
using F1Telemetry.Models;
using F1Telemetry.Models.LapDataPacket;
using System;
using System.Linq;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.CustomModels.SessionHistoryPacket2
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
                this.DriversHistory[i] = new LapHistoryData2((byte)i);
            }
        }

        internal void UpdateDriverHistory(ref LapData current, int i)
        {
            var history = this.DriversHistory[i];

            history.UpdateLapNumber(current.CurrentLapNum);
            history.UpdateLaptime(current.LapPercentege, current.CurrentLapTime);
            history.UpdateLastLapTime(current.LastLapTime, current.CurrentLapNum, !current.IsCurrentLapInvalid);
            history.UpdateSectorsTime(current.Sector1Time, current.Sector2Time);

            var bestSector1 = this.DriversHistory.Min(x => x.BestSector1);
            if (bestSector1 > TimeSpan.Zero && this.OverallBestSector1 != bestSector1) this.OverallBestSector1 = bestSector1;

            var bestSector2 = this.DriversHistory.Min(x => x.BestSector1);
            if (bestSector2 > TimeSpan.Zero && this.OverallBestSector2 != bestSector2) this.OverallBestSector2 = bestSector2;

            var bestSector3 = this.DriversHistory.Min(x => x.BestSector3);
            if (bestSector3 > TimeSpan.Zero && this.OverallBestSector3 != bestSector3) this.OverallBestSector3 = bestSector3;

            //var bestLap = this.DriversHistory.Min(x => x.BestLapTime);
            //if (bestLap > TimeSpan.Zero && this.OverallBestSector1 != bestLap) this.OverallBestSector1 = bestLap;
        }

        public TimeSpan CalculateInterval(int index1, int index2, SessionTypes session)
        {
            var car1 = this.DriversHistory[index1];
            var car2 = this.DriversHistory[index2];

            if (!session.IsRace()) return car1.BestLapTime - car2.BestLapTime;

            var c1LapNum = car1.GetLapNumber();
            var c2LapNum = car2.GetLapNumber();

            var minLap =
                c1LapNum < c2LapNum ?
                c1LapNum :
                c2LapNum;

            var c1percent = car1.GetLapPercent(minLap);
            var c2percent = car2.GetLapPercent(minLap);

            var minPercent =
                c1percent < c2percent ?
                c1percent :
                c2percent;

            return
                car1.GetRaceTime(minLap, minPercent) -
                car2.GetRaceTime(minLap, minPercent);
        }

        internal void Reset()
        {
            int n = 22;
            for (int i = 0; i < n; i++)
            {
                this.DriversHistory[i] = new LapHistoryData2((byte)i);
            }

            this.OverallBestSector1 = TimeSpan.Zero;
            this.OverallBestSector2 = TimeSpan.Zero;
            this.OverallBestSector3 = TimeSpan.Zero;
        }
    }
}
