namespace Iridium360.Connect.Framework.Implementations
{
    internal enum ActionRequestType : int
    {
        SendAlert,
        SendManual,
        InstallUpdates,
        MailboxCheck,
        UpdateMessageStatus,
        PositionUpdateLastKnown,
        PositionUpdate,
        BatteryUpdate,
        ShippingMode,
        Activation,
        Beep,
        FactoryReset,
        GeofenceCentre,
        RequestGprsUpdate
    }


    internal class ActionCommand : BaseCommand
    {
        private readonly ActionRequestType actionRequestType;

        public ActionCommand(string appId, byte key, ActionRequestType actionRequestType) :
            base(CommandType.ActionRequest, appId, key)
        {
            this.actionRequestType = actionRequestType;
        }


        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteByte((byte)actionRequestType);
        }
    }
}
