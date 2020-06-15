﻿#if ANDROID || IPHONE

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Rock;
using Rock.Bluetooth;
using Rock.Commands;
using System.Diagnostics;
using Rock.Helpers;
using System.Linq;
using Rock.Exceptions;
using Rock.Core;
using Rock.Util;

#if IOS
using Foundation;
#endif


#if ANDROID
using UK.Rock7.Connect;
using UK.Rock7.Connect.Protocol;
using UK.Rock7.Connect.Enums;
using Java.Util;
using UK.Rock7.Connect.Device;
using UK.Rock7.Connect.Device.R7generic;
using DeviceParameter = UK.Rock7.Connect.Device.DeviceParameter;
using Android.Runtime;
using Android.App;
#endif

namespace ConnectFramework.Shared
{

    internal class R7ConnectFramework :
#if ANDROID
        Java.Lang.Object, IR7DeviceResponseDelegate, IR7DeviceMessagingDelegate, IR7DeviceDiscoveryDelegate,
#elif IOS
        Foundation.NSObject,
#endif
        IFramework
    {
        public IDevice ConnectedDevice => device;

        public event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults = delegate { };
        public event EventHandler<EventArgs> SearchTimeout = delegate { };
        public event EventHandler<MessageStatusUpdatedEventArgs> _MessageStatusUpdated = delegate { };
        public event EventHandler<MessageReceivedEventArgs> _MessageReceived = delegate { };

        public ConnectComms comms { internal get; set; }
        private R7Device device;
        private ILogger logger;
        private IStorage storage;


        private static R7ConnectFramework instance;
        public static R7ConnectFramework GetInstance(IStorage storage, ILogger logger)
        {
            lock (typeof(R7ConnectFramework))
            {
                if (instance == null)
                {
#if ANDROID
                    ConnectComms.Init(Application.Context);
#endif
                    instance = new R7ConnectFramework(storage, logger);
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        private R7ConnectFramework(IStorage storage, ILogger logger) : base()
        {
            this.logger = logger;
            this.storage = storage;


#if ANDROID
            comms = ConnectComms.GetConnectComms();
            comms.ResponseDelegate = new Java.Lang.Ref.WeakReference(this);
            comms.MessagingDelegate = new Java.Lang.Ref.WeakReference(this);
            comms.DiscoveryDelegate = new Java.Lang.Ref.WeakReference(this);
#elif IOS
            comms = ConnectComms.GetConnectComms;
            comms.ResponseDelegate = new R(this);
            comms.MessagingDelegate = new M(this);
            comms.DiscoveryDelegate = new D(this);
#endif


            Enable();

            device = new R7Device(this);
        }


#if ANDROID
        protected R7ConnectFramework() : base()
        {
        }

        public R7ConnectFramework(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }
#endif



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task Enable()
        {
            return Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);

                EventHandler handler = (s, e) =>
                {
                    r.Set();
                };

                OnDeviceReady += handler;

                comms.EnableWithApplicationIdentifier("8705e62a2e1cac9019be882fa020556b");
                comms.DisableUsageTimeout();

                r.WaitOne(TimeSpan.FromSeconds(10));
                OnDeviceReady -= handler;
            });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Beep()
        {
            await Reconnect(throwOnError: true);
            await Unlock();

            comms.RequestBeep();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task FactoryReset()
        {
            await Reconnect(throwOnError: true);
            await Unlock();

            comms.FactoryReset();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Location> UpdateLocationFromDevice()
        {
            return Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);

                EventHandler<LocationUpdatedEventArgs> handler = (s, e) =>
                {
                    r.Set();
                };

                try
                {
                    ConnectedDevice.LocationUpdated += handler;

                    comms.RequestLastKnownGpsPosition();
                    r.WaitOne(TimeSpan.FromSeconds(10));

                    if (ConnectedDevice.Location == null)
                        throw new Exception();

                    return ConnectedDevice.Location;
                }
                finally
                {
                    ConnectedDevice.LocationUpdated -= handler;
                }
            });

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Unlock()
        {
            return Unlock(pin: null);
        }


        private SemaphoreSlim unlockLocker = new SemaphoreSlim(1, 1);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public Task Unlock(short? pin = null)
        {
            return Task.Run(async () =>
            {
                await unlockLocker.WaitAsync();

                try
                {
#if DEBUG && ANDROID
                    bool a = comms.IsUnlocked().BooleanValue();
                    bool b = device.LockStatus == LockState.Unlocked;

                    if (a != b)
                        Debugger.Break();
#endif

                    if (ConnectedDevice.LockStatus == LockState.Unlocked)
                        return;

                    if (pin == null)
                        pin = storage.GetShort("r7-device-pin", 1234);

                    logger.Log($"[R7] Unlocking with `{pin}`");

                    AutoResetEvent r = new AutoResetEvent(false);

                    EventHandler<LockStatusUpdatedEventArgs> handler = (s, e) =>
                    {
                        r.Set();
                    };

                    try
                    {
                        ConnectedDevice.DeviceLockStatusUpdated += handler;

                        await Reconnect(throwOnError: true);

#if ANDROID
                        comms.Unlock(pin.Value);
#elif IOS
                        comms.Unlock((nuint)pin);
#endif

                        r.WaitOne(TimeSpan.FromSeconds(20));


                        if (ConnectedDevice.IncorrectPin == true)
                            throw new IncorrectPinException();

                        if (ConnectedDevice.LockStatus != LockState.Unlocked)
                            throw new DeviceIsLockedException();

                        storage.PutShort("r7-device-pin", pin.Value);
                        logger.Log("[R7] Unlock success");
                    }
                    finally
                    {
                        ConnectedDevice.DeviceLockStatusUpdated -= handler;
                    }
                }
                catch (Exception e)
                {
                    Debugger.Break();
                    logger.Log("[R7] Unlock error");
                    throw e;
                }
                finally
                {
                    unlockLocker.Release();

                }

            });
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task RequestNewLocation()
        {
            await Reconnect(throwOnError: true);
            comms.RequestCurrentGpsPosition();
        }


        /// <summary>
        /// 
        /// </summary>
        public Task RequestBattery()
        {
            return Task.Run(() =>
            {
                comms.RequestBatteryStatus();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RequestMailboxCheck()
        {
            await Reconnect(throwOnError: true);
            comms.RequestSatelliteMessageCheck();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            comms.Disconnect();
            await Task.Delay(1000);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ForgetDevice()
        {
            await Disconnect();
            storage.Remove("r7-device-pin");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        internal Task<bool> Reconnect(bool throwOnError = true)
        {
            if (deviceId == Guid.Empty)
                throw new DeviceConnectionException();

            return Connect(deviceId, throwOnError: throwOnError);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="flags"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false)
        {
            return Connect(device.Id, force, throwOnError);
        }


        private static SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        private Guid deviceId;



        private static string ToBluetoothAddress(Guid deviceId)
        {
            var address = deviceId
                .ToByteArray()
                .Skip(10)
                .Take(6)
                .ToArray();

            return BitConverter
                .ToString(address)
                .Replace("-", ":")
                .ToUpperInvariant();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flags"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false)
        {
            return Task.Run(async () =>
            {
                if (device.State == DeviceState.Connected)
                    return true;

                if (!force && connectLock.CurrentCount == 0)
                    return true;

                await connectLock.WaitAsync();

                deviceId = id;
                device.SetState(DeviceState.Connecting);

                try
                {
                    AutoResetEvent r = new AutoResetEvent(false);

                    EventHandler<DeviceConnectionChangedEventArgs> handler = (s, e) =>
                    {
                        r.Set();
                    };

                    try
                    {
                        device.ConnectionChanged += handler;

                        await Enable();
#if ANDROID
                        comms.Connect(ToBluetoothAddress(id));
#elif IOS
                        comms.Connect(new Foundation.NSUuid(id.ToString()));
#endif
                        r.WaitOne(TimeSpan.FromSeconds(20));
                    }
                    finally
                    {
                        device.ConnectionChanged -= handler;
                    }

                    bool connected = device.State == DeviceState.Connected;
                    device.SetState(connected ? DeviceState.Connected : DeviceState.Disconnected);

                    if (!connected && throwOnError)
                        throw new DeviceConnectionException();

                    return connected;
                }
                finally
                {
                    connectLock.Release();
                }
            });

        }


        private void Safety(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debugger.Break();
            }
        }

        private T Safety<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RequestAlert()
        {
            await Reconnect(throwOnError: true);
            await Unlock();

            ///TODO: Нужны дополнительные проверки что оно сработало
            comms.RequestAlert();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SendManual()
        {
            await Reconnect(throwOnError: true);
            await Unlock();

            comms.RequestManual();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task SendRawMessageWithDataAndIdentifier(byte[] data, ushort messageId)
        {
            await Reconnect(throwOnError: true);
            //await Unlock();

            MessageStatusUpdatedEventArgs args = null;

            const int attempts = 2;

            for (int i = 1; i <= attempts; i++)
            {
                AutoResetEvent r = new AutoResetEvent(false);

                await Task.Run(() =>
                {
                    var handler = new EventHandler<MessageStatusUpdatedEventArgs>((s, e) =>
                    {
                        if (e.MessageId == messageId)
                        {
                            args = e;
                            r.Set();
                        }
                    });

                    try
                    {
                        _MessageStatusUpdated += handler;

#if ANDROID
                        comms.SendRawMessageWithDataAndIdentifier(data, (short)messageId);
#elif IOS
                        comms.SendRawMessageWithDataAndIdentifier(Foundation.NSData.FromArray(data), (nuint)messageId);
#endif

                        r.WaitOne(TimeSpan.FromMinutes(15));

                    }
                    finally
                    {
                        _MessageStatusUpdated -= handler;
                    }
                });


                ///Success
                if (args?.Status == MessageStatus.ReceivedByDevice)
                    return;


                if (args == null)
                    throw new MessageSendingException($"Message Id={messageId} transfer to device timeout");

                if (args.Status == MessageStatus.ErrorToolong)
                    throw new MessageSendingException($"Message Id={messageId} is too long");

                if (args.Status != MessageStatus.ReceivedByDevice && i + 1 > attempts)
                    throw new MessageSendingException($"Message Id={messageId} transfer error `{args.Status}`");

                await Task.Delay(1000);

            }

        }


        /// <summary>
        /// 
        /// </summary>
        public void StartDeviceSearch()
        {
            Safety(() =>
            {
                comms.StartDiscovery();
            });
        }


        /// <summary>
        /// 
        /// </summary>
        public void StopDeviceSearch()
        {
            Safety(() =>
            {
                comms.StopDiscovery();
            });
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveDeviceParameter(Parameter parameter, Enum value)
        {
            return Task.Run(async () =>
            {
                await Reconnect();
                await Unlock();

                var _parameter = parameter.ToR7();
                var _value = Convert.ToInt32(value);

#if ANDROID
                (comms.CurrentDevice as R7GenericDevice).UpdateParameter(_parameter, _value);
#elif IOS
                comms.CurrentDevice.Update((nuint)(int)_parameter, NSData.FromArray(new byte[] { (byte)Convert.ToInt32(_value) }));
#endif

                ///Запрашиваем новое значение с устройства чтобы убедиться что оно сохранилось
                (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);

                ///Ждем получения этого значения
                await WaitForParameterChanged(_parameter, _value, throwOnError: true);

            });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task UpdateParameters(List<Parameter> ids)
        {
            return Task.Run(async () =>
            {
                await Reconnect();

                foreach (var p in ids)
                {
                    ///Делаем 2 попытки - возможно параметр еще "не готов"
                    for (int i = 1; i <= 2; i++)
                    {
                        try
                        {
                            var a = comms.CurrentDevice.ParameterForIdentifier(p.ToR7().EnumToInt());

#if ANDROID
                            if (a?.Available?.BooleanValue() != true)
                                throw new ParameterUnavailableException(p);
#else
                            if (a?.Available != true)
                                throw new ParameterUnavailableException(p);
#endif

                            var _parameter = a.Identifier.ToR7();

                            ///Запрашиваем параметр с устройства
                            (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);

                            ///Ждем изменения параметра
                            bool updated = await WaitForParameterAny(_parameter, throwOnError: false);

                            ///Если значение параметра не изменилось - 99.9% что устройство требует разблокировки по PIN
                            if (!updated && ConnectedDevice.LockStatus == LockState.Locked)
                            {
                                await Unlock();

                                ///После успешной разблокировки делаем повторную попытку чтения
                                (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);
                                await WaitForParameterAny(_parameter, throwOnError: true);
                            }
                        }
                        catch (ParameterUnavailableException ex)
                        {
                            await Task.Delay(300);
                        }
                    }
                }
            });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Task<bool> WaitForParameterAny(R7GenericDeviceParameter parameter, bool throwOnError = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var current = comms.CurrentDevice.ParameterForIdentifier(parameter.EnumToInt())?.CachedValue;


                    if (current != 999
                    && current != 255
#if ANDROID
                    && current != -1
#endif
                    )
                        return true;

                    AutoResetEvent r = new AutoResetEvent(false);

                    EventHandler<ParameterChangedEventArgs> handler = (s, e) =>
                    {
                        if (e.Parameter.Id.ToR7().EnumToInt() == parameter.EnumToInt())
                            r.Set();
                    };

                    EventHandler<LockStatusUpdatedEventArgs> handler2 = (s, e) =>
                    {
                        if (e.New != LockState.Unlocked)
                            r.Set();
                    };

                    try
                    {
                        ConnectedDevice.ParameterChanged += handler;
                        ConnectedDevice.DeviceLockStatusUpdated += handler2;
                        r.WaitOne(TimeSpan.FromSeconds(10));

                        var @new = comms.CurrentDevice.ParameterForIdentifier(parameter.EnumToInt())?.CachedValue;

                        if (@new == 999
                        || @new == 255
#if ANDROID
                        || @new == -1
#endif
                        )
                        {
                            if (device.LockStatus != LockState.Unlocked)
                                throw new DeviceIsLockedException();

                            throw new Exception();
                        }


                        return true;
                    }
                    finally
                    {
                        ConnectedDevice.ParameterChanged -= handler;
                        ConnectedDevice.DeviceLockStatusUpdated -= handler2;
                    }
                }
                catch (Exception e)
                {
                    if (throwOnError)
                        throw e;

                    return false;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Task<bool> WaitForParameterChanged(R7GenericDeviceParameter parameter, int value, bool throwOnError = false)
        {
            return Task.Run(() =>
            {
                try
                {
                    var current = ConnectedDevice.Parameters.SingleOrDefault(x => x.Id == parameter.FromR7());

                    if (current == null)
                        throw new Exception();

                    if (Convert.ToInt32(current.CachedValue) == value)
                        return true;

                    AutoResetEvent r = new AutoResetEvent(false);

                    EventHandler<ParameterChangedEventArgs> handler = (s, e) =>
                    {
                        if (e.Parameter.Id.ToR7().EnumToInt() == parameter.EnumToInt())
                            r.Set();
                    };

                    try
                    {
                        ConnectedDevice.ParameterChanged += handler;
                        r.WaitOne(TimeSpan.FromSeconds(10));

                        var @new = ConnectedDevice.Parameters.SingleOrDefault(x => x.Id == parameter.FromR7());

                        if (Convert.ToInt32(@new.CachedValue) != value)
                            throw new Exception();

                        return true;
                    }
                    finally
                    {
                        ConnectedDevice.ParameterChanged -= handler;
                    }
                }
                catch (Exception e)
                {
                    if (throwOnError)
                        throw e;

                    return false;
                }
            });
        }






        #region FRAMEWORK CALLBACKS




#if ANDROID
        public void DeviceBatteryUpdated(int p0, Date p1)
#elif IOS
        public void DeviceBatteryUpdated(nuint p0, NSDate p1)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device battery {p0}");
                device.Battery = (uint)p0;
            });
        }


#if ANDROID
        public void CreditBalanceUpdated(int p0)
        {
            throw new NotImplementedException();
        }
#endif

        public void DeviceCommandReceived(R7CommandType p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device command {p0}");
            });
        }





#if ANDROID
        public void DeviceConnected(ConnectDevice _device, R7ActivationState activated, R7LockState locked)
#elif IOS
        public void DeviceConnected(ConnectDevice _device, R7ActivationState activated, bool locked)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device connected {activated} {locked}");
                device.SetState(DeviceState.Connected);
            });
        }



        public void DeviceDisconnected()
        {
            Safety(async () =>
            {
                logger.Log($"[R7] Device disconnected");
                device.SetState(DeviceState.Disconnected);

                await Enable();
            });
        }


        public void DeviceError(R7DeviceError p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device error {p0}");
            });
        }


        public void DeviceLockStatusUpdated(R7LockState p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device lock state {p0}");


                if (p0 == R7LockState.R7LockStateIncorrectPin)
                    device.SetLockStatus(LockState.Locked, incorrectPin: true);

                else if (p0 == R7LockState.R7LockStateLocked)
                    device.SetLockStatus(LockState.Locked);

                else if (p0 == R7LockState.R7LockStateUnlocked)
                    device.SetLockStatus(LockState.Unlocked);

                else
                    device.SetLockStatus(LockState.Unknown);
            });
        }




#if ANDROID
        public void DeviceNameUpdated(string p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device name {p0}");
            });
        }
#endif




