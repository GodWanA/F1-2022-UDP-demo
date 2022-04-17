using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Classes
{
    internal class TrackLayout : IDisposable
    {
        public int Year { get; private set; }
        public string TrackName { get; private set; }
        public List<Point> BaseLine { get; private set; }
        public Dictionary<int, List<Point>> DRSZones { get; private set; }
        public Dictionary<int, List<Point>> MarshalZones { get; private set; }
        public Dictionary<int, List<Point>> SectorZones { get; private set; }
        public TyreCompounds TyreSoft { get; private set; }
        public TyreCompounds TyreMedium { get; private set; }
        public TyreCompounds TyreHard { get; private set; }

        public TrackLayout(int year, string trackName)
        {
            this.Year = year;
            this.TrackName = trackName;
            this.BaseLine = new List<Point>();
            this.DRSZones = new Dictionary<int, List<Point>>();
            this.MarshalZones = new Dictionary<int, List<Point>>();
            this.SectorZones = new Dictionary<int, List<Point>>();
        }

        public void SetBaseline(List<Point> source)
        {
            this.BaseLine = source;
        }

        public void AppendDRS(List<Point> source)
        {
            if (this.DRSZones == null) this.DRSZones = new Dictionary<int, List<Point>>();
            int index = this.DRSZones.Keys.Count;
            this.DRSZones.Add(index, source);
        }

        public void AppendMarshal(List<Point> source)
        {
            if (this.MarshalZones == null) this.MarshalZones = new Dictionary<int, List<Point>>();
            int index = this.MarshalZones.Keys.Count;
            this.MarshalZones.Add(index, source);
        }

        public void AppendSector(List<Point> source)
        {
            if (this.SectorZones == null) this.SectorZones = new Dictionary<int, List<Point>>();
            int index = this.SectorZones.Keys.Count;
            this.SectorZones.Add(index, source);
        }

        public void SetTyres(TyreCompounds soft, TyreCompounds medium, TyreCompounds hard)
        {
            this.TyreSoft = soft;
            this.TyreMedium = medium;
            this.TyreHard = hard;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this.BaseLine.Clear();
                    this.BaseLine = null;

                    this.DRSZones.Clear();
                    this.DRSZones = null;

                    this.MarshalZones.Clear();
                    this.MarshalZones = null;

                    this.SectorZones.Clear();
                    this.SectorZones = null;

                    this.TrackName = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TrackLayout()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private const string trackFile = "Data\\tracks.xml";
        private const string trackFolder = "Data";
        private bool disposedValue;

        internal static List<TrackLayout> Tracks { get; private set; } = TrackLayout.LoadTracks();
        //private static FileSystemWatcher trackUpdater = TrackLayout.CreateUpdate();
        private static bool isLoadingTracks = false;

        private static FileSystemWatcher CreateUpdate()
        {
            var t = new FileSystemWatcher();
            t.Path = TrackLayout.trackFolder;
            t.Filter = "tracks.xml";
            t.NotifyFilter = NotifyFilters.LastWrite;
            t.EnableRaisingEvents = true;

            t.Changed += T_Changed;

            return t;
        }

        private static void T_Changed(object sender, FileSystemEventArgs e)
        {
            //TrackLayout.trackUpdater.EnableRaisingEvents = false;
            if (!TrackLayout.isLoadingTracks)
            {
                TrackLayout.isLoadingTracks = true;
                TrackLayout.Tracks = TrackLayout.LoadTracks();
                TrackLayout.isLoadingTracks = false;
            }
            //TrackLayout.trackUpdater.EnableRaisingEvents = true;
        }

        private static List<TrackLayout> LoadTracks()
        {
            TrackLayout.Tracks?.Clear();

            var tracks = new List<TrackLayout>();

            if (File.Exists(TrackLayout.trackFile))
            {
                using (var stream = new FileStream(TrackLayout.trackFile, FileMode.Open, FileAccess.Read))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);

                    foreach (XmlNode item in xmlDoc.SelectNodes("//Track"))
                    {
                        int year = int.Parse(item.Attributes["Year"].Value);
                        string name = item.Attributes["Name"].Value;

                        var tmp = new TrackLayout(year, name);

                        var soft = (TyreCompounds)int.Parse(item.Attributes["Soft"].Value);
                        var medium = (TyreCompounds)int.Parse(item.Attributes["Medium"].Value);
                        var hard = (TyreCompounds)int.Parse(item.Attributes["Hard"].Value);

                        tmp.SetTyres(soft, medium, hard);

                        var baseline = new List<Point>();
                        foreach (XmlNode inner in item.SelectNodes(".//Baseline//Point"))
                        {
                            var s = inner.InnerText.Trim().Split(';');
                            double x = double.Parse(s[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                            double y = double.Parse(s[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                            baseline.Add(new Point(x, y));
                        }

                        tmp.SetBaseline(baseline);

                        foreach (XmlNode inner in item.SelectNodes(".//Sector"))
                        {
                            var p = new List<Point>();

                            foreach (XmlNode point in inner.SelectNodes(".//Point"))
                            {
                                var s = point.InnerText.Trim().Split(";");
                                double x = double.Parse(s[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                                double y = double.Parse(s[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                                p.Add(new Point(x, y));
                            }

                            tmp.AppendSector(p);
                        }

                        foreach (XmlNode inner in item.SelectNodes(".//DRSzone"))
                        {
                            var p = new List<Point>();

                            foreach (XmlNode point in inner.SelectNodes(".//Point"))
                            {
                                var s = point.InnerText.Trim().Split(";");
                                double x = double.Parse(s[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                                double y = double.Parse(s[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                                p.Add(new Point(x, y));
                            }

                            tmp.AppendDRS(p);
                        }

                        foreach (XmlNode inner in item.SelectNodes(".//MarshalZone"))
                        {
                            var p = new List<Point>();

                            foreach (XmlNode point in inner.SelectNodes(".//Point"))
                            {
                                var s = point.InnerText.Trim().Split(";");
                                double x = double.Parse(s[0], NumberStyles.Any, CultureInfo.InvariantCulture);
                                double y = double.Parse(s[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                                p.Add(new Point(x, y));
                            }

                            tmp.AppendMarshal(p);
                        }

                        tracks.Add(tmp);
                    }
                }
            }

            return tracks;
        }

        internal static void SaveTrack(TrackLayout track)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                CloseOutput = true,
            };

            TrackLayout.Tracks.Clear();
            TrackLayout.Tracks = TrackLayout.LoadTracks();

            if (TrackLayout.Tracks.Contains(track))
            {
                int i = Array.IndexOf(
                    TrackLayout.Tracks.ToArray(),
                    TrackLayout.Tracks.Where(x => x.TrackName == track.TrackName && x.Year == track.Year).FirstOrDefault()
                    );
                TrackLayout.Tracks[i] = track;
            }
            else
            {
                TrackLayout.Tracks.Add(track);
            }

            if (!Directory.Exists(TrackLayout.trackFolder)) Directory.CreateDirectory(TrackLayout.trackFolder);

            using (var writer = XmlWriter.Create(TrackLayout.trackFile, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Tracks");

                foreach (var t in TrackLayout.Tracks)
                {
                    writer.WriteStartElement("Track");
                    writer.WriteAttributeString("Name", t.TrackName);
                    writer.WriteAttributeString("Year", t.Year.ToString());

                    writer.WriteAttributeString("Soft", ((int)t.TyreSoft).ToString());
                    writer.WriteAttributeString("Medium", ((int)t.TyreMedium).ToString());
                    writer.WriteAttributeString("Hard", ((int)t.TyreHard).ToString());

                    writer.WriteStartElement("Baseline");
                    foreach (var b in t.BaseLine)
                    {
                        writer.WriteStartElement("Point");
                        writer.WriteValue(b.ToString().Replace(",", "."));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Sectors");
                    foreach (var s in t.SectorZones)
                    {
                        writer.WriteStartElement("Sector");
                        foreach (var inner in s.Value)
                        {
                            writer.WriteStartElement("Point");
                            writer.WriteValue(inner.ToString().Replace(",", "."));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("DRSZones");
                    foreach (var s in t.DRSZones)
                    {
                        writer.WriteStartElement("DRSzone");
                        foreach (var inner in s.Value)
                        {
                            writer.WriteStartElement("Point");
                            writer.WriteValue(inner.ToString().Replace(",", "."));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("MarshalZones");
                    foreach (var s in t.MarshalZones)
                    {
                        writer.WriteStartElement("MarshalZone");
                        foreach (var inner in s.Value)
                        {
                            writer.WriteStartElement("Point");
                            writer.WriteValue(inner.ToString().Replace(",", "."));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }
        }

        internal static TrackLayout FindNearestMap(string trackName, int season)
        {
            TrackLayout ret = null;

            ret = TrackLayout.Tracks?.Where(x => Regex.IsMatch(x.TrackName, trackName, RegexOptions.IgnoreCase))
                .OrderBy(x => MathF.Abs(x.Year - season)).FirstOrDefault();

            return ret;
        }

        public override bool Equals(object obj)
        {
            if (obj is TrackLayout)
            {
                var o = obj as TrackLayout;
                if (o.TrackName == this.TrackName && o.Year == this.Year) return true;
                else return false;
            }
            else
            {
                //return base.Equals(obj);
                return false;
            }
        }
    }
}
