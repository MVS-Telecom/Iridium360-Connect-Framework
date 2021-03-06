﻿using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Implementations;
using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Messaging.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Message = Iridium360.Connect.Framework.Messaging.Message;

namespace Iridium360.Connect.Framework.Fakes
{
    internal class FakeDeviceParameter : __DeviceParameter
    {
        public FakeDeviceParameter(IFramework framework, IDevice device, Parameter id, Enum value) : base(framework, device, id)
        {
            UpdateCachedValue(new int[] { Convert.ToByte(value) });
        }
    }


    internal class FakeConnectedDevice : IDevice
    {
        public event EventHandler<ParameterChangedEventArgs> ParameterChanged = delegate { };
        public event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged = delegate { };
        public event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated = delegate { };
        public event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated = delegate { };
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
        public event EventHandler<EventArgs> DeviceInfoUpdated = delegate { };

        public Guid Id => framework.deviceId;

        public string Name { get; private set; }

        public string Serial => RockstarHelper.GetSerialFromName(Name);

        public DeviceType? DeviceType => RockstarHelper.GetTypeByName(Name);

        public Location Location => new Location(-27.128921, -109.366282);

        public List<IDeviceParameter> Parameters { get; set; }

        public string Firmware { get; set; }

        public string Hardware { get; set; }

        public LockState LockStatus { get; private set; } = LockState.Unknown;

        public uint? Battery => 64;

        public DeviceState State { get; private set; } = DeviceState.Disconnected;

        public void SetState(DeviceState state)
        {
            this.State = state;
            ConnectionChanged(this, new DeviceConnectionChangedEventArgs()
            {
                State = state,
                ConnectedDevice = this
            });
        }

        public bool? IncorrectPin { get; private set; }


        private FrameworkInstance_FAKE framework;

        public FakeConnectedDevice(string name, FrameworkInstance_FAKE framework)
        {
            this.framework = framework;
            this.Name = name;

            Parameters = new List<IDeviceParameter>()
            {
                new FakeDeviceParameter(framework, this, Parameter.MailboxCheckStatus, MailboxCheckStatus.On),
                new FakeDeviceParameter(framework, this, Parameter.MailboxCheckFrequency, MailboxCheckFrequency.Frequency120min),

                new FakeDeviceParameter(framework, this, Parameter.TrackingStatus, TrackingStatus.On),
                new FakeDeviceParameter(framework, this, Parameter.TrackingFrequency, TrackingFrequency.FrequencyBurst),
                new FakeDeviceParameter(framework, this, Parameter.TrackingBurstFixPeriod, TrackingBurstFixPeriod.Period20min),
                new FakeDeviceParameter(framework, this, Parameter.TrackingBurstTransmitPeriod, TrackingBurstTransmitPeriod.TrackingBurstTransmitPeriod15min),
                new FakeDeviceParameter(framework, this, Parameter.IridiumStatus, IridiumStatus.Inactive),
                new FakeDeviceParameter(framework, this, Parameter.GpsStatus, GpsStatus.Inactive),
            };
        }


        public Task Beep()
        {
            return framework.Beep();
        }

        public Task SaveDeviceParameter(Parameter parameter, Enum value)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(200);

                var p = ((BaseDeviceParameter)Parameters.SingleOrDefault(x => x.Id == parameter));
                p.UpdateCachedValue(new int[] { Convert.ToByte(value) });

