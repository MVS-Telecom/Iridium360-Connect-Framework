using Rock.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Commands
{
    public class DeleteMessageCommand : BaseCommand
    {
        private readonly short messageId;

        public DeleteMessageCommand(short messageId, string appId, short key)
            : base(CommandType.DeleteMessage, appId, key)
        {
            this.messageId = messageId;
        }

        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteInt16(messageId);
        }
    }
}
