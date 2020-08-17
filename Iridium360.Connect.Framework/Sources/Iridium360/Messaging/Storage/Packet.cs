using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public enum PacketStatus : int
    {
        /// <summary>
        /// Пакет еще не трогали
        /// </summary>
        None = 0,

        /// <summary>
        /// Идет передача пакета на устройство
        /// </summary>
        SendingToDevice = 10,

        /// <summary>
        /// Пакет передан на устройство. Идет отправка
        /// </summary>
        Transmitting = 20,

        /// <summary>
        /// Пакет отправлен устройством
        /// </summary>
        Transmitted = 30
    }



    /// <summary>
    /// 
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Id пакета
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id сообщения к которому принадлежит этот пакет
        /// </summary>
        public uint Group { get; set; }

        /// <summary>
        /// Индекс пакета
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        /// Кол-во пакетов в сообщении
        /// </summary>
        public uint TotalParts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Payload { get; set; }

        /// <summary>
        /// Дата отправки пакета устройством
        /// </summary>
        public DateTime? TransmittedDate { get; set; }

        /// <summary>
        /// Статус пакета
        /// </summary>
        public PacketStatus? Status { get; set; }
    }

}
