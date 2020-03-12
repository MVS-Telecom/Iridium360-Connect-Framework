using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Helpers
{
    public static class Checksum
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short Get(byte[] bytes, int value)
        {
            short s = 0;
            for (byte b = 0; value > 0; b++)
            {
                value--;
                int i = (s >> 8 ^ bytes[b]) & 0xFF;
                i ^= i >> 4;
                s = (short)(s << 8 ^ i << 12 ^ i << 5 ^ i);
                s = (short)(s & 0xFFFF);
            }
            return s;
        }
    }
}
