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
        public static IMessageMT Unpack(byte[] buffer, IPacketBuffer partsBuffer = null)
        {
            return Unpack(KnownMTTypes, buffer, partsBuffer) as IMessageMT;
        }
    }
}

