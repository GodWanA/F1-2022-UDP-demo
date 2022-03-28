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
            RobertSchwartzman = 99,
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
            Argentinean = 2,
            Australian = 3,
            Austrian = 4,
            Azerbaijani = 5,
            Bahraini = 6,
            Belgian = 7,
            Bolivian = 8,
            Brazilian = 9,
            British = 10,
            Bulgarian = 11,
            Cameroonian = 12,
            Canadian = 13,
            Chilean = 14,
            Chinese = 15,
            Colombian = 16,
            CostaRican = 17,
            Croatian = 18,
            Cypriot = 19,
            Czech = 20,
            Danish = 21,
            Dutch = 22,
            Ecuadorian = 23,
            English = 24,
            Emirian = 25,
            Estonian = 26,
            Finnish = 27,
            French = 28,
            German = 29,
            Ghanaian = 30,
            Greek = 31,
            Guatemalan = 32,
            Honduran = 33,
            HongKonger = 34,
            Hungarian = 35,
            Icelander = 36,
            Indian = 37,
            Indonesian = 38,
            Irish = 39,
            Israeli = 40,
            Italian = 41,
            Jamaican = 42,
            Japanese = 43,
            Jordanian = 44,
            Kuwaiti = 45,
            Latvian = 46,
            Lebanese = 47,
            Lithuanian = 48,
            Luxembourger = 49,
            Malaysian = 50,
            Maltese = 51,
            Mexican = 52,
            Monegasque = 53,
            NewZealander = 54,
            Nicaraguan = 55,
            NorthernIrish = 56,
            Norwegian = 57,
            Omani = 58,
            Pakistani = 59,
            Panamanian = 60,
            Paraguayan = 61,
            Peruvian = 62,
            Polish = 63,
            Portuguese = 64,
            Qatari = 65,
            Romanian = 66,
            Russian = 67,
            Salvadoran = 68,
            Saudi = 69,
            Scottish = 70,
            Serbian = 71,
            Singaporean = 72,
            Slovakian = 73,
            Slovenian = 74,
            SouthKorean = 75,
            SouthAfrican = 76,
            Spanish = 77,
            Swedish = 78,
            Swiss = 79,
            Thai = 80,
            Turkish = 81,
            Uruguayan = 82,
            Ukrainian = 83,
            Venezuelan = 84,
            Barbadian = 85,
            Welsh = 86,
            Vietnamese = 87,
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
            BlockingBySlowDriving,
            BlockingByWrongWayDriving,
            ReversingOffTheStartLine,
            BigCollision,
            SmallCollision,
            CollisionFailedToHandBackPositionSingle,
            CollisionFailedToHandBackPositionMultiple,
            CornerCuttingGainedTime,
            CornerCuttingOvertakeSingle,
            CornerCuttingOvertakeMultiple,
            CrossedPitExitLane,
            IgnoringBlueFlags,
            IgnoringYellowFlags,
            IgnoringDriveThrough,
            TooManyDriveThroughs,
            DriveThroughReminderServeWithinnLaps,
            DriveThroughReminderServeThisLap,
            PitLaneSpeeding,
            ParkedForTooLong,
            IgnoringTyreRegulations,
            TooManyPenalties,
            MultipleWarnings,
            ApproachingDisqualification,
            TyreRegulationsSelectSingle,
            TyreRegulationsSelectMultiple,
            LapInvalidatedCornerCutting,
            LapInvalidatedRunningWide,
            CornerCuttingRanWideGainedTimeMinor,
            CornerCuttingRanWideGainedTimeSignificant,
            CornerCuttingRanWideGainedTimeExtreme,
            LapInvalidatedWallRiding,
            LapInvalidatedFlashbackUsed,
            LapInvalidatedResetToTrack,
            BlockingThePitlane,
            JumpStart,
            SafetyCarToCarCollision,
            SafetyCarIllegalOvertake,
            SafetyCarExceedingAllowedPace,
            VirtualSafetyCarExceedingAllowedPace,
            FormationLapBelowAllowedSpeed,
            RetiredMechanicalFailure,
            RetiredTerminallyDamaged,
            SafetyCarFallingTooFarBack,
            BlackFlagTimer,
            UnservedStopGoPenalty,
            UnservedDriveThroughPenalty,
            EngineComponentChange,
            GearboxChange,
            LeagueGridPenalty,
            RetryPenalty,
            IllegalTimeGain,
            MandatoryPitstop,
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
    }
}
