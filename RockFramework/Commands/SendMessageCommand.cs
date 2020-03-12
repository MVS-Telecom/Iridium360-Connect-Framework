using Rock.Core;
using Rock.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Commands
{
    public class SendMessageCommand : BaseCommand
    {
        public readonly short MessageId;
        private byte[] body;


        public SendMessageCommand(string appId, short key, short messageId, byte[] body)
            : base(CommandType.SendMessage, appId, key)
        {
            this.MessageId = messageId;
            this.body = body;
        }


        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteInt16(MessageId);
            buffer.WriteBytes(body);
        }
    }
}
