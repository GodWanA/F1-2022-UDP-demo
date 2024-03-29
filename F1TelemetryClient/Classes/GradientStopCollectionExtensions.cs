﻿using System;
using System.Linq;
using System.Windows.Media;

namespace F1TelemetryApp.Classes
{
    public static class GradientStopCollectionExtensions
    {
        public static Color GetRelativeColor(this GradientStopCollection gsc, double offset)
        {
            var point = gsc.FirstOrDefault(f => f.Offset == offset);
            if (point != null) return point.Color;

            GradientStop before = gsc.Where(w => w.Offset == gsc.Min(m => m.Offset)).FirstOrDefault();
            GradientStop after = gsc.Where(w => w.Offset == gsc.Max(m => m.Offset)).FirstOrDefault();

            foreach (var gs in gsc)
            {
                if (gs.Offset < offset && gs.Offset > before.Offset)
                {
                    before = gs;
                }
                if (gs.Offset > offset && gs.Offset < after.Offset)
                {
                    after = gs;
                }
            }

            var color = new Color
            {
                ScA = (float)((offset - before.Offset) * (after.Color.ScA - before.Color.ScA) / (after.Offset - before.Offset) + before.Color.ScA),
                ScR = (float)((offset - before.Offset) * (after.Color.ScR - before.Color.ScR) / (after.Offset - before.Offset) + before.Color.ScR),
                ScG = (float)((offset - before.Offset) * (after.Color.ScG - before.Color.ScG) / (after.Offset - before.Offset) + before.Color.ScG),
                ScB = (float)((offset - before.Offset) * (after.Color.ScB - before.Color.ScB) / (after.Offset - before.Offset) + before.Color.ScB),
            };

            return color;
        }
    }

    public static class ColorExtensions
    {
        public static bool IsLightColor(this Color c)
        {
            var brightness = Math.Sqrt(c.R * c.R * .241 + c.G * c.G * .691 + c.B * c.B * .068);

            if (brightness > 127) return true;
            else return false;

            //if (c.R > 127 && c.G > 127 && c.B > 127) return true;
            //else return false;
        }
    }

    public static class BrushExtension
    {
        public static Brush Negative(this Brush b)
        {
            Brush ret = null;
            const byte m = 255;

            if (b is SolidColorBrush)
            {
                Color c = (b as SolidColorBrush).Color;
                var tmp = new SolidColorBrush(Color.FromArgb(c.A, (byte)(m - c.R), (byte)(m - c.G), (byte)(m - c.B)));

                if (tmp.CanFreeze) tmp.Freeze();
                ret = tmp;
            }

            return ret;
        }

        public static Brush GrayScale(this Brush b)
        {
            Brush ret = null;

            if (b is SolidColorBrush)
            {
                Color c = (b as SolidColorBrush).Color;

                var nc = (byte)Math.Round((c.R + c.G + c.B) / 3.0);
                var tmp = new SolidColorBrush(Color.FromArgb(c.A, nc, nc, nc));

                if (tmp.CanFreeze) tmp.Freeze();
                ret = tmp;
            }

            return ret;
        }
    }
}
