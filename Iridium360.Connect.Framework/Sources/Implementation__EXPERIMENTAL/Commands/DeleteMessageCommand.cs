namespace Iridium360.Connect.Framework.Implementations
{
    internal class DeleteMessageCommand : BaseCommand
    {
        private readonly short messageId;

        public DeleteMessageCommand(short messageId, string appId, byte key)
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
