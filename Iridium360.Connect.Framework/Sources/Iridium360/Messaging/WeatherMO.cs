using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// Запрос прогноза погоды
    /// </summary>
    public class WeatherMO : MessageMO
    {
        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.Weather;

        /// <summary>
        /// Широта точки, на которую нужно прислать прогноз
        /// </summary>
        public double? RequestLat { get; protected set; }

        /// <summary>
        /// Долгота точки, на которую нужно прислать прогноз
        /// </summary>
        public double? RequestLon { get; protected set; }

        /// <summary>
        /// Идентификатор координат по которым запрошен прогноз
        /// Отправляется также в ответе вместо самих координат для экономии
        /// </summary>
        public byte? PointKey { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            WriteLocation(writer);

            if (this.Version >= ProtocolVersion.v4__WeatherExtension)
            {
                if (RequestLat != null && RequestLon != null)
                {
                    writer.Write(true);
                    writer.Write((float)RequestLat, true, 7, 13);
                    writer.Write((float)RequestLon, true, 8, 13);
                    writer.Write((uint)PointKey, 4);
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(BinaryBitReader reader)
        {
            ReadLocation(reader);

            if (this.Version >= ProtocolVersion.v4__WeatherExtension)
            {
                if (reader.ReadBoolean())
                {
                    RequestLat = reader.ReadFloat(true, 7, 13);
                    RequestLon = reader.ReadFloat(true, 8, 13);
                    PointKey = (byte)reader.ReadUInt(4);
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
        /// <param name="version"></param>
        /// <param name="currentLat">Текущее местоположение</param>
        /// <param name="currentLon">Текущее местоположение</param>
        /// <param name="requestLat">Широта точки, на которую нужно прислать прогноз</param>
        /// <param name="requestLon">Долгота точки, на которую нужно прислать прогноз</param>
        /// <param name="pointKey">Короткий идентификатор точки</param>
        /// <returns></returns>
        public static WeatherMO Create(
            ProtocolVersion version,
            double currentLat,
            double currentLon,
            double? requestLat = null,
            double? requestLon = null,
            byte? pointKey = null)
        {
            WeatherMO weather = Create<WeatherMO>(version);

            weather.Lat = currentLat;
            weather.Lon = currentLon;

            weather.RequestLat = requestLat;
            weather.RequestLon = requestLon;

            weather.PointKey = pointKey;

            // --->
            return weather;
        }
    }

}

