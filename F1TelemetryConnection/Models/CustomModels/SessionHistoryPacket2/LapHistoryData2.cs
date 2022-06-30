using System;
using System.Data;
using System.Linq;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CustomModels.SessionHistoryPacket2
{
    public class LapHistoryData2 : ProtoModel
    {
        public DataTable LapDatas { get; private set; }
        public TimeSpan BestLapTime { get; private set; }
        public TimeSpan BestSector1 { get; private set; }
        public TimeSpan BestSector2 { get; private set; }
        public TimeSpan BestSector3 { get; private set; }

        public LapHistoryData2()
        {
            this.LapDatas = new DataTable();

            this.LapDatas.Columns.Add("LapNumber", typeof(int));
            this.LapDatas.Columns.Add("0%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("5%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("10%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("15%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("20%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("25%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("30%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("35%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("40%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("45%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("50%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("55%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("60%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("65%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("70%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("75%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("80%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("85%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("90%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("95%", typeof(TimeSpan));
            this.LapDatas.Columns.Add("100%", typeof(TimeSpan));

            this.LapDatas.Columns.Add("Sector1", typeof(TimeSpan));
            this.LapDatas.Columns.Add("Sector2", typeof(TimeSpan));
            this.LapDatas.Columns.Add("Sector3", typeof(TimeSpan));

            this.LapDatas.Columns.Add("Tyre", typeof(TyreCompounds));
            this.LapDatas.Columns.Add("TyreAge", typeof(int));

            this.AppendEmptyRow();
        }

        private void AppendEmptyRow()
        {
            this.LapDatas.Rows.Add(this.LapDatas.NewRow());
        }

        internal DataRow GetLastLapDatasRow()
        {
            return this.LapDatas.Rows[this.LapDatas.Rows.Count - 1];
        }

        internal void UpdateLapNumber(int lapNumber)
        {
            var row = this.LapDatas.AsEnumerable().Where(x => x["LapNumber"] != DBNull.Value && (int)x["LapNumber"] == lapNumber).FirstOrDefault();
            if (row == null)
            {
                row = this.GetLastLapDatasRow();
            }
            else
            {
                var rows = this.LapDatas.AsEnumerable().Where(x => x["LapNumber"] != DBNull.Value && (int)x["LapNumber"] > lapNumber);

                while (rows.Count() != 0)
                {
                    this.LapDatas.Rows.Remove(rows.FirstOrDefault());
                }
            }

            if (row["LapNumber"] == DBNull.Value)
            {
                row["LapNumber"] = lapNumber;
            }
            else if ((int)row["LapNumber"] != lapNumber)
            {
                this.AppendEmptyRow();
                this.UpdateLapNumber(lapNumber);
            }
        }

        internal void UpdateLaptime(float percent, TimeSpan laptime)
        {
            int p = (int)MathF.Floor(percent);

            if (p > 0 && p % 5 == 0)
            {
                string col = p + "%";
                var row = this.GetLastLapDatasRow();
                //if (row[col] == DBNull.Value) row[col] = laptime;
                row[col] = laptime;
            }
        }

        internal void UpdateSectorsTime(TimeSpan sector1Time, TimeSpan sector2Time)
        {
            var row = this.GetLastLapDatasRow();
            var sector3Time = (TimeSpan)row["100%"] - sector2Time - sector1Time;

            if (sector1Time > TimeSpan.Zero)
            {
                row["Sector1"] = sector1Time;

                var minSectorTime = this.LapDatas.AsEnumerable().Where(x => x["Sector1"] != DBNull.Value).Min(x => (TimeSpan)x["Sector1"]);
                if (minSectorTime > TimeSpan.Zero && this.BestSector1 != sector1Time) this.BestSector1 = minSectorTime;
            }

            if (sector2Time > TimeSpan.Zero)
            {
                row["Sector2"] = sector2Time;

                var minSectorTime = this.LapDatas.AsEnumerable().Where(x => x["Sector2"] != DBNull.Value).Min(x => (TimeSpan)x["Sector2"]);
                if (minSectorTime > TimeSpan.Zero && this.BestSector1 != sector1Time) this.BestSector2 = minSectorTime;
            }

            if (sector3Time > TimeSpan.Zero)
            {
                row["Sector3"] = sector3Time;

                var minSectorTime = this.LapDatas.AsEnumerable().Where(x => x["Sector3"] != DBNull.Value).Min(x => (TimeSpan)x["Sector3"]);
                if (minSectorTime > TimeSpan.Zero && this.BestSector1 != sector1Time) this.BestSector3 = minSectorTime;
            }
        }

        internal void UpdateLastLapTime(TimeSpan lastLapTime, byte currentLapNum)
        {
            if (lastLapTime > TimeSpan.Zero)
            {
                var lapNum = currentLapNum - 1;
                var row = this.LapDatas.AsEnumerable().Where(x => (int)x["LapNumber"] == lapNum).FirstOrDefault();

                if (row != null)
                {
                    row["100%"] = lastLapTime;
                    var bestLap = this.LapDatas.AsEnumerable().Where(x => x["100%"] != DBNull.Value).Min(x => (TimeSpan)x["100%"]);
                    if (bestLap > TimeSpan.Zero && this.BestLapTime != bestLap) this.BestLapTime = bestLap;
                }
            }
        }
    }
}
