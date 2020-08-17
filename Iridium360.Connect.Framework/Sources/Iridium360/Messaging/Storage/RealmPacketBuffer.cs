using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Storage
{
    /// <summary>
    /// 
    /// </summary>
    class MessageRealm : RealmObject
    {
        /// <summary>
        /// 
        /// </summary>
        [PrimaryKey, Indexed]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        public byte Group { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        public bool Transmitted { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    class Part : RealmObject
    {
        /// <summary>
        /// 
        /// </summary>
        [PrimaryKey, Indexed]
        [MapTo("InnerId")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        [MapTo("Id")]
        public int Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalParts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MapTo("Content")]
        public byte[] Payload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset TransmittedDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        public int Status { get; set; }
    }




    /// <summary>
    /// 
    /// </summary>
    internal class RealmPacketBuffer : IPacketBuffer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public Message GetMessageByGroup(uint group)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var source = realm.All<MessageRealm>().SingleOrDefault(x => x.Group == group);

                if (source == null)
                    throw new NullReferenceException($"Message with group `{group}` not found");

                return new Message()
                {
                    Id = source.Id,
                    Group = source.Group,
                };
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SaveMessage(Message message)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                realm.Write(() =>
                {
                    realm.Add(new MessageRealm()
                    {
                        Id = message.Id,
                        Group = message.Group,
                    });
                });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void DeletePackets(uint group)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var toRemove = realm.All<Part>().Where(x => x.Group == group);

                realm.Write(() =>
                {
                    realm.RemoveRange(toRemove);
                });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Packet> GetPackets(uint group)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var parts = realm
                    .All<Part>()
                    .Where(x => x.Group == group)
                    .ToList()
                    .Select(x => BuildPacket(x))
                    .ToList();

                return parts;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetPacketCount(uint group)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var count = realm
                    .All<Part>()
                    .Where(x => x.Group == group)
                    .Count();

                return (uint)count;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public void SavePacket(Packet packet)
        {
            if (string.IsNullOrEmpty(packet.Id))
                throw new ArgumentNullException("Packet id is null or empty");

            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                realm.Write(() =>
                {
                    realm.Add(new Part()
                    {
                        Id = packet.Id,
                        Group = (int)packet.Group,
                        Index = (int)packet.Index,
                        TotalParts = (int)packet.TotalParts,
                        Payload = packet.Payload

                    }, update: true);
                });
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public Packet GetPacket(string packetId)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var source = realm.Find<Part>(packetId);

                if (source == null)
                    return null;

                return BuildPacket(source);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetId"></param>
        public void SetPacketTransmitted(string packetId)
        {
            using (var realm = PacketBufferHelper.GetBufferInstance())
            {
                var source = realm.Find<Part>(packetId);

                if (source == null)
                    throw new NullReferenceException();

                realm.Write(() =>
                {
                    source.Status = (int)PacketStatus.Transmitted;
                    source.TransmittedDate = DateTime.UtcNow;
                });
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        private static Packet BuildPacket(Part source)
        {
            return new Packet()
            {
                Id = source.Id,
                Group = (uint)source.Group,
                Index = (uint)source.Index,
                TotalParts = (uint)source.TotalParts,
                Payload = source.Payload,
                Status = (PacketStatus)source.Status,
                TransmittedDate = source.TransmittedDate == DateTimeOffset.MinValue ? (DateTime?)null : source.TransmittedDate.UtcDateTime
            };
        }

    }
}
