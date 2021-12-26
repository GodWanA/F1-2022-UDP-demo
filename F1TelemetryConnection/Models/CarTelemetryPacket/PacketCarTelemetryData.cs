using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarTelemetryPacket
{
    public class PacketCarTelemetryData : ProtoModel
    {
        public PacketCarTelemetryData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        public PacketHeader Header { get; private set; }
        public CarTelemetryData[] CarTelemetryData { get; private set; }
        public MFDPanels MFDPanelScreen { get; private set; }
        public Gears SuggesterGear { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;

            //CarTelemetryData m_carTelemetryData[22];
            this.CarTelemetryData = new CarTelemetryData[22];

            //uint8 m_mfdPanelIndex;       // Index of MFD panel open - 255 = MFD closed
            //                             // Single player, race – 0 = Car setup, 1 = Pits
            //                             // 2 = Damage, 3 =  Engine, 4 = Temperatures
            //                             // May vary depending on game mode
            index += ByteReader.ToUInt8(array, index, out valb);
            this.MFDPanelScreen = (MFDPanels)valb;

            //uint8 m_mfdPanelIndexSecondaryPlayer;   // See above
            index += ByteReader.ToUInt8(array, index, out valb);
            this.MFDPanelScreen = (MFDPanels)valb;

            //int8 m_suggestedGear;       // Suggested gear for the player (1-8)
            //                            // 0 if no gear suggested
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SuggesterGear = (Gears)valb;

            this.Index = index;
        }
    }
}
