using Iridium360.Connect.Framework.Util;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iridium360.Connect.Framework.Messaging
{


    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageMT : FreeText, IMessageMT
    {

        public static ChatMessageMT Create(
            Subscriber? from,
            ushort? id,
            ushort? conversation,
            string text,
            string subject = null)
        {
            if (from == null && conversation == null)
                throw new ArgumentException("Subscriber or conversation must be specified");

            ChatMessageMT m = new ChatMessageMT();
            m.Id = id;
            m.Subscriber = from;
            m.Conversation = conversation;
            m.Text = text;
            m.Subject = subject;
            return m;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            Flags flags = Flags.EMPTY;
            // --->

            if (this.Subscriber != null)
            {
                flags |= Flags.HasSubscriber;
            }
            if (this.Conversation.HasValue)
            {
                flags |= Flags.HasConversation;
            }
            if (this.Id.HasValue)
            {
                flags |= Flags.HasId;
            }
            if (!string.IsNullOrEmpty(this.Subject))
            {
                flags |= Flags.HasSubject;
            }
            if (!string.IsNullOrEmpty(base.Text))
            {
                flags |= Flags.HasText;
            }
            if (base.ByskyToken != null)
            {
                flags |= Flags.HasByskyToken;
            }
            //
            // --->
            //
            writer.Write((byte)((byte)flags));
            //
            // --->
            //
            if (flags.HasFlag(Flags.HasSubscriber))
            {
                Write(writer, this.Subscriber.Value.Number);
                writer.Write((uint)this.Subscriber.Value.Network, 5);
            }
            if (flags.HasFlag(Flags.HasId))
            {
                writer.Write(this.Id.Value);
            }
            if (flags.HasFlag(Flags.HasConversation))
            {
                writer.Write(this.Conversation.Value);
            }
            if (flags.HasFlag(Flags.HasSubject))
            {
                Write(writer, this.Subject);
            }
            if (flags.HasFlag(Flags.HasText))
            {
                Write(writer, base.Text);
            }
            if (flags.HasFlag(Flags.HasByskyToken))
            {
                writer.Write(base.ByskyToken.Value.Guid.ToByteArray());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(BinaryBitReader reader)
        {
            Flags flags = (Flags)reader.ReadByte();
            if (flags.HasFlag(Flags.HasSubscriber))
            {
                string number = Read(reader);
                SubscriberNetwork network = (SubscriberNetwork)reader.ReadUInt(5);

                this.Subscriber = new Subscriber(number, network);
            }
            if (flags.HasFlag(Flags.HasId))
            {
                this.Id = reader.ReadUInt16();
            }
            if (flags.HasFlag(Flags.HasConversation))
            {
                this.Conversation = new ushort?(reader.ReadUInt16());
            }
            if (flags.HasFlag(Flags.HasSubject))
            {
                this.Subject = Read(reader);
            }
            if (flags.HasFlag(Flags.HasText))
            {
                base.Text = Read(reader);
            }
            if (flags.HasFlag(Flags.HasByskyToken))
            {
                base.ByskyToken = new ShortGuid(reader.ReadBytes(16));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.ChatMessage;


        /// <summary>
        /// 
        /// </summary>
        public override Direction Direction => Direction.MT;

    }



    /// <summary>
    /// 
    /// </summary>
    public class ChatMessageMO : FreeText, IMessageMO
    {

        /// <summary>
        /// 
        /// </summary>
        public override Direction Direction => Direction.MO;


        /// <summary>
        /// 
        /// </summary>
        private ChatMessageMO()
        {
        }


        /// <summary>
        ///  Исходящее сообщение
        /// </summary>
        /// <param name="chatId">Чат</param>
        /// <param name="id">id сообщения</param>
        /// <param name="conversation">id чата</param>
        /// <param name="text">тест</param>
        /// <param name="subject">заголовок</param>
        /// <returns></returns>
        public static ChatMessageMO Create(
            Subscriber? to,
            ushort? id,
            ushort? conversation,
            string text,
            double? lat = null,
            double? lon = null,
            int? alt = null,
            ShortGuid? byskyToken = null)
        {
            if (to == null && conversation == null)
                throw new ArgumentException("Subscriber or conversation must be specified");

            ChatMessageMO emo1 = new ChatMessageMO();
            emo1.Id = id;
            emo1.Subscriber = to;
            emo1.Conversation = conversation;
            emo1.Text = text;
            emo1.Subject = null;
            emo1.Lat = lat;
            emo1.Lon = lon;
            emo1.Alt = alt;
            emo1.ByskyToken = byskyToken;
            return emo1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            Flags flags = Flags.EMPTY;
            // --->

            if (this.Subscriber != null)
            {
                flags |= Flags.HasSubscriber;
            }
            if (this.Conversation.HasValue)
            {
                flags |= Flags.HasConversation;
            }
            if (this.Id.HasValue)
            {
                flags |= Flags.HasId;
            }
            if (!string.IsNullOrEmpty(this.Subject))
            {
                flags |= Flags.HasSubject;
            }
            if (!string.IsNullOrEmpty(base.Text))
            {
                flags |= Flags.HasText;
            }
            if (Lat != null && Lon != null)
            {
                flags |= Flags.HasLocation;
            }
            if (ByskyToken != null)
            {
                flags |= Flags.HasByskyToken;
            }
            //
            // --->
            //
            writer.Write((byte)((byte)flags));
            //
            // --->
            //
            if (flags.HasFlag(Flags.HasSubscriber))
            {
                Write(writer, this.Subscriber.Value.Number);
                writer.Write((uint)this.Subscriber.Value.Network, 5);
            }
            if (flags.HasFlag(Flags.HasId))
            {
                writer.Write(this.Id.Value);
            }
            if (flags.HasFlag(Flags.HasConversation))
            {
                writer.Write(this.Conversation.Value);
            }
            if (flags.HasFlag(Flags.HasSubject))
            {
                Write(writer, this.Subject);
            }
            if (flags.HasFlag(Flags.HasText))
            {
                Write(writer, base.Text);
            }
            if (flags.HasFlag(Flags.HasLocation))
            {
                WriteLocation(writer);
            }
            if (flags.HasFlag(Flags.HasByskyToken))
            {
                writer.Write(base.ByskyToken.Value.Guid.ToByteArray());
            }
        }

        protected override void unpack(BinaryBitReader reader)
        {
            Flags flags = (Flags)reader.ReadByte();
            if (flags.HasFlag(Flags.HasSubscriber))
            {
                string number = Read(reader);
                SubscriberNetwork network = (SubscriberNetwork)reader.ReadUInt(5);

                this.Subscriber = new Subscriber(number, network);
            }
            if (flags.HasFlag(Flags.HasId))
            {
                this.Id = reader.ReadUInt16();
            }
            if (flags.HasFlag(Flags.HasConversation))
            {
                this.Conversation = new ushort?(reader.ReadUInt16());
            }
            if (flags.HasFlag(Flags.HasSubject))
            {
                this.Subject = Read(reader);
            }
            if (flags.HasFlag(Flags.HasText))
            {
                base.Text = Read(reader);
            }
            if (flags.HasFlag(Flags.HasLocation))
            {
                ReadLocation(reader);
            }
            if (flags.HasFlag(Flags.HasByskyToken))
            {
                base.ByskyToken = new ShortGuid(reader.ReadBytes(16));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.ChatMessage;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string ChatId { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public string Subject { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public ushort? Conversation { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public ushort? Id { get; private set; }


        public override string ToString()
        {
            return $"{Text} -> {Subscriber}";
        }
    }
}

