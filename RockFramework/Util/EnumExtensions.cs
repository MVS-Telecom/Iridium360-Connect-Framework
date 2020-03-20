using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rock.Util
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Кэш для атрибутов значений перечислений
        /// </summary>
        private static ConcurrentDictionary<Type, IDictionary<string, Attribute[]>> typeValuesAttrDict = new ConcurrentDictionary<Type, IDictionary<string, Attribute[]>>();


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum value, bool inherit = false) where T : Attribute
        {
            if (null == value)
                return null;
            // --->

            var attribute = GetAttributes(value, inherit).FirstOrDefault(a => a.GetType() == typeof(T));
            return attribute as T;
        }

        /// <summary>
        /// Получить атрибуты у enum-a
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static Attribute[] GetAttributes(this Enum value, bool inherit = false)
        {
            if (null == value)
                return null;
            // --->

            var enumType = value.GetType();
            var enumAttrDict = typeValuesAttrDict.GetOrAdd(enumType, type =>
            {
                var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);
                var dict = new Dictionary<string, Attribute[]>();
                foreach (var field in fields)
                {
                    var enumStrValue = field.Name;
                    var attributes = field.GetCustomAttributes(inherit).Select(a => (Attribute)a).ToArray();
                    dict.Add(enumStrValue, attributes);
                }

                return dict;
            });

            // --->
            enumAttrDict.TryGetValue(value.ToString(), out var result);
            result = result ?? new Attribute[0];
            return result;
        }


        public static bool HasAttribute<T>(this Enum value)  where T : Attribute
        {
            return GetAttribute<T>(value) != null;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <param name="matchTo"></param>
        /// <returns></returns>
        public static bool IsSet(this Enum input, Enum matchTo)
        {
            if (0 == Convert.ToUInt32(input) && 0 == Convert.ToUInt32(matchTo))
                return true;
            // --->
            return (Convert.ToUInt32(input) & Convert.ToUInt32(matchTo)) != 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> Flags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (IsSet(input, value))
                    yield return value;
        }

    }



}
