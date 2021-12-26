using F1Telemetry.Helpers;
using System.Numerics;

namespace F1Telemetry.Models.MotionPacket
{
    public class CarMotionData : ProtoModel
    {
        /// <summary>
        /// World space X,Y,Z positions
        /// </summary>
        public Vector3 WorldVelocity { get; private set; }
        /// <summary>
        /// Velocity in world space X,Y,Z
        /// </summary>
        public Vector3 WorldPosition { get; private set; }
        /// <summary>
        /// World space forward X,Y,Z directions (normalised)
        /// </summary>
        public Vector3 WorldForwardDir { get; private set; }
        /// <summary>
        /// World space right X,Y,Z directions (normalised)
        /// </summary>
        public Vector3 WorldRightDir { get; private set; }
        /// <summary>
        /// G-Force vectors.<br/>
        /// X : Longitudonal | Y : Lateral | Z : Vertical
        /// </summary>
        public Vector3 GForce { get; private set; }
        /// <summary>
        /// Yaw angle in radians
        /// </summary>
        public float Yaw { get; private set; }
        /// <summary>
        /// Pitch angle in radians
        /// </summary>
        public float Pitch { get; private set; }
        /// <summary>
        /// Roll angle in radians
        /// </summary>
        public float Roll { get; private set; }

        public CarMotionData(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        protected override void Reader2021(byte[] array)
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

            this.GForce = new Vector3(fy, fx, fz);

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
    }
}
