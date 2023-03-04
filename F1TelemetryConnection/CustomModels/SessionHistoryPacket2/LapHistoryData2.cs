using F1Telemetry.Models;
using System;
using System.Data;
using System.Linq;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.CustomModels.SessionHistoryPacket2
{
    public class LapHistoryData2 : ProtoModel
    {
        public DataTable LapDatas { get; protected set; }
        public TimeSpan BestLapTime { get; protected set; }
        public TimeSpan BestSector1 { get; protected set; }
        public TimeSpan BestSector2 { get; protected set; }
        public TimeSpan BestSector3 { get; protected set; }

        public byte CarIndex { get; protected set; }

        public LapHistoryData2(byte carIndex)
        {
            this.CarIndex = carIndex;
            this.LapDatas = new DataTable();

            this.LapDatas.Columns.Add("LapNumber", typeof(int));

            for (int i = 0; i <= 100; i++)
            {
                this.LapDatas.Columns.Add($"{i}%", typeof(TimeSpan));
            }

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
            var p = (int)MathF.Round(percent);
            if (p >= 0)
            {
                var row = this.GetLastLapDatasRow();
                if (row != null) row[$"{p}%"] = laptime;
            }
        }

        internal void UpdateSectorsTime(TimeSpan sector1Time, TimeSpan sector2Time)
        {
            var row = this.GetLastLapDatasRow();


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

            if (row["100%"] != DBNull.Value)
            {
                var sector3Time = (TimeSpan)row["100%"] - sector2Time - sector1Time;

                if (sector3Time > TimeSpan.Zero)
                {
                    row["Sector3"] = sector3Time;

                    var minSectorTime = this.LapDatas.AsEnumerable().Where(x => x["Sector3"] != DBNull.Value).Min(x => (TimeSpan)x["Sector3"]);
                    if (minSectorTime > TimeSpan.Zero && this.BestSector1 != sector1Time) this.BestSector3 = minSectorTime;
                }
            }
        }

        internal void UpdateLastLapTime(TimeSpan lastLapTime, byte currentLapNum, bool isValidLap)
        {
            if (lastLapTime > TimeSpan.Zero)
            {
                var lapNum = currentLapNum - 1;
                var row = this.LapDatas.AsEnumerable().Where(x => (int)x["LapNumber"] == lapNum).FirstOrDefault();

                if (row != null)
                {
                    row["100%"] = lastLapTime;

                    if (isValidLap)
                    {
                        var bestLap = this.LapDatas.AsEnumerable().Where(x => x["100%"] != DBNull.Value).Min(x => (TimeSpan)x["100%"]);
                        if (bestLap > TimeSpan.Zero && this.BestLapTime != bestLap) this.BestLapTime = bestLap;
                    }
                }
            }
        }

        internal int GetLapNumber()
        {
            var lastLap = this.GetLastLapDatasRow();

            if (lastLap != null && lastLap["LapNumber"] != DBNull.Value) return (int)lastLap["LapNumber"];
            else return -1;
        }

        internal int GetLapPercent(int lapNumber)
        {
            if(lapNumber < 1) return -1;

            var r = this.LapDatas.AsEnumerable().Where(r => r["LapNumber"]!= DBNull.Value && (int)r["LapNumber"] == lapNumber).FirstOrDefault();
            var p = 0;

            if (r != null)
            {
                for (int i = 0; i <= 100; i++)
                {
                    var c = r[$"{i}%"];

                    if (c != DBNull.Value) p = i;
                    else break;
                }
            }

            return p;
        }

        internal TimeSpan GetRaceTime(int lapnum, int percent)
        {
            if (lapnum <= 0 && percent<=0) return TimeSpan.Zero;

            var enumerable = this.LapDatas.AsEnumerable();
            var fullLaps = enumerable.Where(r => r["LapNumber"] != DBNull.Value && (int)r["LapNumber"] <= lapnum);

            var sum = fullLaps.Sum(x => {
                if (x["100%"] != DBNull.Value) return ((TimeSpan)x["100%"]).TotalMilliseconds;
                else return 0;
                    });

            var c= enumerable.
                Where(r => r["LapNumber"] != DBNull.Value && (int)r["LapNumber"] == lapnum)
                .Select(r => {
                    var c =r[$"{percent}%"];
                    if (c != DBNull.Value) return (TimeSpan)c;
                    else return TimeSpan.Zero;
                })
                .FirstOrDefault();
            
            sum += c.TotalMilliseconds;
            return TimeSpan.FromMilliseconds(sum);
        }
    }
}
