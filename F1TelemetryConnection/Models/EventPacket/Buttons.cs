using F1Telemetry.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class Buttons : PacketEventData
    {
        public Buttons(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        public List<ButtonFlags> PressedButtons { get; private set; }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            uint val;

            //uint32 m_buttonStatus;    // Bit flags specifying which buttons are being pressed
            //                          // currently - see appendices
            index += ByteReader.ToUInt32(array, index, out val);
            this.PressedButtons = Appendences.KeyChecker(val);

            this.Index = index;
        }
    }
}
