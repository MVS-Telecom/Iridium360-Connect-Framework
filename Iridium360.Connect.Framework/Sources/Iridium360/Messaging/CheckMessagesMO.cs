using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class CheckMessagesMO : MessageWithLocation, IMessageMO
    {
        public override Direction Direction => Direction.MO;

        public override MessageType Type => MessageType.CheckMessages;


        [Flags]
        public enum CheckFlags
        {
            EMPTY = 0,
            HasByskyToken = 1,
        }


        /// <summary>
        /// 
        /// </summary>
        private CheckMessagesMO()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="byskyToken"></param>
        /// <returns></returns>
        public static CheckMessagesMO Create(ShortGuid? byskyToken = null)
        {
            CheckMessagesMO emo1 = new CheckMessagesMO();
            emo1.ByskyToken = byskyToken;
            return emo1;
        }


        protected override void pack(BinaryBitWriter writer)
        {
            CheckFlags flags = CheckFlags.EMPTY;
            // --->

            if (ByskyToken != null)
            {
                flags |= CheckFlags.HasByskyToken;
            }

            //
            // --->
            //
            writer.Write((byte)((byte)flags));
            //
            // --->
            //

            if (flags.HasFlag(CheckFlags.HasByskyToken))
            {
                writer.Write(ByskyToken.Value.Guid.ToByteArray());
            }
        }

        protected override void unpack(BinaryBitReader reader)
        {
            CheckFlags flags = (CheckFlags)reader.ReadByte();

            if (flags.HasFlag(CheckFlags.HasByskyToken))
            {
                ByskyToken = new ShortGuid(reader.ReadBytes(16));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ShortGuid? ByskyToken { get; protected set; }
    }
}
