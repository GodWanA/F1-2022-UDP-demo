using System;
using System.Collections.Generic;

namespace F1Telemetry.Helpers
{
    public static class Appendences
    {
        /// <summary>
        /// Type of the packet (In header's PocketID property).
        /// </summary>
        public enum PacketTypes
        {
            /// <summary>
            /// Contains all motion data for player’s car – only sent while player is in control
            /// </summary>
            CarMotion,
            /// <summary>
            /// Data about the session – track, time left
            /// </summary>
            Session,
            /// <summary>
            /// Data about all the lap times of cars in the session
            /// </summary>
            LapData,
            /// <summary>
            /// Various notable events that happen during a session
            /// </summary>
            Event,
            /// <summary>
            /// List of participants in the session, mostly relevant for multiplayer
            /// </summary>
            Participants,
            /// <summary>
            /// Packet detailing car setups for cars in the race
            /// </summary>
            CarSetups,
            /// <summary>
            /// Telemetry data for all cars
            /// </summary>
            CarTelemetry,
            /// <summary>
            /// Status data for all cars
            /// </summary>
            CarStatus,
            /// <summary>
            /// Final classification confirmation at the end of a race
            /// </summary>
            FinalClassification,
            /// <summary>
            /// Information about players in a multiplayer lobby
            /// </summary>
            LobbyInfo,
            /// <summary>
            /// Damage status for all cars
            /// </summary>
            CarDamage,
            /// <summary>
            /// Lap and tyre data for session
            /// </summary>
            SessionHistory
        }

        public enum Teams
        {
            MyTeam = 255,
            Unknown = -1,
            Mercedes = 0,
            Ferrari = 1,
            RedBullRacing = 2,
            Williams = 3,
            AstonMartin = 4,
            Alpine = 5,
            AlphaTauri = 6,
            Haas = 7,
            McLaren = 8,
            AlfaRomeo = 9,
            ArtGP19 = 42,
            Campos19 = 43,
            Carlin19 = 44,
            SauberJuniorCharouz19 = 45,
            Dams19 = 46,
            UniVirtuosi19 = 47,
            MPMotorsport19 = 48,
            Prema19 = 49,
            Trident19 = 50,
            Arden19 = 51,
            ArtGP20 = 70,
            Campos20 = 71,
            Carlin20 = 72,
            Charouz20 = 73,
            Dams20 = 74,
            UniVirtuosi20 = 75,
            MPMotorsport20 = 76,
            Prema20 = 77,
            Trident20 = 78,
            BWT20 = 79,
            Hitech20 = 80,
            Mercedes2020 = 85,
            Ferrari2020 = 86,
            RedBull2020 = 87,
            Williams2020 = 88,
            RacingPoint2020 = 89,
            Renault2020 = 90,
            AlphaTauri2020 = 91,
            Haas2020 = 92,
            McLaren2020 = 93,
            AlfaRomeo2020 = 94,
            AstonMartinDB11V12 = 95,
            AstonMartinVantageF1Edition = 96,
            AstonMartinVantageSafetyCar = 97,
            FerrariF8Tributo = 98,
            FerrariRoma = 99,
            McLaren720S = 100,
            McLarenArtura = 101,
            MercedesAMGGTBlackSeriesSafetyCar = 102,
            MercedesAMGGTRPro = 103,
            F1CustomTeam = 104,
            Prema21 = 106,
            UniVirtuosi21 = 107,
            Carlin21 = 108,
            Hitech21 = 109,
            ArtGP21 = 110,
            MPMotorsport21 = 111,
            Charouz21 = 112,
            Dams21 = 113,
            Campos21 = 114,
            BWT21 = 115,
            Trident21 = 116,
            MercedesAMGGTBlackSeries = 117,
        }

