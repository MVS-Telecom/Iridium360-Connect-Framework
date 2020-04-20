using System;
using System.Diagnostics;
using System.IO;
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
        protected override void unpack(byte[] payload)
        {
            using (MemoryStream stream = new MemoryStream(payload))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    this.Text = Read(reader);
                }
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public abstract class FreeText : Message
    {

        [Flags]
        public enum Flags
        {
            EMPTY = 0,
            HasChatId = 1,
            HasConversation = 2,
            HasText = 4,
            HasSubject = 8,
            HasLocation = 0x10,
            HasId = 0x20,
            Reserver_2 = 0x40,
            Reserver_3 = 0x80
        }


        private const int PAGE_OFFSET = 8;
        private static string page_sym = "\n !\"#$%\x00a4'()*+,-./0123456789:;<=>?@{|}~—\x00ab\x00bb[\\]^_`";
        private static string page_en = "\n @.ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static string page_ru = "\n абдегхийклмнопярстувьызчАБДЕГХИЙКЛМНОПЯРСТУВЗЧ";
        private static string page_ru_ext = "\n жшюцщэфъёЖШЮЦЩЭФЪЁ";


        protected FreeText()
        {
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
            return 0xff;
        }

       

        protected static string Read(BinaryBitReader reader)
        {
            Page? page = null;
            StringBuilder builder = new StringBuilder();
            while (true)
            {
                try
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
                        ////Page? nullable3 = nullable;
                        //if (nullable3.HasValue)
                        //{
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
                                break;
                        }
                        //}
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
            return builder.ToString();
        }

       

        protected static void Write(BinaryBitWriter writer, string text)
        {
            Page? nullable = null;
            foreach (char ch in text)
            {
                byte num2 = 0xff;
                if (nullable.HasValue && (0xff != (num2 = InPage(ch, nullable.Value))))
                {
                    writer.Write6((byte)(num2 + 8));
                }
                else
                {
                    bool flag = true;
                    Page[] pageArray = new Page[] { Page.EN, Page.RU, Page.RU_EXT, Page.SYM };
                    int index = 0;
                    while (true)
                    {
                        if (index < pageArray.Length)
                        {
                            Page page = pageArray[index];
                            if (0xff == (num2 = InPage(ch, page)))
                            {
                                index++;
                                continue;
                            }
                            Page? nullable2 = nullable;
                            Page page2 = page;
                            if (!((((Page)nullable2.GetValueOrDefault()) == page2) & nullable2.HasValue))
                            {
                                nullable = new Page?(page);
                                writer.Write6((byte)nullable.Value);
                            }
                            writer.Write6((byte)(num2 + 8));
                            flag = false;
                        }
                        if (flag)
                        {
                            Debugger.Break();
                        }
                        break;
                    }
                }
            }
            writer.Write6(0);
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
        public string ChatId { get; protected set; }

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
            euro_latin,
            emodzi,
            future_extension
        }
    }
}
