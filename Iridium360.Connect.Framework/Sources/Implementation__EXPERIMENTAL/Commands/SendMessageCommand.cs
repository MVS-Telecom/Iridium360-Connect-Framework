namespace Iridium360.Connect.Framework.Implementations
{
    internal class SendMessageCommand : BaseCommand
    {
        public readonly short MessageId;
        private byte[] body;


        public SendMessageCommand(string appId, byte key, short messageId, byte[] body)
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
