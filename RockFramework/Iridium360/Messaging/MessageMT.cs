namespace Rock.Iridium360.Messaging
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
        public override Rock.Iridium360.Messaging.Direction Direction =>
            Rock.Iridium360.Messaging.Direction.MT;


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

