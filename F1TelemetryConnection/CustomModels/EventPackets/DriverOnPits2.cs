using F1Telemetry.Models.EventPacket;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.CustomModels.EventPackets
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
            EventCode = "DOP2";
            EventType = EventTypes.PitStop;
            EventName = "Pit Stop";

            VehicleIndex = vehicleIndex;
            LapNumber = lap;
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
