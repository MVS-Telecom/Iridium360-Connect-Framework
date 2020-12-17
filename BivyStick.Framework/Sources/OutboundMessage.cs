using System;
using System.Collections.Generic;
using System.Text;

namespace BivyStick.Sources
{
    internal class OutboundMessage
    {
        public byte messageType { get; set; }
        public int dataSize { get; set; }
        public byte[] data { get; set; }
    }
}
