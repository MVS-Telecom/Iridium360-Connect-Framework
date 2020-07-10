using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Iridium360.Connect.Framework.Messaging
{
    public class EmailAttribute : Attribute { }
    public class PhoneAttribute : Attribute { }
    public class SuffixAttribute : Attribute
    {
        public string Value { get; set; }
        public SuffixAttribute(string value)
        {
            Value = value;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum SubscriberNetwork : int
    {
        [Suffix("mobile")]
        Mobile = 0,

        [Suffix("email")]
        Email = 1,

        [Suffix("rockstar")]
        Rockstar = 2,

        [Suffix("portal")]
        Portal = 3,

        [Suffix("group")]
        Group = 4,

        [Suffix("by-sky.net")]
        Bysky = 5,
    }


    public static class AttributeHelper
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

    }

    public class SubscriberSerializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Subscriber);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = (string)reader.Value;
            return Subscriber.FromString(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Subscriber subscriber = (Subscriber)value;
            writer.WriteValue(subscriber.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonConverter(typeof(SubscriberSerializer))]
    public struct Subscriber
    {
        /// <summary>
        /// 
        /// </summary>
        public static Subscriber PORTAL => new Subscriber("iridium360.ru", SubscriberNetwork.Portal);


        /// <summary>
        /// 
        /// </summary>
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                //#if DEBUG
                //                if (value?.Contains("@") == true)
                //                    Debugger.Break();
                //#endif

                number = value;
                UpdateDisplay();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public SubscriberNetwork Network
        {
            get
            {
                return network;
            }
            set
            {
                network = value;
                UpdateDisplay();
            }
        }

        private SubscriberNetwork network;
        private string number;
        private string display;

        /// <inheritdoc />
        public Subscriber(string number, SubscriberNetwork network)
        {
            this.network = SubscriberNetwork.Mobile;
            this.number = null;
            this.display = null;

            Number = number;
            Network = network;
        }

        /// <summary>
        /// Update cached email
        /// </summary>
        private void UpdateDisplay()
        {
            var suffixAttr = Network.GetAttribute<SuffixAttribute>();

            if (suffixAttr == null)
            {
                display = null;
                return;
            }

            if (string.IsNullOrEmpty(Number))
            {
                display = null;
                return;
            }

            display = $"{Number}@{suffixAttr.Value}";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="network"></param>
        /// <returns></returns>
        public static Subscriber Create(string number, SubscriberNetwork network)
        {
            if (string.IsNullOrEmpty(number))
                throw new NullReferenceException("number");

            return new Subscriber(number, network);
        }


        /// <summary>
        /// Deserialize <see cref="Subscriber"/> instance from string like "79549740562@iridium"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultNetwork">Default network for strings without @</param>
        /// <returns></returns>
        public static Subscriber FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Values is null or empty");

            int index = value.LastIndexOf('@');

            if (index == -1)
                throw new Exception("Invalid format");


            string _number = value?.Substring(0, index);
            string _network = SafeSubstring(value, index + 1, value.Length - index - 1);

            if (string.IsNullOrEmpty(_network))
                throw new Exception("Subscriber network is null");

            return new Subscriber(_number, GetNetworkByString(_network));
        }

        public static string SafeSubstring(string text, int start, int length)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            return text.Length <= start ? ""
                : text.Length - start <= length ? text.Substring(start)
                : text.Substring(start, length);
        }

        /// <summary>
        /// Cache like [string, SubscriberNetwork]:
        /// 
        /// "iridium"   |SubscriberNetwork.Iridium
        /// "rockstar"  |SubscriberNetwork.Rockstar
        /// "by-sky"    |SubscriberNetwork.Bysky
        /// 
        /// ...
        /// </summary>
        static readonly Dictionary<string, SubscriberNetwork> cache = Enum
                .GetValues(typeof(SubscriberNetwork))
                .Cast<SubscriberNetwork>()
                .ToDictionary(x => x.GetAttribute<SuffixAttribute>()?.Value ?? string.Empty);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        static private SubscriberNetwork GetNetworkByString(string network)
        {
            if (!cache.ContainsKey(network))
                throw new ArgumentException("Network is not recognized");

            return cache[network];
        }

        /// <summary>
        /// Serialize <see cref="Subscriber"/> instance to string like "79999740562:0"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{display}";
        }

        public static bool operator ==(Subscriber s1, Subscriber s2)
        {
            return s1.ToString() == s2.ToString();
        }
        public static bool operator !=(Subscriber s1, Subscriber s2)
        {
            return s1.ToString() != s2.ToString();
        }

        public static explicit operator Subscriber(string value)
        {
            return FromString(value);
        }

        public static explicit operator string(Subscriber value)
        {
            return value.ToString();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Number != null ? Number.GetHashCode() : 0) * 397) ^ (int)Network;
            }
        }

        private bool Equals(Subscriber other)
        {
            return string.Equals(Number, other.Number) && Network == other.Network;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Subscriber other && Equals(other);
        }
    }

}