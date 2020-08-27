using Iridium360.Connect.Framework.Messaging.Storage;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageMO
    { }


    /// <summary>
    ///  Исх с RockSTAR сообщение
    /// </summary>
    public abstract class MessageMO : MessageWithLocation, IMessageMO
    {
        /// <summary>
        /// 
        /// </summary>
        public sealed override Direction Direction => Direction.MO;

        /// <summary>
        /// 
        /// </summary>
        protected MessageMO()
        {
        }
    }

}

