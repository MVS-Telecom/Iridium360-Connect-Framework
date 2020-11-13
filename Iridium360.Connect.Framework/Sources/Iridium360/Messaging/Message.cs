using Iridium360.Connect.Framework.Messaging.Legacy;
using Iridium360.Connect.Framework.Messaging.Storage;
using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{

    public enum ProtocolVersion : byte
    {
        v1 = 0,
        v2__LocationFix = 1,
        v3__WeatherExtension = 2,
    }

    public abstract class Message
    {
        private static Dictionary<MessageType, System.Type> knownMOTypes = new Dictionary<MessageType, System.Type>();
        private static Dictionary<MessageType, System.Type> knownMTTypes = new Dictionary<MessageType, System.Type>();

        public const byte SIGNATURE = 0x12;


        public static bool CheckSignature(byte[] bytes)
        {
            return CheckSignature(bytes[0]);
        }

        public static bool CheckSignature(byte _byte)
        {
            return _byte == SIGNATURE;
        }


        /// <summary>
        /// Максимальная длина пакета (ограничение фреймворка)
        /// </summary>
        private int MAX_PACKAGE_LENGTH = 332;

        protected abstract void pack(BinaryBitWriter writer);


        public static byte[] Compress(byte[] sourceFile)
        {
            // поток для чтения исходного файла
            using (Stream sourceStream = new MemoryStream(sourceFile))
            {
                // поток для записи сжатого файла
                using (MemoryStream targetStream = new MemoryStream())
                {
                    // поток архивации
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream); // копируем байты из одного потока в другой
                    }

                    return targetStream.GetBuffer();
                }
            }
        }

        public int CalculatePacketsCount()
        {
            return __Pack(returnEmptyPackets: true).Count;
        }

        public List<Packet> Pack(byte group = 0)
        {
            return __Pack(group);
        }


        private List<Packet> __Pack(byte group = 0, bool returnEmptyPackets = false)
        {
            List<Packet> payloads = new List<Packet>();
            byte[] content;

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryBitWriter writer = new BinaryBitWriter((Stream)stream))
                {
                    this.pack(writer);
                    writer.Flush();
                    content = stream.GetBuffer();
                    Array.Resize<byte>(ref content, (int)stream.Length);
                }
            }


            //var compressed = Compress(content);

            //if (this.Length > 0x4b0)
            //{
            //    throw new ArgumentException("Message too long!", "Length");
            //}

            ///8 - длина заголовков
            int length = MAX_PACKAGE_LENGTH - 8;

            int parts = (int)Math.Max(1, Math.Ceiling(content.Length / (double)length));

            if (returnEmptyPackets)
            {
                var list = new List<Packet>();
                for (int i = 0; i < parts; i++)
                    list.Add(null);
                return list;
            }

            if (parts > byte.MaxValue)
                throw new ArgumentException("Message too long!");

            this.TotalParts = (byte)parts;
            this.Index = 0;
            this.Group = group;

            this.Composite = TotalParts > 1 ? Composite.Complex : Composite.Simple;


            while (true)
            {
                this.Payload = content.Skip(this.Index * length).Take(length).ToArray();

                if (this.Payload.Length == 0)
                    break;

                using (MemoryStream stream2 = new MemoryStream())
                {
                    using (BinaryBitWriter writer2 = new BinaryBitWriter((Stream)stream2))
                    {
                        writer2.Write(SIGNATURE);
                        writer2.Write((bool)(this.Direction == Iridium360.Connect.Framework.Messaging.Direction.MO));
                        writer2.Write((bool)(this.Composite == Iridium360.Connect.Framework.Messaging.Composite.Complex));
                        writer2.Write((uint)this.Version, 3);
                        writer2.Write((bool)((((this.Length & 0x700) >> 8) & 4) == 4));
                        writer2.Write((bool)((((this.Length & 0x700) >> 8) & 2) == 2));
                        writer2.Write((bool)((((this.Length & 0x700) >> 8) & 1) == 1));


                        if (this.Composite == Iridium360.Connect.Framework.Messaging.Composite.Complex)
                        {
                            writer2.Write(this.Group);
                            writer2.Write(this.TotalParts);
                            writer2.Write(this.Index);
                        }

                        writer2.Write((byte)this.Type);
                        writer2.Write((byte)(this.Length & 0xff));
                        writer2.Write(this.Payload);
                        byte[] array = stream2.GetBuffer();
                        byte num = 0;
                        int index = 0;
                        while (true)
                        {
                            if (index >= stream2.Length)
                            {
                                writer2.Write(num);
                                writer2.Flush();
                                array = stream2.GetBuffer();
                                Array.Resize<byte>(ref array, (int)stream2.Length);
                                this.Payload = array;
                                break;
                            }
                            num += array[index];
                            index++;
                        }
                    }
                }

                payloads.Add(new Packet()
                {
                    Direction = this.Direction == Direction.MO ? PacketDirection.Outbound : PacketDirection.Inbound,
                    Index = this.Index,
                    Group = this.Group,
                    TotalParts = this.TotalParts,
                    Payload = this.Payload,
                });

                this.Index++;
            }


            return payloads;
        }

        protected abstract void unpack(BinaryBitReader reader);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static ProtocolVersion GetProtocolVersion(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    if (!CheckSignature(reader.ReadByte()))
                        throw new FormatException("Invalid signature!");

                    reader.ReadBoolean();
                    reader.ReadBoolean();
                    return (ProtocolVersion)reader.ReadUInt(3);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Message Unpack(byte[] buffer, IPacketBuffer packetBuffer = null)
        {
            if (packetBuffer == null)
                packetBuffer = new RealmPacketBuffer();


            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    if (!CheckSignature(reader.ReadByte()))
                        throw new FormatException("Invalid signature!");

                    var direction = reader.ReadBoolean() ? Direction.MO : Direction.MT;
                    var composite = reader.ReadBoolean() ? Composite.Complex : Composite.Simple;
                    var version = reader.ReadUInt(3);

                    var length = (0 + (reader.ReadBoolean() ? ((ushort)0x400) : ((ushort)0)))
                        + (reader.ReadBoolean() ? ((ushort)0x200) : ((ushort)0))
                        + (reader.ReadBoolean() ? ((ushort)0x100) : ((ushort)0));

                    int parts = 1;
                    int part = 0;
                    int group = 0;

                    if (composite == Composite.Complex)
                    {
                        group = reader.ReadByte();
                        parts = reader.ReadByte();
                        part = reader.ReadByte();
                    }

                    MessageType messageType = (MessageType)reader.ReadByte();
                    length += reader.ReadByte();

                    byte[] payload = reader.ReadBytes(length);
                    byte sum = reader.ReadByte();
                    int index = 0;

                    while (true)
                    {
                        if (index >= (stream.Length - 1L))
                        {
                            if (sum != 0)
                            {
                                throw new FormatException("Invalid checksum!");
                            }

                            var types = direction == Direction.MT ? KnownMTTypes : KnownMOTypes;
                            var type = types.Where(t => t.Key == messageType).FirstOrDefault();
                            Message message = (Message)Activator.CreateInstance(type.Value, true);

                            message.Version = (ProtocolVersion)version;
                            message.Composite = composite;
                            message.Group = (byte)group;
                            message.Index = (byte)part;
                            message.TotalParts = (byte)parts;
                            message.Payload = payload;

                            var packet = new Packet()
                            {
                                Direction = direction == Direction.MO ? PacketDirection.Outbound : PacketDirection.Inbound,
                                Index = message.Index,
                                Group = message.Group,
                                TotalParts = message.TotalParts,
                                Payload = message.Payload,
                            };
                            packetBuffer.SavePacket(packet);

                            message.ReadyParts = (byte)packetBuffer.GetPacketCount(message.Group, packet.Direction);

                            if (message.Complete)
                            {
                                var __parts = packetBuffer
                                    .GetPackets(message.Group, packet.Direction)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Payload)
                                    .ToList();

                                message.Payload = ByteArrayHelper.Merge(__parts);

                                using (MemoryStream stream2 = new MemoryStream(message.Payload))
                                {
                                    using (BinaryBitReader reader2 = new BinaryBitReader((Stream)stream2))
                                    {
                                        message.unpack(reader2);
                                    }
                                }

                                packetBuffer.DeletePackets(message.Group, packet.Direction);
                            }

                            return message;
                        }

                        sum -= buffer[index];
                        index++;
                    }
                }
            }
        }




        public abstract Iridium360.Connect.Framework.Messaging.Direction Direction { get; }

        public virtual Iridium360.Connect.Framework.Messaging.Composite Composite { get; private set; }


        public ProtocolVersion Version { get; private set; } = ProtocolVersion.v3__WeatherExtension;

        public byte Group { get; private set; }

        public byte TotalParts { get; private set; }

        public byte Index { get; private set; }

        public byte ReadyParts { get; private set; }

        public ushort Length => (ushort)Payload.Length;

        public byte[] Payload { get; private set; }

        public bool Complete => ReadyParts == TotalParts;


        public abstract MessageType Type { get; }


        /// <summary>
        /// 
        /// </summary>
        protected static Dictionary<MessageType, System.Type> KnownMOTypes
        {
            get
            {
                if ((knownMOTypes?.Count ?? 0) == 0)
                {
                    fetchKnownTypes(Direction.MO, ref knownMOTypes);
                }
                return knownMOTypes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected static Dictionary<MessageType, System.Type> KnownMTTypes
        {
            get
            {
                lock (typeof(MessageType))
                {
                    if ((knownMTTypes?.Count ?? 0) == 0)
                    {
                        fetchKnownTypes(Direction.MT, ref knownMTTypes);
                    }
                    return knownMTTypes;
                }
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="knownTypes"></param>
        static void fetchKnownTypes(Direction direction, ref Dictionary<MessageType, Type> knownTypes)
        {
            var types = typeof(Message).Assembly.GetTypes().Where(type => (typeof(Message).IsAssignableFrom(type) && !type.IsAbstract)).ToList();

            foreach (System.Type type in types)
            {
                try
                {
                    Message message = (Message)Activator.CreateInstance(type, true);
                    if (message.Direction == direction)
                        knownTypes.Add(message.Type, type);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                }
            }
        }


        static protected T Create<T>(ProtocolVersion version) where T : Message
        {
            var message = (T)Activator.CreateInstance(typeof(T), true);
            message.Version = version;
            return message;
        }

    }
}