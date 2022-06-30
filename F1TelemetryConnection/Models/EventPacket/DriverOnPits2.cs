
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    /// <summary>
    /// Custom event to log pit stops on race.
    /// </summary>
    public class DriverOnPits2 : PacketEventData
    {
        /// <summary>
        /// Custom event to log pit stops on race.
        /// </summary>
        /// <param name="vehicleIndex">Vehicle index in other packets.</param>
        /// <param name="lap">Lap, when entered to pit</param>
        public DriverOnPits2(int vehicleIndex, int lap)
        {
            this.EventCode = "DOP2";
            this.EventType = EventTypes.PitStop;
            this.EventName = "Pit Stop";

            this.VehicleIndex = vehicleIndex;
            this.LapNumber = lap;
        }

        /// <summary>
        /// Vehicle index in other packages.
        /// </summary>
        public int VehicleIndex { get; private set; }
        /// <summary>
        /// The lapnumber when he entered to pit.
        /// </summary>
        public int LapNumber { get; private set; }
    }
}