        public void DeviceParameterUpdated(DeviceParameter p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device parameter changed `{p0.Label}` [{p0.Characteristic}] -> `{p0.CachedValue}`");
                device.OnParameterChanged(p0);
            });
        }

        private event EventHandler OnDeviceReady = delegate { };


        public void DeviceReady()
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device ready");
                OnDeviceReady(this, new EventArgs());
            });
        }


#if ANDROID
        public void DeviceSerialDump(byte[] p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device dump");
            });
        }
#endif



        public void DeviceStateChanged(R7ConnectionState p0, R7ConnectionState p1)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device state changed {p1} -> {p0}");
            });
        }



#if ANDROID
        public void DeviceStatusUpdated(int p0, int p1)
#elif IOS
        public void DeviceStatusUpdated(nuint p0, nuint p1)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device status updated {p0} -> {p1}");
            });
        }




#if ANDROID
        public void DeviceUsageTimeout()
        {
            Safety(() =>
            {
                logger.Log($"[R7] Usage timeout");
            });
        }
#endif



#if ANDROID
        public void LocationUpdated(Android.Locations.Location p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Location {p0.Latitude} {p0.Longitude}");
                device.SetLocation(new Location(p0.Latitude, p0.Longitude)
                {
                    Latitude = p0.Latitude,
                    Longitude = p0.Longitude,
                    Date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(p0.Time),
                    Accuracy = p0.Accuracy,
                    Altitude = p0.Altitude,
                    Bearing = p0.Bearing,
                    Speed = p0.Speed,
                    Source = LocationSource.Device
                });
            });
        }
