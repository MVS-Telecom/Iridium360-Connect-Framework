using Rock.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Commands
{
    public class AcknowledgeMessageStatusCommand : BaseCommand
    {
        private short MessageId;

        public AcknowledgeMessageStatusCommand(short messageId, string appId, byte keyIndex) : base(CommandType.AcknowledgeMessageStatus, appId, keyIndex)
        {
            this.MessageId = messageId;
        }

        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteInt16(MessageId);
        }
    }
}
