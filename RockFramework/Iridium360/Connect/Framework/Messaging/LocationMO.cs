using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class Location
    {
        public double Lat { get; private set; }
        public double Lon { get; private set; }


        public void pack(BinaryBitWriter writer)
        {
            writer.Write((float)Lat, true, 7, 9);
            writer.Write((float)Lon, true, 8, 9);
        }

        public static Location unpack(BinaryBitReader reader)
        {
            float lat = reader.ReadFloat(true, 7, 9);
            float lon = reader.ReadFloat(true, 8, 9);

            return new Location(lat, lon);
        }


        /// <summary>
        /// 
        /// </summary>
        public Location(double lat, double lon)
        {
            this.Lat = lat;
            this.Lon = lon;
        }


        public override string ToString()
        {
            return $"{Lat},{Lon}";
        }

    }
}
