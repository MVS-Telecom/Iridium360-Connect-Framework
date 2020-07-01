namespace Iridium360.Connect.Framework.Implementations
{
    internal class AcknowledgeMessageStatusCommand : BaseCommand
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
