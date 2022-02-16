using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarStatusPacket
{
    public class CarStatusData : ProtoModel
    {
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
        public ERSModes ERSDeployMode { get; private set; }
        public float ERSHarvestedThisLapMGUK { get; private set; }
        public float ERSHarvestedThisLapMGUH { get; private set; }
        public float ERSDeployedThisLap { get; private set; }
        public bool IsNetworkPaused { get; private set; }
        public TyreCompounds VisualTyreCompound { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            bool valbo;
            float valf;
            ushort valus;

            //uint8 m_tractionControl;          // Traction control - 0 = off, 1 = medium, 2 = full
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TractionControl = (TractionControlSettings)valb;

            //uint8 m_antiLockBrakes;           // 0 (off) - 1 (on)
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsAntiLockBrakes = valbo;

            //uint8 m_fuelMix;                  // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FuelMix = (FuelMixis)valb;

            //uint8 m_frontBrakeBias;           // Front brake bias (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontBrakeBias = valb;

            //uint8 m_pitLimiterStatus;         // Pit limiter status - 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsPitLimiter = valbo;

            //float m_fuelInTank;               // Current fuel mass
            index += ByteReader.ToFloat(array, index, out valf);
            this.FuelInTank = valf;

            //float m_fuelCapacity;             // Fuel capacity
            index += ByteReader.ToFloat(array, index, out valf);
            this.FuelCapacity = valf;

            //float m_fuelRemainingLaps;        // Fuel remaining in terms of laps (value on MFD)
            index += ByteReader.ToFloat(array, index, out valf);
            this.FuelRemainingLaps = valf;

            //uint16 m_maxRPM;                   // Cars max RPM, point of rev limiter
            index += ByteReader.ToUInt16(array, index, out valus);
            this.MaxRPM = valus;

            //uint16 m_idleRPM;                  // Cars idle RPM
            index += ByteReader.ToUInt16(array, index, out valus);
            this.IdleRPM = valus;

            //uint8 m_maxGears;                 // Maximum number of gears
            index += ByteReader.ToUInt8(array, index, out valb);
            this.MaxGears = valb;

            //uint8 m_drsAllowed;               // 0 = not allowed, 1 = allowed
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsDRSAllowed = valbo;

            //uint16 m_drsActivationDistance;    // 0 = DRS not available, non-zero - DRS will be available
            //                                   // in [X] metres
            index += ByteReader.ToUInt16(array, index, out valus);
            this.DRSActivationDistance = valus;

            //uint8 m_actualTyreCompound;    // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
            //                               // 7 = inter, 8 = wet
            //                               // F1 Classic - 9 = dry, 10 = wet
            //                               // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
            //                               // 15 = wet
            index += ByteReader.ToUInt8(array, index, out valb);
            TyreCompounds tyre = TyreCompounds.Unknown;

            if (Enum.TryParse<TyreCompounds>(valb.ToString(), out tyre)) this.ActualTyreCompound = tyre;
            else this.ActualTyreCompound = TyreCompounds.Unknown;

            //uint8 m_visualTyreCompound;       // F1 visual (can be different from actual compound)
            //                                  // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            //                                  // F1 Classic – same as above
            //                                  // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
            //                                  // 21 = medium , 22 = hard
            index += ByteReader.ToUInt8(array, index, out valb);

            if (valb == 24) valb = 24;

            //this.VisualTyreCompound = (TyreCompounds)valb;
            //tyre=( TyreCompounds )Enum.ToObject(TyreCompounds.C1.GetType(), valb);
            if (Enum.TryParse<TyreCompounds>(valb.ToString(), out tyre) && Enum.GetValues(tyre.GetType()).Length > valb) this.VisualTyreCompound = tyre;
            else this.VisualTyreCompound = TyreCompounds.Unknown;

            //uint8 m_tyresAgeLaps;             // Age in laps of the current set of tyres
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TyresAgeLaps = valb;

            //int8 m_vehicleFiaFlags;    // -1 = invalid/unknown, 0 = none, 1 = green
            //                           // 2 = blue, 3 = yellow, 4 = red
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleFIAFlag = (Flags)valb;

            //float m_ersStoreEnergy;           // ERS energy store in Joules
            index += ByteReader.ToFloat(array, index, out valf);
            this.ERSStoreEnergy = valf;

            //uint8 m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = medium
            //                                  // 2 = hotlap, 3 = overtake
            index += ByteReader.ToUInt8(array, index, out valb);
            this.ERSDeployMode = (ERSModes)valb;

            //float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            index += ByteReader.ToFloat(array, index, out valf);
            this.ERSHarvestedThisLapMGUK = valf;

            //float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            index += ByteReader.ToFloat(array, index, out valf);
            this.ERSHarvestedThisLapMGUH = valf;

            //float m_ersDeployedThisLap;       // ERS energy deployed this lap
            index += ByteReader.ToFloat(array, index, out valf);
            this.ERSDeployedThisLap = valf;

            //uint8 m_networkPaused;            // Whether the car is paused in a network game
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsNetworkPaused = valbo;

            this.Index = index;
        }
    }
}
