using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Messaging.Legacy;
using Iridium360.Connect.Framework.Messaging.Storage;
using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageErrorEventArgs : EventArgs
    {
        public string MessageId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageTransmittedEventArgs : EventArgs
    {
        public string MessageId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageResendNeededEventArgs : EventArgs
    {
        public string MessageId { get; set; }
        public int SendAttempt { get; set; }
        public int ResendParts { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageProgressChangedEventArgs : EventArgs
    {
        public string MessageId { get; set; }
        public uint ReadyParts { get; set; }
        public uint TotalParts { get; set; }
        public double Progress => 100d * (ReadyParts / (double)TotalParts);
    }


    /// <summary>
    /// 
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MessageAckedEventArgs : EventArgs
    {
        public string MessageId { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public interface IFrameworkProxy : IFramework
    {
        event EventHandler<MessageAckedEventArgs> MessageAcked;
        event EventHandler<MessageTransmittedEventArgs> MessageTransmitted;
        event EventHandler<MessageResendNeededEventArgs> MessageResendNeeded;
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler<MessageProgressChangedEventArgs> MessageProgressChanged;

        Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> SendMessage(Message message, Action<double> progress = null);
        Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> RetrySendMessage(string messageId, Action<double> progress = null);
    }




    /// <summary>
    /// 
    /// </summary>
    public class FrameworkProxy : IFrameworkProxy
    {
        event EventHandler<DeviceSearchResultsEventArgs> IFramework.DeviceSearchResults
        {
            add => framework.DeviceSearchResults += value;
            remove => framework.DeviceSearchResults -= value;
        }

        event EventHandler<EventArgs> IFramework.SearchTimeout
        {
            add => framework.SearchTimeout += value;
            remove => framework.SearchTimeout -= value;
        }

        event EventHandler<PacketStatusUpdatedEventArgs> IFramework.PacketStatusUpdated
        {
            add => framework.PacketStatusUpdated += value;
            remove => framework.PacketStatusUpdated -= value;
        }

        event EventHandler<PacketReceivedEventArgs> IFramework.PacketReceived
        {
            add => throw new NotSupportedException($"Use `{nameof(MessageReceived)}` event instead");
            remove => throw new NotSupportedException($"Use `{nameof(MessageReceived)}` event instead");
        }


        public event EventHandler<MessageAckedEventArgs> MessageAcked;
        public event EventHandler<MessageTransmittedEventArgs> MessageTransmitted;
        public event EventHandler<MessageResendNeededEventArgs> MessageResendNeeded;
        public event EventHandler<MessageProgressChangedEventArgs> MessageProgressChanged;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        private IFramework framework;
        private ILogger logger;
        private IPacketBuffer buffer;
        private IStorage storage;


        public IDevice ConnectedDevice => framework.ConnectedDevice;



        public FrameworkProxy(IFramework framework, ILogger logger, IPacketBuffer buffer, IStorage storage)
        {
            this.framework = framework;
            this.logger = logger;
            this.buffer = buffer;
            this.storage = storage;

            framework.PacketStatusUpdated += Framework__PacketStatusUpdated;
            framework.PacketReceived += Framework__PacketReceived;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Framework__PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            logger.Log($"[MESSAGE] Packet received -> `0x{e.Payload.ToHexString()}`");

            try
            {
                if (Legacy_MessageMT.CheckSignature(e.Payload))
                {
                    Debugger.Break();

                    var legacy = Legacy_MessageMT.Unpack(e.Payload);

                    if (legacy.Complete)
                    {
                        Debugger.Break();

                        logger.Log($"[MESSAGE] Legacy message received Group={legacy.Group} Index={legacy.Index} Progress={legacy.ReadyParts}/{legacy.TotalParts} -> COMPLETED");

                        var subscriber = legacy.GetSubscriber();
                        var text = legacy.GetText();

                        var message = ChatMessageMT.Create(ProtocolVersion.v1, subscriber, null, null, text);

                        MessageReceived(this, new MessageReceivedEventArgs()
                        {
                            Message = message
                        });
                    }
                    else
                    {
                        logger.Log($"[MESSAGE] Legacy message received Group={legacy.Group} Index={legacy.Index} Progress={legacy.ReadyParts}/{legacy.TotalParts} -> INCOMPLETE - waiting for next parts");
                        Debugger.Break();
                    }
                }
                else if (Framework.Messaging.Message.CheckSignature(e.Payload))
                {
                    Debugger.Break();

                    var message = Message.Unpack(e.Payload);

                    if (message.Complete)
                    {
                        Debugger.Break();

                        logger.Log($"[MESSAGE] Message received Group={message.Group} Index={message.Index} Progress={message.ReadyParts}/{message.TotalParts} -> COMPLETED");

                        if (message is MessageAckMT resendMessage)
                        {
                            Debugger.Break();
                            AckMessage(resendMessage.TargetGroup, resendMessage.ResendIndexes);
                        }
                        else
                        {
                            MessageReceived(this, new MessageReceivedEventArgs()
                            {
                                Message = message
                            });
                        }
                    }
                    else
                    {
                        logger.Log($"[MESSAGE] Message received Group={message.Group} Index={message.Index} Progress={message.ReadyParts}/{message.TotalParts} -> INCOMPLETE - waiting for next parts");
                        Debugger.Break();
                    }
                }
                else
                {
                    Debugger.Break();
                    throw new NotImplementedException("Unknown bytes");
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                logger.Log($"[MESSAGE] Exception occured while parsing `{e.Payload.ToHexString()}` {ex}");
            }


            Debugger.Break();
            e.Handled = true;
        }



        /// <summary>
        /// 
        /// </summary>
        private void AckMessage(byte group, byte[] indexes)
        {
            Debugger.Break();

            var message = buffer.GetMessageByGroup(group, PacketDirection.Outbound);

            if (message == null)
            {
                logger.Log($"[ACK ERROR] Message with group `{group}` not found");
                Debugger.Break();
                return;
            }

            if (indexes.Any())
            {
                var packets = buffer.GetPackets((uint)group, PacketDirection.Outbound);
                var resend = packets.Where(x => indexes.Contains((byte)x.Index)).ToList();


                foreach (var packet in resend)
                    buffer.SetPacketStatus(packet.Id, PacketStatus.None);


                if (message.SendAttempt >= 3)
                {
                    logger.Log($"[ACK ERROR] Resend attempts exceeded for message `{message.Id}`");
                    Debugger.Break();

                    ///TODO:
                }
                else
                {
                    logger.Log($"[ACK] Message `{message.Id}` parts have to be resended {string.Join(", ", resend.Select(x => x.FrameworkId))}");
                    Debugger.Break();

                    message.SendAttempt += 1;
                    buffer.SetMessageSendAttempt(message.Id, message.SendAttempt);

                    MessageResendNeeded(this, new MessageResendNeededEventArgs()
                    {
                        MessageId = message.Id,
                        SendAttempt = message.SendAttempt,
                        ResendParts = indexes.Length
                    });

                    MessageProgressChanged(this, new MessageProgressChangedEventArgs()
                    {
                        MessageId = message.Id,
                        ReadyParts = (uint)(packets.Count - resend.Count),
                        TotalParts = message.TotalParts,
                    });
                }
            }
            else
            {
                logger.Log($"[ACK] Message acked `{message.Id}`");
                Debugger.Break();

                MessageAcked(this, new MessageAckedEventArgs()
                {
                    MessageId = message.Id
                });
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Framework__PacketStatusUpdated(object sender, PacketStatusUpdatedEventArgs e)
        {
            try
            {
                var packet = buffer.GetPacket(e.MessageId);

                if (packet == null)
                {
#if DEBUG
                    if (e.Status != MessageStatus.ReceivedByDevice)
                        Debugger.Break();
#endif
                    e.Handled = true;
                    return;
                }


                switch (e.Status)
                {
                    case MessageStatus.ReceivedByDevice:
                        break;

                    case MessageStatus.Transmitted:
                        logger.Log($"[PACKET] `{e.MessageId}` -> Transmitted");
                        Debugger.Break();

                        buffer.SetPacketStatus(e.MessageId, PacketStatus.Transmitted);
                        break;

                    default:
                        ///Что-то нехорошее
                        logger.Log($"[PACKET] `{e.MessageId}` -> {e.Status}");
                        Debugger.Break();
                        break;
                }



                if (e.Status == MessageStatus.Transmitted)
                {
                    var message = buffer.GetMessageByGroup(packet.Group, packet.Direction);

                    if (message == null)
                    {
                        logger.Log($"Message with group `{packet.Group}` not found");
                        Debugger.Break();
                        e.Handled = true;
                        return;
                    }


                    ///Кол-во отправленных чатей сообщения
                    var transmittedCount = buffer
                        .GetPackets(packet.Group, packet.Direction)
                        .Where(x => x.Status >= PacketStatus.Transmitted)
                        .Count();


                    logger.Log($"[MESSAGE] Message `{message.Id}` progress changed -> {Math.Round(100 * (transmittedCount / (double)packet.TotalParts), 1)}% ({transmittedCount}/{packet.TotalParts})");

                    MessageProgressChanged(this, new MessageProgressChangedEventArgs()
                    {
                        MessageId = message.Id,
                        ReadyParts = (uint)transmittedCount,
                        TotalParts = packet.TotalParts
                    });


                    ///Все части отправлены == сообщение передано
                    if (transmittedCount == packet.TotalParts)
                    {
                        logger.Log($"[MESSAGE] `{message.Id}` ({message.Type}) -> Transmitted");
                        Debugger.Break();

                        MessageTransmitted(this, new MessageTransmittedEventArgs()
                        {
                            MessageId = message.Id
                        });


                        ///HACK: Если сообщение состоит из одной части - подтверждение не придет с сервера
                        ///- считаем что оно не может потеряться (на самом деле может) и не потребует переотправки (на самом деле может потребовать)

                        if (message.TotalParts == 1)
                        {
                            MessageAcked(this, new MessageAckedEventArgs()
                            {
                                MessageId = message.Id
                            });
                        }

                        ///Удаляем пакеты -> они отправлены и больше не нужны
                        //buffer.DeletePackets(packet.Group, packet.Direction);

                        ///TODO:
                        //buffer.DeleteMessage



                        ///TODO: А если сообщение состояло из одной части и она потерялась?
                        if (packet.TotalParts > 1)
                        {
                            Debugger.Break();

                            ///Отправляем подтверждение того что все сообщение ушло
                            Task.Run(async () =>
                            {
                                logger.Log("[MESSAGE SENT] ~~~~~~~~~~~~~~~~~~~~~~");

                                var sent = MessageSentMO.Create(ProtocolVersion.v3__WeatherExtension, (byte)packet.Group);
                                var result2 = await SendMessage(sent);

                                logger.Log("[MESSAGE SENT] ~~~~~~~~~~~~~~~~~~~~~~");
                                Debugger.Break();
                            });
                        }
                    }
                    else
                    {
                        ///Не все пакеты сообщения отправлены
                    }
                }

                e.Handled = true;

            }
            catch (Exception ex)
            {
                logger.Log(ex);
                Debugger.Break();

#if DEBUG
                e.Handled = false;
#else
                e.Handled = false;
#endif
            }
        }


        private static SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> RetrySendMessage(string messageId, Action<double> progress = null)
        {
            try
            {
                await Reconnect();

                var message = buffer.GetMessageById(messageId);

                if (message == null)
                    throw new NullReferenceException($"Message with id `{messageId}` not found");


                var packets = buffer
                    .GetPackets(message.Group, PacketDirection.Outbound)
                    .Where(x => x.Status == PacketStatus.None)
                    .ToList();

                if (packets.Count == 0)
                {
                    Debugger.Break();
                    return (messageId, 0, 0, true);
                }

                buffer.SetMessageSendAttempt(message.Id, message.SendAttempt + 1);

                (int readyParts, int totalParts, bool transferSuccess) = await SendPackets(packets, progress: progress);

                return (messageId, readyParts, totalParts, transferSuccess);
            }
            catch (Exception e)
            {
                Debugger.Break();
                logger.Log($"[RESEND] Error {e}");
                throw e;
            }
            finally
            {

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> SendMessage(Message message, Action<double> progress = null)
        {
            await sendLock.WaitAsync();

            return await Task.Run(async () =>
            {
                ///Сначала убеждаемся что есть подключение к трекеру
                await Reconnect();


                var messageId = ShortGuid.NewGuid().ToString();
                var group = (byte)storage.GetShort("r7-group-id", 1);


                try
                {
                    ///Т.к мы ограничены максимальным кол-вом частей == byte.max - делаем ротэйт
                    buffer.DeleteMessage(group);
                    buffer.DeletePackets(group, PacketDirection.Outbound);


                    var packets = message.Pack(group);

                    logger.Log($"[MESSAGE] Sending message Id=`{messageId}` Parts=`{packets.Count}` Type=`{message.GetType().Name}` Text=`{(message as ChatMessageMO)?.Text}` Location=`{(message as MessageWithLocation)?.Lat}, {(message as MessageWithLocation)?.Lon}`");


                    foreach (var packet in packets)
                        logger.Log($"   => 0x{packet.Payload.ToHexString()}");


                    if (packets.Count > 1)
                        progress?.Invoke(0);



                    ///Сразу увеличиваем - если будет ошибка, то для следующей отправки Group уже будет новый
                    storage.PutShort("r7-group-id", (byte)(group + 1));


                    buffer.SaveMessage(new Storage.Message()
                    {
                        Id = messageId,
                        Group = group,
                        TotalParts = (byte)packets.Count,
                        Type = message.Type,
                        SendAttempt = 1,
                    });


                    (int readyParts, int totalParts, bool transferSuccess) = await SendPackets(packets, progress: progress);

                    return (messageId, readyParts, totalParts, transferSuccess);
                }
                finally
                {
                    sendLock.Release();
                }
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<(int readyParts, int totalParts, bool transferSuccess)> SendPackets(List<Packet> packets, bool throwOnError = false, Action<double> progress = null)
        {
            List<Packet> __packets = new List<Packet>();


            ///Сохраняем пакеты в хранилище
            foreach (var __packet in packets)
            {
                var packet = buffer.GetPacket(__packet.Id);

                if (packet == null || packet.Status < PacketStatus.SendingToDevice)
                {
                    buffer.SavePacket(__packet);
                    __packets.Add(__packet);
                }
            }


            if (__packets.Count == 0)
                return (0, 0, true);


            ///Передаем пакеты на устройство
            for (int i = 0; i < __packets.Count; i++)
            {
                var packet = __packets[i];
                buffer.SetPacketStatus(packet.Id, PacketStatus.SendingToDevice);

                try
                {
                    ushort packetId = await SendData(packet.Payload);
                    packet.FrameworkId = packetId;

                    buffer.SetPacketStatus(packet.Id, PacketStatus.TransferredToDevice, packetId);

                    logger.Log($"[PACKET] Transferred to device with Id={packetId}");
                }
                catch (Exception e)
                {
                    logger.Log($"[PACKET] Transfer to device error {e}");
                    Debugger.Break();

                    buffer.SetPacketStatus(packet.Id, PacketStatus.None);

                    if (throwOnError)
                        throw e;

                    return (i + 1, __packets.Count, false);
                }


                double __progress = 100d * ((i + 1) / (double)packets.Count);

                if (packets.Count > 1)
                    progress?.Invoke(__progress);
            }


            return (__packets.Count, __packets.Count, true);
        }




        public Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false, int attempts = 1) => framework.Connect(id, force, throwOnError, attempts);

        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false) => framework.Connect(device, force, throwOnError);

        public Task Disconnect() => framework.Disconnect();

        public Task ForgetDevice() => framework.ForgetDevice();

        public Task SendManual() => framework.SendManual();

        public Task RequestAlert() => framework.RequestAlert();

        public Task Beep() => framework.Beep();

        public Task GetReceivedMessages() => framework.GetReceivedMessages();

        public Task StartDeviceSearch() => framework.StartDeviceSearch();

        public void StopDeviceSearch() => framework.StopDeviceSearch();

        public Task<ushort> SendData(byte[] data) => framework.SendData(data);

        public void Dispose() => framework.Dispose();

        public Task<bool> Reconnect(bool throwOnError = true) => framework.Reconnect(throwOnError: throwOnError);
    }
}
