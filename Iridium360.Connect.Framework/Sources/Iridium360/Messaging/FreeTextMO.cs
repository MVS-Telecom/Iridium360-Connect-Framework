using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
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
            HasLocation = 16,
            HasId = 32,
            HasByskyToken = 64,
            HasFile = 128,
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


        public static byte[] ReadFully(Stream input)
        {
            input.Position = 0;

            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }



        protected static void WriteBytes(BinaryBitWriter writer, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var bytes = ReadFully(stream);
            var hash = Md5.Get(bytes.Skip(bytes.Length - 5).ToArray()).Take(5).ToArray();

            writer.Write(bytes);

            //HASH
            writer.Write(hash);

            ///END
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }


        protected static Stream ReadBytes(BinaryBitReader reader)
        {
            var stream = new MemoryStream();

            byte? h1 = null;
            byte? h2 = null;
            byte? h3 = null;
            byte? h4 = null;
            byte? h5 = null;

            byte? b1 = null;
            byte? b2 = null;
            byte? b3 = null;
            byte? b4 = null;
            byte? b5 = null;

            byte? e1 = null;
            byte? e2 = null;
            byte? e3 = null;

            while (true)
            {
                h1 = h2;
                h2 = h3;
                h3 = h4;
                h4 = h5;
                h5 = b1;

                b1 = b2;
                b2 = b3;
                b3 = b4;
                b4 = b5;
                b5 = e1;

                e1 = e2;
                e2 = e3;
                e3 = reader.ReadByte();

                stream.WriteByte(e3.Value);


                if (e1 == 0 && e2 == 0 && e3 == 0)
                {
                    if (h1 != null && h2 != null && h3 != null && h4 != null && h5 != null)
                    {
                        var hash = Md5.Get(new byte[] { h1.Value, h2.Value, h3.Value, h4.Value, h5.Value }).Take(5).ToArray();

                        if (hash.SequenceEqual(new byte[] { b1.Value, b2.Value, b3.Value, b4.Value, b5.Value }))
                            break;
                    }
                }
            }

            ///HASH + END
            stream.SetLength(stream.Length - (5 + 3));
            stream.Capacity = (int)stream.Length;
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
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
        public Stream File { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public FileExtension? FileExtension { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public ImageQuality? ImageQuality { get; protected set; }

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

        /// <summary>
        /// 
        /// </summary>
        public ShortGuid? ByskyToken { get; protected set; }


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

