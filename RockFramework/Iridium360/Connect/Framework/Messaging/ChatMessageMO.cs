using Rock;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(byte[] payload)
        {
            throw new NotImplementedException();
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
            string chatId, 
            ushort? id, 
            ushort? conversation, 
            string text, 
            string subject = null,
            Location location = null)
        {
            ChatMessageMO emo1 = new ChatMessageMO();
            emo1.Id = id;
            emo1.ChatId = chatId;
            emo1.Conversation = conversation;
            emo1.Text = text;
            emo1.Subject = subject;
            emo1.Location = location;
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

            if (!string.IsNullOrEmpty(this.ChatId))
            {
                flags |= Flags.HasChatId;
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
            if (Location != null)
            {
                flags |= Flags.HasLocation;
            }
            //
            // --->
            //
            writer.Write((byte)((byte)flags));
            //
            // --->
            //
            if (flags.HasFlag(Flags.HasChatId))
            {
                Write(writer, this.ChatId);
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
            }
        }

        protected override void unpack(byte[] payload)
        {
            using (MemoryStream stream = new MemoryStream(payload))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    Flags flags = (Flags)reader.ReadByte();
                    if (flags.HasFlag(Flags.HasChatId))
                    {
                        this.ChatId = Read(reader);
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
                    }
                }
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

        /// <summary>
        /// 
        /// </summary>
        public Location Location { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public ushort? Conversation { get; private set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public ushort? Id { get; private set; }

    }
}

