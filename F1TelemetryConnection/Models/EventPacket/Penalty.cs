using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class Penalty : PacketEventData
    {
        public Penalty(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public byte VehicleIndex { get; private set; }
        public PenalytyTypes PenaltyType { get; private set; }
        public InfringementTypes InfragmentType { get; private set; }
        public byte OtherVehicleIndex { get; private set; }
        public TimeSpan Time { get; private set; }
        public byte LapNumber { get; private set; }
        public byte PlacesGained { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //uint8 penaltyType;      // Penalty type – see Appendices
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PenaltyType = (PenalytyTypes)valb;

            //uint8 infringementType;     // Infringement type – see Appendices
            index += ByteReader.ToUInt8(array, index, out valb);
            this.InfragmentType = (InfringementTypes)valb;

            //uint8 vehicleIdx;           // Vehicle index of the car the penalty is applied to
            index += ByteReader.ToUInt8(array, index, out valb);
            this.VehicleIndex = valb;

            //uint8 otherVehicleIdx;      // Vehicle index of the other car involved
            index += ByteReader.ToUInt8(array, index, out valb);
            this.OtherVehicleIndex = valb;

            //uint8 time;                 // Time gained, or time spent doing action in seconds
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Time = TimeSpan.FromSeconds(valb);

            //uint8 lapNum;               // Lap the penalty occurred on
            index += ByteReader.ToUInt8(array, index, out valb);
            this.LapNumber = valb;

            //uint8 placesGained;       	// Number of places gained by this
            index += ByteReader.ToUInt8(array, index, out valb);
            this.PlacesGained = valb;

            this.Index = index;
        }
    }
}