#endif


#if IOS
        public void LocationUpdated(CoreLocation.CLLocation p0)
        {
            Safety(() =>
                {
                    logger.Log($"[R7] Location {p0.Coordinate.Latitude} {p0.Coordinate.Longitude}");
                    device.SetLocation(new Location()
                    {
                        Latitude = p0.Coordinate.Latitude,
                        Longitude = p0.Coordinate.Longitude,
                        Date = ((DateTime)p0.Timestamp).ToLocalTime(),
                        Source = LocationSource.Device
                    });
                });
        }
#endif


        bool test = false;

#if ANDROID
        public void InboxUpdated(short messages)
#elif IOS
        public void InboxUpdated(nuint messages)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Inbox {messages}");

                if (messages > 0)
                    Debugger.Break();

                if (messages > 0)
                    comms.RequestNextMessage();

                //#if DEBUG
                //                if (!test)
                //                {
                //                    test = true;
                //                    var buffer = "00045C2542ABC631ADF0F41DA740D80310BE564D64F386539A9F93944675599DF3C49CD3C49CFDC4CC73136D6575CEF1A04A045C47DBC4C413573659150FAA44E0DD384EF2569D93534D7075CE939C4E7C5ABF135536DD3831D3F456711C00D7".ToByteArray();
                //                    MessageReceived(0, NSData.FromArray(buffer));
                //                }
                //#endif
            });
        }



