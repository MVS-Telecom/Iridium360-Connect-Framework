﻿using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Fakes
{
    internal class FakeDeviceParameter : DeviceParameter
    {
        public FakeDeviceParameter(IFramework Rock, IDevice device, Parameter id, Enum value) : base(Rock, device, id)
        {
            UpdateCachedValue(new byte[] { Convert.ToByte(value) });
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

        public List<DeviceParameter> Parameters { get; set; }

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

            Parameters = new List<DeviceParameter>()
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
                Parameters.SingleOrDefault(x => x.Id == parameter).UpdateCachedValue(new byte[] { Convert.ToByte(value) });
            });
        }

        public Task UpdateAllParameters()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1000);
            });
        }

        public Task UpdateParameters(List<Parameter> ids)
        {
            return Task.Run(async () =>
            {
                foreach (var id in ids)
                    await Task.Delay(200);
            });
        }

        public Task Unlock(short pin)
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

        public void RequestBattery()
        {

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
        public event EventHandler<MessageStatusUpdatedEventArgs> _MessageStatusUpdated = delegate { };
        public event EventHandler<MessageReceivedEventArgs> _MessageReceived = delegate { };

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
                return false;
            }
        }


        public async Task Disconnect()
        {
            await Task.Delay(1000);
        }

        public async Task ForgetDevice()
        {
            await Task.Delay(1000);
        }

        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false)
        {
            return Connect(device.Id);
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


        public Task<ushort> SendData(byte[] data)
        {
            ushort _messageId = (ushort)storage.GetShort("message-id", 0);

            return Task.Run(async () =>
            {
                await Task.Delay(1000);

                _ = Task.Run(async () =>
                  {
                      await Task.Delay(6000);


                      _MessageStatusUpdated(this, new MessageStatusUpdatedEventArgs()
                      {
                          MessageId = (short)_messageId,
                          Status = MessageStatus.Transmitted,
                          Handled = false
                      });


                      await Task.Delay(5000);

                      var m = MessageMO.Unpack(data) as ChatMessageMO;
                      var b = ChatMessageMT.Create(m.Subscriber, 0, 0, $"[response] {m.Text}").Pack();

                      _MessageReceived(this, new MessageReceivedEventArgs()
                      {
                          Payload = b,
                          MessageId = (short)(10000 + _messageId),
                          Handled = false
                      });
                  });

                storage.PutShort("message-id", (short)(_messageId + 1));
                return _messageId;
            });
        }

        public void StartDeviceSearch()
        {
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