        public enum Tracks
        {
            Unknown = -1,
            Melbourne = 0,
            PaulRicard = 1,
            Shanghai = 2,
            Sakhir = 3,
            Catalunya = 4,
            Monaco = 5,
            Montreal = 6,
            Silverstone = 7,
            Hockenheim = 8,
            Hungaroring = 9,
            Spa = 10,
            Monza = 11,
            Singapore = 12,
            Suzuka = 13,
            AbuDhabi = 14,
            Texas = 15,
            Brazil = 16,
            Austria = 17,
            Sochi = 18,
            Mexico = 19,
            Baku = 20,
            SakhirShort = 21,
            SilverstoneShort = 22,
            TexasShor = 23,
            SuzukaShort = 24,
            Hanoi = 25,
            Zandvoort = 26,
            Imola = 27,
            Portimao = 28,
            Jeddah = 29,
            Miami = 30,
        }

        public enum Drivers
        {
            Unknown = -1,
            CarlosSainz = 0,
            DaniilKvyat = 1,
            DanielRicciardo = 2,
            FernandoAlonso = 3,
            FelipeMassa = 4,
            KimiRäikkönen = 6,
            LewisHamilton = 7,
            MaxVerstappen = 9,
            NicoHulkenburg = 10,
            KevinMagnussen = 11,
            RomainGrosjean = 12,
            SebastianVettel = 13,
            SergioPerez = 14,
            ValtteriBottas = 15,
            EstebanOcon = 17,
            LanceStroll = 19,
            ArronBarnes = 20,
            MartinGiles = 21,
            AlexMurray = 22,
            LucasRoth = 23,
            IgorCorreia = 24,
            SophieLevasseur = 25,
            JonasSchiffer = 26,
            AlainForest = 27,
            JayLetourneau = 28,
            EstoSaari = 29,
            YasarAtiyeh = 30,
            CallistoCalabresi = 31,
            NaotaIzum = 32,
            HowardClarke = 33,
            WilheimKaufmann = 34,
            MarieLaursen = 35,
            FlavioNieves = 36,
            PeterBelousov = 37,
            KlimekMichalski = 38,
            SantiagoMoreno = 39,
            BenjaminCoppens = 40,
            NoahVisser = 41,
            GertWaldmuller = 42,
            JulianQuesada = 43,
            DanielJones = 44,
            ArtemMarkelov = 45,
            TadasukeMakino = 46,
            SeanGelael = 47,
            NyckDeVries = 48,
            JackAitken = 49,
            GeorgeRussell = 50,
            MaximilianGünther = 51,
            NireiFukuzumi = 52,
            LucaGhiotto = 53,
            LandoNorris = 54,
            SérgioSetteCâmara = 55,
            LouisDelétraz = 56,
            AntonioFuoco = 57,
            CharlesLeclerc = 58,
            PierreGasly = 59,
            BrandonHartley = 60,
            SergeiSirotkin = 61,
            AlexanderAlbon = 62,
            NicholasLatifi = 63,
            DorianBoccolacci = 64,
            NikoKari = 65,
            RobertoMerhi = 66,
            ArjunMaini = 67,
            AlessioLorandi = 68,
            RubenMeijer = 69,
            RashidNair = 70,
            JackTremblay = 71,
            DevonButler = 72,
            LukasWeber = 73,
            AntonioGiovinazzi = 74,
            RobertKubica = 75,
            AlainProst = 76,
            AyrtonSenna = 77,
            NobuharuMatsushita = 78,
            NikitaMazepin = 79,
            GuanyaZhou = 80,
            MickSchumacher = 81,
            CallumIlott = 82,
            JuanManuelCorrea = 83,
            JordanKing = 84,
            MahaveerRaghunathan = 85,
            TatianaCalderon = 86,
            AnthoineHubert = 87,
            GuilianoAlesi = 88,
            RalphBoschung = 89,
            MichaelSchumacher = 90,
            DanTicktum = 91,
            MarcusArmstrong = 92,
            ChristianLundgaard = 93,
            YukiTsunoda = 94,
            JehanDaruvala = 95,
            GulhermeSamaia = 96,
            PedroPiquet = 97,
            FelipeDrugovich = 98,
            RobertShwartzman = 99,
            RoyNissany = 100,
            MarinoSato = 101,
            AidanJackson = 102,
            CasperAkkerman = 103,
            JensonButton = 109,
            DavidCoulthard = 110,
            NicoRosberg = 111,
            OscarPiastri = 112,
            LiamLawson = 113,
            JuriVips = 114,
            TheoPourchaire = 115,
            RichardVerschoor = 116,
            LirimZendeli = 117,
            DavidBeckmann = 118,
            GianlucaPetecof = 119,
            MatteoNannini = 120,
            AlessioDeledda = 121,
            BentViscaal = 122,
            EnzoFittipaldi = 123,
            MarkWebber = 125,
            JacquesVilleneuve = 126,
        }

