using F1Telemetry.Helpers;
using System.Collections.Generic;

namespace F1Telemetry.Models.CarSetupsPacket
{
    public class CarSetupData : ProtoModel
    {
        /// <summary>
        /// Creates CarSetupData object.
        /// </summary>
        /// <param name="index">Start index of packet.</param>
        /// <param name="format">Format of the packet.</param>
        /// <param name="array">Raw byte array.</param>
        public CarSetupData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        /// <summary>
        /// Front wing aero.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte FrontWing { get; private set; }
        /// <summary>
        /// Rear wing aero.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte RearWing { get; private set; }
        /// <summary>
        /// Differential adjustment on throttle (percentage).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte DifferentialOnThrottle { get; private set; }
        /// <summary>
        /// Differential adjustment off throttle (percentage).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte DifferentialOffThrottle { get; private set; }
        /// <summary>
        /// Front camber angle (suspension geometry).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float FrontChamber { get; private set; }
        /// <summary>
        /// Rear camber angle (suspension geometry).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float RearChamber { get; private set; }
        /// <summary>
        /// Front toe angle (suspension geometry).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float FrontToe { get; private set; }
        /// <summary>
        /// Rear toe angle (suspension geometry).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float RearToe { get; private set; }
        /// <summary>
        /// Front suspension.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte FrontSuspension { get; private set; }
        /// <summary>
        /// Rear suspension.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte RearSuspension { get; private set; }
        /// <summary>
        /// Front anti-roll bar.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte FrontAntiRollBar { get; private set; }
        /// <summary>
        /// Front anti-roll bar.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte RearAntiRollBar { get; private set; }
        /// <summary>
        /// Front ride height.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte FrontSuspensionHeight { get; private set; }
        /// <summary>
        /// Rear ride height.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte RearSuspensionHeight { get; private set; }
        /// <summary>
        /// Brake pressure (percentage).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte BrakePressure { get; private set; }
        /// <summary>
        /// Brake bias (percentage).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte BrakeBias { get; private set; }
        /// <summary>
        /// Tyre pressure (PSI).<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> TyrePressure { get; private set; }
        /// <summary>
        /// Fuel load.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float FuelLoad { get; private set; }
        /// <summary>
        /// Ballast.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte Ballast { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            byte uint8;
            float f;
            Dictionary<string, float> d;

            //uint8 m_frontWing;                // Front wing aero
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontWing = uint8;
            //uint8 m_rearWing;                 // Rear wing aero
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RearWing = uint8;
            //uint8 m_onThrottle;               // Differential adjustment on throttle (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DifferentialOnThrottle = uint8;
            //uint8 m_offThrottle;              // Differential adjustment off throttle (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.DifferentialOffThrottle = uint8;
            //float m_frontCamber;              // Front camber angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FrontChamber = f;
            //float m_rearCamber;               // Rear camber angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.RearChamber = f;
            //float m_frontToe;                 // Front toe angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FrontToe = f;
            //float m_rearToe;                  // Rear toe angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.RearToe = f;
            //uint8 m_frontSuspension;          // Front suspension
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontSuspension = uint8;
            //uint8 m_rearSuspension;           // Rear suspension
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RearSuspension = uint8;
            //uint8 m_frontAntiRollBar;         // Front anti-roll bar
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontAntiRollBar = uint8;
            //uint8 m_rearAntiRollBar;          // Front anti-roll bar
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RearAntiRollBar = uint8;
            //uint8 m_frontSuspensionHeight;    // Front ride height
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FrontSuspensionHeight = uint8;
            //uint8 m_rearSuspensionHeight;     // Rear ride height
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.RearSuspensionHeight = uint8;
            //uint8 m_brakePressure;            // Brake pressure (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.BrakePressure = uint8;
            //uint8 m_brakeBias;                // Brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.BrakeBias = uint8;
            //float m_frontTyrePressure;        // Front tyre pressure (PSI)
            //float m_rearTyrePressure;         // Rear tyre pressure (PSI)
            this.Index += ByteReader.ToWheelData.fromFloatOld(array, this.Index, out d);
            this.TyrePressure = d;
            //uint8 m_ballast;                  // Ballast
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.Ballast = uint8;
            //float m_fuelLoad;                 // Fuel load
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FuelLoad = f;
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            byte valb;
            float valf;
            Dictionary<string, float> d;

            //uint8 m_frontWing;                // Front wing aero
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.FrontWing = valb;
            //uint8 m_rearWing;                 // Rear wing aero
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.RearWing = valb;
            //uint8 m_onThrottle;               // Differential adjustment on throttle (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.DifferentialOnThrottle = valb;
            //uint8 m_offThrottle;              // Differential adjustment off throttle (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.DifferentialOffThrottle = valb;
            //float m_frontCamber;              // Front camber angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.FrontChamber = valf;
            //float m_rearCamber;               // Rear camber angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.RearChamber = valf;
            //float m_frontToe;                 // Front toe angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.FrontToe = valf;
            //float m_rearToe;                  // Rear toe angle (suspension geometry)
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.RearToe = valf;
            //uint8 m_frontSuspension;          // Front suspension
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.FrontSuspension = valb;
            //uint8 m_rearSuspension;           // Rear suspension
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.RearSuspension = valb;
            //uint8 m_frontAntiRollBar;         // Front anti-roll bar
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.FrontAntiRollBar = valb;
            //uint8 m_rearAntiRollBar;          // Front anti-roll bar
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.RearAntiRollBar = valb;
            //uint8 m_frontSuspensionHeight;    // Front ride height
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.FrontSuspensionHeight = valb;
            //uint8 m_rearSuspensionHeight;     // Rear ride height
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.RearSuspensionHeight = valb;
            //uint8 m_brakePressure;            // Brake pressure (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.BrakePressure = valb;
            //uint8 m_brakeBias;                // Brake bias (percentage)
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.BrakeBias = valb;
            //float m_rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
            //float m_rearRightTyrePressure;    // Rear right tyre pressure (PSI)
            //float m_frontLeftTyrePressure;    // Front left tyre pressure (PSI)
            //float m_frontRightTyrePressure;   // Front right tyre pressure (PSI)            
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out d);
            this.TyrePressure = d;
            //uint8 m_ballast;                  // Ballast
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Ballast = valb;
            //float m_fuelLoad;                 // Fuel load
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.FuelLoad = valf;
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.TyrePressure.Clear();
                this.TyrePressure = null;
            }

            base.Dispose(disposing);
        }
    }
}
