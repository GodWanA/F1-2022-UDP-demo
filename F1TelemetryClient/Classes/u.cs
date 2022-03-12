using F1Telemetry;
using F1Telemetry.Models.LapDataPacket;
using F1Telemetry.Models.SessionHistoryPacket;
using F1Telemetry.Models.SessionPacket;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Classes
{
    /// <summary>
    /// utility stuffs goes here.
    /// </summary>
    internal static class u
    {
        internal static F1UDP Connention { get; set; }

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

            switch (tyre)
            {
                default:
                    return null;
                // 2019+: 'S' tyre
                case TyreCompounds.C5:
                    return new BitmapImage(new Uri(src + "C5.png"));
                // 2019+: 'M' tyre
                case TyreCompounds.C4:
                    return new BitmapImage(new Uri(src + "C4.png"));
                // 2019+: 'H' tyre
                case TyreCompounds.C3:
                    return new BitmapImage(new Uri(src + "C3.png"));
                // 'I' tyre
                case TyreCompounds.Inter:
                    return new BitmapImage(new Uri(src + "inter.png"));
                // 'W' tyre
                case TyreCompounds.Wet:
                case TyreCompounds.F2Wet:
                    return new BitmapImage(new Uri(src + "wet.png"));
                // 'HS' tyre
                case TyreCompounds.HyperSoft:
                    return new BitmapImage(new Uri(src + "hypersoft.png"));
                // 'US' tyre
                case TyreCompounds.UltraSoft:
                    return new BitmapImage(new Uri(src + "ultrasoft.png"));
                // 'SS' tyre
                case TyreCompounds.SuperSoft:
                case TyreCompounds.F2SuperSoft:
                    return new BitmapImage(new Uri(src + "supersoft.png"));
                // 'S' tyre
                case TyreCompounds.Soft:
                case TyreCompounds.F2Soft:
                    return new BitmapImage(new Uri(src + "soft.png"));
                // 'M' tyre
                case TyreCompounds.Medium:
                case TyreCompounds.F2Medium:
                    return new BitmapImage(new Uri(src + "medium.png"));
                // 'H' tyre
                case TyreCompounds.Hard:
                case TyreCompounds.F2Hard:
                    return new BitmapImage(new Uri(src + "hard.png"));
                // 'SH' tyre
                case TyreCompounds.SuperHard:
                    return new BitmapImage(new Uri(src + "superhard.png"));
            }
        }

        internal static SolidColorBrush PickTeamColor(Teams teamid)
        {
            switch (teamid)
            {
                default:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 255));
                // F1 Cars:
                case Teams.Mercedes:
                case Teams.Mercedes2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 210, 90));
                case Teams.Ferrari:
                case Teams.Ferrari2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 0, 0));
                case Teams.RedBullRacing:
                case Teams.RedBull2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(6, 0, 239));
                case Teams.Alpine:
                case Teams.Renault2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 144, 255));
                case Teams.Haas:
                case Teams.Haas2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                case Teams.AstonMartin:
                case Teams.RacingPoint2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 111, 98));
                case Teams.AlphaTauri:
                case Teams.AlphaTauri2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(43, 69, 98));
                case Teams.McLaren:
                case Teams.McLaren2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 135, 0));
                case Teams.AlfaRomeo:
                case Teams.AlfaRomeo2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(144, 0, 0));
                case Teams.Williams:
                case Teams.Williams2020:
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 90, 255));
            }
        }

        internal static BitmapImage NationalityImage(Nationalities nationality)
        {
            if (nationality == (Nationalities)255) nationality = Nationalities.Unknown;
            return new BitmapImage(new Uri("pack://application:,,,/Images/Flags/" + nationality + ".png"));
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
                lapDatas != null &&
                sessionData != null &&
                prevHistory != null &&
                nextHistory != null
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

            //return sb.ToString().Replace("--","-");
            return sb.ToString();
        }

        public static float CalculateERS(float value)
        {
            return value / 4000000f * 100f;
        }
    }
}
