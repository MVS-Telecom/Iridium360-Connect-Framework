using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Storage
{
    public class InMemoryBuffer : IPacketBuffer
    {
        private List<Packet> packets = new List<Packet>();
        private List<Message> messages = new List<Message>();

        public List<Packet> GetPackets(uint group, PacketDirection direction)
        {
            lock (typeof(InMemoryBuffer))
            {
                return packets.Where(x => x.Group == group && x.Direction == direction).ToList();
            }
        }

        public Packet GetPacket(int id)
        {
            lock (typeof(InMemoryBuffer))
            {
                return packets.LastOrDefault(x => x.FrameworkId == id);
            }
        }

        public uint GetPacketCount(uint group, PacketDirection direction)
        {
            lock (typeof(InMemoryBuffer))
            {
                return (uint)packets.Where(x => x.Group == group && x.Direction == direction).Count();
            }
        }

        public void SavePacket(Packet part)
        {
            lock (typeof(InMemoryBuffer))
            {
                if (!packets.Exists(x => x.Group == part.Group && x.Index == part.Index))
                    packets.Add(part);
            }
        }


        public void DeleteMessage(uint groupId)
        {
            lock (typeof(InMemoryBuffer))
            {
                messages.RemoveAll(x => x.Group == groupId);
            }
        }

        public void DeletePackets(uint group, PacketDirection direction)
        {
            lock (typeof(InMemoryBuffer))
            {
                packets.RemoveAll(x => x.Group == group && x.Direction == direction);
            }
        }

        public void SetPacketTransmitted(int frameworkId)
        {
            lock (typeof(InMemoryBuffer))
            {
                var packet = packets.SingleOrDefault(x => x.FrameworkId == frameworkId);

                throw new NotImplementedException();
            }
        }

        public void SetPacketNotTransmitted(int frameworkId)
        {
            lock (typeof(InMemoryBuffer))
            {
                var packet = packets.SingleOrDefault(x => x.FrameworkId == frameworkId);

                throw new NotImplementedException();
            }
        }

        public Packet GetPacket(string packetId)
        {
            lock (typeof(InMemoryBuffer))
            {
                return packets.SingleOrDefault(x => x.Id == packetId);
            }
        }

        public Message GetMessageByGroup(uint group, PacketDirection direction)
        {
            lock (typeof(InMemoryBuffer))
            {
                return messages.SingleOrDefault(x => x.Group == group);
            }
        }

        public void SaveMessage(Message message)
        {
            lock (typeof(InMemoryBuffer))
            {
                messages.Add(message);
            }
        }

        public Message GetMessageById(string frameworkId)
        {
            throw new NotImplementedException();
        }

        public void SetPacketStatus(string packetId, PacketStatus status, int? frameworkId = null)
        {
            throw new NotImplementedException();
        }

        public void SetPacketStatus(int frameworkId, PacketStatus status)
        {
            throw new NotImplementedException();
        }

        public void SetMessageSendAttempt(string id, int count)
        {
            throw new NotImplementedException();
        }
    }
}
