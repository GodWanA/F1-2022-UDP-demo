using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Helpers
{
    public static class NationalitiesExtension
    {
        public enum ShortMode
        {
            TwoLetterMode,
            ThreeLetterMode,
        }

        public static string CountryCode(this Nationalities obj, ShortMode mode = ShortMode.ThreeLetterMode)
        {
            switch (obj)
            {
                default:
                    if (mode == ShortMode.TwoLetterMode) return "--";
                    else return "---";
                case Nationalities.American:
                    if (mode == ShortMode.TwoLetterMode) return "US";
                    else return "USA";
                case Nationalities.Argentina:
                    if (mode == ShortMode.TwoLetterMode) return "AR";
                    else return "ARG";
                case Nationalities.Australia:
                    if (mode == ShortMode.TwoLetterMode) return "AU";
                    else return "AUS";
                case Nationalities.Austria:
                    if (mode == ShortMode.TwoLetterMode) return "AT";
                    else return "AUT";
                case Nationalities.Azerbaijan:
                    if (mode == ShortMode.TwoLetterMode) return "AZ";
                    else return "AZE";
                case Nationalities.Bahrain:
                    if (mode == ShortMode.TwoLetterMode) return "BH";
                    else return "BHR";
                case Nationalities.Belgium:
                    if (mode == ShortMode.TwoLetterMode) return "BE";
                    else return "BEL";
                case Nationalities.Bolivia:
                    if (mode == ShortMode.TwoLetterMode) return "BO";
                    else return "BOL";
                case Nationalities.Brazil:
                    if (mode == ShortMode.TwoLetterMode) return "BR";
                    else return "BRA";
                case Nationalities.UnitedKingdom:
                    if (mode == ShortMode.TwoLetterMode) return "GB";
                    else return "GBR";
                case Nationalities.Bulgaria:
                    if (mode == ShortMode.TwoLetterMode) return "BG";
                    else return "BGR";
                case Nationalities.Cameroon:
                    if (mode == ShortMode.TwoLetterMode) return "CM";
                    else return "CMR";
                case Nationalities.Canada:
                    if (mode == ShortMode.TwoLetterMode) return "CA";
                    else return "CAN";
                case Nationalities.Chile:
                    if (mode == ShortMode.TwoLetterMode) return "CL";
                    else return "CHL";
                case Nationalities.China:
                    if (mode == ShortMode.TwoLetterMode) return "CN";
                    else return "CHN";
                case Nationalities.Colombia:
                    if (mode == ShortMode.TwoLetterMode) return "CO";
                    else return "COL";
                case Nationalities.CostaRica:
                    if (mode == ShortMode.TwoLetterMode) return "CR";
                    else return "CRI";
                case Nationalities.Croatia:
                    if (mode == ShortMode.TwoLetterMode) return "HR";
                    else return "HRV";
                case Nationalities.Cyprus:
                    if (mode == ShortMode.TwoLetterMode) return "CY";
                    else return "CYP";
                case Nationalities.Czech:
                    if (mode == ShortMode.TwoLetterMode) return "CZ";
                    else return "CZE";
                case Nationalities.Denmark:
                    if (mode == ShortMode.TwoLetterMode) return "DK";
                    else return "DNK";
                case Nationalities.Netherland:
                    if (mode == ShortMode.TwoLetterMode) return "NL";
                    else return "NLD";
                case Nationalities.Ecuador:
                    if (mode == ShortMode.TwoLetterMode) return "EC";
                    else return "ECU";
                case Nationalities.England:
                    if (mode == ShortMode.TwoLetterMode) return "GB-ENG";
                    else return "GB-ENG";
                case Nationalities.UnitedArabEmirates:
                    if (mode == ShortMode.TwoLetterMode) return "AE";
                    else return "ARE";
                case Nationalities.Estonia:
                    if (mode == ShortMode.TwoLetterMode) return "EE";
                    else return "EST";
                case Nationalities.Finland:
                    if (mode == ShortMode.TwoLetterMode) return "FI";
                    else return "FIN";
                case Nationalities.France:
                    if (mode == ShortMode.TwoLetterMode) return "FR";
                    else return "FRA";
                case Nationalities.Germany:
                    if (mode == ShortMode.TwoLetterMode) return "DE";
                    else return "DEU";
                case Nationalities.Ghana:
                    if (mode == ShortMode.TwoLetterMode) return "GH";
                    else return "GHA";
                case Nationalities.Greece:
                    if (mode == ShortMode.TwoLetterMode) return "GR";
                    else return "GRC";
                case Nationalities.Guatemala:
                    if (mode == ShortMode.TwoLetterMode) return "GT";
                    else return "GTM";
                case Nationalities.Honduras:
                    if (mode == ShortMode.TwoLetterMode) return "HN";
                    else return "HND";
                case Nationalities.HongKong:
                    if (mode == ShortMode.TwoLetterMode) return "HK";
                    else return "HKG";
                case Nationalities.Hungary:
                    if (mode == ShortMode.TwoLetterMode) return "HU";
                    else return "HUN";
                case Nationalities.Iceland:
                    if (mode == ShortMode.TwoLetterMode) return "IS";
                    else return "ISL";
                case Nationalities.India:
                    if (mode == ShortMode.TwoLetterMode) return "IN";
                    else return "IND";
                case Nationalities.Indonesia:
                    if (mode == ShortMode.TwoLetterMode) return "ID";
                    else return "IDN";
                case Nationalities.Ireland:
                    if (mode == ShortMode.TwoLetterMode) return "IE";
                    else return "IRL";
                case Nationalities.Israel:
                    if (mode == ShortMode.TwoLetterMode) return "IL";
                    else return "ISR";
                case Nationalities.Italy:
                    if (mode == ShortMode.TwoLetterMode) return "IT";
                    else return "ITA";
                case Nationalities.Jamaica:
                    if (mode == ShortMode.TwoLetterMode) return "JM";
                    else return "JAM";
                case Nationalities.Japan:
                    if (mode == ShortMode.TwoLetterMode) return "JP";
                    else return "JPN";
                case Nationalities.Jordan:
                    if (mode == ShortMode.TwoLetterMode) return "JO";
                    else return "JOR";
                case Nationalities.Kuwait:
                    if (mode == ShortMode.TwoLetterMode) return "KW";
                    else return "KWT";
                case Nationalities.Latvia:
                    if (mode == ShortMode.TwoLetterMode) return "LV";
                    else return "LVA";
                case Nationalities.Lebanon:
                    if (mode == ShortMode.TwoLetterMode) return "LB";
                    else return "LBN";
                case Nationalities.Lithuania:
                    if (mode == ShortMode.TwoLetterMode) return "LT";
                    else return "LTU";
                case Nationalities.Luxembourg:
                    if (mode == ShortMode.TwoLetterMode) return "LU";
                    else return "LUX";
                case Nationalities.Malaysia:
                    if (mode == ShortMode.TwoLetterMode) return "MY";
                    else return "MYS";
                case Nationalities.Malta:
                    if (mode == ShortMode.TwoLetterMode) return "MT";
                    else return "MLT";
                case Nationalities.Mexico:
                    if (mode == ShortMode.TwoLetterMode) return "MX";
                    else return "MEX";
                case Nationalities.Monaco:
                    if (mode == ShortMode.TwoLetterMode) return "MC";
                    else return "MCO";
                case Nationalities.NewZealand:
                    if (mode == ShortMode.TwoLetterMode) return "NZ";
                    else return "NZL";
                case Nationalities.Nicaragua:
                    if (mode == ShortMode.TwoLetterMode) return "NI";
                    else return "NIC";
                case Nationalities.NorthernIreland:
                    if (mode == ShortMode.TwoLetterMode) return "GB-NIR";
                    else return "GB-NIR";
                case Nationalities.Norway:
                    if (mode == ShortMode.TwoLetterMode) return "NO";
                    else return "NOR";
                case Nationalities.Oman:
                    if (mode == ShortMode.TwoLetterMode) return "OM";
                    else return "OMN";
                case Nationalities.Pakistan:
                    if (mode == ShortMode.TwoLetterMode) return "PK";
                    else return "PAK";
                case Nationalities.Panama:
                    if (mode == ShortMode.TwoLetterMode) return "PA";
                    else return "PAN";
                case Nationalities.Paraguay:
                    if (mode == ShortMode.TwoLetterMode) return "PY";
                    else return "PRY";
                case Nationalities.Peru:
                    if (mode == ShortMode.TwoLetterMode) return "PE";
                    else return "PER";
                case Nationalities.Poland:
                    if (mode == ShortMode.TwoLetterMode) return "PL";
                    else return "POL";
                case Nationalities.Portugal:
                    if (mode == ShortMode.TwoLetterMode) return "PT";
                    else return "PRT";
                case Nationalities.Qatar:
                    if (mode == ShortMode.TwoLetterMode) return "QA";
                    else return "QAT";
                case Nationalities.Romania:
                    if (mode == ShortMode.TwoLetterMode) return "RO";
                    else return "ROU";
                case Nationalities.RussianFederation:
                    if (mode == ShortMode.TwoLetterMode) return "RU";
                    else return "RUS";
                case Nationalities.ElSalvador:
                    if (mode == ShortMode.TwoLetterMode) return "SV";
                    else return "SLV";
                case Nationalities.SaudiArabia:
                    if (mode == ShortMode.TwoLetterMode) return "SA";
                    else return "SAU";
                case Nationalities.Scotland:
                    if (mode == ShortMode.TwoLetterMode) return "GB-SCT";
                    else return "GB-SCT";
                case Nationalities.Serbia:
                    if (mode == ShortMode.TwoLetterMode) return "RS";
                    else return "SRB";
                case Nationalities.Singapore:
                    if (mode == ShortMode.TwoLetterMode) return "SG";
                    else return "SGP";
                case Nationalities.Slovakia:
                    if (mode == ShortMode.TwoLetterMode) return "SK";
                    else return "SVK";
                case Nationalities.Slovenia:
                    if (mode == ShortMode.TwoLetterMode) return "SI";
                    else return "SVN";
                case Nationalities.SouthKorea:
                    if (mode == ShortMode.TwoLetterMode) return "KR";
                    else return "KOR";
                case Nationalities.SouthAfrica:
                    if (mode == ShortMode.TwoLetterMode) return "ZA";
                    else return "ZAF";
                case Nationalities.Spain:
                    if (mode == ShortMode.TwoLetterMode) return "ES";
                    else return "ESP";
                case Nationalities.Sweden:
                    if (mode == ShortMode.TwoLetterMode) return "SE";
                    else return "SWE";
                case Nationalities.Switzerland:
                    if (mode == ShortMode.TwoLetterMode) return "CH";
                    else return "CHE";
                case Nationalities.Thailand:
                    if (mode == ShortMode.TwoLetterMode) return "TH";
                    else return "THA";
                case Nationalities.Turkey:
                    if (mode == ShortMode.TwoLetterMode) return "TR";
                    else return "TUR";
                case Nationalities.Uruguay:
                    if (mode == ShortMode.TwoLetterMode) return "UY";
                    else return "URY";
                case Nationalities.Ukraine:
                    if (mode == ShortMode.TwoLetterMode) return "UA";
                    else return "UKR";
                case Nationalities.Venezuela:
                    if (mode == ShortMode.TwoLetterMode) return "VE";
                    else return "VEN";
                case Nationalities.Barbados:
                    if (mode == ShortMode.TwoLetterMode) return "BB";
                    else return "BRB";
                case Nationalities.Wales:
                    if (mode == ShortMode.TwoLetterMode) return "GB-WLS";
                    else return "GB-WLS";
                case Nationalities.VietNam:
                    if (mode == ShortMode.TwoLetterMode) return "VN";
                    else return "VNM";
            }
        }

        public static string EnumToString(string s)
        {
            var r = new Regex(
                            @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])",
                            RegexOptions.IgnorePatternWhitespace
                        );

            return r.Replace(s, " ");
        }
    }
}