                ParameterChanged(this, new ParameterChangedEventArgs()
                {
                    Parameter = p,
                });
            });
        }

        public Task UpdateAllParameters()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1000);

                foreach (var p in Parameters)
                    ParameterChanged(this, new ParameterChangedEventArgs()
                    {
                        Parameter = p
                    });
            });
        }

        public Task UpdateParameters(List<Parameter> ids)
        {
            return Task.Run(async () =>
            {
                foreach (var id in ids)
                    await Task.Delay(200);

                foreach (var id in ids)
                    ParameterChanged(this, new ParameterChangedEventArgs()
                    {
                        Parameter = Parameters.FirstOrDefault(x => x.Id == id),
                    });
            });
        }

        public Task Unlock(short? pin = null)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1000);

                var @old = LockStatus;

                if (pin == 1111)
                {
                    IncorrectPin = true;
                    LockStatus = LockState.Unlocked;
                }
                else
                {
                    IncorrectPin = null;
                    LockStatus = LockState.Locked;
                }

                DeviceLockStatusUpdated(this, new LockStatusUpdatedEventArgs()
                {
                    IncorrectPin = IncorrectPin,
                    New = LockStatus,
                    Old = @old
                });
            });
        }

        public async Task RequestBattery()
        {
            await Task.Delay(200);
        }

        public void RequestLocation()
        {

        }

        public Task<Location> UpdateLocationFromDevice()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(400);
                return Location;
            });
        }

        public Task RequestMailboxCheck()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(400);
            });
        }

        public Task RequestAlert()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
            });
        }

        public Task RequestNewLocation()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(400);
            });
        }

        public Task FactoryReset()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(400);
            });
        }
    }



    internal class FrameworkInstance_FAKE : IFramework
    {
        public event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults = delegate { };
        public event EventHandler<EventArgs> SearchTimeout = delegate { };
        public event EventHandler<PacketStatusUpdatedEventArgs> PacketStatusUpdated = delegate { };
        public event EventHandler<PacketReceivedEventArgs> PacketReceived = delegate { };

        internal Guid deviceId;
        public IDevice ConnectedDevice => device;
        public FakeConnectedDevice device { get; private set; }

        private FakeBluetooth bluetooth;
        private IStorage storage;

        public FrameworkInstance_FAKE(IStorage storage)
        {
            this.storage = storage;
            bluetooth = new FakeBluetooth();
            device = new FakeConnectedDevice(null, this);
        }

        public void ClearCaches()
        {
            //TODO
        }

        public Task Beep()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(300);
            });
        }

        public async Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false, int attemtps = 1)
        {
            try
            {
                deviceId = id;

                if (device.State == DeviceState.Connected)
                    return true;

                device.SetState(DeviceState.Connecting);
                await bluetooth.ConnectToDeviceAsync(id);

                device.SetState(DeviceState.Connected);
                return true;
            }
            catch (Exception e)
            {
                device.SetState(DeviceState.Disconnected);

                if (throwOnError)
                    throw e;

                return false;
            }
        }


        public async Task Disconnect()
        {
            await Task.Delay(1000);
            device.SetState(DeviceState.Disconnected);
        }

        public async Task ForgetDevice()
        {
            await Task.Delay(1000);
        }

        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false, int attempts = 1)
        {
            return Connect(device.Id);
        }


        public async Task<bool> Reconnect(bool force = true, bool throwOnError = false, int attempts = 1)
        {
            await Task.Delay(2000);
            return true;
        }


        public void Dispose()
        {

        }

        public Task RequestAlert()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(300);
            });
        }

        public Task SendManual()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(300);
            });
        }


        private int i = 0;
        private ThreadWorker thread = new ThreadWorker();
        private Random r = new Random();
        private SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private int resendParts = 2;

        public async Task<ushort> SendData(byte[] data)
        {
            try
            {
                await locker.WaitAsync();
                await Task.Delay(1000);


                ushort _messageId = (ushort)storage.GetShort(device.Id, "message-id", 0);
                var ___messageId = _messageId + 1;
                storage.PutShort(device.Id, "message-id", (short)___messageId);

                i++;

                if (i % 8 == 0)
                {
                    throw new Exception("Dummy send error");
                }


                var m = Message.Unpack(data, new InMemoryBuffer());

                if (m is BalanceMO balance)
                {
                    thread.PostDelayed(() =>
                    {
                        var balanceMT = BalanceMT.Create(ProtocolVersion.v3__WeatherExtension, DateTime.UtcNow, DateTime.UtcNow.AddDays(-12), DateTime.UtcNow.AddDays(30), 672, 1000, 328).Pack();

                        PacketReceived(this, new PacketReceivedEventArgs()
                        {
                            Payload = balanceMT[0].Payload,
                            MessageId = (short)(10006 + ___messageId),
                        });

                    }, TimeSpan.FromSeconds(15));
                }
                if (m is MessageSentMO sent)
                {
                    thread.PostDelayed(() =>
                    {
                        var resendIndexes = new byte[resendParts];
                        for (int k = 0; k < resendParts; k++)
                            resendIndexes[k] = (byte)k;

                        resendParts -= 2;

                        if (resendParts < 0)
                            resendParts = 2;

                        var ack = MessageAckMT.Create(ProtocolVersion.v3__WeatherExtension, (byte)sent.SentGroup, resendIndexes).Pack();

                        PacketReceived(this, new PacketReceivedEventArgs()
                        {
                            Payload = ack[0].Payload,
                            MessageId = (short)(10005 + ___messageId),
                        });

                    }, TimeSpan.FromSeconds(10));
                }


                thread.Post(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(4));


                    PacketStatusUpdated(this, new PacketStatusUpdatedEventArgs()
                    {
                        MessageId = (short)_messageId,
                        Status = MessageStatus.Transmitted,
                    });


                    //await Task.Delay(TimeSpan.FromSeconds(6));


                    //var m = Message.Unpack(data) as ChatMessageMO;

                    //if (m != null && m.TotalParts == 1)
                    //{
                    //var p = ChatMessageMT.Create(m.Version, m.Subscriber, m.Id, m.Conversation, m.Text, m.Lat, m.Lon, m.Alt, m.ByskyToken, m.File, m.FileExtension, m.ImageQuality, m.Subject).Pack();

                    //PacketReceived(this, new PacketReceivedEventArgs()
                    //{
                    //    Payload = p[0].Payload,
                    //    MessageId = (short)(10000 + ___messageId),
                    //});
                    //}
                });

                return _messageId;
            }
            finally
            {
                locker.Release();
            }
        }

        public async Task StartDeviceSearch()
        {
            await Task.Yield();

            bluetooth.ScanResults += Bluetooth_ScanResults;
            bluetooth.StartLeScan();
        }

        public void StopDeviceSearch()
        {
            bluetooth.ScanResults -= Bluetooth_ScanResults;
            bluetooth.StopLeScan();
        }

        private void Bluetooth_ScanResults(object sender, ScanResultsEventArgs e)
        {
            DeviceSearchResults(this, new DeviceSearchResultsEventArgs()
            {
                Devices = e.FoundDevices
            });
        }


        public Task GetReceivedMessages()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(300);
            });
        }
    }
}
