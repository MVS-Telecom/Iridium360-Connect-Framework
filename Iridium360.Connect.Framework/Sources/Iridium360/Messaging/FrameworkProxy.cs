using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Messaging.Legacy;
using Iridium360.Connect.Framework.Messaging.Storage;
using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    internal class EventNotHandledException : Exception
    {
        public EventNotHandledException()
        {
        }

        public EventNotHandledException(string message) : base(message)
        {
        }

        public EventNotHandledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EventNotHandledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


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
        public bool Error { get; set; }
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
    public class MessageNotFoundException : Exception
    {
        public MessageNotFoundException()
        {
        }

        public MessageNotFoundException(string message) : base(message)
        {
        }

        public MessageNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MessageNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
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
        event EventHandler<MessageProgressChangedEventArgs> MessageTransmitProgressChanged;
        event EventHandler<MessageProgressChangedEventArgs> MessageTransferProgressChanged;

        /// <summary>
        /// Сбросить пакеты, находящиеся в состоянии "Отправляется на устройство"
        /// </summary>
        void ResetHungPackets();

        /// <summary>
        /// Кол-во пакетов находящихся в отправке (на устройстве)
        /// </summary>
        /// <returns></returns>
        int GetTransmittingPackets();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> SendMessage(Message message, string messageId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [Obsolete("Должен быть один метод SendMessage")]
        Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> RetrySendMessage(string messageId);
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
        public event EventHandler<MessageProgressChangedEventArgs> MessageTransmitProgressChanged;
        public event EventHandler<MessageProgressChangedEventArgs> MessageTransferProgressChanged;
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
        /// <returns></returns>
        public int GetTransmittingPackets()
        {
            return buffer.GetTransmittingPackets();
        }


        /// <summary>
        /// 
        /// </summary>
        public void ResetHungPackets()
        {
            buffer.ResetHungPackets();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Framework__PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            try
            {
                logger.Log($"[MESSAGE] Packet received -> `0x{e.Payload.ToHexString()}`");

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

                        ExecuteEvent(() =>
                        {
                            MessageReceived(this, new MessageReceivedEventArgs()
                            {
                                Message = message
                            });
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

                            ExecuteEvent(() =>
                            {
                                AckMessage(resendMessage.TargetGroup, resendMessage.ResendIndexes);
                            });
                        }
                        else
                        {
                            ExecuteEvent(() =>
                            {
                                MessageReceived(this, new MessageReceivedEventArgs()
                                {
                                    Message = message
                                });
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
                    logger.Log($"[MESSAGE] Unknown bytes");
                }
            }
            catch (EventNotHandledException ex1)
            {
                Debugger.Break();
                logger.Log($"[MESSAGE] External exception occured while parsing `{e.Payload.ToHexString()}` {ex1}");
                throw ex1;
            }
            catch (Exception ex)
            {
                Debugger.Break();
                logger.Log($"[MESSAGE] Exception occured while parsing `{e.Payload.ToHexString()}` {ex}");
            }


            Debugger.Break();
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

                    ExecuteEvent(() =>
                    {
                        MessageResendNeeded(this, new MessageResendNeededEventArgs()
                        {
                            MessageId = message.Id,
                            SendAttempt = message.SendAttempt,
                            ResendParts = indexes.Length
                        });
                    });

                    ExecuteEvent(() =>
                    {
                        MessageTransmitProgressChanged(this, new MessageProgressChangedEventArgs()
                        {
                            MessageId = message.Id,
                            ReadyParts = (uint)(packets.Count - resend.Count),
                            TotalParts = message.TotalParts,
                        });
                    });
                }
            }
            else
            {
                logger.Log($"[ACK] Message acked `{message.Id}`");
                Debugger.Break();

                ExecuteEvent(() =>
                {
                    MessageAcked(this, new MessageAckedEventArgs()
                    {
                        MessageId = message.Id
                    });
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
                    //e.Handled = true;
                    return;
                }


                switch (e.Status)
                {
                    case MessageStatus.ReceivedByDevice:
                        break;

                    case MessageStatus.Transmitted:
                        logger.Log($"[PACKET] `{e.MessageId}` -> Transmitted");
                        //Debugger.Break();
                        break;

                    default:
                        ///Что-то нехорошее
                        logger.Log($"[PACKET] `{e.MessageId}` -> {e.Status}");
                        Debugger.Break();
                        break;
                }



                if (e.Status == MessageStatus.Transmitted)
                {
                    ///Прошлое кол-во отправленных чатей сообщения
                    var oldCount = buffer
                        .GetPackets(packet.Group, packet.Direction, includePayload: false)
                        .Where(x => x.Status >= PacketStatus.Transmitted)
                        .Count();


                    ///Обновляем статус пакета
                    buffer.SetPacketStatus(e.MessageId, PacketStatus.Transmitted);


                    var message = buffer.GetMessageByGroup(packet.Group, packet.Direction);

                    if (message == null)
                    {
                        logger.Log($"Message with group `{packet.Group}` not found");
                        Debugger.Break();
                        //e.Handled = true;
                        return;
                    }


                    ///Новое кол-во отправленных чатей сообщения
                    var transmittedCount = buffer
                        .GetPackets(packet.Group, packet.Direction, includePayload: false)
                        .Where(x => x.Status >= PacketStatus.Transmitted)
                        .Count();


                    ///Такое возможно (глюк фреймворка / глюк / дисконнект блютуза)
                    if (oldCount == transmittedCount)
                    {
                        Console.WriteLine("Transmitted parts count not changed");
                        Debugger.Break();
                        return;
                    }


                    logger.Log($"[MESSAGE] Message `{message.Id}` progress changed -> {Math.Round(100 * (transmittedCount / (double)packet.TotalParts), 1)}% ({transmittedCount}/{packet.TotalParts})");

                    ExecuteEvent(() =>
                    {
                        MessageTransmitProgressChanged(this, new MessageProgressChangedEventArgs()
                        {
                            MessageId = message.Id,
                            ReadyParts = (uint)transmittedCount,
                            TotalParts = packet.TotalParts
                        });
                    });


                    ///Все части отправлены == сообщение передано
                    if (transmittedCount == packet.TotalParts)
                    {
                        logger.Log($"[MESSAGE] `{message.Id}` ({message.Type}) -> Transmitted");
                        //Debugger.Break();

                        ExecuteEvent(() =>
                        {
                            MessageTransmitted(this, new MessageTransmittedEventArgs()
                            {
                                MessageId = message.Id
                            });
                        });

                        ///HACK: Если сообщение состоит из одной части - подтверждение не придет с сервера
                        ///- считаем что оно не может потеряться (на самом деле может) и не потребует переотправки (на самом деле может потребовать)

                        if (message.TotalParts == 1)
                        {
                            ExecuteEvent(() =>
                            {
                                MessageAcked(this, new MessageAckedEventArgs()
                                {
                                    MessageId = message.Id
                                });
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
            }
            catch (EventNotHandledException ex1)
            {
                logger.Log(ex1);
                Debugger.Break();
                throw ex1;
            }
            catch (Exception ex)
            {
                logger.Log(ex);
                Debugger.Break();
            }
        }


        private static SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> RetrySendMessage(string messageId)
        {
            try
            {
                await Reconnect();

                var message = buffer.GetMessageById(messageId);

                if (message == null)
                    throw new MessageNotFoundException($"Message with id `{messageId}` not found");


                var packets = buffer
                    .GetPackets(message.Group, PacketDirection.Outbound)
                    .Where(x => x.Status == null || x.Status < PacketStatus.SendingToDevice)
                    .ToList();

                if (packets.Count == 0)
                {
                    Debugger.Break();
                    return (messageId, 0, 0, true);
                }

                var result = await SendMessagePackets(message.Id, packets);

                return (messageId, result.readyParts, result.totalParts, result.transferSuccess);
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
        public async Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> SendMessage(Message message, string messageId = null)
        {
            await sendLock.WaitAsync();

            return await Task.Run(async () =>
            {
                ///Сначала убеждаемся что есть подключение к трекеру
                await Reconnect();


                if (string.IsNullOrEmpty(messageId))
                    messageId = ShortGuid.NewGuid().ToString();

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


                    ///Сразу увеличиваем - если будет ошибка, то для следующей отправки Group уже будет новый
                    storage.PutShort("r7-group-id", (byte)(group + 1));


                    ///Сохраняем сообщение
                    buffer.SaveMessage(new Storage.Message()
                    {
                        Id = messageId,
                        Group = group,
                        TotalParts = (byte)packets.Count,
                        Type = message.Type,
                        SendAttempt = 1,
                    });

                    ///Сохраняем пакеты сообщения
                    packets.ForEach(x =>
                    {
                        buffer.SavePacket(x);
                    });

                    var result = await SendMessagePackets(messageId, packets);

                    return (messageId, result.readyParts, result.totalParts, result.transferSuccess);
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
        private async Task<(int readyParts, int totalParts, bool transferSuccess)> SendMessagePackets(string messageId, List<Packet> packets, bool throwOnError = false)
        {
#if DEBUG
            foreach (var x in packets)
            {
                var packet = buffer.GetPacket(x.Id);

                if (packet != null && packet.Status != null && packet.Status >= PacketStatus.SendingToDevice)
                    Debugger.Break();
            };
#endif


            int ready = buffer.GetMessageById(messageId).TotalParts - packets.Count;


            MessageTransferProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs()
            {
                MessageId = messageId,
                TotalParts = (uint)(packets.Count + ready),
                ReadyParts = (uint)ready,
            });



            ///Передаем пакеты на устройство
            for (int i = 0; i < packets.Count; i++)
            {
                var packet = packets[i];
                buffer.SetPacketStatus(packet.Id, PacketStatus.SendingToDevice);

                try
                {
                    ushort packetId = await SendData(packet.Payload);
                    packet.FrameworkId = packetId;

                    buffer.SetPacketStatus(packet.Id, PacketStatus.TransferredToDevice, packetId);

                    logger.Log($"[PACKET] Transferred to device with Id={packetId} `0x{packet.Payload.ToHexString()}`");


                    MessageTransferProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs()
                    {
                        MessageId = messageId,
                        TotalParts = (uint)(packets.Count + ready),
                        ReadyParts = (uint)(i + 1 + ready),
                    });
                }
                catch (Exception e)
                {
                    logger.Log($"[PACKET] Transfer to device error {e}");
                    Debugger.Break();

                    buffer.SetPacketStatus(packet.Id, PacketStatus.None);

                    if (throwOnError)
                        throw e;


                    MessageTransferProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs()
                    {
                        MessageId = messageId,
                        TotalParts = (uint)(packets.Count + ready),
                        ReadyParts = (uint)(i + ready),
                        Error = true,
                    });


                    return (i + ready, packets.Count + ready, false);
                }
            }


            return (packets.Count + ready, packets.Count + ready, true);

        }


        /// <summary>
        /// Вызовы всех внешних событий должны быть обернуты этой функцией
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteEvent(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debugger.Break();
                logger.Log($"[EXCEPTION] {ex}");
                throw new EventNotHandledException("Exception occured in event", ex);
            }
        }

        public async Task ForgetDevice()
        {
            await framework.ForgetDevice();
            buffer.DeleteAll();
        }



        public Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false, int attempts = 1) => framework.Connect(id, force, throwOnError, attempts);

        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false, int attempts = 1) => framework.Connect(device, force, throwOnError, attempts);

        public Task Disconnect() => framework.Disconnect();

        public Task SendManual() => framework.SendManual();

        public Task RequestAlert() => framework.RequestAlert();

        public Task Beep() => framework.Beep();

        public Task GetReceivedMessages() => framework.GetReceivedMessages();

        public Task StartDeviceSearch() => framework.StartDeviceSearch();

        public void StopDeviceSearch() => framework.StopDeviceSearch();

        public Task<ushort> SendData(byte[] data) => framework.SendData(data);

        public void Dispose() => framework.Dispose();

        public Task<bool> Reconnect(bool throwOnError = true, int attempts = 1) => framework.Reconnect(throwOnError: throwOnError, attempts: attempts);
    }
}
