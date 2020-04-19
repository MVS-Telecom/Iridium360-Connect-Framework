namespace Iridium360.Connect.Framework.Messaging
{
    using System;

    /// <summary>
    ///  Вх на RockSTAR сообщение
    /// </summary>
    public abstract class MessageMT : Message
    {

        /// <summary>
        /// 
        /// </summary>
        protected MessageMT()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public override Iridium360.Connect.Framework.Messaging.Direction Direction =>
            Iridium360.Connect.Framework.Messaging.Direction.MT;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static MessageMO Unpack(byte[] buffer)
        {
            return Unpack(KnownMTTypes, buffer) as MessageMO;
        }
    }
}

