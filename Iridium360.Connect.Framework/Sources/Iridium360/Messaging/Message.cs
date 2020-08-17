using Iridium360.Connect.Framework.Messaging.Legacy;
using Iridium360.Connect.Framework.Messaging.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    public abstract class Message
    {
        private static Dictionary<MessageType, System.Type> knownMOTypes = new Dictionary<MessageType, System.Type>();
        private static Dictionary<MessageType, System.Type> knownMTTypes = new Dictionary<MessageType, System.Type>();

        private const byte SIGNATURE = 0x12;


        public static bool CheckSignature(byte[] bytes)
        {
            return CheckSignature(bytes[0]);
        }

        public static bool CheckSignature(byte _byte)
        {
            return _byte == SIGNATURE;
        }


        protected Message()
        {
        }


        /// <summary>
        /// Максимальная длина пакета (ограничение фреймворка)
        /// </summary>
        private int MAX_PACKAGE_LENGTH = 332;

        protected abstract void pack(BinaryBitWriter writer);


        public List<Packet> Pack(byte group = 0)
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



            //if (this.Length > 0x4b0)
            //{
            //    throw new ArgumentException("Message too long!", "Length");
            //}

            ///8 - длина заголовков
            int length = MAX_PACKAGE_LENGTH - 8;


            this.TotalParts = (byte)Math.Ceiling(content.Length / (double)length);
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
                    Id = null,
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
        /// <param name="direction"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected static Message Unpack(Dictionary<MessageType, Type> knownTypes, byte[] buffer, IPacketBuffer partsBuffer = null)
        {
            if (partsBuffer == null)
                partsBuffer = new RealmPacketBuffer();


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

                            var type = knownTypes.Where(t => t.Key == messageType).FirstOrDefault();
                            Message message = (Message)Activator.CreateInstance(type.Value, true);

                            message.Version = (ProtocolVersion)version;
                            message.Composite = composite;
                            message.Group = (byte)group;
                            message.Index = (byte)part;
                            message.TotalParts = (byte)parts;
                            message.Payload = payload;

                            partsBuffer.SavePacket(new Packet()
                            {
                                Id = $"{message.Group}:{message.Index}",
                                Index = message.Index,
                                Group = message.Group,
                                TotalParts = message.TotalParts,
                                Payload = message.Payload,
                            });

                            message.ReadyParts = (byte)partsBuffer.GetPacketCount(message.Group);

                            if (message.Complete)
                            {
                                var __parts = partsBuffer
                                    .GetPackets(message.Group)
                                    .OrderBy(x => x.Index)
                                    .Select(x => x.Payload)
                                    .ToList();

                                message.Payload = Merge(__parts);

                                using (MemoryStream stream2 = new MemoryStream(message.Payload))
                                {
                                    using (BinaryBitReader reader2 = new BinaryBitReader((Stream)stream2))
                                    {
                                        message.unpack(reader2);
                                    }
                                }

                                partsBuffer.DeletePackets(message.Group);
                            }

                            return message;
                        }

                        sum -= buffer[index];
                        index++;
                    }
                }
            }
        }

        private static byte[] Merge(List<byte[]> arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }


        public abstract Iridium360.Connect.Framework.Messaging.Direction Direction { get; }

        public virtual Iridium360.Connect.Framework.Messaging.Composite Composite { get; private set; }

        public enum ProtocolVersion : byte
        {
            First = 0,
            LocationFix = 1,
        }

        public ProtocolVersion Version { get; private set; } = ProtocolVersion.LocationFix;

        public byte Group { get; private set; }

        public byte TotalParts { get; private set; }

        public byte Index { get; private set; }

        public byte ReadyParts { get; private set; }

        public ushort Length => (ushort)Payload.Length;

        public byte[] Payload { get; protected set; }

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
                if ((knownMTTypes?.Count ?? 0) == 0)
                {
                    fetchKnownTypes(Direction.MT, ref knownMTTypes);
                }
                return knownMTTypes;
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


    }
}