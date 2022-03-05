using F1Telemetry.Helpers;
using F1Telemetry.Models.EventPacket;
using System;
using System.Collections.Generic;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.CarTelemetryPacket
{
    public class PacketCarTelemetryData : ProtoModel
    {
        /// <summary>
        /// The older game used this packet to store pressed buttons, this field contains to emulate them as Button packet event.
        /// </summary>
        private List<ButtonFlags> pressedButtons;

        /// <summary>
        /// Creates a PacketCarTelemetryData object from raw byte array.
        /// </summary>
        /// <param name="header">Header of the packet</param>
        /// <param name="array">Raw byte array</param>
        public PacketCarTelemetryData(PacketHeader header, byte[] array)
        {
            this.Header = header;
            this.Index = this.Header.Index;
            this.PickReader(this.Header.PacketFormat, array);
        }

        /// <summary>
        /// Packet's header<br/>
        /// All game has to support.
        /// </summary>
        public PacketHeader Header { get; private set; }
        /// <summary>
        /// All cars' current telemetry.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public CarTelemetryData[] CarTelemetryData { get; private set; }
        /// <summary>
        /// Player's current MFD panel.<br/>
        /// Supports:<br/>
        ///     - 2018<br/>
        ///     - 2019<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public MFDPanels MFDPanelScreenPlayer1 { get; private set; }
        /// <summary>
        /// Player 2's current MFD panel.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public MFDPanels MFDPanelScreenPlayer2 { get; private set; }
        /// <summary>
        /// Suggested gear for the player (1-8).<br/>
        /// 'Gear_N' if no gear suggested.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        /// </summary>
        public Gears SuggesterGear { get; private set; }

        protected override void Reader2018(byte[] array)
        {
            uint uint32;

            //CarTelemetryData m_carTelemetryData[20];
            this.ReaderCommon(array, 20);
            //uint32 m_buttonStatus;         // Bit flags specifying which buttons are being
            //                               // pressed currently - see appendices
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.pressedButtons = Appendences.KeyChecker(uint32);
        }

        protected override void Reader2019(byte[] array)
        {
            this.Reader2018(array);
        }

        protected override void Reader2020(byte[] array)
        {
            uint uint32;
            byte uint8;

            this.ReaderCommon(array, 22);
            //uint32 m_buttonStatus;        // Bit flags specifying which buttons are being pressed
            //                              // currently - see appendices
            this.Index += ByteReader.ToUInt32(array, this.Index, out uint32);
            this.pressedButtons = Appendences.KeyChecker(uint32);
            //// Added in Beta 3:
            //uint8 m_mfdPanelIndex;       // Index of MFD panel open - 255 = MFD closed
            //                             // Single player, race – 0 = Car setup, 1 = Pits
            //                             // 2 = Damage, 3 =  Engine, 4 = Temperatures
            //                             // May vary depending on game mode
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MFDPanelScreenPlayer1 = (MFDPanels)uint8;
            //uint8 m_mfdPanelIndexSecondaryPlayer;   // See above
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MFDPanelScreenPlayer2 = (MFDPanels)uint8;
            //int8 m_suggestedGear;       // Suggested gear for the player (1-8)
            //                            // 0 if no gear suggested
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SuggesterGear = (Gears)uint8;
        }

        protected override void Reader2021(byte[] array)
        {
            byte uint8;

            //CarTelemetryData m_carTelemetryData[22];
            this.ReaderCommon(array, 22);

            //uint8 m_mfdPanelIndex;       // Index of MFD panel open - 255 = MFD closed
            //                             // Single player, race – 0 = Car setup, 1 = Pits
            //                             // 2 = Damage, 3 =  Engine, 4 = Temperatures
            //                             // May vary depending on game mode
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MFDPanelScreenPlayer1 = (MFDPanels)uint8;
            //uint8 m_mfdPanelIndexSecondaryPlayer;   // See above
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.MFDPanelScreenPlayer1 = (MFDPanels)uint8;
            //int8 m_suggestedGear;       // Suggested gear for the player (1-8)
            //                            // 0 if no gear suggested
            this.Index += ByteReader.ToUInt8(array, this.Index, out uint8);
            this.SuggesterGear = (Gears)uint8;
        }

        /// <summary>
        /// Common parts of the byte reader.
        /// </summary>
        /// <param name="array">Raw byte array.</param>
        /// <param name="arraySize">Size of car telemetry data array.</param>
        private void ReaderCommon(byte[] array, int arraySize)
        {
            this.CarTelemetryData = new CarTelemetryData[arraySize];

            for (int i = 0; i < this.CarTelemetryData.Length; i++)
            {
                this.CarTelemetryData[i] = new CarTelemetryData(this.Index, this.Header.PacketFormat, array);
                this.Index = this.CarTelemetryData[i].Index;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Header.Dispose();

                for (int i = 0; i < this.CarTelemetryData.Length; i++) this.CarTelemetryData[i].Dispose();
                this.CarTelemetryData = null;
            }

            this.pressedButtons.Clear();
            this.pressedButtons = null;

            base.Dispose(disposing);
        }


        /// <summary>
        /// Creates an emulated Button event packet. The emulated object contains parent object header.
        /// </summary>
        /// <returns>New emulated Buttons event packet.</returns>
        internal Buttons BuildButtonEvent()
        {
            return new Buttons(this.Header, this.pressedButtons);
        }
    }
}
