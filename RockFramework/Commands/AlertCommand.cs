using Rock.Core;

namespace Rock.Commands
{
    public class AlertCommand : BaseCommand
    {
        private readonly GenericAlertType alert;
        private readonly bool position;

        public AlertCommand(string appId, byte key, GenericAlertType alert, bool position)
            : base(CommandType.GenericAlert, appId, key)
        {
            this.alert = alert;
            this.position = position;
        }

        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteByte((byte)((int)alert + 100));
            buffer.WriteByte(position ? (byte)1 : (byte)0);
        }
    }

}
