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
        public DateTimeOffset Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        public byte Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte TotalParts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SendAttempt { get; set; }
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
        public int FrameworkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        [MapTo("Id")]
        public int Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Indexed]
        public int Direction { get; set; }

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
        private static object locker = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public Message GetMessageByGroup(uint group, PacketDirection direction)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.All<MessageRealm>().SingleOrDefault(x => x.Group == group /*&& x.Direction == direction*/);

                    if (source == null)
                        return null;

                    return BuildMessage(source);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkId"></param>
        /// <returns></returns>
        public Message GetMessageById(string messageId)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.Find<MessageRealm>(messageId);

                    if (source == null)
                        return null;

                    return BuildMessage(source);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        public void DeleteMessage(uint groupId)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var toDelete = realm.All<MessageRealm>().Where(x => x.Group == groupId);

                    if (toDelete.Any())
                    {
                        realm.Write(() =>
                        {
                            realm.RemoveRange(toDelete);
                        });
                    }
                }
            }

        }


        /// <summary>
        /// 
        /// </summary>
        public void SetMessageSendAttempt(string id, int attempt)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var message = realm.Find<MessageRealm>(id);

                    realm.Write(() =>
                    {
                        message.SendAttempt = attempt;
                    });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SaveMessage(Message message)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    realm.Write(() =>
                    {
                        realm.Add(new MessageRealm()
                        {
                            Id = message.Id,
                            Date = DateTime.UtcNow,
                            Group = message.Group,
                            TotalParts = message.TotalParts,
                            Type = (int)message.Type
                        });
                    });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void DeletePackets(uint group, PacketDirection direction)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var toRemove = realm.All<Part>().Where(x => x.Group == group && x.Direction == (int)direction);

                    realm.Write(() =>
                    {
                        realm.RemoveRange(toRemove);
                    });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Packet> GetPackets(uint group, PacketDirection direction)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var parts = realm
                        .All<Part>()
                        .Where(x => x.Group == group && x.Direction == (int)direction)
                        .ToList()
                        .Select(x => BuildPacket(x))
                        .ToList();

                    return parts;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public uint GetPacketCount(uint group, PacketDirection direction)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var count = realm
                        .All<Part>()
                        .Where(x => x.Group == group && x.Direction == (int)direction)
                        .Count();

                    return (uint)count;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        public void SavePacket(Packet packet)
        {
            lock (locker)
            {
                if (string.IsNullOrEmpty(packet.Id))
                    throw new ArgumentNullException("Packet id is null or empty");


                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
#if DEBUG
                    var __part = realm.Find<Part>(packet.Id);
                    var __part2 = realm.All<Part>().SingleOrDefault(x => x.Group == packet.Group && x.Index == packet.Index && x.Direction == (int)packet.Direction);

                    if (__part != null || __part2 != null)
                    {
                        Debugger.Break();
                        //throw new InvalidOperationException($"Packet with group `{packet.Group}` already saved in buffer");
                    }
#endif

                    realm.Write(() =>
                    {
                        realm.Add(new Part()
                        {
                            Id = packet.Id,
                            FrameworkId = packet.FrameworkId,
                            Group = (int)packet.Group,
                            Direction = (int)packet.Direction,
                            Index = (int)packet.Index,
                            TotalParts = (int)packet.TotalParts,
                            Payload = packet.Payload,

                            Status = (int)PacketStatus.None,
                            TransmittedDate = DateTimeOffset.MinValue,

                        }, update: true);
                    });
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetId"></param>
        /// <returns></returns>
        public Packet GetPacket(string packetId)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.Find<Part>(packetId);

                    if (source == null)
                        return null;

                    return BuildPacket(source);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkId"></param>
        /// <returns></returns>
        public Packet GetPacket(int frameworkId)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.All<Part>().LastOrDefault(x => x.FrameworkId == frameworkId);

                    if (source == null)
                        return null;

                    return BuildPacket(source);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkId"></param>
        public void SetPacketStatus(string packetId, PacketStatus status, int? frameworkId = null)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.Find<Part>(packetId);

                    if (source == null)
                        throw new NullReferenceException();

                    if (status == PacketStatus.TransferredToDevice && frameworkId == null)
                        throw new ArgumentException("Framework id must be specified");

                    realm.Write(() =>
                    {
                        source.Status = (int)status;
                        source.FrameworkId = frameworkId ?? source.FrameworkId;

                        if (status == PacketStatus.Transmitted)
                            source.TransmittedDate = DateTimeOffset.UtcNow;
                        else
                            source.TransmittedDate = DateTimeOffset.MinValue;
                    });
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetId"></param>
        /// <param name="status"></param>
        /// <param name="frameworkId"></param>
        public void SetPacketStatus(int frameworkId, PacketStatus status)
        {
            lock (locker)
            {
                using (var realm = PacketBufferHelper.GetBufferInstance())
                {
                    var source = realm.All<Part>().SingleOrDefault(x => x.FrameworkId == frameworkId);

                    if (source == null)
                        throw new NullReferenceException();

                    realm.Write(() =>
                    {
                        source.Status = (int)status;

                        if (status == PacketStatus.Transmitted)
                            source.TransmittedDate = DateTimeOffset.UtcNow;
                        else
                            source.TransmittedDate = DateTimeOffset.MinValue;
                    });
                }
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
                //Id = source.Id
                FrameworkId = source.FrameworkId,
                Group = (uint)source.Group,
                Direction = (PacketDirection)source.Direction,
                Index = (uint)source.Index,
                TotalParts = (uint)source.TotalParts,
                Payload = source.Payload,
                Status = (PacketStatus)source.Status,
                TransmittedDate = source.TransmittedDate == DateTimeOffset.MinValue ? (DateTime?)null : source.TransmittedDate.UtcDateTime
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Message BuildMessage(MessageRealm source)
        {
            return new Message()
            {
                Id = source.Id,
                Date = source.Date == DateTimeOffset.MinValue ? (DateTime?)null : source.Date.UtcDateTime,
                Group = source.Group,
                TotalParts = source.TotalParts,
                Type = (MessageType?)source.Type,
                SendAttempt = source.SendAttempt
            };
        }

    }
}
