namespace Iridium360.Connect.Framework.Messaging
{
    public enum MessageType : byte
    {
        /// <summary>
        /// пустое
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Ping/pong
        /// </summary>
        Ping = 1,

        /// <summary>
        ///  пакет байт
        /// </summary>
        Payload = 2,

        /// <summary>
        /// произвольны йтекст
        /// </summary>
        FreeText = 3,

        /// <summary>
        ///  Сообщение в чате
        /// </summary>
        ChatMessage = 4,

        /// <summary>
        ///  Погода
        /// </summary>
        Weather = 5,

        /// <summary>
        /// Проверка новых сообщений
        /// </summary>
        CheckMessages = 6,

        /// <summary>
        /// Сообщение отправлено
        /// </summary>
        Sent = 7,

        /// <summary>
        /// Повторно отправить части сообщения
        /// </summary>
        Ack = 8,

        /// <summary>
        /// 
        /// </summary>
        Balance = 9
    }
}

