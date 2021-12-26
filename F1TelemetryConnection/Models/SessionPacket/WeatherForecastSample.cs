using F1Telemetry.Helpers;
using System;
using static F1Telemetry.Helpers.Appendences;

namespace F1Telemetry.Models.SessionPacket
{
    public class WeatherForecastSample : ProtoModel
    {
        public SessionTypes SeassonType { get; private set; }
        public TimeSpan TimeOffset { get; private set; }
        public WeatherTypes Weather { get; private set; }
        public sbyte TrackTemperature { get; private set; }
        public TemperatureChanges TrackTemperatureChage { get; private set; }
        public sbyte AirTemperature { get; private set; }
        public TemperatureChanges AirTemperatureChage { get; private set; }
        public byte RainPercentage { get; private set; }

        public WeatherForecastSample(int index, int format, byte[] array)
        {
            this.Index = index;
            this.PickReader(format, array);
        }

        protected override void Reader2021(byte[] array)
        {
            int index = this.Index;
            byte valb;
            sbyte valsb;

            //uint8 m_sessionType;              // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1
            //                                  // 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2
            //                                  // 12 = Time Trial
            index += ByteReader.ToUInt8(array, index, out valb);
            this.SeassonType = (SessionTypes)valb;

            //uint8 m_timeOffset;               // Time in minutes the forecast is for
            index += ByteReader.ToUInt8(array, index, out valb);
            this.TimeOffset = TimeSpan.FromMinutes(valb);

            //uint8 m_weather;                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast
            //                                  // 3 = light rain, 4 = heavy rain, 5 = storm
            index += ByteReader.ToUInt8(array, index, out valb);
            this.Weather = (WeatherTypes)valb;

            //int8 m_trackTemperature;         // Track temp. in degrees Celsius
            index += ByteReader.ToInt8(array, index, out valsb);
            this.TrackTemperature = valsb;

            //int8 m_trackTemperatureChange;   // Track temp. change – 0 = up, 1 = down, 2 = no change
            index += ByteReader.ToInt8(array, index, out valsb);
            this.TrackTemperatureChage = (TemperatureChanges)valsb;

            //int8 m_airTemperature;           // Air temp. in degrees celsius
            index += ByteReader.ToInt8(array, index, out valsb);
            this.AirTemperature = valsb;

            //int8 m_airTemperatureChange;     // Air temp. change – 0 = up, 1 = down, 2 = no change
            index += ByteReader.ToInt8(array, index, out valsb);
            this.AirTemperatureChage = (TemperatureChanges)valsb;

            //uint8 m_rainPercentage;           // Rain percentage (0-100)
            index += ByteReader.ToUInt8(array, index, out valb);
            this.RainPercentage = valb;

            this.Index = index;
        }
    }
}
