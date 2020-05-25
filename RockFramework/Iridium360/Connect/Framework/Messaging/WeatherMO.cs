using System;
using System.Collections.Generic;

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
            var biterator = new Biterator();

            biterator.PushFloat((float)Lat, true, 10);
            biterator.PushFloat((float)Lon, true, 10);

            var bytes = biterator.GetUsedBytes();

            writer.Write(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(byte[] payload)
        {
            var biterator = new Biterator(payload);

            Lat = biterator.PopFloat(true, 10);
            Lon = biterator.PopFloat(true, 10);
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

