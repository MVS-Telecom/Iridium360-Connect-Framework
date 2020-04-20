using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    public class BinaryBitReader : BinaryReader
    {
        private bool[] curByte;

        public BinaryBitReader(Stream s) : base(s)
        {
            BitPosition = 8;
            this.curByte = new bool[8];
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            for (int i = index; i < (index + count); i++)
            {
                buffer[i] = this.ReadByte();
            }
            return count;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            for (int i = index; i < (index + count); i++)
            {
                buffer[i] = this.ReadChar();
            }
            return count;
        }

        public override bool ReadBoolean()
        {
            if (this.BitPosition == 8)
            {
                byte[] bytes = new byte[] { (byte)base.ReadByte() };
                new BitArray(bytes).CopyTo(this.curByte, 0);
                this.BitPosition = 0;
            }
            bool flag = this.curByte[this.BitPosition];
            byte bitPosition = this.BitPosition;
            this.BitPosition = (byte)(bitPosition + 1);
            return flag;
        }

        public override byte ReadByte()
        {
            bool[] flagArray = new bool[8];
            byte index = 0;
            while (true)
            {
                if (index >= 8)
                {
                    byte num2 = 0;
                    byte num3 = 0;
                    index = 0;
                    while (index < 8)
                    {
                        if (flagArray[index])
                        {
                            num2 |= (byte)( 1 << (num3 & 0x1f));
                        }
                        num3++;
                        index++;
                    }
                    return num2;
                }
                flagArray[index] = this.ReadBoolean();
                index++;
            }
        }

        public override byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            for (int i = 0; i < count; i++)
            {
                buffer[i] = this.ReadByte();
            }
            return buffer;
        }

        public override char ReadChar()
        {
            this.BitPosition = 8;
            return base.ReadChar();
        }

        public override char[] ReadChars(int count)
        {
            char[] buffer = new char[count];
            this.Read(buffer, 0, count);
            return buffer;
        }

        public override decimal ReadDecimal()
        {
            int[] bits = new int[4];
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = this.ReadInt32();
            }
            return new decimal(bits);
        }

        public override double ReadDouble() =>
            BitConverter.ToDouble(this.ReadBytes(8), 0);

        public override short ReadInt16() =>
            BitConverter.ToInt16(this.ReadBytes(2), 0);

        public override int ReadInt32() =>
            BitConverter.ToInt32(this.ReadBytes(4), 0);

        public override long ReadInt64() =>
            BitConverter.ToInt64(this.ReadBytes(8), 0);

        public override sbyte ReadSByte() =>
            ((sbyte)this.ReadByte());

        public override float ReadSingle() =>
            BitConverter.ToSingle(this.ReadBytes(4), 0);

        public override string ReadString()
        {
            this.BitPosition = 8;
            return base.ReadString();
        }

        public override ushort ReadUInt16() =>
            BitConverter.ToUInt16(this.ReadBytes(2), 0);

        public override uint ReadUInt32() =>
            BitConverter.ToUInt32(this.ReadBytes(4), 0);

        public override ulong ReadUInt64() =>
            BitConverter.ToUInt64(this.ReadBytes(8), 0);

        public byte BitPosition { get; private set; }
    }
}
