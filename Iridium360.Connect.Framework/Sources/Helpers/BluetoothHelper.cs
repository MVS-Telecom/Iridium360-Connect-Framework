using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Helpers
{
    public static class BluetoothHelper
    {
        public static string ToBluetoothAddress(this Guid deviceId)
        {
            var address = deviceId
                .ToByteArray()
                .Skip(10)
                .Take(6)
                .ToArray();

            return BitConverter
                .ToString(address)
                .Replace("-", ":")
                .ToUpperInvariant();
        }

    }

}
