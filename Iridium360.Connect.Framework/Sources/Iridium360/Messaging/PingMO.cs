namespace Iridium360.Connect.Framework.Messaging
{
    using System;

    public class PingMO : MessageMO
    {
        protected PingMO()
        {
        }

        public static PingMO Create(ProtocolVersion version) =>
            Create<PingMO>(version);

        protected override void pack(BinaryBitWriter writer)
        {
        }

        protected override void unpack(BinaryBitReader reader)
        {
        }

        public override MessageType Type =>
            MessageType.Ping;
    }
}

