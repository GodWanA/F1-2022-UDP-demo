using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class WeatherForecastSample : ProtoModel
    {
        /// <summary>
        /// Creates WeatherForecastSample object.
        /// </summary>
        /// <param name="index">Start index</param>
        /// <param name="format">Packet format</param>
        /// <param name="array">Raw byte array</param>
        public WeatherForecastSample(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        private void ReaderCommon(byte[] array)
        {
            byte valb;
            //uint8 m_sessionType;              // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1
            //                                  // 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2
            //                                  // 12 = Time Trial
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.SeassonType = (SessionTypes)valb;

            //uint8 m_timeOffset;               // Time in minutes the forecast is for
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.TimeOffset = TimeSpan.FromMinutes(valb);
        }

        protected override void Reader2020(byte[] array)
        {
            sbyte valsb;
            byte valb;

            this.ReaderCommon(array);

            //uint8 m_weather;                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast
            //                                  // 3 = light rain, 4 = heavy rain, 5 = storm
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.Weather = (WeatherTypes)valb;
            //int8 m_trackTemperature;         // Track temp. in degrees Celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.TrackTemperature = valsb;
            //int8 m_airTemperature;           // Air temp. in degrees celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.AirTemperature = valsb;
        }

        protected override void Reader2021(byte[] array)
        {
            byte valb;
            sbyte valsb;

            this.ReaderCommon(array);

            //uint8 m_weather;                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast
            //                                  // 3 = light rain, 4 = heavy rain, 5 = storm
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);

            switch (valb)
            {
                default:
                    this.Weather = (WeatherTypes)valb;
                    break;
                case 6:
                    this.Weather = WeatherTypes.Storm;
                    break;
                case 5:
                    this.Weather = WeatherTypes.HeavyRain;
                    break;
            }

            //int8 m_trackTemperature;         // Track temp. in degrees Celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.TrackTemperature = valsb;
            //int8 m_trackTemperatureChange;   // Track temp. change – 0 = up, 1 = down, 2 = no change
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.TrackTemperatureChage = (TemperatureChanges)valsb;
            //int8 m_airTemperature;           // Air temp. in degrees celsius
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.AirTemperature = valsb;
            //int8 m_airTemperatureChange;     // Air temp. change – 0 = up, 1 = down, 2 = no change
            this.Index += ByteReader.ToInt8(array, this.Index, out valsb);
            this.AirTemperatureChage = (TemperatureChanges)valsb;
            //uint8 m_rainPercentage;           // Rain percentage (0-100)
            this.Index += ByteReader.ToUInt8(array, this.Index, out valb);
            this.RainPercentage = (int)valb;
        }

        protected override void Reader2022(byte[] array)
        {
            this.Reader2021(array);
        }

        /// <summary>
        /// Type of the session.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public SessionTypes SeassonType { get; private set; }
        /// <summary>
        /// Predicted time offset.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TimeSpan TimeOffset { get; private set; }
        /// <summary>
        /// Type of the wather.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public WeatherTypes Weather { get; private set; }
        /// <summary>
        /// Indicates type of change in track temperature.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public sbyte TrackTemperature { get; private set; }
        /// <summary>
        /// Indicates type of change in track temperature.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TemperatureChanges TrackTemperatureChage { get; private set; }
        /// <summary>
        /// Type of the session.<br/>
        /// Supports:<br/>
        ///     - 2020<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public sbyte AirTemperature { get; private set; }
        /// <summary>
        /// Indicates type of change in air temperature.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public TemperatureChanges AirTemperatureChage { get; private set; }
        /// <summary>
        /// Chance of rain in percentage.<br/>
        /// Supports:<br/>
        ///     - 2021<br/>
        ///     - 2022<br/>
        /// </summary>
        public int RainPercentage { get; private set; }
    }
}
