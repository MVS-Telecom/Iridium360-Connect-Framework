using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    public abstract class ChatMessage : FreeText
    {
        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.ChatMessage;



        /// <summary>
        ///  Исходящее сообщение
        /// </summary>
        /// <param name="chatId">Чат</param>
        /// <param name="id">id сообщения</param>
        /// <param name="conversation">id чата</param>
        /// <param name="text">тест</param>
        /// <param name="subject">заголовок</param>
        /// <returns></returns>
        protected static T Create<T>(
            Subscriber? to,
            ushort? id,
            ushort? conversation,
            string text,
            double? lat = null,
            double? lon = null,
            int? alt = null,
            ShortGuid? byskyToken = null,
            Stream file = null,
            FileExtension? fileExtension = null,
            ImageQuality? imageQuality = null) where T : ChatMessage
        {
            if (to == null && conversation == null)
                throw new ArgumentException("Subscriber or conversation must be specified");

            if (file != null && fileExtension == null)
                throw new ArgumentException("File extension must be specified");

            if (fileExtension?.IsImage() == true && imageQuality == null)
                throw new ArgumentException("Image quality must be specified");

            T emo1 = (T)Activator.CreateInstance(typeof(T), true);
            emo1.Id = id;
            emo1.Subscriber = to;
            emo1.Conversation = conversation;
            emo1.Text = text;
            emo1.Subject = null;
            emo1.Lat = lat;
            emo1.Lon = lon;
            emo1.Alt = alt;
            emo1.ByskyToken = byskyToken;
            emo1.File = file;
            emo1.FileExtension = fileExtension;
            emo1.ImageQuality = imageQuality;
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
            if (File != null)
            {
                flags |= Flags.HasFile;
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
            if (flags.HasFlag(Flags.HasFile))
            {
                writer.Write((uint)base.FileExtension, 4);

                if (base.FileExtension.Value.IsImage())
                    writer.Write((uint)base.ImageQuality, 2);

                WriteBytes(writer, base.File);
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
            if (flags.HasFlag(Flags.HasFile))
            {
                base.FileExtension = (FileExtension)reader.ReadUInt(4);

                if (base.FileExtension.Value.IsImage())
                    base.ImageQuality = (ImageQuality)reader.ReadUInt(2);

                base.File = ReadBytes(reader);
            }
        }


        public override string ToString()
        {
            return $"{Text} -> {Subscriber}";
        }
    }
}
