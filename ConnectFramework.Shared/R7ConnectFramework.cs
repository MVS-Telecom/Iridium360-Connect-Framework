#if ANDROID || IPHONE

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Iridium360.Connect.Framework;
using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Exceptions;

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
        public event EventHandler<PacketStatusUpdatedEventArgs> PacketStatusUpdated;
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        public ConnectComms comms { internal get; set; }
        private R7Device device;
        private ILogger logger;
        private IStorage storage;

        private Lazy<IBluetoothHelper> bluetoothHelper;


        private static R7ConnectFramework instance;
        public static R7ConnectFramework GetInstance(IStorage storage, ILogger logger, Lazy<IBluetoothHelper> bluetoothHelper)
        {
            lock (typeof(R7ConnectFramework))
            {
                if (instance == null)
                {
#if ANDROID
                    ConnectComms.Init(Application.Context);
#endif
                    instance = new R7ConnectFramework(storage, logger, bluetoothHelper);
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        private R7ConnectFramework(IStorage storage, ILogger logger, Lazy<IBluetoothHelper> bluetoothHelper) : base()
        {
            this.logger = logger;
            this.storage = storage;
            this.bluetoothHelper = bluetoothHelper;

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


            //Enable();

            device = new R7Device(this, logger);
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
            return Task.Run(async () =>
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

                await Task.Delay(1000);
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
        private Exception lastUnlockException = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public Task Unlock(short? pin = null)
        {
            return Task.Run(async () =>
            {
                ///ЭТО ВАЖНО!
                ///Возвращаем результат первого вызова для всех вызовов в очереди ожидающих завершения первого
                ///В случае неуспеха первого вызова все последующие также буду неуспешны (например неверный пин) - избегаем этого

                #region

                bool returnLastUnlockResult = false;

                if (unlockLocker.CurrentCount == 0)
                    returnLastUnlockResult = true;

                await unlockLocker.WaitAsync();

                if (returnLastUnlockResult && lastUnlockException != null)
                    throw lastUnlockException;

                lastUnlockException = null;

                #endregion


                try
                {
                    const int attempts = 2;

                    for (int i = 1; i <= attempts; i++)
                    {
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
                            bool _event = false;

                            EventHandler<LockStatusUpdatedEventArgs> handler = (s, e) =>
                            {
                                _event = true;
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

                                bool ok = r.WaitOne(TimeSpan.FromSeconds(15));


                                if (ConnectedDevice.IncorrectPin == true)
                                    throw new IncorrectPinException();

                                if (!ok)
                                    throw new TimeoutException();

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
                            lastUnlockException = e;

                            if (e is TimeoutException)
                            {
                                logger.Log("[R7] Unlock error - timeout");

                                ///Делаем повторную попытку - ЭТО ВАЖНО
                                if (i + 1 > attempts)
                                {
                                    Debugger.Break();
                                    throw e;
                                }
                                else
                                {
                                    logger.Log("[R7] Next unlock attempt...");
                                    await Task.Delay(1000);
                                }
                            }
                            else
                            {
                                logger.Log($"[R7] Unlock error {e}");
                                Debugger.Break();
                                throw e;
                            }
                        }
                    }
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
        public Task<bool> Reconnect(bool throwOnError = true, int attempts = 1)
        {
            if (deviceId == Guid.Empty)
                throw new InvalidOperationException("Device was not connected previosly");

            return Connect(deviceId, throwOnError: throwOnError, attempts: attempts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="flags"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public Task<bool> Connect(IBluetoothDevice device, bool force = true, bool throwOnError = false, int attempts = 1)
        {
            return Connect(device.Id, force, throwOnError, attempts);
        }


        private static SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        internal Guid deviceId;



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
        public Task<bool> Connect(Guid id, bool force = true, bool throwOnError = false, int attempts = 1)
        {
            return Task.Run(async () =>
            {
                if (device.State == DeviceState.Connected)
                    return true;

                if (!force && connectLock.CurrentCount == 0)
                    return true;

                try
                {
                    await connectLock.WaitAsync();

                    deviceId = id;


                    ///Должен быть включен блютуз
                    ///Пытаемся включить его программно, если не получается - выкидываем ошибку
                    if (!bluetoothHelper.Value.IsOn)
                    {
                        var enabled = await bluetoothHelper.Value.TurnOn(force: force);

                        if (!enabled)
                            if (throwOnError)
                                throw new BluetoothTurnedOffException();
                            else
                                return false;
                    }


                    device.SetState(DeviceState.Connecting);



                    ///Подключаемся...
                    List<Exception> exceptions = new List<Exception>();

                    for (int i = 1; i <= attempts; i++)
                    {
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


                            if (device.State == DeviceState.Connected)
                                break;

                        }
                        catch (Exception e)
                        {
                            exceptions.Add(e);
                        }


                        if (i + 1 < attempts)
                        {
                            ///Ждем секунду в случае неудачи
                            await Task.Delay(1000);
                        }
                    }


                    bool connected = device.State == DeviceState.Connected;
                    device.SetState(connected ? DeviceState.Connected : DeviceState.Disconnected);


                    if (!connected && throwOnError)
                        throw new DeviceConnectionException(null, new AggregateException(exceptions));

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
                logger.Log(e);
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
                return default(T);
            }
        }

        private bool Safety(Func<bool> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                Debugger.Break();
                return false;
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


        private static SemaphoreSlim sendLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<ushort> SendData(byte[] data)
        {
            await sendLock.WaitAsync();

            try
            {
                await Reconnect(throwOnError: true, attempts: 2);

                ushort messageId = (ushort)storage.GetShort("message-id", 1);

                PacketStatusUpdatedEventArgs args = null;

                const int attempts = 2;

                for (int i = 1; i <= attempts; i++)
                {
                    await Reconnect(throwOnError: true, attempts: 1);

                    AutoResetEvent r = new AutoResetEvent(false);

                    await Task.Run(() =>
                    {
                        var handler = new EventHandler<PacketStatusUpdatedEventArgs>((s, e) =>
                        {
                            if (e.MessageId == messageId)
                            {
                                args = e;
                                r.Set();
                            }
                        });

                        try
                        {
                            PacketStatusUpdated += handler;

#if ANDROID
                            comms.SendRawMessageWithDataAndIdentifier(data, (short)messageId);
#elif IOS
                            comms.SendRawMessageWithDataAndIdentifier(Foundation.NSData.FromArray(data), (nuint)messageId);
#endif

                            r.WaitOne(TimeSpan.FromMinutes(1));

                        }
                        catch (Exception e)
                        {
                            Debugger.Break();
                        }
                        finally
                        {
                            PacketStatusUpdated -= handler;
                        }
                    });


                    ///Success
                    if (args?.Status == MessageStatus.ReceivedByDevice)
                    {
                        storage.PutShort("message-id", (short)(messageId + 1));
                        return messageId;
                    }

                    if (ConnectedDevice?.State == DeviceState.Connected && args == null)
                        throw new MessageSendingException($"Packet Id={messageId} transfer to device timeout");

                    if (args?.Status == MessageStatus.ErrorToolong)
                        throw new MessageSendingException($"Packet Id={messageId} is too long");

                    if (args?.Status != MessageStatus.ReceivedByDevice && i + 1 > attempts)
                        throw new MessageSendingException($"Packet Id={messageId} transfer error `{args?.Status}`");


                    if (args?.Status == MessageStatus.ErrorCapability)
                    {
                        Debugger.Break();
                        await Disconnect();
                    }


                    await Task.Delay(3000);
                }

                throw new MessageSendingException($"Packet Id={messageId} transfer to error");

            }
            catch (Exception e)
            {
                Debugger.Break();
                throw e;
            }
            finally
            {
                sendLock.Release();
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task GetReceivedMessages()
        {
            await Reconnect();
            comms.RequestNextMessage();
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task StartDeviceSearch()
        {
            var enabled = await bluetoothHelper.Value.TurnOn(force: true);

            if (!enabled)
                throw new BluetoothTurnedOffException();


            await Enable();
            await Task.Delay(500);


            comms.StartDiscovery();

            //HACK
            bluetoothHelper.Value.StartLeScan();
            bluetoothHelper.Value.ScanResults += BluetoothHelper_ScanResults;
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


            Safety(() =>
            {
                bluetoothHelper.Value.StopLeScan();
                bluetoothHelper.Value.ScanResults -= BluetoothHelper_ScanResults;
            });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BluetoothHelper_ScanResults(object sender, ScanResultsEventArgs e)
        {
            var fix = e.FoundDevices.Where(x => x.Mac.ToUpper().StartsWith("68:0A:E2") || x.Mac.ToUpper().StartsWith("CC:CC:CC")).ToList();

            foreach (var d in fix)
                __DiscoveryFoundDevice(d.Mac, d.Name);
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


                var __parameter = comms.CurrentDevice.ParameterForIdentifier(_parameter.EnumToInt());

#if IOS
                if (__parameter == null || !__parameter.Available)
                    throw new InvalidOperationException($"Parameter `{parameter}` unavailable on this device");
#elif ANDROID
                if (__parameter == null || !__parameter.Available.BooleanValue())
                    throw new InvalidOperationException($"Parameter `{parameter}` unavailable on this device");
#endif

#if IOS
                var option = __parameter.Options.SingleOrDefault(x => ((NSNumber)x.Key).Int32Value == _value);
#elif ANDROID
                var option = new Android.Runtime.JavaDictionary<int?, string>(__parameter.Options.Handle, Android.Runtime.JniHandleOwnership.DoNotRegister).ToDictionary(t => t.Key, t => t.Value).SingleOrDefault(x => x.Key == _value);
#endif

                if (option.Key == null)
                    throw new InvalidOperationException($"Option `{value}` for `{parameter}` unavailable on this device");


#if ANDROID
                (comms.CurrentDevice as R7GenericDevice).UpdateParameter(_parameter, _value);
#elif IOS
                comms.CurrentDevice.Update((nuint)(int)_parameter, NSData.FromArray(new byte[] { (byte)Convert.ToInt32(_value) }));
#endif

                ///Запрашиваем новое значение с устройства чтобы убедиться что оно сохранилось
                ///
#if ANDROID
                (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);
#elif IOS
                comms.CurrentDevice.Request(_parameter.EnumToInt());
#endif

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
#if ANDROID
                            (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);
#elif IOS
                            comms.CurrentDevice.Request(_parameter.EnumToInt());
#endif

                            ///Ждем изменения параметра
                            bool updated = await WaitForParameterAny(_parameter, throwOnError: false);

                            ///Если значение параметра не изменилось - 99.9% что устройство требует разблокировки по PIN
                            if (!updated && ConnectedDevice.LockStatus == LockState.Locked)
                            {
                                await Unlock();

                                ///После успешной разблокировки делаем повторную попытку чтения

#if ANDROID
                                (comms.CurrentDevice as R7GenericDevice).RequestParameter(_parameter);
#elif IOS
                                comms.CurrentDevice.Request(_parameter.EnumToInt());
#endif

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

                    EventHandler<DeviceParameter> handler1 = (s, e) =>
                    {
                        if (e.Identifier.ToR7().EnumToInt() == parameter.EnumToInt())
                            r.Set();
                    };

                    EventHandler<LockStatusUpdatedEventArgs> handler2 = (s, e) =>
                    {
                        if (e.New != LockState.Unlocked)
                            r.Set();
                    };

                    try
                    {
                        __DeviceParameterUpdated += handler1;
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
                            if (device.LockStatus == LockState.Locked)
                                throw new DeviceIsLockedException();

                            throw new Exception();
                        }


                        return true;
                    }
                    finally
                    {
                        __DeviceParameterUpdated -= handler1;
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

                    //if (Convert.ToInt32(current.CachedValue) == value)
                    //return true;

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
            Safety(() =>
            {
                logger.Log($"[R7] Credit balance updated = {p0}");
            });
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


        private EventHandler<DeviceParameter> __DeviceParameterUpdated = delegate { };


        public void DeviceParameterUpdated(DeviceParameter p0)
        {
            Safety(() =>
            {
                logger.Log($"[R7] Device parameter changed `{p0.Label}` [{p0.Characteristic}] -> `{p0.CachedValue}`");
                device.OnParameterChanged(p0);

                __DeviceParameterUpdated(this, p0);
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

#if DEBUG
                //if (!test)
                //{
                //    Debugger.Break();
                //    test = true;

                //    var bytes = new List<byte[]>
                //    {
                //        "1208058CA3F19CBE0170827FF4FFD2E91B0027F847FF4780F237004CF08FFEDF447F03C004FFE8FF99D4B7006EF08FFE5F327D09C004FFE8FF0A60F6168009FED1FF1B692F091CE11FFD3FABF202C011FED1FFCB262F0058E01FFD9F014E5E0130C13FFA7F43D51D8023FCA3FF67D5DC02B0C23FFA7F29CD250023FCA3FF3B009A4B0036F847FF6FA4B96660837FF47FCB".ToByteArray(),
                //    };

                //    Task.Run(async () =>
                //    {
                //        foreach (var b in bytes)
                //        {
                //            MessageReceived(0, NSData.FromArray(b));
                //            await Task.Delay(1000);
                //        }
                //    });
                //}
#endif
            });
        }



#if ANDROID
        public void MessageCheckFinished()
        {
            Safety(() =>
            {
                logger.Log($"[R7] Packet check finished");
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
                logger.Log($"[R7] Packet progress completed {messageId}");
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
                logger.Log($"[R7] Packet progress {p0} -> {p1}/{p2 + 1}");
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
                logger.Log($"[R7] Packet received {messageId} -> {data.ToHexString()}");

                Debugger.Break();

                PacketReceived(this, new PacketReceivedEventArgs()
                {
                    MessageId = (short)(messageId + 10000),
                    Payload = data
                });

                return true;
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
                logger.Log($"[R7] Packet status updated {messageId} -> {status}");

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


                PacketStatusUpdated(this, new PacketStatusUpdatedEventArgs()
                {
                    MessageId = (short)messageId,
                    Status = _status,
                    Message = status.ToString(),
                });

                return true;
            });
        }


        private List<IFoundDevice> devices = new List<IFoundDevice>();

#if ANDROID
        public void DiscoveryFoundDevice(string deviceIdentifier, string deviceName)
#elif IOS
        public void DiscoveryFoundDevice(Foundation.NSUuid deviceIdentifier, string deviceName)
#endif
        {
            __DiscoveryFoundDevice(deviceIdentifier.ToString(), deviceName);
        }


        public void __DiscoveryFoundDevice(string deviceIdentifier, string deviceName)
        {
            Safety(() =>
            {
                if (devices.Any(x => x.Id.ToString() == deviceIdentifier.ToString()))
                    return;

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
