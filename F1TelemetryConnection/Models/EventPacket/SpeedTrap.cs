using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class SpeedTrap : PacketEventData
    {
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
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Vehicle index of the vehicle triggering speed trap.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }
        /// <summary>
        /// Top speed achieved in kilometres per hour.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public float Speed { get; private set; }
        /// <summary>
        /// Overall fastest speed in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsOverallFastestInSession { get; private set; }
        /// <summary>
        /// Fastest speed for driver in session.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public bool IsDriverFastestInSession { get; private set; }

        protected override void Reader2020(byte[] array)
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

        protected override void Reader2021(byte[] array)
        {
            bool valbo;
         
            this.Reader2020(array);
            
            //uint8 overallFastestInSession;   // Overall fastest speed in session = 1, otherwise 0
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsOverallFastestInSession = valbo;
            //uint8 driverFastestInSession;    // Fastest speed for driver in session = 1, otherwise 0
            this.Index += ByteReader.ToBoolFromUint8(array, this.Index, out valbo);
            this.IsDriverFastestInSession = valbo;
        }
    }
}
