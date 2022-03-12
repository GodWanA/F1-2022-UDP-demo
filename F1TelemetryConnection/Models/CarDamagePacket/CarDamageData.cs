using F1Telemetry.Helpers;
using System.Collections.Generic;

namespace F1Telemetry.Models.CarDamagePacket
{
    public class CarDamageData : ProtoModel
    {
        public CarDamageData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public CarDamageData(Dictionary<string, float> tyreWear, Dictionary<string, byte> tyreDemage, byte frontLeftWingDemage, byte frontRightWingDemage, byte rearWingDemage, byte engineDemage, byte gearboxDemage, byte exhaustDemage, bool drsFault)
        {
            this.TyreWear = tyreWear;
            this.TyreDemage = tyreDemage;
            this.FrontLeftWingDemage = frontLeftWingDemage;
            this.FrontRightWingDemage = frontRightWingDemage;
            this.RearWingDemage = rearWingDemage;
            this.EngineDemage = engineDemage;
            this.GearBoxDemage = gearboxDemage;
            this.ExhasutDemage = exhaustDemage;
            this.IsDRSFault = drsFault;
        }

        public Dictionary<string, float> TyreWear { get; private set; }
        public Dictionary<string, byte> TyreDemage { get; private set; }
        public Dictionary<string, byte> BrakesDemage { get; private set; }
        public byte FrontLeftWingDemage { get; private set; }
        public byte FrontRightWingDemage { get; private set; }
        public byte RearWingDemage { get; private set; }
        public byte FloorDemage { get; private set; }
        public byte DiffurerDemage { get; private set; }
        public byte SidepodDemage { get; private set; }
        public bool IsDRSFault { get; private set; }
        public byte GearBoxDemage { get; private set; }
        public byte ExhasutDemage { get; private set; }
        public byte EngineDemage { get; private set; }
        public byte GearBoxWear { get; private set; }
        public byte EngineESWear { get; private set; }
        public byte EngineICEWear { get; private set; }
        public byte EngineMGUKWear { get; private set; }
        public byte EngineMGUHWear { get; private set; }
        public byte EngineTCWear { get; private set; }
        public byte EngineCEWear { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            Dictionary<string, float> df;
            Dictionary<string, byte> d8;
            byte uint8;
            bool b;

            //float m_tyresWear[4];                     // Tyre wear (percentage)
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out df);
            this.TyreWear = df;

            //uint8 m_tyresDamage[4];                   // Tyre damage (percentage)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.TyreDemage = d8;

            //uint8 m_brakesDamage[4];                  // Brakes damage (percentage)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.BrakesDemage = d8;

            //uint8 m_frontLeftWingDamage;              // Front left wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontLeftWingDemage = uint8;

            //uint8 m_frontRightWingDamage;             // Front right wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontRightWingDemage = uint8;

            //uint8 m_rearWingDamage;                   // Rear wing damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RearWingDemage = uint8;

            //uint8 m_floorDamage;                      // Floor damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FloorDemage = uint8;

            //uint8 m_diffuserDamage;                   // Diffuser damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DiffurerDemage = uint8;

            //uint8 m_sidepodDamage;                    // Sidepod damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SidepodDemage = uint8;

            //uint8 m_drsFault;                         // Indicator for DRS fault, 0 = OK, 1 = fault
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRSFault = b;

            //uint8 m_gearBoxDamage;                    // Gear box damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.GearBoxDemage = uint8;

            //uint8 m_engineDamage;                     // Engine damage (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineDemage = uint8;

            //uint8 m_engineMGUHWear;                   // Engine wear MGU-H (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineMGUHWear = uint8;

            //uint8 m_engineESWear;                     // Engine wear ES (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineESWear = uint8;

            //uint8 m_engineCEWear;                     // Engine wear CE (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineCEWear = uint8;

            //uint8 m_engineICEWear;                    // Engine wear ICE (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineICEWear = uint8;

            //uint8 m_engineMGUKWear;                   // Engine wear MGU-K (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineMGUKWear = uint8;

            //uint8 m_engineTCWear;                     // Engine wear TC (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.EngineTCWear = uint8;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TyreWear.Clear();
                this.TyreWear = null;
                this.TyreDemage.Clear();
                this.TyreDemage = null;
                this.BrakesDemage.Clear();
                this.BrakesDemage = null;
            }

            base.Dispose(disposing);
        }
    }
}
