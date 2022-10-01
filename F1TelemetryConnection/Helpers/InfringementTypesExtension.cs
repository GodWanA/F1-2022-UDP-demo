using System;
using System.Diagnostics;
using System.Linq;
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

        private static int[] array = Enum.GetValues<InfringementTypes>().Cast<int>().ToArray();

        public static InfringementTypes Parse(int value, int headerFormat)
        {
            var ret = InfringementTypes.Unknown;
            bool ok = array.Contains(value);

            if (ok)
            {
                switch (headerFormat)
                {
                    case 2018:
                    case 2019:
                    case 2020:
                    case 2021:
                        switch (value)
                        {
                            case 40:
                                ret = InfringementTypes.RetiredMechanicalFailure;
                                break;
                            case 41:
                                ret = InfringementTypes.RetiredTerminallyDamaged;
                                break;
                            case 42:
                                ret = InfringementTypes.SafetyCarFallingTooFarBack;
                                break;
                            case 43:
                                ret = InfringementTypes.BlackFlagTimer;
                                break;
                            case 44:
                                ret = InfringementTypes.UnservedStopGoPenalty;
                                break;
                            case 45:
                                ret = InfringementTypes.UnservedDriveThroughPenalty;
                                break;
                            case 46:
                                ret = InfringementTypes.EngineComponentChange;
                                break;
                            case 47:
                                ret = InfringementTypes.GearboxChange;
                                break;
                            case 48:
                                ret = InfringementTypes.LeagueGridPenalty;
                                break;
                            case 49:
                                ret = InfringementTypes.RetryPenalty;
                                break;
                            case 50:
                                ret = InfringementTypes.IllegalTimeGain;
                                break;
                            case 51:
                                ret = InfringementTypes.MandatoryPitstop;
                                break;
                            default:
                                ret = (InfringementTypes)value;
                                break;
                        }
                        break;
                    default:
                        ret = (InfringementTypes)value;
                        break;
                }
            }
            else
            {
                Debug.WriteLine("Unknown Infragmant value: " + value);
            }

            return ret;
        }
    }
}
