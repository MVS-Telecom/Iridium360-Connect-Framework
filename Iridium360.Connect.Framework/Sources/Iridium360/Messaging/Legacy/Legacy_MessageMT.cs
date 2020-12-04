using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging.Storage;
using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        /// Message text
        /// ——
        /// 
        /// Сообщение отправлено из точки(+55°40.61?, +37°15.34?)
        /// </summary>
        static Regex ROCKSTAR = new Regex(@"(?<=Сообщение отправлено из точки\()(?<location>.*)(?=\))", RegexOptions.Compiled);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Location GetLocation()
        {
            try
            {
                string location = ROCKSTAR.Match(RawText.Trim()).Groups["location"]?.Value?.Trim()?.Replace("?", "");

                string lat = location?.Split(',')?.ElementAtOrDefault(0);
                string lon = location?.Split(',')?.ElementAtOrDefault(1);

                if (string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(lon))
                    return null;

                double __lat = double.Parse(lat.Split('°')[0], CultureInfo.InvariantCulture) + (double.Parse(lat.Split('°')[1], CultureInfo.InvariantCulture) / 60d);
                double __lon = double.Parse(lon.Split('°')[0], CultureInfo.InvariantCulture) + (double.Parse(lon.Split('°')[1], CultureInfo.InvariantCulture) / 60d);

                return new Location(__lat, __lon);
            }
            catch
            {
                return null;
            }
        }


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


            ///ROCKSTAR
            if (string.IsNullOrEmpty(text))
            {
                try
                {
                    if (GetLocation() != null)
                    {
                        text = RawText.Trim().Split(new string[] { "———" }, StringSplitOptions.None)?.FirstOrDefault()?.Trim();
                    }
                }
                catch
                {

                }
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

                    var message = new Legacy_MessageMT()
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
                        Index = message.Index,
                        Group = message.Group,
                        TotalParts = message.TotalParts,
                        Payload = message.Payload,
                        Direction = PacketDirection.Inbound
                    });

                    message.ReadyParts = partsBuffer.GetPacketCount(message.Group, PacketDirection.Inbound);

                    if (message.Complete)
                    {
                        var ordered = partsBuffer
                            .GetPackets(message.Group, PacketDirection.Inbound)
                            .OrderBy(x => x.Index)
                            .Select(x => x.Payload)
                            .ToList();

                        message.Payload = ByteArrayHelper.Merge(ordered);

                        partsBuffer.DeletePackets(message.Group, PacketDirection.Inbound);
                    }


                    if (message.Complete)
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        string text = Encoding.GetEncoding(1251).GetString(message.Payload).Trim();
                        var parts = text.Split(new string[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

                        message.Address = parts[0];
                        message.RawText = parts[1];
                    }

                    return message;

                }
            }
        }

    }
}