#if ANDROID
        public void MessageCheckFinished()
        {
            Safety(() =>
            {
                logger.Log($"[R7] Message check finished");
            });
        }
#endif



#if ANDROID
        public void MessageProgressCompleted(short messageId)
#elif IOS
        public void MessageProgressCompleted(nuint messageId)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Message progress completed {messageId}");
            });
        }



#if ANDROID
        public void MessageProgressUpdated(short p0, int p1, int p2)
#elif IOS
        public void MessageProgressUpdated(nuint p0, nuint p1, nint p2)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Message progress {p0} -> {p1}/{p2}");
            });
        }





#if ANDROID
        public bool MessageReceived(short messageId, byte[] _data)
#elif IOS
        public bool MessageReceived(ushort messageId, Foundation.NSData _data)
#endif
        {
            return Safety(() =>
            {
                byte[] data = null;
#if ANDROID
                data = _data;
#elif IOS
                data = _data.ToArray();
#endif
                logger.Log($"[R7] Message received {messageId} -> {data.ToHexString()}");

                Debugger.Break();

                var args = new MessageReceivedEventArgs()
                {
                    Handled = false,
                    MessageId = (short)messageId,
                    Payload = data
                };

                _MessageReceived(this, args);

                if (args.Handled)
                    return true;
                else
                    return false;
            });
        }




