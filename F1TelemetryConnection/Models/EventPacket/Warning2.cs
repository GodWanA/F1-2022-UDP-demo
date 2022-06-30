using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    /// <summary>
    /// Custom event to log warnings.
    /// </summary>
    public class Warning2 : PacketEventData
    {
        /// <summary>
        /// Custon event to log warnings.
        /// </summary>
        /// <param name="vehicleIndex">Vehicle index in other packets.</param>
        /// <param name="totalWarnings">Driver's total number of warnings.</param>
        /// <param name="lapNumber">Lap number on when occured.</param>
        /// <param name="reason">The reason why he get this warning.</param>
        public Warning2(int vehicleIndex, int totalWarnings, int lapNumber, InfringementTypes reason)
        {
            this.EventCode = "WAR2";
            this.EventType = EventTypes.Warning;
            this.EventName = "Warning";

            this.VehicleIndex = vehicleIndex;
            this.TotalWarnings = totalWarnings;
            this.LapNumber = lapNumber;
            this.Reason = reason;
        }

        /// <summary>
        /// Vehicle index in other packages.
        /// </summary>
        public int VehicleIndex { get; private set; }

        /// <summary>
        /// Driver's total number of warnings.
        /// </summary>
        public int TotalWarnings { get; private set; }

        /// <summary>
        /// Lap number when warning given.
        /// </summary>
        public int LapNumber { get; private set; }

        /// <summary>
        /// The reason why driver get this warning.
        /// </summary>
        public InfringementTypes Reason { get; private set; }
    }
}
