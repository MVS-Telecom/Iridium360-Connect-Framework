namespace Iridium360.Connect.Framework.Messaging
{

    /// <summary>
    ///  Исх с RockSTAR сообщение
    /// </summary>
    public abstract class MessageMO : Message
    {

        /// <summary>
        /// 
        /// </summary>
        protected MessageMO()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public sealed override Iridium360.Connect.Framework.Messaging.Direction Direction =>
            Iridium360.Connect.Framework.Messaging.Direction.MO;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static MessageMO Unpack(byte[] buffer)
        {
            return Unpack(KnownMOTypes, buffer) as MessageMO;
        }
    }

}

