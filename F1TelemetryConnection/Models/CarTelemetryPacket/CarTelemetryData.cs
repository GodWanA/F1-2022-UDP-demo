using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarTelemetryPacket
{
    public class CarTelemetryData : ProtoModel
    {
        /// <summary>
        /// Creates a CarTelemetryData object from raw byte array.
        /// </summary>
        /// <param name="format">Packet format</param>
        /// <param name="index">Start index of packet</param>
        /// <param name="array">Raw byte array</param>
        public CarTelemetryData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        /// <summary>
        /// Speed of car in kilometres per hour.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort Speed { get; private set; }
        /// <summary>
        /// Amount of throttle applied (0 to 100).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float Throttle { get; private set; }
        /// <summary>
        /// Steering (-1.0 (full lock left) to 1.0 (full lock right)).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float Steer { get; private set; }
        /// <summary>
        /// Amount of brake applied (0.0 to 1.0).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float Brake { get; private set; }
        /// <summary>
        /// Amount of clutch applied (0.0 to 1.0).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float Clutch { get; private set; }
        /// <summary>
        /// Gear selected.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Gears Gear { get; private set; }
        /// <summary>
        /// Gear selected.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public string GearString { get; private set; }
        /// <summary>
        /// Engine RPM.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort EngineRPM { get; private set; }
        /// <summary>
        /// Indicates DRS is in use.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsDRS { get; private set; }
        /// <summary>
        /// Rev lights indicator (percentage).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte RevLightPercent { get; private set; }
        /// <summary>
        /// Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort RevLightBitValue { get; private set; }
        /// <summary>
        /// Brakes temperature (celsius).<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        /// </summary>
        public Dictionary<string, ushort> BrakesTemperature { get; private set; }
        /// <summary>
        /// Tyres surface temperature (celsius).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Dictionary<string, byte> TyresSurfaceTemperature { get; private set; }
        /// <summary>
        /// Tyres inner temperature (celsius).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Dictionary<string, byte> TyresInnerTemperature { get; private set; }
        /// <summary>
        /// Engine temperature (celsius).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public ushort EngineTemperature { get; private set; }
        /// <summary>
        /// Tyres pressure (PSI).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Dictionary<string, float> TyresPressure { get; private set; }
        /// <summary>
        /// Driving surface, see appendices.<br/>
        /// Supports:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Dictionary<string, SurfaceTypes> SurfaceType { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            ushort uint16;
            byte uint8;
            sbyte int8;
            bool b;
            Dictionary<string, ushort> d16;
            //Dictionary<string, byte> d8;
            Dictionary<string, float> df;

            //uint16 m_speed;                      // Speed of car in kilometres per hour
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.Speed = uint16;
            //uint8 m_throttle;                   // Amount of throttle applied (0 to 100)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Throttle = uint8 / 100.0f;
            //int8 m_steer;                      // Steering (-100 (full lock left) to 100 (full lock right))
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.Steer = int8 / 100.0f;
            //uint8 m_brake;                      // Amount of brake applied (0 to 100)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Brake = uint8 / 100.0f;
            //uint8 m_clutch;                     // Amount of clutch applied (0 to 100)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Clutch = uint8 / 100.0f;
            //int8 m_gear;                       // Gear selected (1-8, N=0, R=-1)
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.SetGear(int8);
            //uint16 m_engineRPM;                  // Engine RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineRPM = uint16;
            //uint8 m_drs;                        // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRS = b;
            //uint8 m_revLightsPercent;           // Rev lights indicator (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RevLightPercent = uint8;
            //uint16 m_brakesTemperature[4];       // Brakes temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.BrakesTemperature = d16;
            //uint16 m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.TyresSurfaceTemperature = CarTelemetryData.ConvertTemperature(d16);
            //uint16 m_tyresInnerTemperature[4];   // Tyres inner temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.TyresInnerTemperature = CarTelemetryData.ConvertTemperature(d16);
            //uint16 m_engineTemperature;          // Engine temperature (celsius)
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineTemperature = uint16;
            //float m_tyresPressure[4];           // Tyres pressure (PSI)
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out df);
            this.TyresPressure = df;
        }

        private static Dictionary<string, byte> ConvertTemperature(Dictionary<string, ushort> tmp)
        {
            var ret = new Dictionary<string, byte>();

            var keys = tmp.Keys.ToArray();
            var values = tmp.Values.ToArray();

            for (int i = 0; i < tmp.Count; i++)
            {
                ret.Add(keys[i], (byte)values[i]);
            }

            return ret;
        }

        protected override void Reader2019(byte[] array)
        {
            ushort uint16;
            Dictionary<string, ushort> d16;
            Dictionary<string, float> df;

            //uint16 m_speed;                    // Speed of car in kilometres per hour
            //float m_throttle;                 // Amount of throttle applied (0.0 to 1.0)
            //float m_steer;                    // Steering (-1.0 (full lock left) to 1.0 (full lock right))
            //float m_brake;                    // Amount of brake applied (0.0 to 1.0)
            //uint8 m_clutch;                   // Amount of clutch applied (0 to 100)
            //int8 m_gear;                     // Gear selected (1-8, N=0, R=-1)
            //uint16 m_engineRPM;                // Engine RPM
            //uint8 m_drs;                      // 0 = off, 1 = on
            //uint8 m_revLightsPercent;         // Rev lights indicator (percentage)
            this.ReaderCommon(array);
            //uint16 m_brakesTemperature[4];     // Brakes temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.BrakesTemperature = d16;
            //uint16 m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.TyresSurfaceTemperature = CarTelemetryData.ConvertTemperature(d16);
            //uint16 m_tyresInnerTemperature[4]; // Tyres inner temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.TyresInnerTemperature = CarTelemetryData.ConvertTemperature(d16);
            //uint16 m_engineTemperature;        // Engine temperature (celsius)
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineTemperature = uint16;
            //float m_tyresPressure[4];         // Tyres pressure (PSI)
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out df);
            this.TyresPressure = df;
            //uint8 m_surfaceType[4];           // Driving surface, see appendices
            this.ReadSurfaceType(array);
        }

        protected override void Reader2020(byte[] array)
        {
            ushort uint16;
            Dictionary<string, ushort> d16;
            Dictionary<string, byte> d8;
            Dictionary<string, float> df;

            this.ReaderCommon(array);
            //uint16 m_brakesTemperature[4];     // Brakes temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.BrakesTemperature = d16;
            //uint16 m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.TyresSurfaceTemperature = d8;
            //uint16 m_tyresInnerTemperature[4]; // Tyres inner temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.TyresInnerTemperature = d8;
            //uint16 m_engineTemperature;        // Engine temperature (celsius)
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineTemperature = uint16;
            //float m_tyresPressure[4];         // Tyres pressure (PSI)
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out df);
            this.TyresPressure = df;
            //uint8 m_surfaceType[4];           // Driving surface, see appendices
            this.ReadSurfaceType(array);
        }

        protected override void Reader2021(byte[] array)
        {
            ushort uint16;
            Dictionary<string, ushort> d16;
            Dictionary<string, byte> d8;
            Dictionary<string, float> df;

            this.ReaderCommon(array);
            //uint16 m_revLightsBitValue;        // Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.RevLightBitValue = uint16;
            //uint16 m_brakesTemperature[4];     // Brakes temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint16(array, this.Index, out d16);
            this.BrakesTemperature = d16;
            //uint8 m_tyresSurfaceTemperature[4]; // Tyres surface temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.TyresSurfaceTemperature = d8;
            //uint8 m_tyresInnerTemperature[4]; // Tyres inner temperature (celsius)
            this.Index += ByteReader.ToWheelData.FromUint8(array, this.Index, out d8);
            this.TyresInnerTemperature = d8;
            //uint16 m_engineTemperature;        // Engine temperature (celsius)
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineTemperature = uint16;
            //float m_tyresPressure[4];         // Tyres pressure (PSI)
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out df);
            this.TyresPressure = df;
            //uint8 m_surfaceType[4];           // Driving surface, see appendices
            this.ReadSurfaceType(array);
        }

        /// <summary>
        /// Common part of packet readers
        /// </summary>
        /// <param name="array">Raw byte array</param>
        private void ReaderCommon(byte[] array)
        {
            ushort uint16;
            float f;
            byte uint8;
            sbyte int8;
            bool b;

            //uint16 m_speed;                    // Speed of car in kilometres per hour
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.Speed = uint16;
            //float m_throttle;                 // Amount of throttle applied (0.0 to 1.0)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.Throttle = f;
            //float m_steer;                    // Steering (-1.0 (full lock left) to 1.0 (full lock right))
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.Steer = f;
            //float m_brake;                    // Amount of brake applied (0.0 to 1.0)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.Brake = f;
            //uint8 m_clutch;                   // Amount of clutch applied (0 to 100)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Clutch = uint8 / 100.0f;
            //int8 m_gear;                     // Gear selected (1-8, N=0, R=-1)
            this.Index += ByteReader.ToInt8(array, this.Index, out int8);
            this.SetGear(int8);
            //uint16 m_engineRPM;                // Engine RPM
            this.Index += ByteReader.ToUInt16(array, this.Index, out uint16);
            this.EngineRPM = uint16;
            //uint8 m_drs;                      // 0 = off, 1 = on
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out b);
            this.IsDRS = b;
            //uint8 m_revLightsPercent;         // Rev lights indicator (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RevLightPercent = uint8;
        }

        /// <summary>
        /// Reads byte array and sets value of surface type property.
        /// </summary>
        /// <param name="array">Raw byte array</param>
        private void ReadSurfaceType(byte[] array)
        {
            byte uint8;

            this.SurfaceType = new Dictionary<string, SurfaceTypes>();

            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SurfaceType.Add("RearLeft", (SurfaceTypes)uint8);
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SurfaceType.Add("RearRight", (SurfaceTypes)uint8);
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SurfaceType.Add("FrontLeft", (SurfaceTypes)uint8);
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SurfaceType.Add("FrontRight", (SurfaceTypes)uint8);
        }

        private void SetGear(sbyte int8)
        {
            this.Gear = (Gears)int8;
            this.GearString = Regex.Replace(this.Gear.ToString(), "gear_", "", RegexOptions.IgnoreCase);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.BrakesTemperature?.Clear();
                this.BrakesTemperature = null;
                this.TyresSurfaceTemperature?.Clear();
                this.TyresSurfaceTemperature = null;
                this.TyresInnerTemperature?.Clear();
                this.TyresInnerTemperature = null;
                this.TyresPressure?.Clear();
                this.TyresPressure = null;
                this.SurfaceType?.Clear();
                this.SurfaceType = null;
            }

            base.Dispose(disposing);
        }
    }
}