#if ANDROID
        public bool MessageStatusUpdated(short messageId, R7MessageStatus status)
#elif IOS
        public bool MessageStatusUpdated(ushort messageId, R7MessageStatus status)
#endif
        {
            return Safety(() =>
            {
                logger.Log($"[R7] Message status updated {messageId} -> {status}");

                MessageStatus _status = MessageStatus.Error;

                if (status == R7MessageStatus.R7MessageStatusReceivedByDevice)
                {
                    _status = MessageStatus.ReceivedByDevice;
                }
                else if (status == R7MessageStatus.R7MessageStatusTransmitted)
                {
                    _status = MessageStatus.Transmitted;
                    Debugger.Break();
                }
                else
                {
                    Debugger.Break();
                }

                var args = new MessageStatusUpdatedEventArgs()
                {
                    Handled = false,
                    MessageId = (short)messageId,
                    Status = _status
                };

                _MessageStatusUpdated(this, args);

                if (args.Handled)
                    return true;
                else
                    return false;
            });
        }


        private List<IBluetoothDevice> devices = new List<IBluetoothDevice>();

#if ANDROID
        public void DiscoveryFoundDevice(string deviceIdentifier, string deviceName)
#elif IOS
        public void DiscoveryFoundDevice(Foundation.NSUuid deviceIdentifier, string deviceName)
#endif
        {
            Safety(() =>
            {
                devices.Add(new R7BluetoothDevice(deviceIdentifier.ToString(), deviceName));

                DeviceSearchResults(this, new DeviceSearchResultsEventArgs()
                {
                    Devices = devices
                });
            });
        }


        public void DiscoveryStarted()
        {
            Safety(() =>
            {
                devices.Clear();
                logger.Log($"[R7] Discovery started");
            });
        }


        public void DiscoveryStopped()
        {
            Safety(() =>
            {
                logger.Log($"[R7] Discovery stopped");
            });
        }



#if ANDROID
        public void DiscoveryUpdatedDevice(string p0, string p1, int p2)
#elif IOS
        public void DiscoveryUpdatedDevice(NSUuid p0, string p1, NSNumber p2)
#endif
        {
            Safety(() =>
            {
                logger.Log($"[R7] Discovery update {p0} {p1} {p2}");
            });
        }




        #endregion

    }
}

#endif