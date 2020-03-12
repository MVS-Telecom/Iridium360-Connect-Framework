using Rock.Core;
using Rock.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rock.Commands
{
    public class GetNextMessageCommand : BaseCommand
    {
        public GetNextMessageCommand(string appId, short key)
            : base(CommandType.GetNextMessage, appId, key)
        {

        }


        protected override void AddCustomPayload(ByteBuffer buffer)
        {
        }
    }

}
