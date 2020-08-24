using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class ResendMessagePartsMT : MessageMT
    {
        public override MessageType Type => MessageType.Resend;

        public byte[] ResendIndexes { get; private set; }
        public byte ResendGroup { get; private set; }

        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write(ResendGroup);

            writer.Write((byte)ResendIndexes.Length);

            foreach (byte index in ResendIndexes)
                writer.Write(index);
        }

        protected override void unpack(BinaryBitReader reader)
        {
            ResendGroup = reader.ReadByte();

            int length = reader.ReadByte();
            ResendIndexes = new byte[length];

            for (int i = 0; i < length; i++)
                ResendIndexes[i] = reader.ReadByte();
        }

        public static ResendMessagePartsMT Create(byte resendGroup, byte[] resendIndexes)
        {
            var mt = new ResendMessagePartsMT();
            mt.ResendGroup = resendGroup;
            mt.ResendIndexes = resendIndexes;
            return mt;
        }
    }
}
