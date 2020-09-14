using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class MessageAckMT : MessageMT
    {
        public override MessageType Type => MessageType.Resend;

        public byte[] ResendIndexes { get; private set; }
        public byte TargetGroup { get; private set; }

        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write(TargetGroup);

            writer.Write((byte)ResendIndexes.Length);

            foreach (byte index in ResendIndexes)
                writer.Write(index);
        }

        protected override void unpack(BinaryBitReader reader)
        {
            TargetGroup = reader.ReadByte();

            int length = reader.ReadByte();
            ResendIndexes = new byte[length];

            for (int i = 0; i < length; i++)
                ResendIndexes[i] = reader.ReadByte();
        }

        public static MessageAckMT Create(ProtocolVersion version, byte targetGroup, byte[] resendIndexes)
        {
            var mt = Create<MessageAckMT>(version);
            mt.TargetGroup = targetGroup;
            mt.ResendIndexes = resendIndexes;
            return mt;
        }
    }
}
