using Iridium360.Connect.Framework.Messaging.Storage;

namespace Iridium360.Connect.Framework.Messaging
{

    /// <summary>
    /// 
    /// </summary>
    public interface IMessageMT
    { }


    /// <summary>
    ///  Вх на RockSTAR сообщение
    /// </summary>
    public abstract class MessageMT : Message, IMessageMT
    {

        /// <summary>
        /// 
        /// </summary>
        public override Direction Direction => Direction.MT;


        /// <summary>
        /// 
        /// </summary>
        protected MessageMT()
        {
        }
    }
}

