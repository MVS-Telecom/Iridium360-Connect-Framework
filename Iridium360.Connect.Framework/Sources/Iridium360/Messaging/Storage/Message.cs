using Realms;
using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Storage
{
    public class Message
    {
        public string Id { get; set; }
        public DateTime? Date { get; set; }
        public byte Group { get; set; }
        public byte TotalParts { get; set; }
        public byte[] Bytes { get; set; }
    }
}
