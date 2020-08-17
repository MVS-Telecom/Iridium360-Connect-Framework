using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Iridium360.Connect.Framework.Messaging.Legacy
{
    public class Legacy_MessageMT
    {
        private const byte SIGNATURE = 0x11;

        public uint Group { get; private set; }
        public uint Index { get; private set; }
        public uint TotalParts { get; private set; }
        public uint ReadyParts { get; private set; }
        public byte[] Payload { get; private set; }
        public bool Complete => ReadyParts == TotalParts;



        public string Address { get; private set; }
        public string RawText { get; private set; }





        /// <summary>
        /// FROM79153905090(12.05.20-10:45(UTC)): sms text
        /// </summary>
        static Regex SMS = new Regex(@"^FROM.*\({1}.*\){1}:(?<text>.*)$", RegexOptions.Compiled);

        /// <summary>
        /// From:example@mail.com|Sub:this is subject|this is text
        /// </summary>
        static Regex EMAIL = new Regex(@"^From:{1}.*\|{1}Sub\:{1}(?<sub>.*)\|{1}(?<text>.*)$", RegexOptions.Compiled);



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            if (!Complete)
                throw new InvalidOperationException("Not all packet parts are received");


            string text = null;


            ///SMS
            try
            {
                text = SMS.Match(RawText.Trim()).Groups["text"]?.Value?.Trim();
            }
            catch
            {
            }


            //EMAIL
            if (string.IsNullOrEmpty(text))
            {
                try
                {
                    string subject = EMAIL.Match(RawText.Trim()).Groups["sub"]?.Value?.Trim();
                    string body = EMAIL.Match(RawText.Trim()).Groups["text"]?.Value?.Trim();

                    if (!string.IsNullOrEmpty(subject))
                        text = $"[{subject}] {body}";
                    else
                        text = $"{body}";
                }
                catch
                {
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                Debugger.Break();
                text = RawText.Trim();
            }

            return text;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Subscriber? GetSubscriber()
        {
            if (!Complete)
                throw new InvalidOperationException("Not all packet parts are received");

            try
            {
                var number = Address.Trim();
                var network = SubscriberNetwork.Mobile;

                if (CheckEmail(number))
                    network = SubscriberNetwork.Email;
                else if (RockstarHelper.GetTypeBySerial(number) != null)
                    network = SubscriberNetwork.Rockstar;

                return new Subscriber(number, network);
            }
            catch (Exception e)
            {
                Debugger.Break();
                return null;
            }
        }


        private static bool CheckEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public static bool CheckSignature(byte[] bytes)
        {
            return CheckSignature(bytes[0]);
        }

        public static bool CheckSignature(byte _byte)
        {
            return _byte == SIGNATURE;
        }


        private Legacy_MessageMT()
        {
        }

        public byte[] Pack()
        {
            throw new NotSupportedException();
        }


        public static Legacy_MessageMT Unpack(byte[] buffer, IPacketBuffer partsBuffer = null)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    if (!CheckSignature(reader.ReadByte()))
                        throw new FormatException("Invalid signature!");

                    var group = reader.ReadUInt(16);
                    var totalParts = reader.ReadUInt(8);
                    var partIndex = reader.ReadUInt(8);
                    var payload = reader.ReadAllBytes();

                    var part = new Legacy_MessageMT()
                    {
                        Group = group,
                        Payload = payload,
                        Index = partIndex,
                        TotalParts = totalParts,
                    };


                    if (partsBuffer == null)
                        partsBuffer = new RealmPacketBuffer();


                    partsBuffer.SavePacket(new Packet()
                    {
                        Id = $"{part.Group}:{part.Index}",
                        Index = part.Index,
                        Group = part.Group,
                        TotalParts = part.TotalParts,
                        Payload = part.Payload,
                    });

                    part.ReadyParts = partsBuffer.GetPacketCount(part.Group);

                    if (part.Complete)
                    {
                        var ordered = partsBuffer
                            .GetPackets(part.Group)
                            .OrderBy(x => x.Index)
                            .Select(x => x.Payload)
                            .ToList();

                        part.Payload = Merge(ordered);

                        partsBuffer.DeletePackets(part.Group);
                    }


                    if (part.Complete)
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        string text = Encoding.GetEncoding(1251).GetString(part.Payload).Trim();
                        var parts = text.Split(new string[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

                        part.Address = parts[0];
                        part.RawText = parts[1];
                    }

                    return part;

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

    }
}
