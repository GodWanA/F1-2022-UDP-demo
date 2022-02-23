using F1Telemetry.Helpers;
using System.Collections.Generic;

namespace F1Telemetry.Models.CarSetupsPacket
{
    public class CarSetupData : ProtoModel
    {
        public CarSetupData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        public byte FrontWing { get; private set; }
        public byte RearWing { get; private set; }
        public byte DifferentialOnThrottle { get; private set; }
        public byte DifferentialOffThrottle { get; private set; }
        public float FrontChamber { get; private set; }
        public float RearChamber { get; private set; }
        public float FrontToe { get; private set; }
        public float RearToe { get; private set; }
        public byte FrontSuspension { get; private set; }
        public byte RearSuspension { get; private set; }
        public byte FrontAntiRollBar { get; private set; }
        public byte RearAntiRollBar { get; private set; }
        public byte FrontSuspensionHeight { get; private set; }
        public byte RearSuspensionHeight { get; private set; }
        public byte BreakPressure { get; private set; }
        public byte BrakeBias { get; private set; }
        public Dictionary<string, float> TyrePressure { get; private set; }
        public float FuelLoad { get; private set; }
        public byte Ballast { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            float valf;
            Dictionary<string, float> d;

            //uint8 m_frontWing;                // Front wing aero
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontWing = valb;

            //uint8 m_rearWing;                 // Rear wing aero
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RearWing = valb;

            //uint8 m_onThrottle;               // Differential adjustment on throttle (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DifferentialOnThrottle = valb;

            //uint8 m_offThrottle;              // Differential adjustment off throttle (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.DifferentialOffThrottle = valb;

            //float m_frontCamber;              // Front camber angle (suspension geometry)
            index += ByteReader.ToFloat(array, index, out valf);
            this.FrontChamber = valf;

            //float m_rearCamber;               // Rear camber angle (suspension geometry)
            index += ByteReader.ToFloat(array, index, out valf);
            this.RearChamber = valf;

            //float m_frontToe;                 // Front toe angle (suspension geometry)
            index += ByteReader.ToFloat(array, index, out valf);
            this.FrontToe = valf;

            //float m_rearToe;                  // Rear toe angle (suspension geometry)
            index += ByteReader.ToFloat(array, index, out valf);
            this.RearToe = valf;

            //uint8 m_frontSuspension;          // Front suspension
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontSuspension = valb;

            //uint8 m_rearSuspension;           // Rear suspension
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RearSuspension = valb;

            //uint8 m_frontAntiRollBar;         // Front anti-roll bar
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontAntiRollBar = valb;

            //uint8 m_rearAntiRollBar;          // Front anti-roll bar
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RearAntiRollBar = valb;

            //uint8 m_frontSuspensionHeight;    // Front ride height
            index += ByteReader.ToUInt8(array, index, out valb);
            this.FrontSuspensionHeight = valb;

            //uint8 m_rearSuspensionHeight;     // Rear ride height
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RearSuspensionHeight = valb;

            //uint8 m_brakePressure;            // Brake pressure (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.BreakPressure = valb;

            //uint8 m_brakeBias;                // Brake bias (percentage)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.BrakeBias = valb;

            //float m_rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
            //float m_rearRightTyrePressure;    // Rear right tyre pressure (PSI)
            //float m_frontLeftTyrePressure;    // Front left tyre pressure (PSI)
            //float m_frontRightTyrePressure;   // Front right tyre pressure (PSI)            
            index = ByteReader.ToWheelData.FromFloat(array, index, out d);
            this.TyrePressure = d;

            //uint8 m_ballast;                  // Ballast
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Ballast = valb;

            //float m_fuelLoad;                 // Fuel load
            index += ByteReader.ToFloat(array, index, out valf);
            this.FuelLoad = valf;

            this.Index = index;
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
