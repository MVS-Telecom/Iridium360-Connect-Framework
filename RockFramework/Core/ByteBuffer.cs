using Rock.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
namespace Rock.Core
{
    /// <summary>
    /// This class handles byte buffer arrays.
    /// https://gist.github.com/JustSomeBacon/9236037bb1d97d83cbc8169d0b53cadd
    /// </summary>
    public class ByteBuffer : IDisposable
    {
        /// <summary>
        /// The byte buffer.
        /// </summary>
        public byte[] Bytes => stream.ToArray();
        private readonly MemoryStream stream;

        private readonly BigEndianBinaryReader reader;
        private readonly BigEndianBinaryWriter writer;

        private bool IsReadable => reader != null;
        private bool IsWritable => writer != null;


        private void BeforeRead()
        {
            if (!IsReadable)
                throw new InvalidOperationException("Buffer not readable!");
        }
        
        private void BeforeWrite()
        {
            if (!IsWritable)
                throw new InvalidOperationException("Buffer not writable!");
        }

        /// <summary>
        /// FOR READING
        /// </summary>
        /// <param name="buffer">The byte buffer to use.</param>
        public ByteBuffer(byte[] buffer)
        {
            stream = new MemoryStream(buffer);
            reader = new BigEndianBinaryReader(stream);
        }

        /// <summary>
        /// FOR WRITING
        /// </summary>
        /// <param name="size"></param>
        public ByteBuffer(int size = 0)
        {
            if (size > 0)
                stream = new MemoryStream(new byte[size]);
            else
                stream = new MemoryStream();
            writer = new BigEndianBinaryWriter(stream);
        }

        public static ByteBuffer Allocate(int size)
        {
            return new ByteBuffer(size);
        }

        /// <summary>
        /// Reads a byte from the current buffer position.
        /// </summary>
        /// <returns>Returns the value.</returns>
        public byte ReadByte()
        {
            BeforeRead();
            return reader.ReadByte();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            BeforeRead();
            try
            {

                var max = (int)(reader.BaseStream.Length - reader.BaseStream.Position);

                if (length > max || length < 0)
                    length = max;

                var buffer = reader.ReadBytes(length);

                Array.Resize(ref buffer, length);

                return buffer;

            }
            catch(Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw;
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public byte[] ReadAllBytes()
        {
            try
            {
                var length = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
                // --->
                BeforeRead();
                var buffer = ReadBytes(length);
                return buffer;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        public ByteBuffer SkipBytes(int length)
        {
            BeforeRead();
            ReadBytes(length);
            return this;
        }

        /// <summary>
        /// Reads a signed short from the current buffer position.
        /// </summary>
        /// <returns>Returns the value.</returns>
        public short ReadInt16()
        {
            BeforeRead();
            return reader.ReadInt16();
        }

        /// <summary>
        /// Reads a signed int from the current buffer position.
        /// </summary>
        /// <returns>Returns the value.</returns>
        public int ReadInt32()
        {
            BeforeRead();
            return reader.ReadInt32();
        }
        /// <summary>
        /// Reads a bool from the current buffer position.
        /// </summary>
        /// <returns>Returns the value.</returns>
        public bool ReadBoolean()
        {
            BeforeRead();
            return reader.ReadBoolean();
        }

        /// <summary>
        /// Writes a byte to the current buffer position.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteByte(byte value)
        {
            BeforeWrite();
            writer.Write(value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value)
        {
            BeforeWrite();
            writer.Write(value);
        }

        /// <summary>
        /// Writes a signed short to the current buffer position.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(short value)
        {
            BeforeWrite();
            writer.Write(value);
        }

        /// <summary>
        /// Writes a signed int to the current buffer position.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt32(int value)
        {
            BeforeWrite();
            writer.Write(value);
        }

        /// <summary>
        /// Writes a boolean to the current buffer position.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteBoolean(bool value)
        {
            BeforeWrite();
            writer.Write(value);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            reader?.Dispose();
            writer?.Dispose();
            stream?.Dispose();
        }
    }
}