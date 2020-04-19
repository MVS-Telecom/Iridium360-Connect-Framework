using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace Iridium360.Connect.Framework.Messaging
{
    public class BinaryBitWriter : BinaryWriter
    {
        private bool[] curByte;
        private BitArray ba;

        public BinaryBitWriter(Stream s) : base(s)
        {
            this.BitPosition = 0;
            this.curByte = new bool[8];
        }

        private static byte ConvertToByte(bool[] bools)
        {
            byte num = 0;
            byte num2 = 0;
            for (int i = 0; i < 8; i++)
            {
                if (bools[i])
                {
                    num |= (byte)( 1 << (num2 & 0x1f));
                }
                num2++;
            }
            return num;
        }

        public override void Flush()
        {
            this.flushBitBuffer();
            base.Flush();
        }

        private void flushBitBuffer()
        {
            if (this.BitPosition != 0)
            {
                base.Write(ConvertToByte(this.curByte));
                this.BitPosition = 0;
                this.curByte = new bool[8];
            }
        }

        public void Skip(byte count)
        {
            while (count > 0)
            {
                this.Write(false);
                count--;
            }
        }

        public override void Write(bool value)
        {
            this.curByte[this.BitPosition] = value;
            byte bitPosition = this.BitPosition;
            this.BitPosition = (byte)(bitPosition + 1);
            if (this.BitPosition == 8)
            {
                this.flushBitBuffer();
            }
        }

        public override void Write(byte value)
        {
            byte[] bytes = new byte[] { (byte)value };
            this.ba = new BitArray(bytes);
            for (byte i = 0; i < 8; i++)
            {
                this.Write(this.ba[i]);
            }
        }

        public override void Write(char value)
        {
            this.flushBitBuffer();
            base.Write(value);
        }

        public override void Write(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            for (int i = 0; i < bits.Length; i++)
            {
                this.Write(bits[i]);
            }
        }

        public override void Write(double value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(short value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(int value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(long value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(sbyte value)
        {
            this.Write((byte)((byte)value));
        }

        public override void Write(float value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(string value)
        {
            this.flushBitBuffer();
            base.Write(value);
        }

        public override void Write(ushort value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(uint value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(ulong value)
        {
            this.Write(BitConverter.GetBytes(value));
        }

        public override void Write(byte[] buffer)
        {
            this.Write(buffer, 0, buffer.Length);
        }

        public override void Write(char[] value)
        {
            this.Write(value, 0, value.Length);
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            for (int i = index; i < (index + count); i++)
            {
                this.Write(buffer[i]);
            }
        }

        public override void Write(char[] chars, int index, int count)
        {
            for (int i = index; i < (index + count); i++)
            {
                this.Write(chars[i]);
            }
        }

        public void Write6(byte b)
        {
            BinaryBitWriter writer = this;
            if (b > 0x3f)
            {
                throw new InvalidOperationException();
            }
            writer.Write((bool)((b & 1) == 1));
            writer.Write((bool)(((b >> 1) & 1) == 1));
            writer.Write((bool)(((b >> 2) & 1) == 1));
            writer.Write((bool)(((b >> 3) & 1) == 1));
            writer.Write((bool)(((b >> 4) & 1) == 1));
            writer.Write((bool)(((b >> 5) & 1) == 1));
        }

        public byte BitPosition { get; private set; }
    }
}

