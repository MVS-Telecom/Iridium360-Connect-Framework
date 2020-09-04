using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            WriteLocation(writer);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(BinaryBitReader reader)
        {
            ReadLocation(reader);
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
        public static WeatherMO Create(ProtocolVersion version, double lat, double lon)
        {
            WeatherMO weather = Create<WeatherMO>(version);

            weather.Lat = lat;
            weather.Lon = lon;

            // --->
            return weather;
        }
    }

}

