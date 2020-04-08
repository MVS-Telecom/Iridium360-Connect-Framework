using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Iridium360.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class i360Forecast
    {
        /// <summary>
        /// Дата и время прогноза
        /// </summary>
        [JsonProperty("h")]
        public int HourOffset { get; set; }

        /// <summary>
        /// Температура в цельсиях
        /// </summary>
        [JsonProperty("t")]
        public int? Temperature { get; set; }

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
        public bool? SnowRisk { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum MoonPhase
    {
        /// <summary>
        /// не известна
        /// </summary>
        None = 0,

        /// <summary>
        /// Новолуние 
        /// </summary>
        [Translation("Новолуние")]
        NewMoon = 1,

        /// <summary>
        /// Молодая луна
        /// </summary>
        [Translation("Молодая луна")]
        WaxingСresent = 2,

        /// <summary>
        /// Первая четверть
        /// </summary>
        [Translation("Первая четверть")]
        FirstQuarter = 3,

        /// <summary>
        /// Растущая луна
        /// </summary>
        [Translation("Растущая луна")]
        WaxingGibbous = 4,

        /// <summary>
        /// Полнолуние
        /// </summary>
        [Translation("Полнолуние")]
        FullMoon = 5,

        /// <summary>
        /// Убывающая луна
        /// </summary>
        [Translation("Убывающая луна")]
        WanningGibbous = 6,

        /// <summary>
        /// Последняя четверть
        /// </summary>
        [Translation("Последняя четверть")]
        LastGuarter = 7,

        /// <summary>
        /// Старая луна
        /// </summary>
        [Translation("Старая луна")]
        WaningCrescent = 8,
    }



    /// <summary>
    /// 
    /// </summary>
    public class i360DayInfo
    {
        private static DateTime START = new DateTime(2018, 1, 1);

        /// <summary>
        /// Кол-во дней с 1 января 2018 года
        /// </summary>
        [JsonProperty("day")]
        private int _day { get; set; }


        /// <summary>
        /// Дата (в UTC)
        /// </summary>
        [JsonIgnore]
        public DateTime DateDay
        {
            get
            {
                return DateTime.SpecifyKind(START.AddDays(_day), DateTimeKind.Utc);
            }
            set
            {
                _day = (int)(value - START).TotalDays;
            }
        }

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

                return DateTime.SpecifyKind(DateDay.AddMinutes(_sunRise.Value), DateTimeKind.Local);
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

                return DateTime.SpecifyKind(DateDay.AddMinutes(_sunSet.Value), DateTimeKind.Local);
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
        [JsonProperty("moon")]
        public MoonPhase MoonPhase { get; set; }

        /// <summary>
        /// Список прогнозов по временным интервалам
        /// </summary>
        [JsonProperty("forecasts")]
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
        [JsonProperty("lat")]
        public double Lat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lon")]
        public double Lon { get; set; }

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
        public int? TimeOffset { get; set; }

        /// <summary>
        /// Информация по дням прогноза
        /// </summary>
        [JsonProperty("days")]
        public List<i360DayInfo> DayInfos { get; set; }
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
