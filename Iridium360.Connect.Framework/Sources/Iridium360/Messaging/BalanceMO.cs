using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public class BalanceMO : MessageMO
    {
        public override MessageType Type => MessageType.Balance;

        protected override void pack(BinaryBitWriter writer)
        {

        }

        protected override void unpack(BinaryBitReader reader)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="conversation"></param>
        /// <param name="text"></param>
        /// <param name="subject"></param>
        public static BalanceMO Create(ProtocolVersion version)
        {
            BalanceMO request = Create<BalanceMO>(version);

            // --->
            return request;
        }
    }
}
