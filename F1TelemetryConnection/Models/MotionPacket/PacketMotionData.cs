using F1Telemetry.Helpers;
using System.Collections.Generic;
using System.Numerics;

namespace F1Telemetry.Models.MotionPacket
{
    public class PacketMotionData : ProtoModel
    {
        /// <summary>
        /// Creates a PacketMotionData from raw byte array
        /// </summary>
        /// <param name="header">Header packet of the object.</param>
        /// <param name="array">Raw byte array</param>
        public PacketMotionData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Reads and fill player specific datas from byte array.
        /// </summary>
        /// <param name="array">raw byte array</param>
        private void ReadPlayerData(byte[] array)
        {
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
            //float m_suspensionAcceleration[4];   // RL, RR, FL, FR
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.SuspensionAcceleration = tmp;
            //float m_wheelSpeed[4];               // Speed of each wheel
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.WheelSpeed = tmp;
            //float m_wheelSlip[4];                // Slip ratio for each wheel
            this.Index += ByteReader.ToWheelData.FromFloat(array, this.Index, out tmp);
            this.WheelSlip = tmp;
            //float m_localVelocityX;              // Velocity in local space
            //float m_localVelocityY;              // Velocity in local space
            //float m_localVelocityZ;              // Velocity in local space
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;
            //float m_angularVelocityX;            // Angular velocity x-component
            //float m_angularVelocityY;            // Angular velocity y-component
            //float m_angularVelocityZ;            // Angular velocity z-component
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;
            //float m_angularAccelerationX;        // Angular velocity x-component
            //float m_angularAccelerationY;        // Angular velocity y-component
            //float m_angularAccelerationZ;        // Angular velocity z-component
            this.Index += ByteReader.ToVector3.fromFloat(array, this.Index, out v);
            this.LocalVelocity = v;
            //float m_frontWheelsAngle;            // Current front wheels angle in radians
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FrontWheelAngle = f;
        }

        protected override void Reader2018(byte[] array)
        {
            //CarMotionData m_carMotionData[20];    // Data for all cars on track
            this.CarMotionData = new CarMotionData[20];
            for (int i = 0; i < 20; i++)
            {
                this.CarMotionData[i] = new CarMotionData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarMotionData[i].Index;
            }

            this.ReadPlayerData(array);
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            //CarMotionData m_carMotionData[22];      // Data for all cars on track
            this.CarMotionData = new CarMotionData[22];
            for (int i = 0; i < 22; i++)
            {
                this.CarMotionData[i] = new CarMotionData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarMotionData[i].Index;
            }

            this.ReadPlayerData(array);
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
                this.Header.Dispose();

                for (int i = 0; i < this.CarMotionData?.Length; i++) this.CarMotionData[i]?.Dispose();
                this.CarMotionData = null;

                this.SuspensionAcceleration?.Clear();
                this.SuspensionVelocity?.Clear();
                this.SuspensionPosition?.Clear();
                this.WheelSlip?.Clear();
                this.WheelSpeed?.Clear();

                this.SuspensionAcceleration = null;
                this.SuspensionVelocity = null;
                this.SuspensionPosition = null;
                this.WheelSlip = null;
                this.WheelSpeed = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// Data for all cars on track<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public CarMotionData[] CarMotionData { get; private set; }
        /// <summary>
        /// Player's suspension position.<br/>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> SuspensionPosition { get; private set; }
        /// <summary>
        /// Player's suspension velocity.<br/>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> SuspensionVelocity { get; private set; }
        /// <summary>
        /// Player's suspension acceleration.<br/>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> SuspensionAcceleration { get; private set; }
        /// <summary>
        /// Player's wheel speed.<br>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> WheelSpeed { get; private set; }
        /// <summary>
        /// Player's wheel slip.<br/>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Dictionary<string, float> WheelSlip { get; private set; }
        /// <summary>
        /// Player's local velocity.<br/>
        /// Element's key:<br/>
        ///     - "RearLeft"<br/>
        ///     - "RearRight"<br/>
        ///     - "FrontLeft"<br/>
        ///     - "FrontRight"<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 LocalVelocity { get; private set; }
        /// <summary>
        /// Player's front wheel angle.<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float FrontWheelAngle { get; private set; }

    }
}
