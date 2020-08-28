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
        TransferredToDevice = 20,

        /// <summary>
        /// Пакет отправлен устройством
        /// </summary>
        Transmitted = 30
    }


    /// <summary>
    /// 
    /// </summary>
    public enum PacketDirection : int
    {
        Outbound = 0,
        Inbound = 1
    }


    /// <summary>
    /// 
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Id пакета
        /// </summary>
        public string Id => $"{Group}@{Index}@{(int)Direction}";

        /// <summary>
        /// Сквозной id пакета (от фреймворка)
        /// </summary>
        public int FrameworkId { get; set; }

        /// <summary>
        /// Id сообщения к которому принадлежит этот пакет
        /// </summary>
        public uint Group { get; set; }

        /// <summary>
        /// Вх/исх
        /// </summary>
        public PacketDirection Direction { get; set; }

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
        /// 
        /// </summary>
        public bool Transmitted => Status >= PacketStatus.Transmitted;

        /// <summary>
        /// Статус пакета
        /// </summary>
        public PacketStatus? Status { get; set; }
    }

}