        public enum ButtonFlags
        {
            CrossOrA = 0x00000001,
            TriangleOrY = 0x00000002,
            CircleOrB = 0x00000004,
            SquareOrX = 0x00000008,
            DpadLeft = 0x00000010,
            DpadRight = 0x00000020,
            DpadUp = 0x00000040,
            DpadDown = 0x00000080,
            OptionsOrMenu = 0x00000100,
            L1orLB = 0x00000200,
            R1orRB = 0x00000400,
            L2orLT = 0x00000800,
            R2orRT = 0x00001000,
            LeftStickClick = 0x00002000,
            RightStickClick = 0x00004000,
            RightStickLeft = 0x00008000,
            RightStickRight = 0x00010000,
            RightStickUp = 0x00020000,
            UDPAction1 = 0x00100000,
            UDPAction2 = 0x00200000,
            UDPAction3 = 0x00400000,
            UDPAction4 = 0x00800000,
            UDPAction5 = 0x01000000,
            UDPAction6 = 0x02000000,
            UDPAction7 = 0x04000000,
            UDPAction8 = 0x08000000,
            UDPAction9 = 0x10000000,
            UDPAction10 = 0x20000000,
            UDPAction11 = 0x40000000,
            // UDPAction12 = 0x80000000, - Currently can't cast to enum value
        }

        public static List<ButtonFlags> KeyChecker(UInt32 uint32b)
        {
            List<ButtonFlags> b = new List<ButtonFlags>();

            Appendences.HasKey(uint32b, ButtonFlags.CrossOrA, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.CircleOrB, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.DpadDown, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.DpadLeft, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.DpadRight, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.DpadUp, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.L1orLB, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.L2orLT, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.LeftStickClick, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.OptionsOrMenu, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.R1orRB, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.R2orRT, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.RightStickClick, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.RightStickLeft, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.RightStickRight, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.RightStickUp, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.SquareOrX, ref b);
            Appendences.HasKey(uint32b, ButtonFlags.TriangleOrY, ref b);

            return b;
        }

        private static void HasKey(UInt32 uint32b, ButtonFlags key, ref List<ButtonFlags> b)
        {
            if (((ButtonFlags)uint32b).HasFlag(key)) b.Add(key);
        }

        public static List<LapValid> LapValidChecker(byte val)
        {
            List<LapValid> b = new List<LapValid>();

            Appendences.IsValid(val, LapValid.FullLapValid, ref b);
            Appendences.IsValid(val, LapValid.Sector1Valid, ref b);
            Appendences.IsValid(val, LapValid.Sector2Valid, ref b);
            Appendences.IsValid(val, LapValid.Sector3Valid, ref b);

            return b;
        }

        private static void IsValid(byte uint32b, LapValid key, ref List<LapValid> b)
        {
            if (((LapValid)uint32b).HasFlag(key)) b.Add(key);
        }

        public enum LapValid
        {
            FullLapValid = 0x01,
            Sector1Valid = 0x02,
            Sector2Valid = 0x04,
            Sector3Valid = 0x08,
        }

        public enum SurfaceTypes
        {
            Tarmac = 0,
            Rumblestrip = 1,
            Concrete = 2,
            Rock = 3,
            Gravel = 4,
            Mud = 5,
            Sand = 6,
            Grass = 7,
            Water = 8,
            Cobblestone = 9,
            Metal = 10,
            Ridged = 11,
        }

