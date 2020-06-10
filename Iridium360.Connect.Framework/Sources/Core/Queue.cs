using Rock.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rock
{
    /// <summary>
    /// 
    /// </summary>
    public class Queue
    {
        public bool HasItems => this.queue.Any();
        public MessageChunk current = null;
        private List<MessageChunk> queue = new List<MessageChunk>();

        public MessageChunk Shift()
        {
            lock (queue)
            {
                current = queue.ElementAtOrDefault(0);

                if (current != null)
                    queue.RemoveAt(0);
            }

            return current;
        }


        public void Push(List<MessageChunk> values)
        {
            if (values != null)
            {
                lock (this.queue)
                {
                    this.queue.AddRange(values);
                }
            }
        }


        public void RemoveByType(CommandType commandType)
        {
            lock (this.queue)
            {
                this.queue.RemoveAll(x =>
                {
                    return x.CommandType == commandType;
                });
            }
        }


        public int GetSize()
        {
            lock (this.queue)
            {
                return this.queue.Count;
            }
        }
    }
}
