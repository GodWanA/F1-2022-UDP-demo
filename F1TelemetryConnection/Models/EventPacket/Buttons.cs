using F1Telemetry.Helpers;
using System.Collections.Generic;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.EventPacket
{
    public class Buttons : PacketEventData
    {
        /// <summary>
        /// Creates a Buttons object from raw byte array.
        /// </summary>
        /// <param name="e">Parent event data</param>
        /// <param name="array">Raw byte array</param>
        public Buttons(PacketEventData e, byte[] array)
        {
            this.Header = e.Header;
            this.EventCode = e.EventCode;
            this.EventType = e.EventType;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// List of currently pressed buttons.<br/>
        /// Supported:<br/>
        ///     - 2018 (emulated)<br/>
        ///     - 2019 (emulated)<br/>
        ///     - 2020 (emulated)<br/>
        ///     - 2021<br/>
        /// </summary>
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.PressedButtons.Clear();
                this.PressedButtons = null;
            }

            base.Dispose(disposing);
        }
    }
}
