using Iridium360.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class WeatherMT : MessageMT
    {
        /// <summary>
        /// 
        /// </summary>
        public List<i360PointForecast> Forecasts { get; set; }

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
            writer.Write((uint)Forecasts.Count, 2);

            foreach (var f in Forecasts)
            {
                writer.Write((float)f.Lat, true, 8, 10);
                writer.Write((float)f.Lon, true, 8, 10);

                ///->

                writer.Write(f.TimeOffset, 6); //[-12...14]h часовое пояс
                writer.Write((uint)f.DayInfos.Count, 3); //7 максимальное кол-во суток в прогнозе


                foreach (var d in f.DayInfos)
                {
                    writer.Write((uint)d.Day, 14);
                    writer.Write((uint)d.Forecasts.Count, 3); //7 максимальное кол-во прогнозов в сутках


                    foreach (var ff in d.Forecasts)
                    {
                        writer.Write((uint)ff.HourOffset, 5); //[0...24]h час
                        writer.Write(ff.Temperature, 7);  //[-70...50]C температура


                        ///->

                        double? _pressure = ff.Pressure;

                        if (_pressure != null)
                            _pressure -= 580;

                        writer.Write((uint?)_pressure, 8);  //[0...221] == [580...800]мм рт давление

                        ///->

                        int? _cloud = ff.Cloud;

                        if (_cloud != null)
                            _cloud = (int)Math.Round(_cloud.Value / 20d);

                        writer.Write((uint?)_cloud, 5); //[0...20] == [0...100]% облачность

                        ///->

                        double? _precipitation = ff.Precipitation;

                        if (_precipitation.Value >= 0.05 && _precipitation.Value <= 0.1)
                            _precipitation = 1;
                        else
                            _precipitation = (int)Math.Round(_precipitation.Value * 4d);

                        writer.Write((uint?)_precipitation, 8); //[0...240] == [0...60]мм осадки

                        ///->

                        int? _windDirection = ff.WindDirection;

                        if (_windDirection != null)
                            _windDirection = (int)(_windDirection.Value / 45d);

                        writer.Write((uint?)_windDirection, 4); //[0...8] == [0...360] направление ветра

                        ///->

                        double? _windSpeed = ff.WindSpeed;

                        if (_windSpeed != null)
                            _windSpeed = (int)Math.Round(_windSpeed.Value * 2d);

                        writer.Write((uint?)_windSpeed, 6); //[0...60] == [0...120]м/с скорость ветра

                        ///->
                        writer.WriteNullable(ff.SnowRisk);  //[0...1] вероятность снега
                    }

                }
            }

            //var bytes = biterator.GetUsedBytes();
            //writer.Write(bytes, biterator.currentByte * 8 + biterator.currentBit);
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

                    var _points = reader.ReadUInt(2);
                    var points = new List<i360PointForecast>();

                    for (int i = 0; i < _points; i++)
                    {
                        var f = new i360PointForecast();

                        f.Lat = reader.ReadFloat(true, 8, 10);
                        f.Lon = reader.ReadFloat(true, 8, 10);

                        ///->

                        f.TimeOffset = reader.ReadInt(6);

                        var _days = reader.ReadUInt(3);
                        var days = new List<i360DayInfo>();

                        for (int j = 0; j < _days; j++)
                        {
                            var d = new i360DayInfo();

                            d.Day = (int)reader.ReadUInt(14);

                            var _forecasts = reader.ReadUInt(3);
                            var forecasts = new List<i360Forecast>();

                            for (int k = 0; k < _forecasts; k++)
                            {
                                var ff = new i360Forecast();

                                ff.HourOffset = (int)reader.ReadUInt(5);
                                ff.Temperature = reader.ReadInt(7);

                                ///->

                                uint? _pressure = reader.ReadUIntNullable(8);

                                if (_pressure != null)
                                    ff.Pressure = (int)_pressure + 580;

                                ///->

                                uint? _cloud = reader.ReadUIntNullable(5);

                                if (_cloud != null)
                                    ff.Cloud = (int)_cloud * 20;

                                ///->
                                ///
                                uint? _precipitation = reader.ReadUIntNullable(8);

                                if (_precipitation != null)
                                    ff.Precipitation = _precipitation / 4d;

                                ///->

                                uint? _windDirection = reader.ReadUIntNullable(4);

                                if (_windDirection != null)
                                    ff.WindDirection = (int)Math.Round(_windDirection.Value * 45d);

                                ///-->

                                uint? _windSpeed = reader.ReadUIntNullable(6);

                                if (_windSpeed != null)
                                    ff.WindSpeed = _windSpeed / 2d;

                                ///->

                                ff.SnowRisk = reader.ReadBoolNullable();

                                ///->

                                forecasts.Add(ff);
                            }

                            d.Forecasts = forecasts;
                            days.Add(d);

                        }

                        f.DayInfos = days;
                        points.Add(f);
                    }


                    Forecasts = points;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private WeatherMT() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="conversation"></param>
        /// <param name="text"></param>
        /// <param name="subject"></param>
        public static WeatherMT Create(List<i360PointForecast> forecasts)
        {
            WeatherMT weather = new WeatherMT();

            weather.Forecasts = forecasts;
            // --->
            return weather;
        }
    }

}

