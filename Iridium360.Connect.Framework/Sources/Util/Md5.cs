using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Iridium360.Connect.Framework.Util
{
    public static class Md5
    {
        public static string Get(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }
}
