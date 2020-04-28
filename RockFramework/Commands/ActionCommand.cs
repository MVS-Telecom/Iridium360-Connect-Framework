using Rock.Core;
using Rock.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rock.Commands
{
    public enum MessageStatus : int
    {
        Pending,
        ReceivedByDevice,
        QueuedForTransmission,
        Transmitting,
        Transmitted,
        Received,
        ErrorToolong,
        ErrorNoCredit,
        ErrorCapability,
        Error
    }

    public enum LockState : int
    {
        Unknown = 0,
        Unlocked = 1,
        Locked = 2,
        IncorrectPin = 3,
    }

    public enum ActionRequestType : int
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


    public class ActionCommand : BaseCommand
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
