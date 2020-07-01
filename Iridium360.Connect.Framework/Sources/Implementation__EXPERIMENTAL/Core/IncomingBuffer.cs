using Iridium360.Connect.Framework.Helpers;
using System;
using System.Linq;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class IncomingBuffer
    {
        private DateTime date;
        private byte[] buffer;

#if DEBUG
        static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(60);
#else
        static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(10);
#endif

        public IncomingBuffer()
        {
            flush(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateTime()
        {
            lock (this)
            {
                this.date = DateTime.Now.Add(TIMEOUT);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void flush(bool value)
        {
            lock (this)
            {
                updateTime();
                //if (value || this.buffer == null || this.buffer.Length > 0)
                //{
                //}
                this.buffer = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void add(byte[] data)
        {
            lock (this)
            {
                if (DateTime.Now > this.date)
                {
                    flush(false);
                }

                updateTime();

                if (this.buffer != null)
                {
                    this.buffer = this.buffer.Concat(data).ToArray();
                }
                else
                {
                    this.buffer = data;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] TryGet()
        {
            lock (this)
            {
                if (this.buffer?.Length > 2)
                {
                    int count = ((this.buffer[0] & 0xFF) << 8) + ((this.buffer[1] & 0xFF) << 0);
                    if (this.buffer.Length >= count + 2)
                    {
                        byte[] copy = this.buffer.GetCopy();
                        flush(true);
                        return copy;
                    }
                }
            }
            return null;
        }

        public override string ToString()
        {
            return this.buffer.ToHexString();
        }
    }
}
