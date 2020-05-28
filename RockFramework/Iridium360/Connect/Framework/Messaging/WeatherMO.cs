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
        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.Weather;

        /// <summary>
        /// 
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            Location.pack(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(BinaryBitReader reader)
        {
            Location = Location.unpack(reader);
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
            weather.Location = new Location(lat, lon);

            // --->
            return weather;
        }
    }

}