        public enum Nationalities
        {
            Unknown = 0,
            American = 1,
            Argentina = 2,
            Australia = 3,
            Austria = 4,
            Azerbaijan = 5,
            Bahrain = 6,
            Belgium = 7,
            Bolivia = 8,
            Brazil = 9,
            UnitedKingdom = 10,
            Bulgaria = 11,
            Cameroon = 12,
            Canada = 13,
            Chile = 14,
            China = 15,
            Colombia = 16,
            CostaRica = 17,
            Croatia = 18,
            Cyprus = 19,
            Czech = 20,
            Denmark = 21,
            Netherland = 22,
            Ecuador = 23,
            England = 24,
            UnitedArabEmirates = 25,
            Estonia = 26,
            Finland = 27,
            France = 28,
            Germany = 29,
            Ghana = 30,
            Greece = 31,
            Guatemala = 32,
            Honduras = 33,
            HongKong = 34,
            Hungary = 35,
            Iceland = 36,
            India = 37,
            Indonesia = 38,
            Ireland = 39,
            Israel = 40,
            Italy = 41,
            Jamaica = 42,
            Japan = 43,
            Jordan = 44,
            Kuwait = 45,
            Latvia = 46,
            Lebanon = 47,
            Lithuania = 48,
            Luxembourg = 49,
            Malaysia = 50,
            Malta = 51,
            Mexico = 52,
            Monaco = 53,
            NewZealand = 54,
            Nicaragua = 55,
            NorthernIreland = 56,
            Norway = 57,
            Oman = 58,
            Pakistan = 59,
            Panama = 60,
            Paraguay = 61,
            Peru = 62,
            Poland = 63,
            Portugal = 64,
            Qatar = 65,
            Romania = 66,
            RussianFederation = 67,
            ElSalvador = 68,
            SaudiArabia = 69,
            Scotland = 70,
            Serbia = 71,
            Singapore = 72,
            Slovakia = 73,
            Slovenia = 74,
            SouthKorea = 75,
            SouthAfrica = 76,
            Spain = 77,
            Sweden = 78,
            Switzerland = 79,
            Thailand = 80,
            Turkey = 81,
            Uruguay = 82,
            Ukraine = 83,
            Venezuela = 84,
            Barbados = 85,
            Wales = 86,
            VietNam = 87,
        }

        public enum PenalytyTypes
        {
            DriveThrough,
            StopGo,
            GridPenalty,
            PenaltyReminder,
            TimePenalty,
            Warning,
            Disqualified,
            RemovedFromFormationLap,
            ParkedTooLongTimer,
            TyreRegulations,
            ThisLapInvalidated,
            ThisAndNextLapInvalidated,
            ThisLapInvalidatedWithoutReason,
            ThisAndNextLapInvalidatedWithoutReason,
            ThisAndPreviousLapInvalidated,
            ThisAndPreviousLapInvalidatedWithoutReason,
            Retired,
            BlackFlagTimer,
        }

