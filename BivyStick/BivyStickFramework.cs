using BivyStick.Sources;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BivyStick
{
    public class DeviceDiscoveredEventArgs : EventArgs
    {
        public BivyStickDevice Device { get; set; }
    }

    public class BivyStickDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class BatteryUpdatedEventArgs : EventArgs
    {
        public int Value { get; set; }
    }



    /// <summary>
    /// 
    /// </summary>
    public class BivyStickFramework
    {
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };
        public event EventHandler DeviceDiscoveryTimeout = delegate { };
        public event EventHandler DeviceConnected = delegate { };
        public event EventHandler DeviceDisconnected = delegate { };
        public event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated = delegate { };

        private IDevice device;
        public bool IsDeviceConnected
        {
            get
            {
                return device?.State == Plugin.BLE.Abstractions.DeviceState.Connected;
            }
        }

        public Guid DeviceId => device?.Id ?? Guid.Empty;

        public string Name => device?.Name;

        public int? Battery { get; private set; }


        public BivyStickFramework()
        {
            var adapter = CrossBluetoothLE.Current.Adapter;

            adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;
            adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Console.WriteLine($"[BivyStick] DISCONNECTED");

            lock (typeof(BivyStickFramework))
            {
                if (device != null)
                {
                    device = null;
                    Battery = null;
                    DeviceDisconnected(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceConnectionLost(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceErrorEventArgs e)
        {
            Console.WriteLine($"[BivyStick] CONNECTION LOST -> {e.ErrorMessage}");

            lock (typeof(BivyStickFramework))
            {
                if (device != null)
                {
                    device = null;
                    Battery = null;
                    DeviceDisconnected(this, new EventArgs());
                }
            }
        }


        public async Task Disconnect()
        {
            if (device != null && device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
            {
                var adapter = CrossBluetoothLE.Current.Adapter;
                await adapter.DisconnectDeviceAsync(device);
            }
        }


        public async Task StartSearch()
        {
            try
            {
                var adapter = CrossBluetoothLE.Current.Adapter;

                if (!adapter.IsScanning)
                {
                    adapter.ScanTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                    adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            DeviceDiscoveryTimeout(this, new EventArgs());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StopSearch()
        {
            try
            {
                var adapter = CrossBluetoothLE.Current.Adapter;
                if (adapter.IsScanning)
                {
                    adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
                    adapter.ScanTimeoutElapsed -= Adapter_ScanTimeoutElapsed;
                    await adapter.StopScanningForDevicesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (e.Device.Name == "Bivystick2")
            {
                Console.WriteLine($"[BivyStick] FOUND {e.Device.Name} -> {e.Device.Id}");

                DeviceDiscovered(this, new DeviceDiscoveredEventArgs()
                {
                    Device = new BivyStickDevice()
                    {
                        Id = e.Device.Id,
                        Name = e.Device.Name
                    }
                });
            }
        }

        private ICharacteristic write;
        private ICharacteristic battery;
        private SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        public async Task Connect(Guid id)
        {
            await connectLock.WaitAsync();

            try
            {
                await StopSearch();

                if (IsDeviceConnected)
                {
                    Console.WriteLine($"[BivyStick] Device already connected");
                    return;
                }

                var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                var adapter = CrossBluetoothLE.Current.Adapter;
                device = await adapter.ConnectToKnownDeviceAsync(id, new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: true), cancel.Token);

                if (device.State != Plugin.BLE.Abstractions.DeviceState.Connected)
                    throw new Exception("Connection error");

                DeviceConnected(this, new EventArgs());

                var services = await device.GetServicesAsync();

                foreach (var service in services)
                {
                    var characterictics = await service.GetCharacteristicsAsync();

                    foreach (var characterictic in characterictics)
                    {
                        Console.WriteLine($"[BivyStick] FOUND `{characterictic.Name}` -> {characterictic.Id} [{characterictic.Properties}]");

                        if (characterictic.CanUpdate && characterictic.CanRead)
                        {
                            characterictic.ValueUpdated += (_s, e) =>
                            {
                                Console.WriteLine($"[BivyStick] CHANGED `{e.Characteristic.Name}` -> 0x{e.Characteristic.Value.ToHexString()}");
                            };
                            await characterictic.StartUpdatesAsync();
                        }
                    }

                    if (write == null)
                        write = characterictics.FirstOrDefault(x => x.Id == Guid.Parse("6A7C1902-9D85-4C27-B811-4F4EF0BA1B56"));


                    if (battery == null)
                        battery = characterictics.FirstOrDefault(x => x.Name == "Battery Level");

                    if (battery != null)
                    {
                        battery.ValueUpdated += (s, e) =>
                        {
                            Console.WriteLine($"[BivyStick] BATTERY -> {e.Characteristic.Value[0]}%");

                            Battery = e.Characteristic.Value[0];

                            BatteryUpdated(this, new BatteryUpdatedEventArgs()
                            {
                                Value = Battery.Value
                            });
                        };
                        await battery.StartUpdatesAsync();
                        await battery.ReadAsync();
                    }

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                throw e;
            }
            finally
            {
                connectLock.Release();
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="text"></param>
        public async Task SendSms(string phoneNumber, string text)
        {
            try
            {
                Console.WriteLine($"[BivyStick] SMS -> {phoneNumber}: {text}");

                var message = createSendMessage(phoneNumber, text);
                var command = createWriteOutboundMessage(message.dataSize, message.data, null);

                await SendCommand(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="text"></param>
        public async Task SendEmail(string email, string text)
        {
            try
            {
                Console.WriteLine($"[BivyStick] EMAIL -> {email}: {text}");

                var message = createSendEmailMessage(email, text);
                var command = createWriteOutboundMessage(message.dataSize, message.data, null);

                await SendCommand(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debugger.Break();
                throw e;
            }
        }


        private static SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task SendCommand(WriteCommand command)
        {
            await locker.WaitAsync();

            try
            {
                Console.WriteLine($"[BivyStick] TRANSFER STARTED -> 0x{command.Command.ToHexString()} ({command.Command.Length} bytes)");

                const int chunksize = 20;

                for (int start = 0; start < command.Command.Length; start += chunksize + 1)
                {
                    var packet = ArrayHelper.GetCopy(command.Command, start, start + chunksize);
                    var ok = await write.WriteAsync(packet);

                    Console.WriteLine($"[BivyStick] PROGRESS {Math.Round(((start + packet.Length) / (double)command.Command.Length) * 100d)}% -> {packet.ToHexString()}");

                    if (!ok)
                        throw new Exception();
                }

                Console.WriteLine($"[BivyStick] TRANSFER COMPLETED");
            }
            finally
            {
                locker.Release();
            }
        }




        internal sealed class BivyStickMessageHandlerKt
        {
            public const sbyte MESSAGE_TYPE_ADD_CHECKIN_TARGETS = 24;
            public const sbyte MESSAGE_TYPE_APP_CHECKIN = 67;
            public const sbyte MESSAGE_TYPE_EMAIL_RECEIVED = 20;
            public const sbyte MESSAGE_TYPE_EMAIL_RECEIVED_NEW = 66;
            public const sbyte MESSAGE_TYPE_EMAIL_SEND = -127;
            public const sbyte MESSAGE_TYPE_EMAIL_WITH_LOC_SEND = -125;
            public const sbyte MESSAGE_TYPE_FREE_LOCATION = 2;
            public const sbyte MESSAGE_TYPE_FREE_MAILBOX = 17;
            public const sbyte MESSAGE_TYPE_PAID_LOCATION = 3;
            public const sbyte MESSAGE_TYPE_PAID_MAILBOX = 18;
            public const sbyte MESSAGE_TYPE_REMOVE_CHECKIN_TARGETS = 25;
            public const sbyte MESSAGE_TYPE_SET_CHECKIN_MSG = 23;
            public const sbyte MESSAGE_TYPE_SOS_CANCEL_RECEIVE = 16;
            public const sbyte MESSAGE_TYPE_SOS_CANCEL_SEND = -104;
            public const sbyte MESSAGE_TYPE_SOS_INITIATE = -106;
            public const sbyte MESSAGE_TYPE_SOS_INITIATED = 11;
            public const sbyte MESSAGE_TYPE_SOS_INITIATED_NEW = 71;
            public const sbyte MESSAGE_TYPE_SOS_LOCATION = 12;
            public const sbyte MESSAGE_TYPE_SOS_TEXT_ACK = 73;
            public const sbyte MESSAGE_TYPE_SOS_TEXT_RECEIVE_NEW = 72;
            public const sbyte MESSAGE_TYPE_SOS_TEXT_SEND = -105;
            public const sbyte MESSAGE_TYPE_TEXT_RECEIVE = 1;
            public const sbyte MESSAGE_TYPE_TEXT_RECEIVE_NEW = 65;
            public static readonly sbyte MESSAGE_TYPE_TEXT_SEND = sbyte.MinValue;
            public const sbyte MESSAGE_TYPE_TEXT_WITH_LOC_SEND = -126;
            public const sbyte MESSAGE_TYPE_TRACKING = 4;
            public const sbyte MESSAGE_TYPE_WEATHER_PREMIUM_PART1 = 8;
            public const sbyte MESSAGE_TYPE_WEATHER_PREMIUM_PART2 = 9;
            public const sbyte MESSAGE_TYPE_WEATHER_REQUEST_BASIC = 5;
            public const sbyte MESSAGE_TYPE_WEATHER_REQUEST_PREMIUM = 6;
            public const sbyte MESSAGE_TYPE_WEATHER_STANDARD = 7;
            public const sbyte SOS_ACK_TYPE_RECEIVED = 1;
            public const sbyte SOS_ACK_TYPE_SEEN = 2;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        internal OutboundMessage createSendEmailMessage(string str, string str2)
        {
            OutboundMessage outboundMessage = new OutboundMessage();// (Byte)null, 0, (byte[])null, 7, (DefaultConstructorMarker)null);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] bytes2 = Encoding.UTF8.GetBytes(str2);
            int length = bytes.Length + bytes2.Length + 6;
            outboundMessage.messageType = BitConverter.GetBytes(BivyStickMessageHandlerKt.MESSAGE_TYPE_EMAIL_SEND)[0];

            using (MemoryStream stream = new MemoryStream(length))
            {
                using (BinaryWriter allocate = new BinaryWriter(stream))
                {
                    //allocate.order(ByteOrder.LITTLE_ENDIAN);
                    byte messageType = outboundMessage.messageType;
                    allocate.Write((byte)messageType);
                    allocate.Write((int)getCurrentTime());
                    allocate.Write((byte)bytes.Length);
                    allocate.Write((byte[])bytes);
                    allocate.Write((byte[])bytes2);
                    outboundMessage.dataSize = length;
                    outboundMessage.data = stream.ToArray();

                    return outboundMessage;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal OutboundMessage createSendMessage(string phoneNumber, string text)
        {
            OutboundMessage outboundMessage = new OutboundMessage();// (Byte)null, 0, (byte[])null, 7, (DefaultConstructorMarker)null);
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            int length = bytes.Length + 13;
            outboundMessage.messageType = Byte.MinValue;

            using (MemoryStream stream = new MemoryStream(length))
            {
                using (BinaryWriter allocate = new BinaryWriter(stream))
                {
                    //allocate.order(ByteOrder.LITTLE_ENDIAN);
                    byte messageType = outboundMessage.messageType;
                    allocate.Write((byte)messageType);
                    allocate.Write((int)getCurrentTime());
                    allocate.Write((long)long.Parse(phoneNumber));
                    allocate.Write((byte[])bytes);
                    outboundMessage.dataSize = length;
                    outboundMessage.data = stream.ToArray();

                    return outboundMessage;
                }
            }
        }


        internal enum SendMessages
        {
            SEND_MESSAGE = ((byte)40)
        }


        private WriteCommand createWriteOutboundMessage(int i, byte[] bArr, Message message)
        {
            int i2 = i;
            byte[] bArr2 = bArr;
            Message message2 = message;
            //String str = TAG;
            //Log.d(str, "createWriteCommandItem: totalDataSize: " + i2 + ", commandData: " + bArr2);
            int i3 = i2 + 3;

            MemoryStream stream = new MemoryStream(512);
            BinaryWriter allocate = new BinaryWriter(stream);

            //allocate.order(ByteOrder.LITTLE_ENDIAN);
            allocate.Write((byte)150);
            allocate.Write((short)i3);
            allocate.Write((byte)SendMessages.SEND_MESSAGE);
            allocate.Write((short)i2);
            allocate.Write(bArr2);
            WriteCommand writeCommand = new WriteCommand((byte[])null, (Message)null, false, false, false, false, false, 0);
            writeCommand.WriteCommand = true;
            writeCommand.Command = stream.ToArray();
            writeCommand.Message = message2;
            writeCommand.MSG = true;
            writeCommand.TotalDataSize = i3 + 3;

            var hex = writeCommand.Command.ToHexString();

            return writeCommand;
        }

        private int getCurrentTime()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
