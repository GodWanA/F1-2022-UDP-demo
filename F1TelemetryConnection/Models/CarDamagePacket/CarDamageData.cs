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
        public byte EngineDemage { get; private set; }
        public byte GearBoxWear { get; private set; }
        public byte EngineESWear { get; private set; }
        public byte EngineICEWear { get; private set; }
        public byte EngineMGUKWear { get; private set; }
        public byte EngineMGUHWear { get; private set; }
        public byte EngineTCWear { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            Dictionary<string, float> df;
            Dictionary<string, byte> db;
            byte valb;
            bool valbo;

            //float m_tyresWear[4];                     // Tyre wear (percentage)
            index += ByteReader.ToWheelData.FromFloat(array, index, out df);
            this.TyreWear = df;

            //uint8 m_tyresDamage[4];                   // Tyre damage (percentage)
            index += ByteReader.ToWheelData.FromUint8(array, index, out db);
            this.TyreDemage = db;

            //uint8 m_brakesDamage[4];                  // Brakes damage (percentage)
            index += ByteReader.ToWheelData.FromUint8(array, index, out db);
            this.BrakesDemage = db;

            //uint8 m_frontLeftWingDamage;              // Front left wing damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontLeftWingDemage = valb;

            //uint8 m_frontRightWingDamage;             // Front right wing damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontRightWingDemage = valb;

            //uint8 m_rearWingDamage;                   // Rear wing damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RearWingDemage = valb;

            //uint8 m_floorDamage;                      // Floor damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FloorDemage = valb;

            //uint8 m_diffuserDamage;                   // Diffuser damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DiffurerDemage = valb;

            //uint8 m_sidepodDamage;                    // Sidepod damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SidepodDemage = valb;

            //uint8 m_drsFault;                         // Indicator for DRS fault, 0 = OK, 1 = fault
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsDRSFault = valbo;

            //uint8 m_gearBoxDamage;                    // Gear box damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.GearBoxDemage = valb;

            //uint8 m_engineDamage;                     // Engine damage (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineDemage = valb;

            //uint8 m_engineMGUHWear;                   // Engine wear MGU-H (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineMGUHWear = valb;

            //uint8 m_engineESWear;                     // Engine wear ES (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineESWear = valb;

            //uint8 m_engineCEWear;                     // Engine wear CE (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineESWear = valb;

            //uint8 m_engineICEWear;                    // Engine wear ICE (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineICEWear = valb;

            //uint8 m_engineMGUKWear;                   // Engine wear MGU-K (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineMGUKWear = valb;

            //uint8 m_engineTCWear;                     // Engine wear TC (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.EngineTCWear = valb;

            this.Index = index;
        }
    }
}
