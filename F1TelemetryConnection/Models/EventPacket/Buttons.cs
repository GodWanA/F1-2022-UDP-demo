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
            this.EventName = e.EventName;
            this.Index = e.Index;

            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Creates an emulated Buttons object from other packets.
        /// </summary>
        /// <param name="header">Header of parent packet</param>
        /// <param name="pressedButtons">List of currently pressed buttons on controller.</param>
        public Buttons(PacketHeader header, List<ButtonFlags> pressedButtons)
        {
            this.Header = header;
            this.PressedButtons = pressedButtons;
            this.EventCode = "BUTN";
            this.ButtonStatus = EventTypes.ButtonStatus;
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
        public EventTypes ButtonStatus { get; }

        protected override void Reader2021(byte[] array)
        {
            uint val;

            //uint32 m_buttonStatus;    // Bit flags specifying which buttons are being pressed
            //                          // currently - see appendices
            this.Index += ByteReader.ToUInt32(array, this.Index, out val);
            this.PressedButtons = Appendences.KeyChecker(val);
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
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
