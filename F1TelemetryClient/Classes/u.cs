using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

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
    }
}
