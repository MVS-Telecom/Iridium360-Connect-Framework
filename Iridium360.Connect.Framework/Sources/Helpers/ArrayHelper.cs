using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Helpers
{
    public static class C0056cb
    {
        /* renamed from: a */
        private static byte[] m151a(byte[] bArr, int i)
        {
            byte[] bArr2 = new byte[i];
            Array.Copy(bArr, 0, bArr2, 0, Math.Min(bArr.Length, i));
            return bArr2;
        }

        /* renamed from: a */
        public static byte[] m152a(byte[] bArr, int i, int i2)
        {
            if (i >= 0 && i2 >= 0)
                return bArr.Length < i ? m151a(bArr, i + i2) : bArr;

            throw new ArgumentException();
        }

        /* renamed from: a */
        public static byte[] m153a(ArrayList bArr)
        {
            int i = 0;
            foreach (byte[] length in bArr)
            {
                i += length.Length;
            }
            byte[] bArr2 = new byte[i];
            int i2 = 0;
            foreach (byte[] bArr3 in bArr)
            {
                Array.Copy(bArr3, 0, bArr2, i2, bArr3.Length);
                i2 += bArr3.Length;
            }
            return bArr2;
        }

    }

    public static class ArrayHelper
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