        public enum InfringementTypes
        {
            Unknown = -1,
            BlockingBySlowDriving = 0,
            BlockingByWrongWayDriving = 1,
            ReversingOffTheStartLine = 2,
            BigCollision = 3,
            SmallCollision = 4,
            CollisionFailedToHandBackPositionSingle = 5,
            CollisionFailedToHandBackPositionMultiple = 6,
            CornerCuttingGainedTime = 7,
            CornerCuttingOvertakeSingle = 8,
            CornerCuttingOvertakeMultiple = 9,
            CrossedPitExitLane = 10,
            IgnoringBlueFlags = 11,
            IgnoringYellowFlags = 12,
            IgnoringDriveThrough = 13,
            TooManyDriveThroughs = 14,
            DriveThroughReminderServeWithinNLaps = 15,
            DriveThroughReminderServeThisLap = 16,
            PitLaneSpeeding = 17,
            ParkedForTooLong = 18,
            IgnoringTyreRegulations = 19,
            TooManyPenalties = 20,
            MultipleWarnings = 21,
            ApproachingDisqualification = 22,
            TyreRegulationsSelectSingle = 23,
            TyreRegulationsSelectMultiple = 24,
            LapInvalidatedCornerCutting = 25,
            LapInvalidatedRunningWide = 26,
            CornerCuttingRanWideGainedTimeMinor = 27,
            CornerCuttingRanWideGainedTimeSignificant = 28,
            CornerCuttingRanWideGainedTimeExtreme = 29,
            LapInvalidatedWallRiding = 30,
            LapInvalidatedFlashbackUsed = 31,
            LapInvalidatedResetToTrack = 32,
            BlockingThePitlane = 33,
            JumpStart = 34,
            SafetyCarToCarCollision = 35,
            SafetyCarIllegalOvertake = 36,
            SafetyCarExceedingAllowedPace = 37,
            VirtualSafetyCarExceedingAllowedPace = 38,
            FormationLapBelowAllowedSpeed = 39,
            FormationLapParking = 40,
            RetiredMechanicalFailure = 41,
            RetiredTerminallyDamaged = 42,
            SafetyCarFallingTooFarBack = 43,
            BlackFlagTimer = 44,
            UnservedStopGoPenalty = 45,
            UnservedDriveThroughPenalty = 46,
            EngineComponentChange = 47,
            GearboxChange = 48,
            ParcFerméChange = 49,
            LeagueGridPenalty = 50,
            RetryPenalty = 51,
            IllegalTimeGain = 52,
            MandatoryPitstop = 53,
            AttributeAssigned = 54,
        }

        public enum Gears
        {
            Gear_R = -1,
            Gear_N = 0,
            Gear_1 = 1,
            Gear_2 = 2,
            Gear_3 = 3,
            Gear_4 = 4,
            Gear_5 = 5,
            Gear_6 = 6,
            Gear_7 = 7,
            Gear_8 = 8,
        }

        public enum Flags
        {
            InvalidOrUnknown = -1,
            None = 0,
            Green = 1,
            Blue = 2,
            Yellow = 3,
            Red = 4,
        }

        public enum ResultSatuses
        {
            Unknown = -1,
            Invalid = 0,
            Inactive = 1,
            Active = 2,
            Finished = 3,
            DidNotFinish = 4,
            Disqualified = 5,
            NotClassiFied = 6,
            Retired = 7,
        }


        public enum TyreCompounds
        {
            Unknown = -1,
            HyperSoft,
            UltraSoft,
            SuperSoft,
            Soft,
            Medium,
            Hard,
            SuperHard,
            Inter,
            Wet,
            ClassicDry,
            ClassicWet,
            F2SuperSoft,
            F2Soft,
            F2Medium,
            F2Hard,
            F2Wet,
            C5,
            C4,
            C3,
            C2,
            C1,
        }

        public enum BreakingAssists
        {
            Unknown = -1,
            off = 0,
            Low = 1,
            Medium = 2,
            High = 3,
        }

        public enum ForecastAccuracies
        {
            Perfect,
            Approximate,
        }

        public enum WeatherTypes
        {
            Unknown = -1,
            Clear,
            LitghtCloud,
            Overcast,
            LightRain,
            HeavyRain,
            Storm,
        }

        public enum SessionTypes
        {
            Unknown,
            Practice1,
            Practice2,
            Practice3,
            ShortPractice,
            Quallifying1,
            Quallifying2,
            Quallifying3,
            ShortQuallifying,
            OneShotQuallifying,
            Race,
            Race2,
            TimeTrial,
            Race3,
        }

        public enum Formulas
        {
            F1Modern,
            F1Classic,
            F2,
            F1Generic,
            // 2022:
            Beta,
            Supercars,
            Esport,
            F2_2021,
        }

