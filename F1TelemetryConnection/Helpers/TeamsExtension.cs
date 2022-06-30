using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Helpers
{
    public static class TeamsExtension
    {
        public static string GetTeamName(this Teams team, int year = 2021)
        {
            switch (team)
            {
                default:
                    return "Unknown Team";

                // F1:

                case Teams.MyTeam:
                    return "My Team";
                case Teams.Mercedes:
                case Teams.Mercedes2020:
                    return "Mercedes";
                case Teams.Ferrari:
                case Teams.Ferrari2020:
                    return "Ferrari";
                case Teams.RedBullRacing:
                case Teams.RedBull2020:
                    if (year < 2019) return "Red Bull Racing Tag Heuer";
                    else return "Red Bull Racing - Honda";
                case Teams.Williams:
                case Teams.Williams2020:
                    return "Williams - Mercedes";
                case Teams.AstonMartin:
                case Teams.RacingPoint2020:
                    if (year < 2019) return "Force India - Mercedes";
                    else if (year < 2021) return "Racing Point BWT - Mercedes";
                    else return "Aston Martin - Mercedes";
                case Teams.Alpine:
                case Teams.Renault2020:
                    if (year < 2021) return "Renault";
                    else return "Alpine - Renault";
                case Teams.AlphaTauri:
                case Teams.AlphaTauri2020:
                    if (year < 2017) return "Torro Rosso Ferrari";
                    else if (year < 2018) return "Torro Rosso";
                    else if (year < 2020) return "Torro Rosso - Honda";
                    else return "Alpha Tauri - Honda";
                case Teams.Haas:
                case Teams.Haas2020:
                    return "Haas - Ferrari";
                case Teams.McLaren:
                case Teams.McLaren2020:
                    if (year < 2019) return "McLaren - Honda";
                    else if (year < 2021) return "McLaren - Renault";
                    else return "McLaren - Mercedes";
                case Teams.AlfaRomeo:
                case Teams.AlfaRomeo2020:
                    if (year < 2018) return "Sauber - Ferrari";
                    else return "Alfa Romeo Racing - Ferrari";

                // F2:

                case Teams.ArtGP19:
                case Teams.ArtGP20:
                case Teams.ArtGP21:
                    return "ART Grand Prix";
                case Teams.Campos19:
                case Teams.Campos20:
                case Teams.Campos21:
                    return "Campos Racing";
                case Teams.Carlin19:
                case Teams.Carlin20:
                case Teams.Carlin21:
                    return "Carlin";
                case Teams.SauberJuniorCharouz19:
                case Teams.Charouz20:
                case Teams.Charouz21:
                    return "Charouz Racing System";
                case Teams.Dams19:
                case Teams.Dams20:
                case Teams.Dams21:
                    return "DAMS";
                case Teams.UniVirtuosi19:
                case Teams.UniVirtuosi20:
                case Teams.UniVirtuosi21:
                    return "Virtuosi Racing";
                case Teams.MPMotorsport19:
                case Teams.MPMotorsport20:
                case Teams.MPMotorsport21:
                    return "MP Motorsport";
                case Teams.Prema19:
                case Teams.Prema20:
                case Teams.Prema21:
                    return "PREMA Racing";
                case Teams.Trident19:
                case Teams.Trident20:
                case Teams.Trident21:
                    return "Trident";
                case Teams.Arden19:
                    return "BWT Arden";
                case Teams.BWT20:
                case Teams.BWT21:
                    return "HWA RACELAB";
                case Teams.Hitech20:
                case Teams.Hitech21:
                    return "Hitech Grand Prix";
            }
        }
    }
}
