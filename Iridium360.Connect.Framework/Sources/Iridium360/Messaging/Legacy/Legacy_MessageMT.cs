using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Messaging.Legacy
{
    public interface IPartsBuffer
    {
        uint GetPartsCount(uint id);
        List<IPart> GetParts(uint id);
        void SavePart(IPart part);
        void Clear(uint id);
    }


    public interface IPart
    {
        uint Id { get; }
        uint Index { get; }
        uint TotalParts { get; }
        byte[] Content { get; }
    }



    public class Legacy_MessageMT : IPart
    {
        private const byte SIGNATURE = 0x11;


        public byte[] Content { get; private set; }
        public uint Id { get; private set; }
        public uint Index { get; private set; }
        public uint TotalParts { get; private set; }
        public uint ReadyParts { get; private set; }
        public bool Complete { get; private set; }

        public string Address { get; private set; }
        public string Text { get; private set; }


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
            throw new InvalidOperationException();
        }


        public static Legacy_MessageMT Unpack(IPartsBuffer partsBuffer, byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using (BinaryBitReader reader = new BinaryBitReader((Stream)stream))
                {
                    if (!CheckSignature(reader.ReadByte()))
                        throw new FormatException("Invalid signature!");

                    var id = reader.ReadUInt(16);
                    var totalParts = reader.ReadUInt(8);
                    var partIndex = reader.ReadUInt(8);
                    var content = reader.ReadAllBytes();

                    var part = new Legacy_MessageMT()
                    {
                        Id = id,
                        Content = content,
                        Index = partIndex,
                        TotalParts = totalParts,
                        Complete = false
                    };


                    if (part.TotalParts > 1)
                    {
                        var count = partsBuffer.GetPartsCount(part.Id);

                        part.ReadyParts = count + 1;

                        if (count + 1 == part.TotalParts)
                        {
                            var parts = partsBuffer.GetParts(part.Id);
                            parts.Add(part);

                            var ordered = parts.OrderBy(x => x.Index).Select(x => x.Content).ToList();
                            var merged = Merge(ordered);

                            part.Content = merged;
                            part.Complete = true;

                            partsBuffer.Clear(part.Id);
                        }
                        else
                        {
                            partsBuffer.SavePart(part);
                        }
                    }
                    else
                    {
                        part.ReadyParts = 1;
                        part.Complete = true;
                    }


                    if (part.Complete)
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                        string text = Encoding.GetEncoding(1251).GetString(part.Content).Trim();
                        var parts = text.Split(new string[] { ":" }, 2, StringSplitOptions.RemoveEmptyEntries);

                        part.Address = parts[0];
                        part.Text = parts[1];
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



    public class InMemoryBuffer : IPartsBuffer
    {
        private List<IPart> parts = new List<IPart>();

        public List<IPart> GetParts(uint id)
        {
            lock (typeof(InMemoryBuffer))
            {
                return parts.Where(x => x.Id == id).ToList();
            }
        }

        public uint GetPartsCount(uint id)
        {
            lock (typeof(InMemoryBuffer))
            {
                return (uint)parts.Where(x => x.Id == id).Count();
            }
        }

        public void SavePart(IPart part)
        {
            lock (typeof(InMemoryBuffer))
            {
                if (!parts.Exists(x => x.Id == part.Id && x.Index == part.Index))
                    parts.Add(part);
            }
        }

        public void Clear(uint id)
        {
            lock (typeof(InMemoryBuffer))
            {
                parts.RemoveAll(x => x.Id == id);
            }
        }
    }
}
