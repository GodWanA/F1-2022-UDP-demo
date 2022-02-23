using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using static F1Telemetry.Helpers.Appendences;

namespace F1TelemetryApp.Classes
{
    /// <summary>
    /// utility stuffs goes here.
    /// </summary>
    public static class u
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Az Image osztáy Source propertibe ezzel a függvénnyel rakható be bitmap a Resource-ből
        /// </summary>
        /// <param name="bitmap">A kép Bitmap formátumban</param>
        /// <returns></returns>
        public static BitmapSource KepForras(Bitmap bitmap)
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

        public static BitmapImage TyreCompoundToImage(TyreCompounds tyre)
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

        private static DateTime lastClean;

        public static void BeginClean()
        {
            //var now = DateTime.Now;
            //if (now - lastClean > TimeSpan.FromSeconds(0.1))
            //{
            //GC.WaitForFullGCApproach();
            //GC.WaitForFullGCComplete();
            //GC.WaitForPendingFinalizers();
            GC.Collect();
            //}
        }
    }
}
