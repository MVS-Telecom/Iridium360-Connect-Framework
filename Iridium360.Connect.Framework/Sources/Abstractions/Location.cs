using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework
{
    public enum LocationSource : int
    {
        Device = 0,
        Smartphone = 1
    }


    public class Location
    {
        public float? Accuracy { get; set; }
        public double Latitude { get; set; }
        public double? Altitude { get; set; }
        public double Longitude { get; set; }
        public float? Speed { get; set; }
        public float? Bearing { get; set; }
        public DateTime? Date { get; set; }
        public LocationSource? Source { get; set; }

        public Location()
        {

        }

        public Location(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }


        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
