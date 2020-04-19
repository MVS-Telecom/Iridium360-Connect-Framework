using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class EmptyMO : MessageMO
    {
        protected EmptyMO()
        {
        }

        public static EmptyMO Create(string greeting = "Hello world ;)")
        {
            EmptyMO ymo1 = new EmptyMO();
            ymo1.Greeting = greeting;
            return ymo1;
        }

        protected override void pack(BinaryBitWriter writer)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(this.Greeting);
            writer.Write(bytes);
        }

        protected override void unpack(byte[] payload)
        {
            this.Greeting = Encoding.UTF8.GetString(payload);
        }

        public override MessageType Type =>
            MessageType.Empty;

        public string Greeting { get; protected set; }
    }
}

