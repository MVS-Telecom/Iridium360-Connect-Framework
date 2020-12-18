#if ANDROID || IPHONE

using Iridium360.Connect.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Util;

namespace BivyStick.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class BivyStickDiscoveredDevice : IFoundDevice
    {
        public DeviceType? DeviceType => Iridium360.Connect.Framework.DeviceType.BivyStick;

        public string Serial => null;

        public object Native => device;

        public Guid Id => device.Id;

        public string Mac => device.Id.ToBluetoothAddress();

        public string Name => device.Name;

        public bool IsConnected => throw new NotSupportedException();


        private BivyStick.BivyStickDevice device;

        public BivyStickDiscoveredDevice(BivyStick.BivyStickDevice device)
        {
            this.device = device;
        }


        public Task<List<IGattService>> GetServicesAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

    }



    /// <summary>
    /// 
    /// </summary>
    public class BivyStickDevice : IDevice
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id => framework.DeviceId;

        /// <summary>
        /// 
        /// </summary>
        public string Name => framework.Name;

        /// <summary>
        /// 
        /// </summary>
        public string Serial => null;

        /// <summary>
        /// 
        /// </summary>
        public DeviceType? DeviceType => Iridium360.Connect.Framework.DeviceType.BivyStick;

        /// <summary>
        /// 
        /// </summary>
        public Location Location => null;

        /// <summary>
        /// 
        /// </summary>
        public List<IDeviceParameter> Parameters => new List<IDeviceParameter>();

        /// <summary>
        /// 
        /// </summary>
        public string Firmware => null;

        /// <summary>
        /// 
        /// </summary>
        public uint? Battery => (uint?)framework.Battery;

        /// <summary>
        /// 
        /// </summary>
        public DeviceState State
        {
            get
            {
                if (framework.IsDeviceConnected)
                {
                    return DeviceState.Connected;
                }
                else
                {
                    return DeviceState.Disconnected;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LockState LockStatus => LockState.Unlocked;

        /// <summary>
        /// 
        /// </summary>
        public bool? IncorrectPin => null;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ParameterChangedEventArgs> ParameterChanged = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Iridium360.Connect.Framework.BatteryUpdatedEventArgs> BatteryUpdated = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> DeviceInfoUpdated = delegate { };


        private BivyStickFramework framework;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="framework"></param>
        public BivyStickDevice(BivyStickFramework framework)
        {
            this.framework = framework;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Beep()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task FactoryReset()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestAlert()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestBattery()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestMailboxCheck()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestNewLocation()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveDeviceParameter(Parameter parameter, Enum value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public Task Unlock(short? pin = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task UpdateAllParameters()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Location> UpdateLocationFromDevice()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task UpdateParameters(List<Parameter> ids)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BivyConnectFramework : IFramework, IFrameworkProxy
    {
        public IDevice ConnectedDevice => device;

        public event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults = delegate { };
        public event EventHandler<EventArgs> SearchTimeout = delegate { };
        public event EventHandler<PacketStatusUpdatedEventArgs> PacketStatusUpdated = delegate { };
        public event EventHandler<PacketReceivedEventArgs> PacketReceived = delegate { };

        public event EventHandler<MessageAckedEventArgs> MessageAcked = delegate { };
        public event EventHandler<MessageTransmittedEventArgs> MessageTransmitted = delegate { };
        public event EventHandler<MessageResendNeededEventArgs> MessageResendNeeded = delegate { };
        public event EventHandler<MessageReceivedEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageProgressChangedEventArgs> MessageTransmitProgressChanged = delegate { };
        public event EventHandler<MessageProgressChangedEventArgs> MessageTransferProgressChanged = delegate { };

        private readonly BivyStickFramework framework;
        private readonly BivyStickDevice device;


        /// <summary>
        /// 
        /// </summary>
        public BivyConnectFramework()
        {
            framework = new BivyStickFramework();
            device = new BivyStickDevice(framework);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Beep()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force"></param>
        /// <param name="throwOnError"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public async Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false, int attempts = 1)
        {
            try
            {
                await framework.Connect(id);
                return true;
            }
            catch (Exception e)
            {
                if (throwOnError)
                    throw e;

                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="force"></param>
        /// <param name="throwOnError"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false, int attempts = 1)
        {
            return Connect(device.Id, force, throwOnError, attempts);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task ForgetDevice()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task GetReceivedMessages()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        /// <param name="throwOnError"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public Task<bool> Reconnect(bool force = true, bool throwOnError = true, int attempts = 1)
        {
            if (device == null)
                throw new InvalidOperationException("Device was not connected before");

            return Connect(device.Id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestAlert()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task<ushort> SendData(byte[] data)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task SendManual()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task StartDeviceSearch()
        {
            framework.DeviceDiscovered += Framework_DeviceDiscovered;
            await framework.StartSearch();
        }


        /// <summary>
        /// 
        /// </summary>
        public async void StopDeviceSearch()
        {
            framework.DeviceDiscovered -= Framework_DeviceDiscovered;
            await framework.StopSearch();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Framework_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            DeviceSearchResults(this, new DeviceSearchResultsEventArgs()
            {
                Devices = new List<IFoundDevice>()
                {
                    new BivyStickDiscoveredDevice(e.Device)
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetHungPackets()
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetTransmittingPackets()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> SendMessage(Message message, string messageId)
        {
            if (message is ChatMessageMO chatMessage)
            {
                switch (chatMessage.Subscriber.Value.Network)
                {
                    case SubscriberNetwork.Email:
                        await framework.SendEmail(chatMessage.Subscriber.Value.Number, chatMessage.Text);
                        return (messageId, 1, 1, true);

                    case SubscriberNetwork.Mobile:
                        await framework.SendSms(chatMessage.Subscriber.Value.Number, chatMessage.Text);
                        return (messageId, 1, 1, true);

                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public Task<(string messageId, int readyParts, int totalParts, bool transferSuccess)> RetrySendMessage(string messageId)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

        }
    }
}

#endif
