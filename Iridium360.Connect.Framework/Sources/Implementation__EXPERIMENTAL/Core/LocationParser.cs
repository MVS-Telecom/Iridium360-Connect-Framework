using Iridium360.Connect.Framework.Helpers;

namespace Iridium360.Connect.Framework.Implementations
{
    internal static class LocationParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="revisedPositionFormat"></param>
        /// <returns></returns>
        public static Location Parse(byte[] data, string firmware)
        {
            bool revisedPositionFormat = DeviceHelper.IsCapabilitySupported(firmware, DeviceCapability.RevisedPositionFormat);

            Location location = revisedPositionFormat
                ? ParseLocation_RevisedFormat(data)
                : ParseLocation(data);

#if DEBUG
            Location location1 = ParseLocation_RevisedFormat(data);
            Location location2 = ParseLocation(data);
#endif

            return location;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_data"></param>
        /// <returns></returns>
        public static Location ParseLocation(byte[] _data)
        {
            double d = ((

                ((_data[0] & (ulong)255) << 17) +
                ((_data[1] & (ulong)255) << 9) +
                ((_data[2] & (ulong)255) << 1) +
                ((_data[3] & (ulong)128) >> 7))

                / 100000.0d) - 90.0d;

            double d2 = ((

                ((_data[3] & (ulong)sbyte.MaxValue) << 19) +
                ((_data[4] & (ulong)255) << 11) +
                ((_data[5] & (ulong)255) << 3) +
                ((_data[6] & (ulong)224) >> 5))

                / 100000.0d) - 180.0d;


            int i = ((_data[6] & 31) << 12) + ((_data[7] & 255) << 4) + ((_data[8] & 240) >> 4);

            int i2 = ((_data[9] & 31) << 4) + ((_data[10] & 240) >> 4);
            float f = ((_data[11] >> 2) & 1) == 1 ? ((float)(((_data[10] & 15) << 6) + ((_data[11] & 252) >> 2))) / 10.0f : 0.0f;
            int i3 = ((_data[11] & 3) << 12) + ((_data[12] & 255) << 4) + ((_data[13] & 240) >> 4);
            if (i3 > 15800)
            {
                i3 -= 16384;
            }
            double d3 = (((double)(((_data[13] & 15) << 7) + ((_data[14] & 254) >> 1))) / 10.0d) - 40.0d;
            //Date a = m316a(((bArr[14] & 1) << 8) + ((bArr[15] & 255) >> 0), i);

            Location location = new Location();
            location.Accuracy = 0.0f;
            location.Latitude = d;
            location.Longitude = d2;
            location.Altitude = (double)i3;
            //location.Time=a.getTime();
            location.Speed = f;
            location.Bearing = (float)i2;
            return location;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Location ParseLocation_RevisedFormat(byte[] data)
        {
            byte[] _data = data.GetCopy(2, data.Length - 1);

            double d = ((

                ((_data[0] & (ulong)255) << 17) +
                ((_data[1] & (ulong)255) << 9) +
                ((_data[2] & (ulong)255) << 1) +
                ((_data[3] & (ulong)128) >> 7))

                / 100000.0d) - 90.0d;

            double d2 = ((

                ((_data[3] & (ulong)sbyte.MaxValue) << 19) +
                ((_data[4] & (ulong)255) << 11) +
                ((_data[5] & (ulong)255) << 3) +
                ((_data[6] & (ulong)224) >> 5))

                / 100000.0d) - 180.0d;


            int i = ((_data[6] & 31) << 12) + ((_data[7] & 255) << 4) + ((_data[8] & 240) >> 4);

            int i2 = ((_data[9] & 31) << 4) + ((_data[10] & 240) >> 4);
            int i3 = ((_data[11] & 252) >> 2) + ((_data[10] & 15) << 6);
            float f = 0.0f;
            if (((_data[11] >> 2) & 1) == 1)
            {
                f = ((float)i3) / 10.0f;
            }
            int i4 = ((_data[11] & 3) << 12) + ((_data[12] & 255) << 4) + ((_data[13] & 240) >> 4);
            if (i4 > 15800)
            {
                i4 -= 16384;
            }
            byte b = (byte)(_data[15] & 63);
            //DateTime withDayOfMonth = new DateTime().toDateTime(DateTimeZone.UTC).withTimeAtStartOfDay().withDayOfMonth(1);
            //if (withDayOfMonth.Month % 2 == 0)
            //{
            //    withDayOfMonth = withDayOfMonth.AddMonths(-1);
            //}

            //DateTime plusSeconds = withDayOfMonth
            //    .AddDays(b - 1)
            //    .AddSeconds(i);

            //if (plusSeconds.isAfter((ReadableInstant)new DateTime()))
            //{
            //    plusSeconds = plusSeconds.AddMonths(-2);
            //}
            float f2 = (float)(((double)_data[16]) / 4.0d);

            Location location = new Location();
            location.Accuracy = 0.0f;
            location.Latitude = d;
            location.Longitude = d2;
            location.Altitude = (double)i4;
            //location.setTime(plusSeconds.getMillis());
            location.Speed = f;
            location.Bearing = (float)i2;
            location.Accuracy = f2;
            return location;
        }
    }
}
