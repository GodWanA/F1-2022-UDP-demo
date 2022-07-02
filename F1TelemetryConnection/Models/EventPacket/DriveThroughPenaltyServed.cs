using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class DriveThroughPenaltyServed : PacketEventData
    {
        /// <summary>
        /// Creates a DriveThroughPenaltyServed object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public DriveThroughPenaltyServed(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Vehicle index of the vehicle serving drive through.<br/>
        /// Supported:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            byte valb;

            //uint8 vehicleIdx;                 // Vehicle index of the vehicle serving drive through
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.VehicleIndex = valb;
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
        }
    }
}
