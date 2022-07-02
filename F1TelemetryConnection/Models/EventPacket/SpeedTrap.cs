using F1Telemetry.Helpers;
using System;

namespace F1Telemetry.Models.EventPacket
{
    public class SpeedTrap : PacketEventData
    {
        private static float maxSpeed;
        private static byte fastestIndex;
        public static float[] MaxSpeeds = new float[22];

        /// <summary>
        /// Creates a SpeedTrap object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public SpeedTrap(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Vehicle index of the vehicle triggering speed trap.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }
        /// <summary>
        /// Top speed achieved in kilometres per hour.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float Speed { get; private set; }
        /// <summary>
        /// Overall fastest speed in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsOverallFastestInSession { get; private set; }
        /// <summary>
        /// Fastest speed for driver in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public bool IsDriverFastestInSession { get; private set; }
        /// <summary>
        /// Fastest speed for driver in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte FastestVehicleIndexInSession { get; private set; }
        /// <summary>
        /// Fastest speed for driver in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public float FastestSpeedInSession { get; private set; }

        private void ReaderCommon(byte[] array)
        {
            byte valb;
            float valf;

            //uint8 vehicleIdx;       // Vehicle index of the vehicle triggering speed trap
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.VehicleIndex = valb;
            //float speed;            // Top speed achieved in kilometres per hour
            this.Index += ByteReader.ToFloat(array, this.Index, out valf);
            this.Speed = valf;
        }

        private void CalculateOverAllFastestInSession()
        {
            if (this.Speed > SpeedTrap.maxSpeed)
            {
                this.IsOverallFastestInSession = true;
                this.FastestSpeedInSession = this.Speed;
                this.FastestVehicleIndexInSession = this.VehicleIndex;

                SpeedTrap.maxSpeed = this.Speed;
                SpeedTrap.fastestIndex = this.VehicleIndex;
            }
            else
            {
                this.FastestSpeedInSession = SpeedTrap.maxSpeed;
                this.FastestVehicleIndexInSession = SpeedTrap.fastestIndex;
            }
        }

        protected override void Reader2020(byte[] array)
        {
            this.ReaderCommon(array);
            this.CalculateOverAllFastestInSession();

            if (this.Speed > SpeedTrap.MaxSpeeds[this.VehicleIndex])
            {
                SpeedTrap.MaxSpeeds[this.VehicleIndex] = this.Speed;
                this.IsDriverFastestInSession = true;
            }
        }

        protected override void Reader2021(byte[] array)
        {
            bool valbo;

            this.ReaderCommon(array);
            this.CalculateOverAllFastestInSession();

            //uint8 overallFastestInSession;   // Overall fastest speed in session = 1, otherwise 0
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsOverallFastestInSession = valbo;
            //uint8 driverFastestInSession;    // Fastest speed for driver in session = 1, otherwise 0
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsDriverFastestInSession = valbo;

        }

        protected override void Reader2022(byte[] array)
        {
            this.ReaderCommon(array);

            byte uint8;
            float f;

            //uint8 fastestVehicleIdxInSession;// Vehicle index of the vehicle that is the fastest
            //                                 // in this session
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.FastestVehicleIndexInSession = uint8;
            //float fastestSpeedInSession;      // Speed of the vehicle that is the fastest
            //                                  // in this session
            this.Index += ByteReader.ToFloat(array, this.Index, out f);
            this.FastestSpeedInSession = f;
        }

        internal static void ResetHelpers()
        {
            SpeedTrap.maxSpeed = 0f;
            SpeedTrap.MaxSpeeds = new float[22];
        }
    }
}
