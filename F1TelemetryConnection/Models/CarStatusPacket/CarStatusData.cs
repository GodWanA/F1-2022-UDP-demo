using F1Telemetry.Helpers;
using F1Telemetry.Models.CarDamagePacket;
using System.Collections.Generic;
using System.Linq;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarStatusPacket
{
    public class CarStatusData : ProtoModel
    {
        private Dictionary<string, byte> tyreWear;
        private Dictionary<string, byte> tyreDemage;
        private byte frontLeftWingDemage;
        private byte frontRightWingDemage;
        private byte rearWingDemage;
        private byte engineDemage;
        private byte gearboxDemage;
        private byte exhaustDemage;
        private bool drsFault;

        private static float ERSCapacity2021 = 4000000f;

        public CarStatusData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public TractionControlSettings TractionControl { get; private set; }
        public bool IsAntiLockBrakes { get; private set; }
        public FuelMixis FuelMix { get; private set; }
        public byte FrontBrakeBias { get; private set; }
        public bool IsPitLimiter { get; private set; }
        public float FuelInTank { get; private set; }
        public float FuelCapacity { get; private set; }
        public float FuelRemainingLaps { get; private set; }
        public ushort MaxRPM { get; private set; }
        public ushort IdleRPM { get; private set; }
        public byte MaxGears { get; private set; }
        public bool IsDRSAllowed { get; private set; }
        public ushort DRSActivationDistance { get; private set; }
        public TyreCompounds ActualTyreCompound { get; private set; }
        public byte TyresAgeLaps { get; private set; }
        public Flags VehicleFIAFlag { get; private set; }
        public float ERSStoreEnergy { get; private set; }
        public float ERSStoreEnergyPercent { get; private set; }
        public ERSModes ERSDeployMode { get; private set; }
        public float ERSHarvestedThisLapMGUK { get; private set; }
        public float ERSHarvestedThisLapMGUKPercent { get; private set; }
        public float ERSHarvestedThisLapMGUH { get; private set; }
        public float ERSHarvestedThisLapMGUHPercent { get; private set; }
        public float ERSDeployedThisLap { get; private set; }
        public float ERSDeployedThisLapPercent { get; private set; }
        public bool IsNetworkPaused { get; private set; }
        public TyreCompounds VisualTyreCompound { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            byte uint8;
            bool b;
            float f;
            ushort uint16;
            Dictionary<string, byte> d8;
            sbyte int8;

            //uint8 m_tractionControl;          // 0 (off) - 2 (high)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TractionControl = (TractionControlSettings)uint8;
            //uint8 m_antiLockBrakes;           // 0 (off) - 1 (on)
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAntiLockBrakes = b;
            //uint8 m_fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FuelMix = (FuelMixis)uint8;
            //uint8 m_frontBrakeBias;           // Front brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontBrakeBias = uint8;
            //uint8 m_pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitLimiter = b;
            //float m_fuelInTank;               // Current fuel mass
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelInTank = f;
            //float m_fuelCapacity;             // Fuel capacity
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelCapacity = f;
            //uint16 m_maxRPM;                   // Cars max RPM, point of rev limiter
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.MaxRPM = uint16;
            //uint16 m_idleRPM;                  // Cars idle RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.IdleRPM = uint16;
            //uint8 m_maxGears;                 // Maximum number of gears
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MaxGears = uint8;
            //uint8 m_drsAllowed;               // 0 = not allowed, 1 = allowed, -1 = unknown
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSAllowed = b;
            //uint8 m_tyresWear[4];             // Tyre wear percentage
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreWear = d8;
            //uint8 m_tyreCompound;             // Modern - 0 = hyper soft, 1 = ultra soft
            //                                  // 2 = super soft, 3 = soft, 4 = medium, 5 = hard
            //                                  // 6 = super hard, 7 = inter, 8 = wet
            //                                  // Classic - 0-6 = dry, 7-8 = wet
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ActualTyreCompound = (TyreCompounds)uint8;
            this.VisualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_tyresDamage[4];           // Tyre damage (percentage)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreDemage = d8;
            //uint8 m_frontLeftWingDamage;      // Front left wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontLeftWingDemage = uint8;
            //uint8 m_frontRightWingDamage;     // Front right wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontRightWingDemage = uint8;
            //uint8 m_rearWingDamage;           // Rear wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.rearWingDemage = uint8;
            //uint8 m_engineDamage;             // Engine damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.engineDemage = uint8;
            //uint8 m_gearBoxDamage;            // Gear box damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.gearboxDemage = uint8;
            //uint8 m_exhaustDamage;            // Exhaust damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.exhaustDemage = uint8;
            //int8 m_vehicleFiaFlags;          // -1 = invalid/unknown, 0 = none, 1 = green
            //                                 // 2 = blue, 3 = yellow, 4 = red
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.VehicleFIAFlag = (Flags)int8;
            //float m_ersStoreEnergy;           // ERS energy store in Joules
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSStoreEnergy = f;
            this.ERSStoreEnergyPercent = CarStatusData.CalculateERSPercent(f);
            //uint8 m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = low, 2 = medium
            //                                  // 3 = high, 4 = overtake, 5 = hotlap
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ERSDeployMode = (ERSModes)uint8;
            //float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUK = f;
            //this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUH = f;
            //this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersDeployedThisLap;       // ERS energy deployed this lap
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSDeployedThisLap = f;
            this.ERSDeployedThisLapPercent = CarStatusData.CalculateERSPercent(f);
        }

        protected override void Reader2019(byte[] array)
        {
            byte uint8;
            bool b;
            float f;
            ushort uint16;
            Dictionary<string, byte> d8;
            sbyte int8;

            //uint8 m_tractionControl;          // 0 (off) - 2 (high)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TractionControl = (TractionControlSettings)uint8;
            //uint8 m_antiLockBrakes;           // 0 (off) - 1 (on)
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAntiLockBrakes = b;
            //uint8 m_fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FuelMix = (FuelMixis)uint8;
            //uint8 m_frontBrakeBias;           // Front brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontBrakeBias = uint8;
            //uint8 m_pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitLimiter = b;
            //float m_fuelInTank;               // Current fuel mass
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelInTank = f;
            //float m_fuelCapacity;             // Fuel capacity
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelCapacity = f;
            //float m_fuelRemainingLaps;        // Fuel remaining in terms of laps (value on MFD)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelRemainingLaps = f;
            //uint16 m_maxRPM;                   // Cars max RPM, point of rev limiter
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.MaxRPM = uint16;
            //uint16 m_idleRPM;                  // Cars idle RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.IdleRPM = uint16;
            //uint8 m_maxGears;                 // Maximum number of gears
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MaxGears = uint8;
            //uint8 m_drsAllowed;               // 0 = not allowed, 1 = allowed, -1 = unknown
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSAllowed = b;
            //uint8 m_tyresWear[4];             // Tyre wear percentage
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreWear = d8;
            //uint8 m_actualTyreCompound;    // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
            //                               // 7 = inter, 8 = wet
            //                               // F1 Classic - 9 = dry, 10 = wet
            //                               // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
            //                               // 15 = wet
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ActualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_tyreVisualCompound;       // F1 visual (can be different from actual compound)
            //                                  // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            //                                  // F1 Classic – same as above
            //                                  // F2 – same as above
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.VisualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_tyresDamage[4];           // Tyre damage (percentage)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreDemage = d8;
            //uint8 m_frontLeftWingDamage;      // Front left wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontLeftWingDemage = uint8;
            //uint8 m_frontRightWingDamage;     // Front right wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontRightWingDemage = uint8;
            //uint8 m_rearWingDamage;           // Rear wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.rearWingDemage = uint8;
            //uint8 m_engineDamage;             // Engine damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.engineDemage = uint8;
            //uint8 m_gearBoxDamage;            // Gear box damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.gearboxDemage = uint8;
            //int8 m_vehicleFiaFlags;    // -1 = invalid/unknown, 0 = none, 1 = green
            //                           // 2 = blue, 3 = yellow, 4 = red
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.VehicleFIAFlag = (Flags)int8;
            //float m_ersStoreEnergy;           // ERS energy store in Joules
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSStoreEnergy = f;
            this.ERSStoreEnergyPercent = CarStatusData.CalculateERSPercent(f);
            //uint8 m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = low, 2 = medium
            //                                  // 3 = high, 4 = overtake, 5 = hotlap
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ERSDeployMode = (ERSModes)uint8;
            //float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUK = f;
            //this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUH = f;
            //this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersDeployedThisLap;       // ERS energy deployed this lap
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSDeployedThisLap = f;
            this.ERSDeployedThisLapPercent = CarStatusData.CalculateERSPercent(f);
        }

        protected override void Reader2020(byte[] array)
        {
            byte uint8;
            bool b;
            float f;
            ushort uint16;
            Dictionary<string, byte> d8;
            sbyte int8;

            //uint8 m_tractionControl;          // 0 (off) - 2 (high)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TractionControl = (TractionControlSettings)uint8;
            //uint8 m_antiLockBrakes;           // 0 (off) - 1 (on)
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAntiLockBrakes = b;
            //uint8 m_fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FuelMix = (FuelMixis)uint8;
            //uint8 m_frontBrakeBias;           // Front brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontBrakeBias = uint8;
            //uint8 m_pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitLimiter = b;
            //float m_fuelInTank;               // Current fuel mass
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelInTank = f;
            //float m_fuelCapacity;             // Fuel capacity
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelCapacity = f;
            //float m_fuelRemainingLaps;        // Fuel remaining in terms of laps (value on MFD)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelRemainingLaps = f;
            //uint16 m_maxRPM;                   // Cars max RPM, point of rev limiter
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.MaxRPM = uint16;
            //uint16 m_idleRPM;                  // Cars idle RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.IdleRPM = uint16;
            //uint8 m_maxGears;                 // Maximum number of gears
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MaxGears = uint8;
            //uint8 m_drsAllowed;               // 0 = not allowed, 1 = allowed, -1 = unknown
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSAllowed = b;
            //// Added in Beta3:
            //uint16 m_drsActivationDistance;    // 0 = DRS not available, non-zero - DRS will be available
            //                                   // in [X] metres
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.DRSActivationDistance = uint16;
            //uint8 m_tyresWear[4];             // Tyre wear percentage
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreWear = d8;
            //uint8 m_actualTyreCompound;     // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
            //                                // 7 = inter, 8 = wet
            //                                // F1 Classic - 9 = dry, 10 = wet
            //                                // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
            //                                // 15 = wet
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ActualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_visualTyreCompound;        // F1 visual (can be different from actual compound)
            //                                   // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            //                                   // F1 Classic – same as above
            //                                   // F2 – same as above
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.VisualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_tyresAgeLaps;             // Age in laps of the current set of tyres
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TyresAgeLaps = uint8;
            //uint8 m_tyresDamage[4];           // Tyre damage (percentage)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.tyreDemage = d8;
            //uint8 m_frontLeftWingDamage;      // Front left wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontLeftWingDemage = uint8;
            //uint8 m_frontRightWingDamage;     // Front right wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.frontRightWingDemage = uint8;
            //uint8 m_rearWingDamage;           // Rear wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.rearWingDemage = uint8;
            //// Added Beta 3:
            //uint8 m_drsFault;                 // Indicator for DRS fault, 0 = OK, 1 = fault
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.drsFault = b;
            //uint8 m_engineDamage;             // Engine damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.engineDemage = uint8;
            //uint8 m_gearBoxDamage;            // Gear box damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.gearboxDemage = uint8;
            //int8 m_vehicleFiaFlags;          // -1 = invalid/unknown, 0 = none, 1 = green
            //                                 // 2 = blue, 3 = yellow, 4 = red
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.VehicleFIAFlag = (Flags)int8;
            //float m_ersStoreEnergy;           // ERS energy store in Joules
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSStoreEnergy = f;
            this.ERSStoreEnergyPercent = CarStatusData.CalculateERSPercent(f);
            //uint8 m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = medium
            //                                  // 2 = overtake, 3 = hotlap
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetERS2020(uint8);
            //float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUK = f;
            //this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUH = f;
            //this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersDeployedThisLap;       // ERS energy deployed this lap
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSDeployedThisLap = f;
            this.ERSDeployedThisLapPercent = CarStatusData.CalculateERSPercent(f);
        }

        protected override void Reader2021(byte[] array)
        {
            byte uint8;
            bool b;
            float f;
            ushort uint16;
            sbyte int8;

            //uint8 m_tractionControl;          // Traction control - 0 = off, 1 = medium, 2 = full
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TractionControl = (TractionControlSettings)uint8;
            //uint8 m_antiLockBrakes;           // 0 (off) - 1 (on)
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsAntiLockBrakes = b;
            //uint8 m_fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FuelMix = (FuelMixis)uint8;
            //uint8 m_frontBrakeBias;           // Front brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontBrakeBias = uint8;
            //uint8 m_pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsPitLimiter = b;
            //float m_fuelInTank;               // Current fuel mass
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelInTank = f;
            //float m_fuelCapacity;             // Fuel capacity
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelCapacity = f;
            //float m_fuelRemainingLaps;        // Fuel remaining in terms of laps (value on MFD)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelRemainingLaps = f;
            //uint16 m_maxRPM;                   // Cars max RPM, point of rev limiter
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.MaxRPM = uint16;
            //uint16 m_idleRPM;                  // Cars idle RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.IdleRPM = uint16;
            //uint8 m_maxGears;                 // Maximum number of gears
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MaxGears = uint8;
            //uint8 m_drsAllowed;               // 0 = not allowed, 1 = allowed
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSAllowed = b;
            //uint16 m_drsActivationDistance;    // 0 = DRS not available, non-zero - DRS will be available
            //                                   // in [X] metres
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.DRSActivationDistance = uint16;
            //uint8 m_actualTyreCompound;    // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
            //                               // 7 = inter, 8 = wet
            //                               // F1 Classic - 9 = dry, 10 = wet
            //                               // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
            //                               // 15 = wet
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.ActualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_visualTyreCompound;       // F1 visual (can be different from actual compound)
            //                                  // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            //                                  // F1 Classic – same as above
            //                                  // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
            //                                  // 21 = medium , 22 = hard
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.VisualTyreCompound = (TyreCompounds)uint8;
            //uint8 m_tyresAgeLaps;             // Age in laps of the current set of tyres
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.TyresAgeLaps = uint8;
            //int8 m_vehicleFiaFlags;    // -1 = invalid/unknown, 0 = none, 1 = green
            //                           // 2 = blue, 3 = yellow, 4 = red
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.VehicleFIAFlag = (Flags)int8;
            //float m_ersStoreEnergy;           // ERS energy store in Joules
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSStoreEnergy = f;
            this.ERSStoreEnergyPercent = CarStatusData.CalculateERSPercent(f);
            //uint8 m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = medium
            //                                  // 2 = hotlap, 3 = overtake
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SetERS2020(uint8);
            //float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUK = f;
            //this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUKPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSHarvestedThisLapMGUH = f;
            //this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f, ERSCapacity2021 * 2f);
            this.ERSHarvestedThisLapMGUHPercent = CarStatusData.CalculateERSPercent(f);
            //float m_ersDeployedThisLap;       // ERS energy deployed this lap
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.ERSDeployedThisLap = f;
            this.ERSDeployedThisLapPercent = CarStatusData.CalculateERSPercent(f);
            //uint8 m_networkPaused;            // Whether the car is paused in a network game
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsNetworkPaused = b;
        }

        private void SetERS2020(byte val)
        {
            switch (val)
            {
                case 0:
                    this.ERSDeployMode = ERSModes.None;
                    break;
                case 1:
                    this.ERSDeployMode = ERSModes.Medium;
                    break;
                case 2:
                    this.ERSDeployMode = ERSModes.Hotlap;
                    break;
                case 3:
                    this.ERSDeployMode = ERSModes.Overtake;
                    break;
            }
        }

        private static float CalculateERSPercent(float value, float capacity = float.NaN)
        {
            if (float.IsNaN(capacity)) capacity = CarStatusData.ERSCapacity2021;
            return value / capacity * 100f;
        }

        internal CarDamageData BuildCardemageData()
        {
            Dictionary<string, float> tmp = new Dictionary<string, float>();
            var keys = this.tyreWear.Keys.ToArray();
            var values = this.tyreWear.Values.ToArray();

            for (int i = 0; i < this.tyreWear.Count; i++)
            {
                tmp.Add(keys[i], (float)values[i]);
            }

            var ret = new CarDamageData(
                    tmp,
                    this.tyreDemage,
                    this.frontLeftWingDemage,
                    this.frontRightWingDemage,
                    this.rearWingDemage,
                    this.engineDemage,
                    this.gearboxDemage,
                    this.exhaustDemage,
                    this.drsFault
                );

            return ret;
        }
    }
}
