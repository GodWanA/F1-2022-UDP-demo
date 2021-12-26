using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace F1Telemetry.Models.MotionPacket
{
    public class PacketMotionData : ProtoModel
    {
        /// <summary>
        /// Packet's header
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// Data for all cars on track
        /// </summary>
        public CarMotionData[] CarMotionData { get; private set; }
        public Dictionary<string, float> SuspensionPosition { get; private set; }
        public Dictionary<string, float> SuspensionVelocity { get; private set; }
        public Dictionary<string, float> SuspensionAcceleration { get; private set; }
        public Dictionary<string, float> WheelSpeed { get; private set; }
        public Dictionary<string, float> WheelSlip { get; private set; }
        public Vector3 LocalVelocity { get; private set; }
        public float FrontWheelAngle { get; private set; }

        public PacketMotionData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        protected override void Reader2021(byte[] array)
        {
            //CarMotionData m_carMotionData[22];      // Data for all cars on track
            this.CarMotionData = new CarMotionData[22];
            for (int i = 0; i < 22; i++)
            {
                this.CarMotionData[i] = new CarMotionData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarMotionData[i].Index;
            }

            Dictionary<string, float> tmp;
            Vector3 v;
            float f;

            //// Extra player car ONLY data
            //float m_suspensionPosition[4];       // Note: All wheel arrays have the following order:
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.SuspensionPosition = tmp;

            //float m_suspensionVelocity[4];       // RL, RR, FL, FR
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.SuspensionVelocity = tmp;

            //float m_suspensionAcceleration[4];  // RL, RR, FL, FR
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.SuspensionAcceleration = tmp;

            //float m_wheelSpeed[4];              // Speed of each wheel
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.WheelSpeed = tmp;

            //float m_wheelSlip[4];                // Slip ratio for each wheel
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.WheelSlip = tmp;

            //float m_localVelocityX;             // Velocity in local space
            //float m_localVelocityY;             // Velocity in local space
            //float m_localVelocityZ;             // Velocity in local space
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;

            //float m_angularVelocityX;       // Angular velocity x-component
            //float m_angularVelocityY;            // Angular velocity y-component
            //float m_angularVelocityZ;            // Angular velocity z-component
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;

            //float m_angularAccelerationX;        // Angular velocity x-component
            //float m_angularAccelerationY;   // Angular velocity y-component
            //float m_angularAccelerationZ;        // Angular velocity z-component
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;

            //float m_frontWheelsAngle;            // Current front wheels angle in radians
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FrontWheelAngle = f;
        }
    }
}
