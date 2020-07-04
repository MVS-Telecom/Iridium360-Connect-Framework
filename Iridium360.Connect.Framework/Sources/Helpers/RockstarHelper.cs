using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Iridium360.Connect.Framework.Helpers
{
    public static class RockstarHelper
    {
        private static readonly Regex RockStar = new Regex(@"^[2]\d{4}$", RegexOptions.Compiled);
        private static readonly Regex RockFleet = new Regex(@"^[5]\d{4}$", RegexOptions.Compiled);
        private static readonly Regex RockAir = new Regex(@"^[1]\d{5}$", RegexOptions.Compiled);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public static DeviceType? GetTypeBySerial(string serial)
        {
            if (RockStar.IsMatch(serial))
                return DeviceType.RockStar;

            if (RockFleet.IsMatch(serial))
                return DeviceType.RockFleet;

            if (RockAir.IsMatch(serial))
                return DeviceType.RockAir;

            return null;
        }

        /// <summary>
        /// Получить тип устройства по названию (в названии есть серийный номер)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DeviceType? GetTypeByName(string name)
        {
            string serial = GetSerialFromName(name);
            return GetTypeBySerial(serial);
        }


        /// <summary>
        /// Получить серийный номер из блютуз имени устройства
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSerialFromName(string name)
        {
            ///HACK: Для ДЕМО устройств
            name = name?.Replace("ᴰᴱᴹᴼ", "");

            string serial = name?.Trim()?.Split(' ')?.LastOrDefault() ?? string.Empty;
            return serial;
        }
    }
}
