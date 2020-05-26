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
                    num |= (byte)(1 << (num2 & 0x1f));
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
















        public void WriteNullable(bool? value)
        {
            int _value = value == null ? int.MaxValue : (value == true ? 1 : 0);

            var array = new BitArray(BitConverter.GetBytes(_value));

            for (int i = 0; i < 2; i++)
                Write(array[i]);

            Trace();
        }


        public void Write(uint value, int bits)
        {
            if (Math.Pow(2, bits) <= value)
                throw new ArgumentOutOfRangeException($"Value {value} bigger then supported range [0..{Math.Pow(2, bits) - 1}]");


            var array = new BitArray(BitConverter.GetBytes(value));

            for (int i = 0; i < bits; i++)
                Write(array[i]);

            Trace();
        }

        public void Write(uint? value, int bits)
        {
            if (value != null && Math.Pow(2, bits) - 1 <= value)
                throw new ArgumentOutOfRangeException($"Value {value} bigger then supported range [0..{Math.Pow(2, bits) - 1}]");


            if (value == null)
            {
                value = uint.MaxValue;
            }

            var array = new BitArray(BitConverter.GetBytes(value.Value));

            for (int i = 0; i < bits; i++)
                Write(array[i]);

            Trace();
        }





        public void Write(int value, int bits)
        {
            if (Math.Pow(2, bits) <= value)
                throw new ArgumentOutOfRangeException($"Value {value} bigger then supported range [0..{Math.Pow(2, bits) - 1}]");

            bool negative = false;

            if (value < 0)
            {
                negative = true;
                value = -value;
            }

            var array = new BitArray(BitConverter.GetBytes(value));

            for (int i = 0; i < bits; i++)
                Write(array[i]);

            Write(negative);
            Trace();
        }

        public void Write(int? value, int bits)
        {
            if (value != null && Math.Pow(2, bits) - 1 <= value)
                throw new ArgumentOutOfRangeException($"Value {value} bigger then supported range [0..{Math.Pow(2, bits) - 1}]");

            bool negative = false;

            if (value == null)
            {
                value = int.MaxValue;
            }
            if (value < 0)
            {
                negative = true;
                value = -value;
            }

            var array = new BitArray(BitConverter.GetBytes(value.Value));

            for (int i = 0; i < bits; i++)
                Write(array[i]);

            Write(negative);
            Trace();
        }

        public void Write(float value, bool signed, int bits, int decimalBits = 14)
        {
            if (Math.Pow(2, bits) - 1 <= value)
                throw new ArgumentOutOfRangeException($"Value {value} bigger then supported range [0..{Math.Pow(2, bits) - 1}]");

            if (value < 0 && !signed)
                throw new InvalidOperationException("Value is negative but `singed` flag is false");


            bool negative = value < 0;

            value = Math.Abs(value);
            var x = value - Math.Truncate(value);
            uint a = (uint)(x * Math.Pow(2, decimalBits));
            uint b = (uint)Math.Truncate(value);

            Write(b, bits);
            Write(a, decimalBits);

            if (signed)
            {
                Write(negative);
            }

            Trace();
        }


        private void Trace()
        {
            System.Diagnostics.Debug.WriteLine(BitPosition);
            Console.WriteLine(BitPosition);
            //Convert.ToString(byteArray[20], 2).PadLeft(8, '0');
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

        public void Write(byte[] buffer, int bitsCount)
        {
            var array = new BitArray(buffer);

            for (int i = 0; i < bitsCount; i++)
                Write(array[i]);
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

