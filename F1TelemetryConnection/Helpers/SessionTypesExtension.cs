using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Helpers
{
    public static class SessionTypesExtension
    {
        private static bool isShortName = false;

        public static string GetSessionType(this SessionTypes session, bool isShortName = false)
        {
            SessionTypesExtension.isShortName = isShortName;

            switch (session)
            {
                default:
                    return SessionTypesExtension.Selector("UNK", "Unknown");
                case SessionTypes.Practice1:
                    return SessionTypesExtension.Selector("P1", "Practice 1");
                case SessionTypes.Practice2:
                    return SessionTypesExtension.Selector("P2", "Practice 2");
                case SessionTypes.Practice3:
                    return SessionTypesExtension.Selector("P3", "Practice 3");
                case SessionTypes.ShortPractice:
                    return SessionTypesExtension.Selector("SP", "Short Practice");
                case SessionTypes.Quallifying1:
                    return SessionTypesExtension.Selector("Q1", "Quallifying 1");
                case SessionTypes.Quallifying2:
                    return SessionTypesExtension.Selector("Q2", "Quallifying 2");
                case SessionTypes.Quallifying3:
                    return SessionTypesExtension.Selector("Q3", "Quallifying 3");
                case SessionTypes.ShortQuallifying:
                    return SessionTypesExtension.Selector("SQ", "Short Quallyfying");
                case SessionTypes.OneShotQuallifying:
                    return SessionTypesExtension.Selector("OSQ", "One-Shot Quallyfying");
                case SessionTypes.Race:
                    return SessionTypesExtension.Selector("RACE", "Race");
                case SessionTypes.TimeTrial:
                    return SessionTypesExtension.Selector("TT", "Time Trial");
                case SessionTypes.Race2:
                    return SessionTypesExtension.Selector("SPRINT", "Sprint race");
                case SessionTypes.Race3:
                    return SessionTypesExtension.Selector("UNK", "Unknown");
            }
        }

        private static string Selector(string shortName, string fullName)
        {
            if (SessionTypesExtension.isShortName) return shortName;
            else return fullName;
        }
    }
}
