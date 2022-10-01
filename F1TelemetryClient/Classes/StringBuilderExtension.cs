using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F1TelemetryApp.Classes
{
    internal static class StringBuilderExtension
    {
        public static StringBuilder AppendWithSeparator(this StringBuilder sb, string text, string separator)
        {
            if (sb.Length > 0) sb.Append(separator);
            sb.Append(text);
            return sb;
        }

        public static bool Contains(this StringBuilder sb, string text)
        {
            if (sb.ToString().Contains(text)) return true;
            else return false;
        }
    }
}
