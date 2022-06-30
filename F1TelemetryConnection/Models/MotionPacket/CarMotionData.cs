using F1Telemetry.Helpers;
using System.Numerics;

namespace F1Telemetry.Models.MotionPacket
{
    public class CarMotionData : ProtoModel
    {
        /// <summary>
        /// Creates CarMotionData object from raw byte array.
        /// </summary>
        /// <param name="index">Start index of the current object</param>
        /// <param name="format">Packet fromat</param>
        /// <param name="array">Raw byte array</param>
        public CarMotionData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        protected override void Reader2018(byte[] array)
        {
            Vector3 v;
            float fx, fy, fz;
            int index = this.Index;

            //float m_worldPositionX;           // World space X position
            //float m_worldPositionY;           // World space Y position
            //float m_worldPositionZ;           // World space Z position
            index += ByteReader.ToVector3.fromFloat(array, index, out v);
            this.WorldPosition = v;

            //float m_worldVelocityX;           // Velocity in world space X
            //float m_worldVelocityY;           // Velocity in world space Y
            //float m_worldVelocityZ;           // Velocity in world space Z
            index += ByteReader.ToVector3.fromFloat(array, index, out v);
            this.WorldVelocity = v;

            //int16 m_worldForwardDirX;         // World space forward X direction (normalised)
            //int16 m_worldForwardDirY;         // World space forward Y direction (normalised)
            //int16 m_worldForwardDirZ;         // World space forward Z direction (normalised)
            index += ByteReader.ToVector3.fromInt16(array, index, out v);
            this.WorldForwardDir = v;

            //int16 m_worldRightDirX;           // World space right X direction (normalised)
            //int16 m_worldRightDirY;           // World space right Y direction (normalised)
            //int16 m_worldRightDirZ;           // World space right Z direction (normalised)
            index += ByteReader.ToVector3.fromInt16(array, index, out v);
            this.WorldRightDir = v;

            //float m_gForceLateral;            // Lateral G-Force component
            index += ByteReader.ToFloat(array, index, out fx);
            //float m_gForceLongitudinal;       // Longitudinal G-Force component
            index += ByteReader.ToFloat(array, index, out fy);
            //float m_gForceVertical;           // Vertical G-Force component
            index += ByteReader.ToFloat(array, index, out fz);

            this.GForce = new Vector3(fx, fy, fz);

            //float m_yaw;                      // Yaw angle in radians
            index += ByteReader.ToFloat(array, index, out fy);
            this.Yaw = fy;
            //float m_pitch;                    // Pitch angle in radians
            index += ByteReader.ToFloat(array, index, out fy);
            this.Pitch = fy;
            //float m_roll;                     // Roll angle in radians
            index += ByteReader.ToFloat(array, index, out fy);
            this.Roll = fy;

            this.Index = index;
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2018(array);
        }

        /// <summary>
        /// World space X,Y,Z positions<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 WorldVelocity { get; private set; }
        /// <summary>
        /// Velocity in world space X,Y,Z<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 WorldPosition { get; private set; }
        /// <summary>
        /// World space forward X,Y,Z directions (normalised)<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 WorldForwardDir { get; private set; }
        /// <summary>
        /// World space right X,Y,Z directions (normalised)<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 WorldRightDir { get; private set; }
        /// <summary>
        /// G-Force vectors.<br/>
        /// X : Longitudonal | Y : Lateral | Z : Vertical<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public Vector3 GForce { get; private set; }
        /// <summary>
        /// Yaw angle in radians<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float Yaw { get; private set; }
        /// <summary>
        /// Pitch angle in radians<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float Pitch { get; private set; }
        /// <summary>
        /// Roll angle in radians<br/>
        /// Supported:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float Roll { get; private set; }
    }
}
