using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;


namespace Iridium360.Connect.Framework.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class i360Forecast
    {
        /// <summary>
        /// Дата и время прогноза
        /// </summary>
        [JsonIgnore]
        public int HourOffset => Date.Hour;

        /// <summary>
        /// Дата
        /// </summary>
        [JsonProperty("d")]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Температура в цельсиях
        /// </summary>
        [JsonProperty("t")]
        public int Temperature { get; set; }

        /// <summary>
        /// Давление в мм рт.с
        /// </summary>
        [JsonProperty("p")]
        public int? Pressure { get; set; }

        /// <summary>
        /// Облачность в процентах (%)
        /// </summary>
        [JsonProperty("c")]
        public int? Cloud { get; set; }

        /// <summary>
        /// Высота облаков в метрах
        /// </summary>
        [JsonProperty("ch")]
        public int? CloudHeight { get; set; }

        /// <summary>
        /// Осадки в мм
        /// </summary>
        [JsonProperty("pr")]
        public double? Precipitation { get; set; }

        /// <summary>
        /// Направление ветра (в градусах)
        /// </summary>
        [JsonProperty("wd")]
        public int? WindDirection { get; set; }

        /// <summary>
        /// Скорость ветра м/с
        /// </summary>
        [JsonProperty("ws")]
        public double? WindSpeed { get; set; }

        /// <summary>
        /// Вероятность снега
        /// </summary>
        [JsonProperty("s")]
        public bool SnowRisk { get; set; }

        /// <summary>
        /// Видимость в метрах
        /// </summary>
        [JsonProperty("v")]
        public int? Visibility { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class IconAttribute : Attribute
    {
        public string Key { get; set; }

        public IconAttribute(string key) : base()
        {
            Key = key;
        }
    }


    /// <summary>
    /// фазы луны
    /// </summary>
    public enum MoonPhase
    {
        /// <summary>
        /// Новолуние ic_moon_phase_first_quarter
        /// </summary>
        [Translation("$=moon_new_moon$$")]
        [Icon("ic_moon_phase_new_moon")]
        NewMoon = 1,

        /// <summary>
        /// Молодая луна
        /// </summary>
        [Translation("$=moon_waxing_crescent$$")]
        [Icon("ic_moon_phase_waxing_crescent")]
        WaxingСresent = 2,

        /// <summary>
        /// Первая четверть
        /// </summary>
        [Translation("$=moon_first_quarter$$")]
        [Icon("ic_moon_phase_first_quarter")]
        FirstQuarter = 3,

        /// <summary>
        /// Растущая луна
        /// </summary>
        [Translation("$=moon_waxing_gibbous$$")]
        [Icon("ic_moon_phase_waxing_gibbous")]
        WaxingGibbous = 4,

        /// <summary>
        /// Полнолуние
        /// </summary>
        [Translation("$=moon_full_moon$$")]
        [Icon("ic_moon_phase_full_moon")]
        FullMoon = 5,

        /// <summary>
        /// Убывающая луна
        /// </summary>
        [Translation("$=moon_waning_gibbous$$")]
        [Icon("ic_moon_phase_waning_gibbous")]
        WanningGibbous = 6,

        /// <summary>
        /// Последняя четверть
        /// </summary>
        [Translation("$=moon_last_quarter$$")]
        [Icon("ic_moon_phase_last_quarter")]
        LastGuarter = 7,

        /// <summary>
        /// Старая луна
        /// </summary>
        [Translation("$=moon_waning_crescent$$")]
        [Icon("ic_moon_phase_waning_crescent")]
        WaningCrescent = 8,
    }


    /// <summary>
    /// 
    /// </summary>
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class i360DayInfo
    {
        /// <summary>
        /// Дата (в UTC)
        /// </summary>
        [JsonProperty("date")]
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int Day => (int)(Date - new DateTime(2020, 1, 1)).TotalDays;


        /// <summary>
        /// Восход солнца (в минутах)
        /// </summary>
        [JsonProperty("sunrise")]
        private int? _sunRise { get; set; }

        /// <summary>
        /// Закат солнца (в минутах)
        /// </summary>
        [JsonProperty("sunset")]
        private int? _sunSet { get; set; }


        /// <summary>
        /// Восход (локальное время)
        /// </summary>
        [JsonIgnore]
        public DateTime? SunRise
        {
            get
            {
                if (_sunRise == null)
                    return null;

                return DateTime.SpecifyKind(Date.AddMinutes(_sunRise.Value), DateTimeKind.Local);
            }
            set
            {
                if (value != null)
                    _sunRise = (int)value.Value.TimeOfDay.TotalMinutes;
                else
                    _sunRise = null;
            }
        }


        /// <summary>
        /// Закат (локальное время)
        /// </summary>
        [JsonIgnore]
        public DateTime? SunSet
        {
            get
            {
                if (_sunSet == null)
                    return null;

                return DateTime.SpecifyKind(Date.AddMinutes(_sunSet.Value), DateTimeKind.Local);
            }
            set
            {
                if (value != null)
                    _sunSet = (int)value.Value.TimeOfDay.TotalMinutes;
                else
                    _sunSet = null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("l")]
        public int? DayLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("moon")]
        public MoonPhase? MoonPhase { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("gust")]
        public double? WindGust { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class i360PointForecast
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lat")]
        public double? Lat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lon")]
        public double? Lon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("key")]
        public string PointKey { get; set; }

        /// <summary>
        /// Название места 
        /// </summary>
        [JsonProperty("place")]
        public string PlaceName { get; set; }

        /// <summary>
        /// Часовой пояс
        /// </summary>
        [JsonProperty("zone")]
        public int TimeOffset { get; set; }

        /// <summary>
        /// Информация по дням прогноза
        /// </summary>
        [JsonProperty("days")]
        public List<i360DayInfo> DayInfos { get; set; }

        /// <summary>
        /// Список прогнозов по временным интервалам
        /// </summary>
        [JsonProperty("forecasts")]
        public List<i360Forecast> Forecasts { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class i360WeatherForecast
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("data")]
        public List<i360PointForecast> Forecasts { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class i360ForecastPoint
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
        /// 
        /// </summary>
        public string Key { get; set; }
    }

}
