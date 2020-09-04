using Iridium360.Connect.Framework.Util;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageMO : ChatMessage, IMessageMO
    {

        /// <summary>
        /// 
        /// </summary>
        public override Direction Direction => Direction.MO;


        /// <summary>
        /// 
        /// </summary>
        private ChatMessageMO()
        {
        }


        /// <summary>
        ///  Исходящее сообщение
        /// </summary>
        /// <param name="chatId">Чат</param>
        /// <param name="id">id сообщения</param>
        /// <param name="conversation">id чата</param>
        /// <param name="text">тест</param>
        /// <param name="subject">заголовок</param>
        /// <returns></returns>
        public static ChatMessageMO Create(
            ProtocolVersion version,
            Subscriber? to,
            ushort? id,
            ushort? conversation,
            string text,
            double? lat = null,
            double? lon = null,
            int? alt = null,
            ShortGuid? byskyToken = null,
            Stream file = null,
            FileExtension? fileExtension = null,
            ImageQuality? imageQuality = null)
        {
            return ChatMessage.Create<ChatMessageMO>(
                version,
                to,
                id,
                conversation,
                text,
                lat,
                lon,
                alt,
                byskyToken,
                file,
                fileExtension,
                imageQuality);
        }
    }
}

