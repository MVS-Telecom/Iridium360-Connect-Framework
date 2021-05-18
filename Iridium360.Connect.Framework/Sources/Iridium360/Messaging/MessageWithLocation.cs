using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// Сообщение с координатами откуда оно было отправлено
    /// </summary>
    public abstract class MessageWithLocation : Message
    {
        /// <summary>
        /// Широта точки откуда было отправлено сообщение
        /// </summary>
        public double? Lat { get; protected set; }

        /// <summary>
        /// Долгота точки откуда было отправлено сообщение
        /// </summary>
        public double? Lon { get; protected set; }

        /// <summary>
        /// Высота точки откуда было отправлено сообщение
        /// </summary>
        public int? Alt { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteLocation(BinaryBitWriter writer)
        {
            if (Version >= ProtocolVersion.v2__LocationFix)
            {
                writer.Write((float)Lat, true, 7, 13);
                writer.Write((float)Lon, true, 8, 13);
            }
            else
            {
                writer.Write((float)Lat, true, 7, 9);
                writer.Write((float)Lon, true, 8, 9);
            }


            if (Alt != null)
            {
                writer.Write(true);
                writer.Write((uint)Math.Min(16383, Math.Max(0, Alt.Value)), 14);
            }
            else
            {
                writer.Write(false);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected void ReadLocation(BinaryBitReader reader)
        {
            if (Version >= ProtocolVersion.v2__LocationFix)
            {
                Lat = reader.ReadFloat(true, 7, 13);
                Lon = reader.ReadFloat(true, 8, 13);
            }
            else
            {
                Lat = reader.ReadFloat(true, 7, 9);
                Lon = reader.ReadFloat(true, 8, 9);
            }


            bool hasAlt = reader.ReadBoolean();

            if (hasAlt)
                Alt = (int)reader.ReadUInt(14);
        }
    }
}
