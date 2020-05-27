using System;
using System.Collections.Generic;
using System.IO;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherMO : MessageMO
    {
        public double Lat { get; set; }
        public double Lon { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.Weather;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write((float)Lat, true, 7, 9);
            writer.Write((float)Lon, true, 8, 9);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(byte[] payload)
        {
            using (var stream = new MemoryStream(payload))
            {
                using (var reader = new BinaryBitReader(stream))
                {
                    Lat = reader.ReadFloat(true, 7, 9);
                    Lon = reader.ReadFloat(true, 8, 9);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private WeatherMO() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="conversation"></param>
        /// <param name="text"></param>
        /// <param name="subject"></param>
        public static WeatherMO Create(double lat, double lon)
        {
            WeatherMO weather = new WeatherMO();
            weather.Lat = lat;
            weather.Lon = lon;
            // --->
            return weather;
        }
    }

}

