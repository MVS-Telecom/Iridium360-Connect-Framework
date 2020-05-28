using System;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    public class PayloadMO : MessageMO
    {
        public static PayloadMO Create(byte[] payload)
        {
            PayloadMO dmo1 = new PayloadMO();
            dmo1.Payload = payload;
            return dmo1;
        }

        protected override void pack(BinaryBitWriter writer)
        {
            writer.Write(this.Payload);
        }

        protected override void unpack(BinaryBitReader reader)
        {
            this.Payload = reader.ReadAllBytes();
        }

        public override MessageType Type =>
            MessageType.Payload;

        public byte[] Payload { get; protected set; }
    }
}

