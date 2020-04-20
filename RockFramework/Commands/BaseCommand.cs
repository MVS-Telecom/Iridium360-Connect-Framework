using Rock.Core;
using Rock.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rock.Commands
{
    public enum CommandType : int
    {
        Unknown,
        SendMessage,
        GetNextMessage,
        DeleteMessage,
        AcknowledgeMessageStatus,
        Internal,
        SendFileSegment,
        Pin,
        ActionRequest,
        GenericAlert,
        SerialDump,
        SetGprsConfig,
        GetGprsConfig
    }


    public abstract class BaseCommand
    {
        public CommandType CommandType;
        public readonly string _AppId;
        private readonly short _Key;


        public BaseCommand(CommandType commandType, string appId, short key)
        {
            this.CommandType = commandType;
            this._AppId = appId;
            this._Key = key;
        }


        /// <summary>
        /// 
        /// </summary>
        public byte[] AppId
        {
            get
            {
                var bytes = Convert.FromBase64String(_AppId).ToList().Take(5).ToArray();

                if (bytes.Length != 5)
                    throw new ArgumentException("AppId must be exactly 5 bytes length");

                return bytes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte Key
        {
            get
            {
                return (byte)_Key;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<byte[]> GetPackets()
        {
            var payload = BuildPayload();

            if (payload == null && payload.Length == 0)
                throw new ArgumentException("Payload is null or empty");

            var packets = SplitPackets(payload);
            return packets;
        }


        /// <summary>
        /// Разделить массив байтов на пакеты по 20 байтов
        /// </summary>
        /// <returns></returns>
        private static List<byte[]> SplitPackets(byte[] payload)
        {
            const float packetMaxLength = 20f;

            List<byte[]> packets = new List<byte[]>();
            int totalCount = (int)Math.Ceiling((payload.Length / packetMaxLength));

            for (byte b = 0; b < totalCount; b++)
            {
                int start = (int)(b * packetMaxLength);
                int size = (int)Math.Min(packetMaxLength, payload.Length - b * packetMaxLength);
                int end = start + size - 1;

                byte[] packet = payload.GetCopy(start, end);

                packet = C0056cb.m152a(packet, 20, 0);
                packets.Add(packet);
            }

            return packets;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private byte[] BuildPayload()
        {
            var body = new ByteBuffer();

            ///Кастомные данные в зависимости от пакета
            AddCustomPayload(body);


            /// 2 -> Length
            /// 1 -> CommandType
            /// 5 -> AppId
            /// 1 -> Key
            var payloadLength = body.Bytes.Length + (2 + 1 + 5 + 1);

            /// ~ Минимальный размер пакета => 20 байтов
            /// ~ 2 байта -> хэш-сумма в конце пакета
            var packageLength = Math.Max(20, payloadLength + 2);
            var buffer = ByteBuffer.Allocate(packageLength);

            ///Данные, одинаковые для всех пакетов
            buffer.WriteInt16((short)payloadLength);
            buffer.WriteByte((byte)CommandType);
            buffer.WriteBytes(AppId);
            buffer.WriteByte(Key);

            ///Кастомные данные в зависимости от пакета
            buffer.WriteBytes(body.Bytes);

            ///Хэш сумма
            buffer.WriteInt16(Checksum.Get(buffer.Bytes, payloadLength));

            return buffer.Bytes;
        }







        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract void AddCustomPayload(ByteBuffer buffer);
    }



    /// <summary>
    /// 
    /// </summary>
    public class DeserializedCommand
    {
        public string AppId { get; set; }
        public short Key { get; set; }
        public CommandType CommandType { get; set; }
        public readonly byte[] Payload;
        public readonly short? MessageId;
        public readonly ActionRequestType? ActionRequestType;


        private DeserializedCommand(CommandType commandType, string appId, short key, byte[] payload, short? messageId, ActionRequestType? actionRequestType)
        {
            this.AppId = AppId;
            this.Key = key;
            this.Payload = payload;
            this.MessageId = messageId;
            this.CommandType = commandType;
            this.ActionRequestType = actionRequestType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DeserializedCommand Parse(byte[] data)
        {
            var buffer = new ByteBuffer(data);

            var payloadLength = buffer.ReadInt16();
            var commandType = (CommandType)buffer.ReadByte();
            var appId = Convert.ToBase64String(buffer.ReadBytes(5));
            var key = buffer.ReadByte();
            var inner = buffer.ReadBytes(payloadLength);

#if DEBUG
            var text = Encoding.UTF8.GetString(inner);
#endif

            if (inner.Length > 0)
            {
                var body = new ByteBuffer(inner);

                switch (commandType)
                {
                    case CommandType.ActionRequest:
                        {
                            var actionRequestType = (ActionRequestType)body.ReadByte();
                            return new DeserializedCommand(commandType, appId, key, null, null, actionRequestType);
                        }

                    case CommandType.SendMessage:
                        {
                            var messageId = body.ReadInt16();
                            return new DeserializedCommand(commandType, appId, key, null, messageId, null);
                        }

                    case CommandType.GetNextMessage:
                        {
                            var messageId = body.ReadInt16();
                            var payload = body.ReadAllBytes();
                            return new DeserializedCommand(commandType, appId, key, payload, messageId, null);
                        }

                    default:
                        //throw new NotImplementedException();
#if DEBUG
                        //Debugger.Break();
#endif
                        break;
                }
            }


            return new DeserializedCommand(commandType, appId, key, null, null, null);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class DeserializedStatus
    {
        public string AppId { get; set; }
        public short Key { get; set; }
        public short? MessageId { get; set; }

        private DeserializedStatus(string appId, short key, short? messageId)
        {
            this.AppId = appId;
            this.Key = key;
            this.MessageId = messageId;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DeserializedStatus Parse(byte[] data)
        {
            ByteBuffer wrap = new ByteBuffer(data);
            string appId = Convert.ToBase64String(wrap.ReadBytes(5));
            short key = (short)wrap.ReadByte();
            short messageId = wrap.ReadInt16();

            return new DeserializedStatus(appId, key, messageId);
        }
    }
}
