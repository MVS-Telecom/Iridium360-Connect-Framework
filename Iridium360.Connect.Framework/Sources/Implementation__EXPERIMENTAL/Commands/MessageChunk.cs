using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class MessageChunk
    {
        public readonly byte[] Payload;
        public readonly short? MessageId;
        public readonly int Index;
        public readonly int TotalCount;
        public readonly CommandType CommandType;

        public MessageChunk(byte[] payload, short? messageId, int index, int totalCount, CommandType commandType)
        {
            this.Payload = payload;
            this.MessageId = messageId;
            this.Index = index;
            this.TotalCount = totalCount;
            this.CommandType = commandType;
        }
    }
}
