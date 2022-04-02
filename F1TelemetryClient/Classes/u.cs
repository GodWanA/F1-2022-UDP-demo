using F1Telemetry;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Classes
{
    /// <summary>
    /// utility stuffs goes here.
    /// </summary>
    internal static class u
    {
        internal static F1UDP Connention { get; set; }
        internal static ushort TrackLength { get; set; }
        internal static Dictionary<Flags, System.Windows.Media.Brush> FlagColors { get; set; } = u.FillColors();

        private static Dictionary<Flags, System.Windows.Media.Brush> FillColors()
        {
            var ret = new Dictionary<Flags, System.Windows.Media.Brush>();

            ret.Add(Flags.InvalidOrUnknown, System.Windows.Media.Brushes.Transparent);
            ret.Add(Flags.None, System.Windows.Media.Brushes.Transparent);
            ret.Add(Flags.Green, new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 198, 1)));
            ret.Add(Flags.Blue, new SolidColorBrush(System.Windows.Media.Color.FromRgb(66, 120, 230)));
            ret.Add(Flags.Yellow, new SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 198, 0)));
            ret.Add(Flags.Red, new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 0, 47)));

            foreach (System.Windows.Media.Brush item in ret.Values)
            {
                if (item.CanFreeze) item.Freeze();
            }

            return ret;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Az Image osztáy Source propertibe ezzel a függvénnyel rakható be bitmap a Resource-ből
        /// </summary>
        /// <param name="bitmap">A kép Bitmap formátumban</param>
        /// <returns></returns>
        internal static BitmapSource KepForras(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException("bitmap");

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        internal static BitmapImage TyreCompoundToImage(TyreCompounds tyre)
        {
            string src = "pack://application:,,,/Images/Tyres/";
            BitmapImage ret = null;

            switch (tyre)
            {
                default:
                    ret = null;
                    break;
                // 2019+: 'S' tyre
                case TyreCompounds.C5:
                    ret = new BitmapImage(new Uri(src + "C5.png"));
                    break;
                // 2019+: 'M' tyre
                case TyreCompounds.C4:
                    ret = new BitmapImage(new Uri(src + "C4.png"));
                    break;
                // 2019+: 'H' tyre
                case TyreCompounds.C3:
                    ret = new BitmapImage(new Uri(src + "C3.png"));
                    break;
                // 'I' tyre
                case TyreCompounds.Inter:
                    ret = new BitmapImage(new Uri(src + "inter.png"));
                    break;
                // 'W' tyre
                case TyreCompounds.Wet:
                case TyreCompounds.F2Wet:
                    ret = new BitmapImage(new Uri(src + "wet.png"));
                    break;
                // 'HS' tyre
                case TyreCompounds.HyperSoft:
                    ret = new BitmapImage(new Uri(src + "hypersoft.png"));
                    break;
                // 'US' tyre
                case TyreCompounds.UltraSoft:
                    ret = new BitmapImage(new Uri(src + "ultrasoft.png"));
                    break;
                // 'SS' tyre
                case TyreCompounds.SuperSoft:
                case TyreCompounds.F2SuperSoft:
                    ret = new BitmapImage(new Uri(src + "supersoft.png"));
                    break;
                // 'S' tyre
                case TyreCompounds.Soft:
                case TyreCompounds.F2Soft:
                    ret = new BitmapImage(new Uri(src + "soft.png"));
                    break;
                // 'M' tyre
                case TyreCompounds.Medium:
                case TyreCompounds.F2Medium:
                    ret = new BitmapImage(new Uri(src + "medium.png"));
                    break;
                // 'H' tyre
                case TyreCompounds.Hard:
                case TyreCompounds.F2Hard:
                    ret = new BitmapImage(new Uri(src + "hard.png"));
                    break;
                // 'SH' tyre
                case TyreCompounds.SuperHard:
                    ret = new BitmapImage(new Uri(src + "superhard.png"));
                    break;
            }

            if (ret != null && ret.CanFreeze) ret.Freeze();
            return ret;
        }

        internal static double ImageMiltiplier(IEnumerable<double> x, double forWhat, out double a)
        {
            var min = x.Min();
            var max = x.Max();

            a = min;

            return forWhat / Math.Abs(max - min);
        }

        internal static SolidColorBrush PickTeamColor(Teams teamid)
        {
            SolidColorBrush ret = null;

            switch (teamid)
            {
                default:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 255));
                    break;
                // F1 Cars:
                case Teams.Mercedes:
                case Teams.Mercedes2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 210, 90));
                    break;
                case Teams.Ferrari:
                case Teams.Ferrari2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 0, 0));
                    break;
                case Teams.RedBullRacing:
                case Teams.RedBull2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(6, 0, 239));
                    break;
                case Teams.Alpine:
                case Teams.Renault2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 144, 255));
                    break;
                case Teams.Haas:
                case Teams.Haas2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                    break;
                case Teams.AstonMartin:
                case Teams.RacingPoint2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 111, 98));
                    break;
                case Teams.AlphaTauri:
                case Teams.AlphaTauri2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(43, 69, 98));
                    break;
                case Teams.McLaren:
                case Teams.McLaren2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 135, 0));
                    break;
                case Teams.AlfaRomeo:
                case Teams.AlfaRomeo2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(144, 0, 0));
                    break;
                case Teams.Williams:
                case Teams.Williams2020:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 90, 255));
                    break;
                // F2 Cars:
                case Teams.ArtGP19:
                case Teams.ArtGP20:
                case Teams.ArtGP21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(180, 179, 180));
                    break;
                case Teams.Arden19:
                case Teams.BWT20:
                case Teams.BWT21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(252, 185, 229));
                    break;
                case Teams.Campos19:
                case Teams.Campos20:
                case Teams.Campos21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(235, 193, 16));
                    break;
                case Teams.Carlin19:
                case Teams.Carlin20:
                case Teams.Carlin21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(36, 62, 246));
                    break;
                case Teams.SauberJuniorCharouz19:
                case Teams.Charouz20:
                case Teams.Charouz21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(132, 2, 10));
                    break;
                case Teams.Dams19:
                case Teams.Dams20:
                case Teams.Dams21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 212, 250));
                    break;
                case Teams.Hitech20:
                case Teams.Hitech21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 232, 232));
                    break;
                case Teams.MPMotorsport19:
                case Teams.MPMotorsport20:
                case Teams.MPMotorsport21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(247, 64, 26));
                    break;
                case Teams.Prema19:
                case Teams.Prema20:
                case Teams.Prema21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(232, 3, 9));
                    break;
                case Teams.Trident19:
                case Teams.Trident20:
                case Teams.Trident21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(14, 17, 133));
                    break;
                case Teams.UniVirtuosi19:
                case Teams.UniVirtuosi20:
                case Teams.UniVirtuosi21:
                    ret = new SolidColorBrush(System.Windows.Media.Color.FromRgb(251, 236, 32));
                    break;
            }

            if (ret.CanFreeze) ret.Freeze();
            return ret;
        }

        internal static BitmapImage NationalityImage(Nationalities nationality)
        {
            if (nationality == (Nationalities)255) nationality = Nationalities.Unknown;
            var ret = new BitmapImage(new Uri("pack://application:,,,/Images/Flags/" + nationality + ".png"));
            if (ret.CanFreeze) ret.Freeze();
            return ret;
        }

        internal static String PickTeamName(Teams teamid)
        {
            switch (teamid)
            {
                default:
                    return null;
                // F1 Cars:
                case Teams.Mercedes:
                case Teams.Mercedes2020:
                    return "Mercedes-AMG Petronas";
                case Teams.Ferrari:
                case Teams.Ferrari2020:
                    return "Scuderia Ferrari";
                case Teams.RedBullRacing:
                case Teams.RedBull2020:
                    return "Red Bull Racing";
                case Teams.Alpine:
                    return "Alpine";
                case Teams.Renault2020:
                    return "Renault";
                case Teams.Haas:
                case Teams.Haas2020:
                    return "HAAS";
                case Teams.AstonMartin:
                    return "Aston Martin";
                case Teams.RacingPoint2020:
                    return "Racing Point";
                case Teams.AlphaTauri:
                case Teams.AlphaTauri2020:
                    return "Alpha Tauri";
                case Teams.McLaren:
                case Teams.McLaren2020:
                    return "McLaren Racing";
                case Teams.AlfaRomeo:
                case Teams.AlfaRomeo2020:
                    return "Alfa Romeo Sauber";
                case Teams.Williams:
                case Teams.Williams2020:
                    return "Williams Racing";
                // MyTeam:
                case Teams.MyTeam:
                    return "MyTeam";
                // F2:                    
                case Teams.ArtGP19:
                case Teams.ArtGP20:
                case Teams.ArtGP21:
                    return "ART Grand Prix";
                case Teams.Arden19:
                    return "BWT Arden";
                case Teams.BWT20:
                    return "BWT HWA Racelab";
                case Teams.BWT21:
                    return "HWA Racelab";
                case Teams.Campos19:
                case Teams.Campos20:
                case Teams.Campos21:
                    return "CamposRacing";
                case Teams.Carlin19:
                case Teams.Carlin20:
                case Teams.Carlin21:
                    return "Carlin";
                case Teams.SauberJuniorCharouz19:
                    return "Sauber Junior Team by Charouz";
                case Teams.Charouz20:
                case Teams.Charouz21:
                    return "Charouz Racing System";
                case Teams.Dams19:
                case Teams.Dams20:
                case Teams.Dams21:
                    return "DAMS";
                case Teams.Hitech20:
                case Teams.Hitech21:
                    return "Hitech Grand Prix";
                case Teams.MPMotorsport19:
                case Teams.MPMotorsport20:
                case Teams.MPMotorsport21:
                    return "MP Motorsport";
                case Teams.Prema19:
                case Teams.Prema20:
                case Teams.Prema21:
                    return "Prema Racing";
                case Teams.Trident19:
                case Teams.Trident20:
                case Teams.Trident21:
                    return "Trident";
                case Teams.UniVirtuosi19:
                case Teams.UniVirtuosi20:
                case Teams.UniVirtuosi21:
                    return "UNI-Virtuosi Racing";
            }
        }

        internal static string CalculateDelta(
           PacketLapData lapDatas,
           PacketSessionHistoryData prevHistory,
           PacketSessionHistoryData nextHistory,
           out System.Windows.Media.Brush fontColor
        )
        {
            StringBuilder sb = new StringBuilder();
            fontColor = System.Windows.Media.Brushes.White;
            var sessionData = u.Connention.LastSessionDataPacket;

            if (
                lapDatas != null
                && sessionData != null
                && prevHistory != null
                && nextHistory != null
            )
            {
                switch (sessionData.SessionType)
                {
                    case SessionTypes.Race:
                    case SessionTypes.Race2:
                        int lap = nextHistory.NumberOfLaps;
                        int sector = 0;

                        if (lap > 0)
                        {
                            if (nextHistory.LapHistoryData[lap - 1].Sector3Time != TimeSpan.Zero) sector = 3;
                            else if (nextHistory.LapHistoryData[lap - 1].Sector2Time != TimeSpan.Zero) sector = 2;
                            else if (nextHistory.LapHistoryData[lap - 1].Sector1Time != TimeSpan.Zero) sector = 1;
                        }

                        var deltaTime = prevHistory.GetTimeSum(lap, sector) - nextHistory.GetTimeSum(lap, sector);

                        var deltaLaps = (int)MathF.Round(lapDatas.Lapdata[nextHistory.CarIndex].TotalLapDistance - lapDatas.Lapdata[prevHistory.CarIndex].TotalLapDistance) / sessionData.TrackLength;

                        if (Math.Abs(deltaTime.TotalSeconds) <= 1) fontColor = System.Windows.Media.Brushes.Orange;
                        else if (Math.Abs(deltaTime.TotalSeconds) <= 2) fontColor = System.Windows.Media.Brushes.Yellow;

                        if (deltaTime >= TimeSpan.Zero) sb.Append("+");
                        else sb.Append("-");

                        if (deltaLaps > 0) sb.Append(deltaTime.ToString(@"s\.f") + "(+" + deltaLaps + " L)");
                        else sb.Append(deltaTime.ToString(@"s\.fff"));
                        break;
                    default:
                        if (prevHistory.BestLapTimeLapNumber > 0 && nextHistory.BestLapTimeLapNumber > 0)
                        {
                            var curLaptime = prevHistory.LapHistoryData[prevHistory.BestLapTimeLapNumber - 1].LapTime;
                            var prevLaptime = nextHistory.LapHistoryData[nextHistory.BestLapTimeLapNumber - 1].LapTime;

                            if (curLaptime != TimeSpan.Zero && prevLaptime != TimeSpan.Zero)
                            {
                                deltaTime = curLaptime - prevLaptime;

                                if (deltaTime >= TimeSpan.Zero) sb.Append("+");
                                else sb.Append("-");

                                sb.Append(deltaTime.ToString(@"s\.fff"));
                            }
                        }
                        break;
                }
            }

            if (fontColor.CanFreeze) fontColor.Freeze();
            //return sb.ToString().Replace("--","-");
            return sb.ToString();
        }

        internal static float CalculateERS(float value)
        {
            return value / 4000000f * 100f;
        }
    }
}
