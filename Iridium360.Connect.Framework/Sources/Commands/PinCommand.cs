using Rock.Core;
using Rock.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace Rock.Commands
{
    public class PinCommand : BaseCommand
    {
        private readonly short oldPin;
        private readonly short newPin;


        public PinCommand(string appId, byte key, short oldPin, short newPin)
            : base(CommandType.Pin, appId, key)
        {
            this.oldPin = oldPin;
            this.newPin = newPin;
        }


        protected override void AddCustomPayload(ByteBuffer buffer)
        {
            buffer.WriteInt16(oldPin);
            buffer.WriteInt16(newPin);
        }
    }
}
