using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BivyStick.Sources
{
    internal static class ArrayHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<T> GetCopy<T>(this List<T> list, int start, int end)
        {
            return list
                .Skip(start)
                .Take(end - start + 1)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<T> GetCopySafety<T>(this List<T> list, int start, int end)
        {
            int count = end - start + 1;

            if (count > list.Count - start)
                count = list.Count - start;

            return list
                .Skip(start)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static byte[] GetCopy(this byte[] list, int start, int end)
        {
            return list.ToList().GetCopy(start, end).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static byte[] GetCopySafety(this byte[] list, int start, int end)
        {
            return list.ToList().GetCopySafety(start, end).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static byte[] GetCopy(this byte[] list)
        {
            return list.GetCopy(0, list.Length - 1);
        }
    }
}
