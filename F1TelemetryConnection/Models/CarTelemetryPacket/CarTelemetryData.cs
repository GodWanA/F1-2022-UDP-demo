using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarTelemetryPacket
{
    public class CarTelemetryData : ProtoModel
    {
        public CarTelemetryData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public ushort Speed { get; private set; }
        public float Throttle { get; private set; }
        public float Steer { get; private set; }
        public float Brake { get; private set; }
        public float Clutch { get; private set; }
        public Gears Gear { get; private set; }
        public ushort EngineRPM { get; private set; }
        public bool IsDRS { get; private set; }
        public byte RevLightPercent { get; private set; }
        public ushort RevLightBitValue { get; private set; }
        public Dictionary<string, ushort> BrakesTemperature { get; private set; }
        public Dictionary<string, byte> TyresSurfaceTemperature { get; private set; }
        public Dictionary<string, byte> TyresInnerTemperature { get; private set; }
        public ushort EngineTemperature { get; private set; }
        public Dictionary<string, float> TyresPressure { get; private set; }
        public Dictionary<string, SurfaceTypes> SurfaceType { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            ushort valus;
            float valf;
            byte valb;
            sbyte valsb;
            bool valbo;
            Dictionary<string, ushort> d16;
            Dictionary<string, byte> d8;
            Dictionary<string, float> df;

            //uint16 m_speed;                    // Speed of car in kilometres per hour
            index += ByteReader.ToUInt16(array, index, out valus);
            this.Speed = valus;

            //float m_throttle;                 // Amount of throttle applied (0.0 to 1.0)
            index += ByteReader.ToFloat(array, index, out valf);
            this.Throttle = valf;

            //float m_steer;                    // Steering (-1.0 (full lock left) to 1.0 (full lock right))
            index += ByteReader.ToFloat(array, index, out valf);
            this.Steer = valf;

            //float m_brake;                    // Amount of brake applied (0.0 to 1.0)
            index += ByteReader.ToFloat(array, index, out valf);
            this.Brake = valf;

            //uint8 m_clutch;                   // Amount of clutch applied (0 to 100)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Clutch = 100.0f / valb;

            //int8 m_gear;                     // Gear selected (1-8, N=0, R=-1)
            index += ByteReader.ToInt8(array, index, out valsb);
            this.Gear = (Gears)valsb;

            //uint16 m_engineRPM;                // Engine RPM
            index += ByteReader.ToUInt16(array, index, out valus);
            this.EngineRPM = valus;

            //uint8 m_drs;                      // 0 = off, 1 = on
            index += ByteReader.ToBoolFromUint8(array, index, out valbo);
            this.IsDRS = valbo;

            //uint8 m_revLightsPercent;         // Rev lights indicator (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RevLightPercent = valb;

            //uint16 m_revLightsBitValue;        // Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
            index += ByteReader.ToUInt16(array, index, out valus);
            this.RevLightBitValue = valus;

            //uint16 m_brakesTemperature[4];     // Brakes temperature (celsius)
            index += ByteReader.ToWheelData.FromUint16(array, index, out d16);
            this.BrakesTemperature = d16;

            //uint8 m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
            index += ByteReader.ToWheelData.FromUint8(array, index, out d8);
            this.TyresSurfaceTemperature = d8;

            //uint8 m_tyresInnerTemperature[4]; // Tyres inner temperature (celsius)
            index += ByteReader.ToWheelData.FromUint8(array, index, out d8);
            this.TyresInnerTemperature = d8;

            //uint16 m_engineTemperature;        // Engine temperature (celsius)
            index += ByteReader.ToUInt16(array, index, out valus);
            this.EngineTemperature = valus;

            //float m_tyresPressure[4];         // Tyres pressure (PSI)
            index += ByteReader.ToWheelData.FromFloat(array, index, out df);
            this.TyresPressure = df;

            //uint8 m_surfaceType[4];           // Driving surface, see appendices
            this.SurfaceType = new Dictionary<string, SurfaceTypes>();

            index += ByteReader.ToUInt8(array, index, out valb);
            this.SurfaceType.Add("RearLeft", (SurfaceTypes)valb);
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SurfaceType.Add("RearRight", (SurfaceTypes)valb);
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SurfaceType.Add("FrontLeft", (SurfaceTypes)valb);
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SurfaceType.Add("FrontRight", (SurfaceTypes)valb);

            this.Index = index;
        }
    }
}
