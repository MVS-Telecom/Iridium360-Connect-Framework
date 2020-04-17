namespace Rock.Iridium360.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public abstract class Message
    {
        private static Dictionary<MessageType, System.Type> knownTypes = new Dictionary<MessageType, System.Type>();

        protected Message()
        {
        }

        protected abstract void pack(BinaryBitWriter writer);
        public byte[] Pack()
        {
            byte[] buffer;
            byte[] buffer3;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryBitWriter writer = new BinaryBitWriter((Stream)stream))
                {
                    this.pack(writer);
                    writer.Flush();
                    buffer = stream.GetBuffer();
                    Array.Resize<byte>(ref buffer, (int)stream.Length);
                }
            }
            this.Length = (ushort)buffer.Length;
            if (this.Length > 0x4b0)
            {
                throw new ArgumentException("Message too long!", "Length");
            }
            using (MemoryStream stream2 = new MemoryStream())
            {
                using (BinaryBitWriter writer2 = new BinaryBitWriter((Stream)stream2))
                {
                    writer2.Write((bool)(this.Direction == Rock.Iridium360.Messaging.Direction.MO));
                    writer2.Write((bool)(this.Composite == Rock.Iridium360.Messaging.Composite.Complex));
                    writer2.Write(false);
                    writer2.Write(false);
                    writer2.Write(false);
                    writer2.Write((bool)((((this.Length & 0x700) >> 8) & 4) == 4));
                    writer2.Write((bool)((((this.Length & 0x700) >> 8) & 2) == 2));
                    writer2.Write((bool)((((this.Length & 0x700) >> 8) & 1) == 1));
                    if (this.Composite == Rock.Iridium360.Messaging.Composite.Complex)
                    {
                        writer2.Write(this.Parts);
                        writer2.Write(this.Part);
                    }
                    writer2.Write((byte)this.Type);
                    writer2.Write((byte)(this.Length & 0xff));
                    writer2.Write(buffer);
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
                            buffer3 = array;
                            break;
                        }
                        num += array[index];
                        index++;
                    }
                }
            }
            return buffer3;
        }

        protected abstract void unpack(byte[] payload);


        public static Message Unpack(byte[] buffer)
        {
            Message message2;
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    Rock.Iridium360.Messaging.Direction direction = reader.ReadBoolean() ? Rock.Iridium360.Messaging.Direction.MO : Rock.Iridium360.Messaging.Direction.MT;
                    Rock.Iridium360.Messaging.Composite composite = reader.ReadBoolean() ? Rock.Iridium360.Messaging.Composite.Complex : Rock.Iridium360.Messaging.Composite.Simple;
                    bool flag = reader.ReadBoolean();
                    flag = reader.ReadBoolean();
                    flag = reader.ReadBoolean();
                    var count = ((0 + (reader.ReadBoolean() ? ((ushort)0x400) : ((ushort)0)))
                        + (reader.ReadBoolean() ? ((ushort)0x200) : ((ushort)0)))
                        + (reader.ReadBoolean() ? ((ushort)0x100) : ((ushort)0));
                    int parts = 0;
                    int part = 0;
                    if (composite == Rock.Iridium360.Messaging.Composite.Complex)
                    {
                        parts = reader.ReadByte();
                        part = reader.ReadByte();
                    }
                    MessageType messageType = (MessageType)reader.ReadByte();
                    count += reader.ReadByte();
                    byte[] payload = reader.ReadBytes(count);
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
                            Message message = (Message)Activator.CreateInstance(Enumerable.FirstOrDefault<KeyValuePair<MessageType, System.Type>>((IEnumerable<KeyValuePair<MessageType, System.Type>>)KnownTypes, delegate (KeyValuePair<MessageType, System.Type> x)
                            {
                                return ((MessageType)x.Key) == messageType;
                            }).Value, true);
                            message.Composite = composite;
                            message.Part = (byte)part;
                            message.Parts = (byte)parts;
                            message.Length = (ushort)count;
                            message.unpack(payload);
                            message2 = message;
                            break;
                        }
                        sum -= buffer[index];
                        index++;
                    }
                }
            }
            return message2;
        }

        public abstract Rock.Iridium360.Messaging.Direction Direction { get; }

        public virtual Rock.Iridium360.Messaging.Composite Composite { get; private set; }

        public byte Parts { get; private set; }

        public byte Part { get; private set; }

        public ushort Length { get; protected set; }

        public abstract MessageType Type { get; }

        protected static Dictionary<MessageType, System.Type> KnownTypes
        {
            get
            {
                if ((knownTypes == null) || (knownTypes.Count == 0))
                {
                    var types = typeof(Message).Assembly.GetTypes().Where(type => (typeof(Message).IsAssignableFrom(type) && !type.IsAbstract)).ToList();

                    foreach (System.Type type in types)
                    {
                        try
                        {
                            Message message = (Message)Activator.CreateInstance(type, true);
                            knownTypes.Add(message.Type, type);
                        }
                        catch (Exception)
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }
                    }
                }
                return knownTypes;
            }
        }


    }
}