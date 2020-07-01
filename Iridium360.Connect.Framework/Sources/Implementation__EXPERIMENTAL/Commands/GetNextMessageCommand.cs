namespace Iridium360.Connect.Framework.Implementations
{
    internal class GetNextMessageCommand : BaseCommand
    {
        public GetNextMessageCommand(string appId, byte key)
            : base(CommandType.GetNextMessage, appId, key)
        {

        }


        protected override void AddCustomPayload(ByteBuffer buffer)
        {
        }
    }

}
