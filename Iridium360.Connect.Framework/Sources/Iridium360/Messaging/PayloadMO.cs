using System;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    public class PayloadMO : MessageMO
    {
        public static PayloadMO Create(ProtocolVersion version, byte[] content)
        {
            PayloadMO dmo1 = Create<PayloadMO>(version);
            dmo1.Content = content;
            return dmo1;
        }

        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write(this.Content);
        }

        protected override void unpack(BinaryBitReader reader)
        {
            this.Content = reader.ReadAllBytes();
        }

        public override MessageType Type =>
            MessageType.Payload;

        public byte[] Content { get; protected set; }
    }
}