        public enum SafetyCarStatuses
        {
            NoSafetyCar,
            FullSafetyCar,
            VirtualSafetyCar,
            FormationLap,
        }

        public enum GearboxAssists
        {
            Unknown = -1,
            Manual = 1,
            ManualAndSuggestedGear = 3,
            auto = 3,
        }

        public enum RacingLineSatuses
        {
            Off,
            CornersOnly,
            Full,
        }

        public enum RacingLineTypes
        {
            Line_2D,
            Line_3D
        }

        public enum PitStatuses
        {
            Unknown = -1,
            None = 0,
            Pitting = 1,
            InPitArea = 2,
        }

        public enum Sectors
        {
            Unknown = -1,
            Sector1 = 0,
            Sector2 = 1,
            Sector3 = 2,
        }

        public enum DriverSatuses
        {
            Unknown = -1,
            InGarage = 0,
            FlyingLap = 1,
            InLap = 2,
            OutLap = 3,
            OnTrack = 4,
        }

        public enum EventTypes
        {
            Unknown,
            SessionStarted,
            SessionEnded,
            FastestLap,
            Retirement,
            DRSenabled,
            DRSdisabled,
            TeamMateInPits,
            ChequeredFlag,
            RaceWinner,
            PenaltyIssued,
            SpeedTrapTriggered,
            StartLights,
            LightsOut,
            DriveThroughServed,
            StopGoServed,
            Flashback,
            ButtonStatus,

            // custom events:
            Warning = 300,
            PitStop = 301,
        }

        public enum TelemetrySettings
        {
            Restricted,
            Public,
        }

        public enum ERSModes
        {
            None,
            Low,
            Medium,
            High,
            Overtake,
            Hotlap,
        }

        public enum TractionControlSettings
        {
            Off,
            Medium,
            Full,
        }

        public enum FuelMixis
        {
            Lean,
            Standard,
            Rich,
            Max,
        }

        public enum DRSStatuses
        {
            Unknown = -1,
            NotAllowed = 0,
            Allowed = 1,
        }

        public enum ReadyStatuses
        {
            NotReady,
            Ready,
            Spectating,
        }

        public enum TemperatureChanges
        {
            Up,
            Down,
            NoChange,
        }

        public enum MFDPanels
        {
            CarSetup = 0,
            Pits = 1,
            Demage = 2,
            Engine = 3,
            Temperatures = 4,
            Closed = 255,
        }

        public enum GameModes
        {
            EventMode = 0,
            GrandPrix = 3,
            TimeTrial = 5,
            Splitscreen = 6,
            OnlineCustom = 7,
            OnlineLeague = 8,
            CareerInvitational = 11,
            ChampionshipInvitational = 12,
            Championship = 13,
            OnlineChampionship = 14,
            OnlineWeekly_Event = 15,
            Career22 = 19,
            Career22Online = 20,
            Benchmark = 127,
        }

        public enum Rulesets
        {
            PracticeAndQualifying = 0,
            Race = 1,
            TimeTrial = 2,
            TimeAttack = 4,
            CheckpointChallenge = 6,
            Autocross = 8,
            Drift = 9,
            AverageSpeedZone = 10,
            RivalDuel = 11,
        }

        public enum SessionLengths
        {
            None = 0,
            VeryShort = 2,
            Short = 3,
            Medium = 4,
            MediumLong = 5,
            Long = 6,
            Full = 7,
        }

        internal static TyreCompounds SetVisualTyre(byte val)
        {
            // F1 visual (can be different from actual compound)
            // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            // F1 Classic – same as above
            // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
            // 21 = medium , 22 = hard
            switch (val)
            {
                case 15:
                    return TyreCompounds.F2Wet;
                case 19:
                    return TyreCompounds.F2SuperSoft;
                case 20:
                    return TyreCompounds.F2Soft;
                case 21:
                    return TyreCompounds.F2Medium;
                case 22:
                    return TyreCompounds.F2Hard;

                default:
                    return (TyreCompounds)val;
            }
        }
    }
}
