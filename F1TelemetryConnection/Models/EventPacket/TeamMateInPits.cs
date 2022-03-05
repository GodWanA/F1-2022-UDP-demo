using F1Telemetry.Helpers;

namespace F1Telemetry.Models.EventPacket
{
    public class TeamMateInPits : PacketEventData
    {
        /// <summary>
        /// Creates a TeamMateInPits object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public TeamMateInPits(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Car index in other pacekts.<br/>
        /// Supported:<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //uint8 vehicleIdx; // Vehicle index of team mate
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleIndex = valb;

            this.Index = index;
        }
    }
}
