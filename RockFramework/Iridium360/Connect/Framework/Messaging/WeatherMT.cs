using Iridium360.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        public i360PointForecast Forecast { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.Weather;



        /// <summary>
        /// https://docs.google.com/spreadsheets/d/1tB0OXeDGpwxoqD_LBXcdUXCvs6o8JoNoyXLs8IM4N0M/edit#gid=0
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write((uint)Forecast.TimeOffset + 12, 5); //[-12...14]h часовое пояс

            if (Forecast.DayInfos.Count != 4)
                throw new ArgumentOutOfRangeException("Days != 4");

            if (Forecast.DayInfos.Any(x => x.Forecasts.Count != 4))
                throw new ArgumentOutOfRangeException("Forecasts != 4");

            foreach (var d in Forecast.DayInfos)
            {
                writer.Write((uint)d.DateDay.Day, 5);

                foreach (var ff in d.Forecasts)
                {
                    writer.Write((uint)ff.HourOffset, 5); //[0...24]h час
                    writer.Write((uint)ff.Temperature + 70, 7);  //[-70...50]C температура


                    ///->

                    double? _pressure = ff.Pressure;

                    if (_pressure != null)
                        _pressure -= 580;

                    writer.Write((uint?)_pressure, 8);  //[0...221] == [580...800]мм рт давление

                    ///->

                    int? _cloud = ff.Cloud;

                    if (_cloud != null)
                        _cloud = (int)Math.Round(_cloud.Value / 15d);

                    writer.Write((uint?)_cloud, 4); //[0...15] == [0...100]% облачность

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
                        _windSpeed = (int)Math.Round(_windSpeed.Value);

                    writer.Write((uint?)_windSpeed, 6); //[0...60]м/с скорость ветра

                    ///->
                    writer.Write(ff.SnowRisk);  //[0...1] вероятность снега
                }

            }
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
                    Forecast = new i360PointForecast();

                    Forecast.TimeOffset = (int)reader.ReadUInt(5) - 12;

                    var days = new List<i360DayInfo>();
                    var dates = new int[4];

                    for (int j = 0; j < 4; j++)
                    {
                        var d = new i360DayInfo();

                        dates[j] = (int)reader.ReadUInt(5);

                        int month = 0;

                        if (j > 0 && dates[j] < dates[j - 1])
                            month = 1;

                        d.DateDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month + month, dates[j], 0, 0, 0, DateTimeKind.Utc);


                        var forecasts = new List<i360Forecast>();

                        for (int k = 0; k < 4; k++)
                        {
                            try
                            {
                                var ff = new i360Forecast();

                                ff.HourOffset = (int)reader.ReadUInt(5);
                                ff.Temperature = (int)reader.ReadUInt(7) - 70;

                                ///->

                                uint? _pressure = reader.ReadUIntNullable(8);

                                if (_pressure != null)
                                    ff.Pressure = (int)_pressure + 580;

                                ///->

                                uint? _cloud = reader.ReadUIntNullable(4);

                                if (_cloud != null)
                                    ff.Cloud = Math.Min(100, (int)_cloud * 15);

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

                                ff.WindSpeed = reader.ReadUIntNullable(6);

                                ///->

                                ff.SnowRisk = reader.ReadBoolean();

                                ///->

                                forecasts.Add(ff);
                            }
                            catch (Exception e)
                            {
                                Debugger.Break();
                            }
                        }

                        d.Forecasts = forecasts;
                        days.Add(d);

                    }

                    Forecast.DayInfos = days;
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
        public static WeatherMT Create(i360PointForecast forecast)
        {
            WeatherMT weather = new WeatherMT();

            weather.Forecast = forecast;
            // --->
            return weather;
        }
    }

}

