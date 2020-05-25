using System;
using System.Collections.Generic;
using System.IO;

namespace Iridium360.Connect.Framework.Messaging
{
    public class i360Forecast
    {
        /// <summary>
        /// Дата и время прогноза
        /// </summary>
        public int HourOffset { get; set; }


        /// <summary>
        /// Температура в цельсиях
        /// </summary>
        public int Temperature { get; set; }


        /// <summary>
        /// Давление в мм рт.с
        /// </summary>
        public int? _pressure { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? Pressure
        {
            get
            {
                if (_pressure == null)
                    return null;

                return _pressure + 580;
            }
            set
            {
                if (value == null)
                {
                    _pressure = value;
                    return;
                }

                _pressure = value.Value - 580;
            }
        }

        /// <summary>
        /// Облачность в процентах (%)
        /// </summary>
        public int? _cloud { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? Cloud
        {
            get
            {
                if (_cloud == null)
                    return null;

                return (int)Math.Round(_cloud.Value * 20d);
            }
            set
            {
                if (value == null)
                {
                    _cloud = null;
                    return;
                }

                _cloud = (int)Math.Round(value.Value / 20d);
            }
        }

        /// <summary>
        /// Осадки в мм
        /// </summary>
        public int? _precipitation { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double? Precipitation
        {
            get
            {
                if (_precipitation == null)
                    return null;

                ///[0, 240] --> [0.0, 60.0] 
                return _precipitation / 4d;
            }
            set
            {
                if (value == null)
                {
                    _precipitation = null;
                    return;
                }

                if (value.Value >= 0.05 && value.Value <= 0.1)
                {
                    _precipitation = 1;
                }
                else
                {
                    ///[0.0, 60.0] --> [0, 240]
                    _precipitation = (int)Math.Round(value.Value * 4d);
                }

            }
        }


        /// <summary>
        /// Направление ветра (в градусах)
        /// </summary>
        public int? _windDirection { get; set; }

        public int? WindDirection
        {
            get
            {
                if (_windDirection == null)
                    return null;

                ///[0, 8] --> [0, 360]  
                return (int)Math.Round(_windDirection.Value * 45d);
            }
            set
            {
                if (value == null)
                {
                    _windDirection = null;
                    return;
                }


                ///[0, 360] --> [0, 8]
                _windDirection = (int)Math.Round(value.Value / 45d);
            }
        }

        /// <summary>
        /// Скорость ветра м/с
        /// </summary>
        public int? _windSpeed { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public double? WindSpeed
        {
            get
            {
                if (_windSpeed == null)
                    return null;

                ///[0, 60] --> [0.0, 30.0]
                return _windSpeed.Value / 2d;
            }
            set
            {
                if (value == null)
                {
                    _windSpeed = null;
                    return;
                }

                ///[0.0, 30.0] --> [0, 60]
                _windSpeed = (int)Math.Round(value.Value * 2);
            }
        }


        /// <summary>
        /// Вероятность снега
        /// </summary>
        public bool? SnowRisk { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class i360DayInfo
    {
        private static DateTime START = new DateTime(2020, 1, 1);

        /// <summary>
        /// Кол-во дней с 1 января 2018 года
        /// </summary>
        public int Day { get; set; }


        /// <summary>
        /// Дата (в UTC)
        /// </summary>
        public DateTime DateDay
        {
            get
            {
                return DateTime.SpecifyKind(START.AddDays(Day), DateTimeKind.Utc);
            }
            set
            {
                Day = (int)(value - START).TotalDays;
            }
        }


        /// <summary>
        /// Список прогнозов по временным интервалам
        /// </summary>
        public List<i360Forecast> Forecasts { get; set; }

    }


    /// <summary>
    /// 
    /// </summary>
    public class i360PointForecast
    {
        /// <summary>
        /// 
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Lon { get; set; }

        /// <summary>
        /// Часовой пояс
        /// </summary>
        public int TimeOffset { get; set; }

        /// <summary>
        /// Информация по дням прогноза
        /// </summary>
        public List<i360DayInfo> DayInfos { get; set; }
    }



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



        public static int GetBits(int max)
        {
            int value = Math.Max(1, (int)Math.Ceiling(Math.Log(max, 2)));
            return value;
        }

        public static int GetBits(int min, int max)
        {
            return GetBits(max - min + 1);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="biterator"></param>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        private void location(Biterator biterator, double lat, double lon)
        {

        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            var biterator = new Biterator();

            biterator.PushInt(Forecasts.Count,
                GetBits(max: 4));

            foreach (var f in Forecasts)
            {
                biterator.PushFloat((float)f.Lat, true, 5);
                biterator.PushFloat((float)f.Lon, true, 5);

                biterator.PushInt(f.TimeOffset,
                    GetBits(min: -12, max: 14));

                biterator.PushUInt((uint)f.DayInfos.Count,
                    GetBits(max: 5));

                foreach (var d in f.DayInfos)
                {
                    biterator.PushUInt((uint)d.Day,
                        GetBits(min: 0, max: 14600));

                    biterator.PushUInt((uint)d.Forecasts.Count,
                        GetBits(max: 8));

                    foreach (var ff in d.Forecasts)
                    {
                        biterator.PushUInt((uint)ff.HourOffset,
                            GetBits(min: 0, max: 24));

                        biterator.PushInt(ff.Temperature,
                            GetBits(min: -70, max: 50));

                        biterator.PushUInt((uint)(ff._cloud ?? 21),
                            GetBits(min: 0, max: 21));

                        biterator.PushUInt((uint)(ff._precipitation ?? 241),
                            GetBits(min: 0, max: 241));

                        biterator.PushUInt((uint)(ff._windDirection ?? 9),
                            GetBits(min: 0, max: 9));

                        biterator.PushUInt((uint)(ff._windSpeed ?? 61),
                            GetBits(min: 0, max: 61));

                        biterator.PushUInt((uint)(ff._pressure ?? 221),
                            GetBits(min: 0, max: 221));

                        biterator.PushUInt((uint)(ff.SnowRisk != null ? (ff.SnowRisk.Value ? 1 : 0) : 2),
                            GetBits(min: 0, max: 2));
                    }

                }
            }

            var b = biterator.GetUsedBytes();
            writer.Write(b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(byte[] payload)
        {
            var biterator = new Biterator(payload);

            var size = biterator.PopInt(GetBits(max: 4));
            var list = new List<i360PointForecast>();

            for (int i = 0; i < size; i++)
            {
                var f = new i360PointForecast();

                f.Lat = biterator.PopFloat(true, 5);
                f.Lon = biterator.PopFloat(true, 5);

                f.TimeOffset = biterator.PopInt(GetBits(min: -12, max: 14));

                var size2 = biterator.PopUInt(GetBits(max: 5));
                var list2 = new List<i360DayInfo>();

                for (int j = 0; j < size2; j++)
                {
                    var d = new i360DayInfo();

                    d.Day = (int)biterator.PopUInt(GetBits(min: 0, max: 14600));

                    var size3 = biterator.PopUInt(GetBits(max: 8));
                    var list3 = new List<i360Forecast>();

                    for (int k = 0; k < size3; k++)
                    {
                        var ff = new i360Forecast();

                        ff.HourOffset = (int)biterator.PopUInt(GetBits(min: 0, max: 24));

                        ff.Temperature = biterator.PopInt(GetBits(min: -70, max: 50));

                        ff._cloud = (int)biterator.PopUInt(GetBits(min: 0, max: 21));

                        if (ff._cloud == 21)
                            ff._cloud = null;

                        ff._precipitation = (int)biterator.PopUInt(GetBits(min: 0, max: 241));

                        if (ff._precipitation == 241)
                            ff._precipitation = null;

                        ff._windDirection = (int)biterator.PopUInt(GetBits(min: 0, max: 9));

                        if (ff._windDirection == 9)
                            ff._windDirection = null;

                        ff._windSpeed = (int)biterator.PopUInt(GetBits(min: 0, max: 61));

                        if (ff._windSpeed == 61)
                            ff._windSpeed = null;

                        ff._pressure = (int)biterator.PopUInt(GetBits(min: 0, max: 221));

                        if (ff._pressure == 221)
                            ff._pressure = null;

                        int? _snowRisk = (int)biterator.PopUInt(GetBits(min: 0, max: 2));

                        if (_snowRisk == 2)
                            ff.SnowRisk = null;
                        else
                            ff.SnowRisk = _snowRisk == 1 ? true : false;

                        list3.Add(ff);
                    }

                    d.Forecasts = list3;
                    list2.Add(d);

                }

                f.DayInfos = list2;
                list.Add(f);
            }


            Forecasts = list;
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

