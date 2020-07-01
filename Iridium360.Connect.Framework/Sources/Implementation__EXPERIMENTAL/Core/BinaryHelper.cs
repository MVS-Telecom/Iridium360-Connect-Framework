using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Iridium360.Connect.Framework.Implementations
{


    public class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input) : base(input) { }
        public BigEndianBinaryReader(Stream input, bool leaveOpen) : base(input, System.Text.Encoding.UTF8, leaveOpen) { }


        private static void Resize(ref byte[] bytes, int targetLength)
        {
            if (bytes.Length < targetLength)
            {
                int extra = targetLength - bytes.Length;
                bytes = new byte[extra].Concat(bytes).ToArray();
            }
        }

        public override ushort ReadUInt16()
        {
            byte[] b = ReadBytes(2);
            Resize(ref b, 2);

            return (ushort)(b[1] + (b[0] << 8));
        }

        public override short ReadInt16() => (short)this.ReadUInt16();

        public override uint ReadUInt32()
        {
            byte[] b = ReadBytes(4);
            Resize(ref b, 4);
            return b[3] + ((uint)b[2] << 8) + ((uint)b[1] << 16) + ((uint)b[0] << 24);
        }


        public override int ReadInt32() => (int)this.ReadUInt32();

        public override ulong ReadUInt64()
        {
            byte[] b = ReadBytes(8);
            Resize(ref b, 8);
            return b[7] + ((ulong)b[6] << 8) + ((ulong)b[5] << 16) + ((ulong)b[4] << 24) + ((ulong)b[3] << 32) + ((ulong)b[2] << 40) + ((ulong)b[1] << 48) + ((ulong)b[0] << 56);
        }

        public override long ReadInt64() => (long)this.ReadUInt64();
    }


    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream output) : base(output) { }
        public BigEndianBinaryWriter(Stream input, bool leaveOpen) : base(input, System.Text.Encoding.UTF8, leaveOpen) { }

        public override void Write(ushort value)
        {
            base.Write(
                (ushort)
                (
                    ((value & 0x00ff) << 8) +
                    ((value & 0xff00) >> 8)
                )
            );
        }

        public override void Write(short value)
        {
            base.Write(
                (short)
                (
                    ((value & 0x00ff) << 8) +
                    ((value & 0xff00) >> 8)
                )
            );
        }


        public override void Write(uint value)
        {
            base.Write(
                ((value & 0x000000ffU) << 24) +
                ((value & 0x0000ff00U) << 8) +
                ((value & 0x00ff0000U) >> 8) +
                ((value & 0xff000000U) >> 24)
            );
        }


        public override void Write(int value) => this.Write((uint)value);

        public override void Write(ulong value)
        {
            base.Write(
                ((value & 0x00000000000000ffUL) << 56) +
                ((value & 0x000000000000ff00UL) << 40) +
                ((value & 0x0000000000ff0000UL) << 24) +
                ((value & 0x00000000ff000000UL) << 8) +
                ((value & 0x000000ff00000000UL) >> 8) +
                ((value & 0x0000ff0000000000UL) >> 24) +
                ((value & 0x00ff000000000000UL) >> 40) +
                ((value & 0xff00000000000000UL) >> 56)
            );
        }

        public override void Write(long value) => this.Write((ulong)value);
    }

}