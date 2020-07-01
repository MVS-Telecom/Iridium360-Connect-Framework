using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class FreeTextMO : FreeText, IMessageMO
    {
        /// <summary>
        /// 
        /// </summary>
        public override MessageType Type => MessageType.FreeText;

        /// <summary>
        /// 
        /// </summary>
        public override Direction Direction => Direction.MO;

        /// <summary>
        /// 
        /// </summary>
        protected FreeTextMO() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static FreeTextMO Create(string text)
        {
            var tmo1 = new FreeTextMO();
            tmo1.Text = text;
            return tmo1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void pack(BinaryBitWriter writer)
        {
            Write(writer, this.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        protected override void unpack(BinaryBitReader reader)
        {
            this.Text = Read(reader);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class FreeText : MessageWithLocation
    {

        [Flags]
        public enum Flags
        {
            EMPTY = 0,
            HasSubscriber = 1,
            HasConversation = 2,
            HasText = 4,
            HasSubject = 8,
            HasLocation = 0x10,
            HasId = 0x20,
            Reserver_2 = 0x40,
            Reserver_3 = 0x80
        }


        private const int PAGE_OFFSET = 8;
        private static string page_sym = "\n !\"#$%\x00a4'()*+,-./0123456789:;<=>?@{|}~—\x00ab\x00bb[\\]^_`’";
        private static string page_en = "\n @.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static string page_ru = "\n абдегхийклмнопярстувьызчАБДЕГХИЙКЛМНОПЯРСТУВЗЧ";
        private static string page_ru_ext = "\n жшюцщэфъёЖШЮЦЩЭФЪЁ";


        protected FreeText()
        {
        }


        private static Page FindPage(char c)
        {
            if (page_en.IndexOf(c) > 0)
                return Page.EN;

            if (page_ru.IndexOf(c) > 0)
                return Page.RU;

            if (page_ru_ext.IndexOf(c) > 0)
                return Page.RU_EXT;

            if (page_sym.IndexOf(c) > 0)
                return Page.SYM;

            return Page.UNICODE_FORCE;
        }

        private static byte InPage(char c, Page page)
        {
            int index = 0xff;
            if (page == Page.EN)
            {
                index = page_en.IndexOf(c);
                if (index >= 0)
                {
                    return (byte)index;
                }
            }
            if (page == Page.RU)
            {
                index = page_ru.IndexOf(c);
                if (index >= 0)
                {
                    return (byte)index;
                }
            }
            if (page == Page.RU_EXT)
            {
                index = page_ru_ext.IndexOf(c);
                if (index >= 0)
                {
                    return (byte)index;
                }
            }
            if (page == Page.SYM)
            {
                index = page_sym.IndexOf(c);
                if (index >= 0)
                {
                    return (byte)index;
                }
            }

            if (page == Page.UNICODE_FORCE)
            {
                throw new NotSupportedException();
            }

            return 0xff;
        }



        protected static string Read(BinaryBitReader reader)
        {
            Page? page = null;
            StringBuilder builder = new StringBuilder();
            byte[] bytes = new byte[] { };

            while (true)
            {
                try
                {
                    if (page == Page.UNICODE_FORCE)
                    {
                        var b = reader.ReadByte();

                        if (b == (byte)Page.END)
                            break;

                        Array.Resize(ref bytes, bytes.Length + 1);
                        bytes[bytes.Length - 1] = b;
                    }
                    else
                    {
                        int num = (((((0 + (reader.ReadBoolean() ? 1 : 0))
                            + ((reader.ReadBoolean() ? 1 : 0) << 1))
                            + ((reader.ReadBoolean() ? 1 : 0) << 2))
                            + ((reader.ReadBoolean() ? 1 : 0) << 3))
                            + ((reader.ReadBoolean() ? 1 : 0) << 4))
                            + ((reader.ReadBoolean() ? 1 : 0) << 5);

                        if (num < 8)
                        {
                            page = (Page?)num;

                            if (page == Page.END)
                                break;
                        }
                        else
                        {
                            switch (page)
                            {
                                case Page.SYM:
                                    builder.Append(page_sym[num - 8]);
                                    break;

                                case Page.EN:
                                    builder.Append(page_en[num - 8]);
                                    break;

                                case Page.RU:
                                    builder.Append(page_ru[num - 8]);
                                    break;

                                case Page.RU_EXT:
                                    builder.Append(page_ru_ext[num - 8]);
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }
                        }
                    }

                }
                catch (EndOfStreamException)
                {
                    break;
                }
                catch (Exception)
                {
                    Debugger.Break();
                }
            }


            var bb = Encoding.UTF8.GetString(bytes);
            builder.Append(bb);

            return builder.ToString();
        }



        protected static void Write(BinaryBitWriter writer, string text)
        {
            Page? currentPage = null;

            for (int i = 0; i < text.Length; i++)
            {
                var ch = text[i];

                Page page = FindPage(ch);

                if (page == Page.UNICODE_FORCE)
                {
                    string str = text.Substring(i, text.Length - i);

                    writer.Write6((byte)Page.UNICODE_FORCE);
                    writer.Write(Encoding.UTF8.GetBytes(str));
                    writer.Write((byte)Page.END);
                    return;
                }

                if (page != currentPage)
                    writer.Write6((byte)page);

                writer.Write6((byte)(InPage(ch, page) + 8));

                currentPage = page;
            }
            writer.Write6((byte)Page.END);

        }

        //public override MessageType Type =>
        //    MessageType.FreeText;

        /// <summary>
        /// 
        /// </summary>
        public string Text { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        public Subscriber? Subscriber { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort? Conversation { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort? Id { get; protected set; }


        private enum Page
        {
            END,
            SYM,
            EN,
            RU,
            RU_EXT,
            UNICODE_FORCE,
            euro_latin,
            emodzi,
            future_extension,
        }
    }
}

