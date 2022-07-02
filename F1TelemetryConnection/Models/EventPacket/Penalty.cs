using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class Penalty : PacketEventData
    {
        /// <summary>
        /// Creates a Penalty object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public Penalty(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Car index in other pacekts.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte VehicleIndex { get; private set; }
        /// <summary>
        /// Type of penalty.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public PenalytyTypes PenaltyType { get; private set; }
        /// <summary>
        /// Type of infragment.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public InfringementTypes InfragmentType { get; private set; }
        /// <summary>
        /// Vehicle index of the other car involved.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte OtherVehicleIndex { get; private set; }
        /// <summary>
        /// Time gained, or time spent doing action in seconds.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan Time { get; private set; }
        /// <summary>
        /// Lap the penalty occurred on.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte LapNumber { get; private set; }
        /// <summary>
        /// Number of places gained by this.<br/>
        /// Supported:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public byte PlacesGained { get; private set; }

        protected override void Reader2020(byte[] array)
        {
            byte valb;

            //uint8 penaltyType;      // Penalty type – see Appendices
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.PenaltyType = (PenalytyTypes)valb;

            //uint8 infringementType;     // Infringement type – see Appendices
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.InfragmentType = (InfringementTypes)valb;

            //uint8 vehicleIdx;           // Vehicle index of the car the penalty is applied to
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.VehicleIndex = valb;

            //uint8 otherVehicleIdx;      // Vehicle index of the other car involved
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.OtherVehicleIndex = valb;

            //uint8 time;                 // Time gained, or time spent doing action in seconds
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Time = TimeSpan.FromSeconds(valb);

            //uint8 lapNum;               // Lap the penalty occurred on
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.LapNumber = valb;

            //uint8 placesGained;       	// Number of places gained by this
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.PlacesGained = valb;
        }

        protected override void Reader2021(byte[] array)
        {
            this.Reader2020(array);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2020(array);
        }
    }
}
