using System;
using System.Text.RegularExpressions;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Helpers
{
    public static class InfringementTypesExtension
    {
        private static string EnumToString(string s)
        {
            var r = new Regex(
                            @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])",
                            RegexOptions.IgnorePatternWhitespace
                        );

            return r.Replace(s, " ");
        }

        public static string GetName(this InfringementTypes infringement)
        {
            return InfringementTypesExtension.EnumToString(infringement.ToString());
        }
    }
}
