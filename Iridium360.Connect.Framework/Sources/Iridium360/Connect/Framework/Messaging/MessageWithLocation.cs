using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public abstract class MessageWithLocation : Message
    {
        public double? Lat { get; protected set; }
        public double? Lon { get; protected set; }
        public uint? Alt { get; protected set; }


        protected void WriteLocation(BinaryBitWriter writer)
        {
            if (Direction != Direction.MO)
                throw new InvalidOperationException();

            if (Lat == null)
                throw new ArgumentException("Latitude not specified");

            if (Lon == null)
                throw new ArgumentException("Longitude not specified");

            if (Alt == null)
                throw new ArgumentException("Altitude not specified");


            writer.Write((float)Lat, true, 7, 9);
            writer.Write((float)Lon, true, 8, 9);
            writer.Write((uint)Math.Min(16383, Alt.Value), 14);
        }

        protected void ReadLocation(BinaryBitReader reader)
        {
            Lat = reader.ReadFloat(true, 7, 9);
            Lon = reader.ReadFloat(true, 8, 9);
            Alt = reader.ReadUInt(14);
        }
    }
}
