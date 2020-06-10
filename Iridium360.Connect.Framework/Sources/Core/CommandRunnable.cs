using Rock.Commands;
using Rock.Threading;
using Rock.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rock
{

    /// <summary>
    /// 
    /// </summary>
    public class CommandRunnable : BaseRunnable
    {
        public override string Name => $"{base.Name} {command.CommandType}";

        private FrameworkInstance framework;
        private BaseCommand command;


        public CommandRunnable(FrameworkInstance framework, BaseCommand command, ILogger logger, Action onFinish, Action<Exception> onError)
            : base(logger, onFinish, onError)
        {
            this.framework = framework;
            this.command = command;
        }


        protected override async Task InternalRun()
        {
            var values = new List<MessageChunk>();
            var packets = command.GetPackets();

            for (int i = 0; i < packets.Count; i++)
            {
                byte[] payload = packets[i];

                values.Add(new MessageChunk(
                    payload,
                    (command as SendMessageCommand)?.MessageId,
                    i + 1,
                    packets.Count,
                    command.CommandType));

            }

            framework.queue.Push(values);

            await framework.ProcessQueue();
        }
    }
}
