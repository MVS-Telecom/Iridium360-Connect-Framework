using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Storage
{
    public class InMemoryBuffer : IPacketBuffer
    {
        private List<Packet> packets = new List<Packet>();

        public List<Packet> GetPackets(uint messageId)
        {
            lock (typeof(InMemoryBuffer))
            {
                return packets.Where(x => x.Group == messageId).ToList();
            }
        }

        public uint GetPacketCount(uint messageId)
        {
            lock (typeof(InMemoryBuffer))
            {
                return (uint)packets.Where(x => x.Group == messageId).Count();
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

        public void DeletePackets(uint messageId)
        {
            lock (typeof(InMemoryBuffer))
            {
                packets.RemoveAll(x => x.Group == messageId);
            }
        }

        public void SetPacketTransmitted(string packetId)
        {
            lock (typeof(InMemoryBuffer))
            {
                var packet = packets.SingleOrDefault(x => x.Id == packetId);

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

        public Message GetMessageByGroup(uint group)
        {
            throw new NotImplementedException();
        }

        public void SaveMessage(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
