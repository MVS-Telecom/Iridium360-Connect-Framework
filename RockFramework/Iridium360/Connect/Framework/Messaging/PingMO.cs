namespace Iridium360.Connect.Framework.Messaging
{
    using System;

    public class PingMO : MessageMO
    {
        protected PingMO()
        {
        }

        public static PingMO Create() =>
            new PingMO();

        protected override void pack(BinaryBitWriter writer)
        {
        }

        protected override void unpack(byte[] payload)
        {
        }

        public override MessageType Type =>
            MessageType.Ping;
    }
}

