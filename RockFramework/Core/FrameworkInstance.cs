using Rock.Bluetooth;
using Rock.Commands;
using Rock.Core;
using Rock.Exceptions;
using Rock.Helpers;
using Rock.Threading;
using Rock.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rock
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public byte[] Payload { get; set; }
        public bool Handled { get; set; } = false;
    }

    public class MessageStatusUpdatedEventArgs : EventArgs
    {
        public short MessageId { get; set; }
        public MessageStatus Status { get; set; }
        public bool Handled { get; set; } = false;
    }


    public class DeviceSearchResultsEventArgs : EventArgs
    {
        public List<IBluetoothDevice> Devices { get; set; }
    }


    public class MessageProgressEventArgs : EventArgs
    {
        public short MessageId { get; set; }
    }



    public interface IStorage
    {
        void PutString(string key, string value);
        string GetString(string key, string defaultValue);
        short GetShort(string key, short defaultValue);
        void PutShort(string key, short value);
    }


    [Flags]
    public enum ConnectFlags : int
    {
        None = 0,

        /// <summary>
        /// Пользователь вручную нажал кнопку "ПОДКЛЮЧИТЬСЯ"
        /// </summary>
        UserClicked = 1,

        /// <summary>
        /// Внутренняя попытка подключиться (например, обновление/сохранение каких-то параметров и т.д.)
        /// </summary>
        Internal = 2,

        /// <summary>
        /// 
        /// </summary>
        SkipIfConnecting = 4
    }


    public interface IFramework : IDisposable
    {
        event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults;
        event EventHandler<EventArgs> SearchTimeout;
        event EventHandler<MessageStatusUpdatedEventArgs> _MessageStatusUpdated;
        event EventHandler<MessageReceivedEventArgs> _MessageReceived;


        IDevice ConnectedDevice { get; }


        Task<bool> Connect(
            Guid id,
            ConnectFlags flags = ConnectFlags.None,
            bool throwOnError = false);

        Task<bool> Connect(
            IBluetoothDevice device,
            ConnectFlags flags = ConnectFlags.None,
            bool throwOnError = false);



        /// <summary>
        /// Отправить текущие координаты
        /// </summary>
        Task SendManual();

        /// <summary>
        /// Отправить SOS
        /// </summary>
        Task SendAlert();

        /// <summary>
        /// "БИИП"
        /// </summary>
        Task Beep();


        void StartDeviceSearch();
        void StopDeviceSearch();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="shortId"></param>
        /// <returns></returns>
        Task SendRawMessageWithDataAndIdentifier(byte[] data, short messageId);
    }


    /// <summary>
    /// 
    /// </summary>
    public class FrameworkInstance : IFramework
    {
        public event EventHandler<DeviceSearchResultsEventArgs> DeviceSearchResults = delegate { };
        public event EventHandler<EventArgs> SearchTimeout = delegate { };
        public event EventHandler<MessageStatusUpdatedEventArgs> _MessageStatusUpdated = delegate { };
        public event EventHandler<MessageReceivedEventArgs> _MessageReceived = delegate { };



        private IBluetooth bluetooth;
        private ThreadWorker worker;
        private IStorage storage;
        private ILogger logger;


        /// <summary>
        /// Номер ключа в списке всех ключей. Они захардкодены в исходниках фреймворка и во всех пакетах передается именно номер, а не сам ключ!
        /// </summary>
        internal readonly byte KeyIndex;

        /// <summary>
        /// Локально сгенерированный Id приложения. Используется во всех пакетах, чтобы разруливать пакеты от устройства для "меня"
        /// </summary>
        internal readonly string AppId;


        private Guid deviceMac;

        /// <summary>
        /// Все найденные GATT характеристики устройства
        /// </summary>
        private List<IGattCharacteristic> gatts = new List<IGattCharacteristic>();

        /// <summary>
        /// 
        /// </summary>
        public Device ConnectedDevice { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        IDevice IFramework.ConnectedDevice => ConnectedDevice;


        private SemaphoreSlim unlockSemaphore = new SemaphoreSlim(1, 1);
        private SemaphoreSlim readSemaphore = new SemaphoreSlim(1, 1);
        private SemaphoreSlim writeSemaphore = new SemaphoreSlim(1, 1);

        private const string DEFAULT_APP_ID = "//////8=";
        private const byte DEFAULT_KEY_INDEX = 255;

        public FrameworkInstance(
            IBluetooth bluetooth,
            IStorage storage,
            byte? keyIndex = null,
            byte[] appId = null,
            ILogger logger = null)
        {

            this.bluetooth = bluetooth;
            this.bluetooth.BluetoothStateChanged += Bluetooth_BluetoothStateChanged;
            this.bluetooth.DeviceConnectionLost += Bluetooth_DeviceConnectionLost;

            this.worker = new ThreadWorker();
            this.storage = storage;
            this.logger = logger ?? new ConsoleLogger();


            //if (appId == null)
            //    appId = new byte[] { 100, 101, 102, 103, 104 };


            this.KeyIndex = keyIndex ?? DEFAULT_KEY_INDEX;
            this.AppId = Convert.ToBase64String(appId ?? new byte[] { 255, 255, 255, 255, 255 });

            this.ConnectedDevice = new Device(this);
        }


        /// <summary>
        /// Блютуз устройство отключено
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bluetooth_DeviceConnectionLost(object sender, EventArgs e)
        {
            logger.Log("[CONNECT] Device connection lost");

            bluetoothDevice?.Dispose();
            bluetoothDevice = null;
            gatts.Clear();

            ConnectedDevice.SetState(DeviceState.Disconnected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bluetooth_BluetoothStateChanged(object sender, BluetoothStateChangedEventArgs e)
        {
            logger.Log($"[BLUETOOH] Bluetooth state changed -> {e.IsEnabled}");
        }



        /// <summary>
        /// 
        /// </summary>
        public async void StartDeviceSearch()
        {
            try
            {
                rockstars.Clear();

                this.bluetooth.ScanResults += Bluetooth_ScanResults;
                this.bluetooth.ScanTimeout += Bluetooth_ScanTimeout;

                await bluetooth.TurnOn();

                bluetooth.StartLeScan();
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void StopDeviceSearch()
        {
            try
            {
                bluetooth.StopLeScan();
                this.bluetooth.ScanResults -= Bluetooth_ScanResults;
                this.bluetooth.ScanTimeout -= Bluetooth_ScanTimeout;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }


        private void Bluetooth_ScanTimeout(object sender, EventArgs e)
        {
            if (rockstars.Count == 0)
                SearchTimeout(this, new EventArgs());
        }


        private List<IBluetoothDevice> rockstars = new List<IBluetoothDevice>();


        private static readonly Regex RockStar = new Regex(@"^[2]\d{4}$", RegexOptions.Compiled);
        private static readonly Regex RockFleet = new Regex(@"^[5]\d{4}$", RegexOptions.Compiled);
        private static readonly Regex RockAir = new Regex(@"^[1]\d{5}$", RegexOptions.Compiled);


        private static bool IsSatelliteDevice(IBluetoothDevice device)
        {
            ///Соответствие по MAC адресу
            if (
                device?.Mac?.ToUpperInvariant()?.StartsWith("00:07:80") == true ||
                device?.Mac?.ToUpperInvariant()?.StartsWith("88:6B:0F") == true ||
                device?.Mac?.ToUpperInvariant()?.StartsWith("88:0B:81") == true)
                return true;


            ///Соответствие по серийному номеру из названия устройства
            string serial = device?.Name?.Trim()?.Split(' ')?.LastOrDefault() ?? string.Empty;

            if (RockStar.IsMatch(serial) || RockFleet.IsMatch(serial) || RockAir.IsMatch(serial))
                return true;

            return false;
        }


        private void Bluetooth_ScanResults(object sender, ScanResultsEventArgs e)
        {
            var _rockstars = e.FoundDevices
                .Where(x => IsSatelliteDevice(x))
                .ToList();

            if (_rockstars.Any() && _rockstars.Count != this.rockstars.Count)
            {
                this.rockstars = _rockstars;

#if DEBUG
                //Connect(rockstars.FirstOrDefault());
#endif

                DeviceSearchResults(this, new DeviceSearchResultsEventArgs()
                {
                    Devices = this.rockstars
                });
            }
        }


        private IBluetoothDevice bluetoothDevice;
        private SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);


        /// <summary>
        /// Повторно подключиться к устройству
        /// </summary>
        /// <returns></returns>
        public async Task Reconnect(ConnectFlags flags = ConnectFlags.None, bool throwOnError = false)
        {
            if (deviceMac == Guid.Empty)
            {
#if DEBUG
                Debugger.Break();
#endif
                return;
            }

            await Connect(
                deviceMac,
                flags: flags,
                throwOnError: throwOnError);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public Task<bool> Connect(IBluetoothDevice device, ConnectFlags context = ConnectFlags.None, bool throwOnError = false)
        {
            return Connect(
                device.Id,
                flags: context,
                throwOnError: throwOnError);
        }


        /// <summary>
        /// 
        /// </summary>
        private IGattCharacteristic outbox;
        private IGattCharacteristic battery;
        private IGattCharacteristic location;
        private IGattCharacteristic messageStatus;


        internal bool IsGprsAttached { get; private set; }
        internal bool IsRawMessagingAvailable { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="deviceMac"></param>
        /// <returns></returns>
        public async Task<bool> Connect(Guid deviceMac, ConnectFlags flags = ConnectFlags.None, bool throwOnError = false)
        {
            ///Не ждем завершения попытки подключения
            if (flags.IsSet(ConnectFlags.SkipIfConnecting) && connectLock.CurrentCount == 0)
            {
                ConsoleLogger.WriteLine("[CONNECT] Skip connecting attempt - already connecting");
                return true;
            }



            await connectLock.WaitAsync();


            try
            {
                ///Уже подключены
                if (bluetoothDevice?.IsConnected == true)
                    return true;


                gatts.Clear();
                ConnectedDevice.SetState(DeviceState.Connecting, flags);


                bluetoothDevice?.Dispose();
                bluetoothDevice = null;

                this.deviceMac = deviceMac;


                logger.Log($"[CONNECT] Connecting to `{deviceMac}`...");


                bluetoothDevice = await bluetooth.ConnectToDeviceAsync(deviceMac);
                var services = await bluetoothDevice.GetServicesAsync();


                string guids = string.Join(", ", services.Select(x => x.Id.ToString()));

                foreach (var service in services)
                {
                    var _chars = await service.GetCharacteristicsAsync();
                    this.gatts.AddRange(_chars.ToList());
                }


                foreach (var gatt in gatts)
                {
                    gatt.ValueUpdated += (ss, e) => worker.Post(() => { OnGattChanged(e.Characteristic); }, "gatt changed");
                }

                this.battery = gatts.FirstOrDefault(x => x.Id.ToString() == "8c0a3f8b-fccb-482a-8406-e6ad57b324f4");
                this.outbox = gatts.FirstOrDefault(x => x.Id.ToString() == "cf17c69c-9d80-4ffb-8fa3-f81a83170cef");
                var indicator = gatts.FirstOrDefault(x => x.Id.ToString() == "d0701859-7e41-47b1-af19-fb305f98ab51");
                var common = gatts.FirstOrDefault(x => x.Id.ToString() == "a84c417a-3380-4a4b-a885-f926a647bb3c");
                var inbox = gatts.FirstOrDefault(x => x.Id.ToString() == "4de3e821-2f25-4da0-b696-d06f81f46a52");
                this.messageStatus = gatts.FirstOrDefault(x => x.Id.ToString() == "a7a6f930-1ad0-4a68-9d85-1228fc3e5c19");
                var screenStatus = gatts.FirstOrDefault(x => x.Id.ToString() == "049e8f08-78a3-443a-9517-58dab1ce721d");
                this.location = gatts.FirstOrDefault(x => x.Id.ToString() == "d6f3af9a-cea4-4220-a7ee-8eced1534af3");

                await indicator.StartUpdatesAsync();
                await inbox.StartUpdatesAsync();
                await messageStatus.StartUpdatesAsync();
                await common.StartUpdatesAsync();
                await screenStatus.StartUpdatesAsync();


                string _firmware = await GetDeviceFirmware();
                string _hardware = await GetDeviceHardware();


                IsRawMessagingAvailable = await GetRawMessagingAvailable();
                IsGprsAttached = await GetGprsAttached();



                ConnectedDevice.Hardware = _hardware;
                ConnectedDevice.Firmware = _firmware;
                ConnectedDevice.Discovered(gatts
                    .Select(x => x.Id)
                    .ToList());


#if DEBUG
                //if (gatts.Count != 66)
                //Debugger.Break();
#endif


                //#if DEBUG
                //                var r = Guid.Parse("40a40e43-8ac5-42a4-ae1c-7f822194a671");
                //                var g = string.Join("\n", gatts.Select(x => x.Id));
                //                var a = gatts.Where(x => x.Id == r);
                //                var rr = gatts.FirstOrDefault(x => x.Id.ToString() == "40a40e43-8ac5-42a4-ae1c-7f822194a671");

                //#endif


                var enums = Enum
                    .GetValues(typeof(Parameter))
                    .Cast<Parameter>()
                    .ToList();

                gatts.ForEach(gatt =>
                {
                    if (enums.Any(e => e.GetAttribute<GattCharacteristicAttribute>()?.Value == gatt.Id.ToString()))
                    {
                        var name = enums.FirstOrDefault(e => e.GetAttribute<GattCharacteristicAttribute>()?.Value == gatt.Id.ToString());
                        gatt.Name = name.ToString();
                    }
                    else
                    {
                        gatt.Name = Parameter.Unknown.ToString();
                    }
                });


                ConnectedDevice.SetState(DeviceState.Connected);


                ///Проверяем наличие "непрочитаных" входящих сообщений
                PostReadGatt(inbox);
                ///Проверяем наличие "непрочитаных" статусов сообщений
                PostReadGatt(messageStatus);

                RequestMessageStatus();



                logger.Log($"[CONNECT] Done");

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                //Debugger.Break();
#endif
                logger.Log($"[CONNECT] Connection error {e.Message}");


                ConnectedDevice.SetState(DeviceState.Disconnected);


                if (throwOnError)
                {
                    if (e is RockException)
                        throw e;
                    else
                        throw new Exceptions.DeviceConnectionException(null, e);
                }

                return false;
            }
            finally
            {
                connectLock.Release();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetRawMessagingAvailable()
        {
            var rawMessaging = gatts.FirstOrDefault(x => x.Id.ToString() == "c7de1b97-882d-4c52-9a79-8f87bd9a9a4f");

            if (rawMessaging == null)
                return false;

            await rawMessaging.ReadAsync();

            byte[] value = rawMessaging.Value;
            bool ok = value != null && value[0] == 1;
            return ok;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetGprsAttached()
        {
            var gprsAttached = gatts.FirstOrDefault(x => x.Id.ToString() == "0e1cc964-da87-4ba7-9490-05045b1d06ab");

            if (gprsAttached == null)
                return false;

            await gprsAttached.ReadAsync();

            byte[] value = gprsAttached.Value;
            bool ok = value != null && (value[0] & 1) == 1;
            return ok;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDeviceFirmware()
        {
            var firmware = gatts.FirstOrDefault(x => x.Id.ToString() == "00002a28-0000-1000-8000-00805f9b34fb");
            var bytes = await firmware.ReadAsync();
            return Process_Firmware(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetDeviceHardware()
        {
            var hardware = gatts.FirstOrDefault(x => x.Id.ToString() == "b266cf58-8aa9-4491-a3e6-4876b4ee6efc");
            var bytes = await hardware.ReadAsync();
            return Process_Hardware(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        public void RequestBattery()
        {
            PostReadGatt(battery);
            PostCommand(new ActionCommand(AppId, KeyIndex, ActionRequestType.BatteryUpdate));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void RequestLocation()
        {
            PostCommand(new ActionCommand(AppId, KeyIndex, ActionRequestType.PositionUpdateLastKnown));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Location> UpdateLocation()
        {
            return await Task.Run(() =>
            {
                bool success = false;
                AutoResetEvent r = new AutoResetEvent(false);

                var handler = new EventHandler<LocationUpdatedEventArgs>((s, e) =>
                {
                    success = true;
                    r.Set();
                });

                try
                {
                    ConnectedDevice.LocationUpdated += handler;

                    RequestLocation();
                    r.WaitOne(TimeSpan.FromSeconds(30));

                }
                finally
                {
                    ConnectedDevice.LocationUpdated -= handler;
                }

                if (!success)
                    throw new TimeoutException();

                return ConnectedDevice.Location;

            });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal async Task ReadDeviceParameter(Guid id, bool reconnectIfNeeded = true, bool checkSuccess = false)
        {
            var gatt = gatts.FirstOrDefault(x => x.Id == id);

            if (gatt == null)
                throw new NullReferenceException($"Unknown GATT characteristic `{id}`");

            await ReadDeviceParameter(gatt, reconnectIfNeeded, checkSuccess);
        }



        /// <summary>
        /// 
        /// </summary>
        internal async Task ReadDeviceParameter(IGattCharacteristic gatt, bool reconnectIfNeeded = true, bool checkSuccess = false)
        {
            if (reconnectIfNeeded)
                await Reconnect(throwOnError: true);


            //await Unlock();
            await gatt.ReadAsync();


            if (checkSuccess)
            {
                bool updated = await WaitForDeviceParameter(gatt.Id);

                ///Если значение параметра не изменилось - 99.9% что устройство требует разблокировки по PIN
                if (!updated && ConnectedDevice.LockStatus == LockState.Locked)
                {
                    await Unlock();

                    ///После успешной разблокировки делаем повторную попытку чтения
                    worker.Post(() =>
                    {
                        return ReadDeviceParameter(gatt, reconnectIfNeeded, checkSuccess);

                    }, "read gatt retry");
                }
            }
        }




        /// <summary>
        /// Ждем когда параметр устройства примет какое-то значение
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WaitForDeviceParameter(Guid id)
        {
            var parameter = ConnectedDevice.GetParameter(id);

            if (parameter == null)
                throw new NullReferenceException($"Parameter `{id}` not found");


            ///Уже есть значение
            if (parameter.HasCachedValue)
                return true;


            return await Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);

                EventHandler<ParameterChangedEventArgs> handler = (s, e) =>
                {
                    ///Изменилось значение ожидаемого параметра
                    if (e.Parameter.GattId == id)
                        r.Set();
                };

                ConnectedDevice.ParameterChanged += handler;

                ///Ждем не более 5 сек
                r.WaitOne(TimeSpan.FromSeconds(5));

                ConnectedDevice.ParameterChanged -= handler;

                ///Есть изменилось?
                return parameter.HasCachedValue;
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatt"></param>
        private void PostReadGatt(IGattCharacteristic gatt)
        {
            worker.Post(() =>
            {
                return gatt.ReadAsync();

            }, $"post read gatt `{gatt.Name}`");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="checkSuccess"></param>
        /// <returns></returns>
        internal async Task WriteDeviceParameter(Guid id, byte[] data, bool checkSuccess = false)
        {
            await Reconnect(throwOnError: true);
            await Unlock();

            var gatt = gatts.FirstOrDefault(x => x.Id == id);

            if (gatt == null)
                throw new NullReferenceException($"Unknown GATT characteristic `{id}`");


            await gatt.WriteAsync(data);


            if (checkSuccess)
            {
                ///1 - Убеждаемся, что записанное значение равно тому что после записи возвращает устройство
                var saved = await gatt.ReadAsync();

                if (!saved.SequenceEqual(data))
                    throw new Exception("GATT write error - new data not equals to written");

                //bool updated = await WaitForParameter(id, data);

                /////2 - Если после записи нового значения устройство вернуло старое - 99% что устройство не разблокировано <see cref="Unlock(short?)"/>
                //if (!updated)
                //    throw new NotUnlockedException();
            }
        }



        /// <summary>
        /// Разблокировать устройство
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public async Task Unlock(short? pin = null)
        {
            if (!IsConnected)
                throw new NotConnectedException();


            ///Уже разблокировано
            if (ConnectedDevice.LockStatus == LockState.Unlocked)
                return;

            await unlockSemaphore.WaitAsync();

            try
            {
                ///Берем ранее сохраненный ПИН
                if (pin == null)
                    pin = storage.GetShort("device-pin", 1234);

                if (pin == null)
                    throw new ArgumentNullException("Pin is null");


                await PostCommandAsync(new PinCommand(AppId, KeyIndex, pin.Value, pin.Value));
                await WaitForUnlocked();

                ///При успешной разблокировке сохраняем новый ПИН
                storage.PutShort("device-pin", pin.Value);
            }
            finally
            {
                unlockSemaphore.Release();
            }
        }


        /// <summary>
        /// Ожидаем разблокировки устройства
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotConnectedException">Устройство не подключено</exception>
        /// <exception cref="IncorrectPinException">Некорректный PIN</exception>
        /// <exception cref="Exception">Какая-то ошибка</exception>
        private async Task WaitForUnlocked()
        {
            if (ConnectedDevice.LockStatus == LockState.Unlocked)
                return;

            await Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);

                EventHandler<LockStatusUpdatedEventArgs> handler = (s, e) =>
                {
                    r.Set();
                };

                ConnectedDevice.DeviceLockStatusUpdated += handler;

                r.WaitOne(TimeSpan.FromSeconds(20));

                ConnectedDevice.DeviceLockStatusUpdated -= handler;


                if (!IsConnected)
                    throw new NotConnectedException();

                if (ConnectedDevice.IncorrectPin == true)
                    throw new IncorrectPinException();

                if (ConnectedDevice.LockStatus != LockState.Unlocked)
                    throw new Exception("Unable to unlock device");
            });
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task Beep()
        {
#if DEBUG
            byte[] b = this.incoming.TryGet();
            Console.WriteLine($"[HEX] {b?.ToHexString()}");
            incoming.flush(true);
            PostReadGatt(gatts.FirstOrDefault(x => x.Id.ToString() == "4de3e821-2f25-4da0-b696-d06f81f46a52"));
#endif

            await Reconnect(throwOnError: true);
            await Unlock();
            await PostCommandAsync(new ActionCommand(AppId, KeyIndex, ActionRequestType.Beep));
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task SendAlert()
        {
            await Reconnect(throwOnError: true);
            await Unlock();
            await PostCommandAsync(new ActionCommand(AppId, KeyIndex, ActionRequestType.SendAlert));
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task SendManual()
        {
            await Reconnect(throwOnError: true);
            await Unlock();
            await PostCommandAsync(new ActionCommand(AppId, KeyIndex, ActionRequestType.SendManual));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task RequestMailboxCheck()
        {
            await Reconnect(throwOnError: true);
            await Unlock();
            await PostCommandAsync(new ActionCommand(AppId, KeyIndex, ActionRequestType.MailboxCheck));
        }


        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="alert"></param>
        //        /// <param name="position"></param>
        //        public void RaiseGenericAlert(CommandGenericAlertType alert, bool position)
        //        {
        //#if RELEASE
        //            if (isConnected().booleanValue() && !isLocked().booleanValue())
        //#endif

        //            PostCommand(new AlertCommand(installationId, appIdIndex, alert, position));

        //        }


        internal readonly Queue queue = new Queue();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatt"></param>
        private void ForceReadGatt(IGattCharacteristic gatt)
        {
            worker.Post(async () =>
            {
                await gatt.ReadAsync();

            }, "force read gatt");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatt"></param>
        private void OnGattChanged(IGattCharacteristic gatt)
        {
            string name = string.IsNullOrEmpty(gatt.Name)
                ? gatt.Id.ToString()
                : gatt.Name;

            logger.Log($"[GATT CHANGED] `{name}` -> {gatt.Value.ToHexString()}");


            switch (gatt.Id.ToString())
            {
                case "d0701859-7e41-47b1-af19-fb305f98ab51":
                    Process_IncomingCommand(gatt.Value);
                    break;

                case "a84c417a-3380-4a4b-a885-f926a647bb3c":

                    short s = new ByteBuffer(gatt.Value).ReadInt16();


                    if (s == 11 || s == 12)
                    {
                        //this.gattManager.readData(this.GATT_FIRMARE);
                    }
                    else if (s == 87 || s == 88)
                    {
                        //this.gattManager.readData(this.GATT_IMEI);
                    }
                    else if (s == 39 || s == 40)
                    {
                        //this.gattManager.readData(this.GATT_SCREEN_LOCK);
                    }
                    else if (s == 83 || s == 84)
                    {
                        //this.gattManager.readData(this.GATT_LOCATION);
                    }
                    else if (s == 106 || s == 107)
                    {
                        //this.gattManager.readData(this.GATT_BATTERY);
                    }
                    else if (s == 15 || s == 16)
                    {
                        //requestCharacteristic(f414k);
                    }
                    else if (s == 19 || s == 20)
                    {
                        //requestCharacteristic(f415l);
                    }
                    else if (s == 201 || s == 202)
                    {
                        //requestCharacteristic(f416m);
                    }
                    //else if ((s == 217 || s == 221 || s == 225 || s == 228 || s == 231) && s == 231 && this.responseDelegate != null && this.responseDelegate.get() != null)
                    //{
                    //    Log.i("CONNECT", "GPRS NOTIFY");
                    //    requestCharacteristic(this.connectedDevice.parameterForAttribute(217).getCharaceristic());
                    //    requestCharacteristic(this.connectedDevice.parameterForAttribute(221).getCharaceristic());
                    //    requestCharacteristic(this.connectedDevice.parameterForAttribute(225).getCharaceristic());
                    //    requestCharacteristic(this.connectedDevice.parameterForAttribute(228).getCharaceristic());
                    //    requestCharacteristic(this.connectedDevice.parameterForAttribute(231).getCharaceristic());
                    //}

                    break;

                case "8c0a3f8b-fccb-482a-8406-e6ad57b324f4":
                    Process_Battery(gatt.Value);
                    break;

                case "049e8f08-78a3-443a-9517-58dab1ce721d":
                    Process_LockStatus(gatt.Value);
                    break;

                case "a7a6f930-1ad0-4a68-9d85-1228fc3e5c19":
                    Process_MessageStatus(gatt.Value);
                    break;

                case "d6f3af9a-cea4-4220-a7ee-8eced1534af3":
                    Process_Location(gatt.Value);
                    break;

                case "4de3e821-2f25-4da0-b696-d06f81f46a52":
                    Process_Inbox(gatt.Value);
                    break;


                default:

                    var parameter = ConnectedDevice.GetParameter(gatt.Id);

                    if (parameter != null)
                    {
                        ConnectedDevice.OnParameterUpdated(gatt.Id, gatt.Value);
                    }
                    else
                    {
#if DEBUG
                        //Debugger.Break();
#endif
                        logger.Log($"Gatt `{gatt.Id}` value changed but not implemented");
                    }

                    break;
            }
        }


        private readonly IncomingBuffer incoming = new IncomingBuffer();

        /// <summary>
        /// Входящая команда (?) от устройства
        /// </summary>
        /// <param name="data"></param>
        private void Process_IncomingCommand(byte[] data)
        {
            var hex = data.ToHexString();
            logger.Log($"[FROM DEVICE] --> {hex}");


            this.incoming.add(data);
            logger.Log($"[BUFFER] --> {this.incoming}");

            byte[] b = this.incoming.TryGet();

            if (b != null)
            {
                var command = DeserializedCommand.Parse(data);

                if (command == null)
                    return;

                logger.Log($"[FROM DEVICE] --> AppId={command.AppId} Key={command.Key} Type={command.CommandType} ActionRequest={command.ActionRequestType} MessageId={command.MessageId}");

                try
                {
                    HandleCommandFromDevice(command);
                }
                catch (Exception e)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    logger.Log($"[EXCEPTION] Exception occured while handling incoming command {e}");
                }


                ///
                //processQueue();

            }
            else
            {
                logger.Log($"[FROM DEVICE] --> Waiting for next part");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="commandType"></param>
        /// <param name="payload"></param>
        private void HandleCommandFromDevice(DeserializedCommand command)
        {
            switch (command.CommandType)
            {
                ///Новое входящее сообщение
                case CommandType.GetNextMessage:
                    {
                        //if (command.MessageId == 0)
                        //    throw new ArgumentNullException("Message id is 0");

                        if (command.MessageId == null)
                            throw new ArgumentNullException("Message id is null");

                        if (command.Payload == null || command.Payload.Length == 0)
                            throw new ArgumentNullException("Message payload is null or empty");


                        var args = new MessageReceivedEventArgs()
                        {
                            MessageId = command.MessageId.Value,
                            Payload = command.Payload,
                            Handled = false
                        };

                        _MessageReceived(this, args);




                        //if (args.Handled)
                        //{
                        //    PostCommand(new DeleteMessageCommand(command.MessageId.Value, AppId, KeyIndex));
                        //    RequestNextMessage();
                        //}
                    }
                    break;


                ///Сообщение принято устройством (точно не известно, нужно ли это событие вообщее)
                case CommandType.SendMessage:
                    {
                        if (command.MessageId == null)
                            throw new ArgumentNullException("Message id is null");

                        var args = new MessageStatusUpdatedEventArgs()
                        {
                            MessageId = command.MessageId.Value,
                            Status = MessageStatus.ReceivedByDevice,
                        };

                        _MessageStatusUpdated(this, args);

                        if (args.Handled)
                        {
                            //PostCommand(new AcknowledgeMessageStatusCommand(command.MessageId.Value, command.AppId, command.Key));
                        }

                        //handler.post(new MessageStatusReceivedByDeviceRunnable(this, s));
                    }
                    break;

                case CommandType.ActionRequest:

                    switch (command.ActionRequestType)
                    {
                        case ActionRequestType.UpdateMessageStatus:
                            PostReadGatt(messageStatus);
                            break;

                        case ActionRequestType.PositionUpdate:
                        case ActionRequestType.PositionUpdateLastKnown:

                            PostReadGatt(location);

                            break;

                        case ActionRequestType.MailboxCheck:
                        case ActionRequestType.SendAlert:
                        case ActionRequestType.SendManual:
                        case ActionRequestType.GeofenceCentre:

                            //throw new NotImplementedException();
                            //handler.post(new C0096do(this, commandType));

                            break;

                        case ActionRequestType.BatteryUpdate:

                            PostReadGatt(battery);

                            break;

                        case ActionRequestType.ShippingMode:

                            throw new NotImplementedException();
                            //Log.w("CONNECT", "Disconnecting in 1 second");
                            //handler.postDelayed(new C0099dr(this), 1000);

                            break;

                    }
                    break;

                case CommandType.AcknowledgeMessageStatus:

                    //RequestMessageStatus();

                    break;

                case CommandType.DeleteMessage:

                    break;

                case CommandType.Internal:
                case CommandType.GenericAlert:

                    throw new NotImplementedException();
                    //handler.post(new DeviceCommandReceivedRunnable(this, CommandType));

                    break;

                case CommandType.SerialDump:

                    // DN
                    //throw new NotImplementedException();
                    //handler.post(new C0101dt(this, bArr));

                    break;

                case CommandType.GetGprsConfig:

                    throw new NotImplementedException();
                    //m388j(bArr);

                    break;

                case CommandType.SetGprsConfig:

                    break;

                case CommandType.Pin:

                    break;

                default:

                    //handler.post(new C0102du(this));

                    break;
            }



            //bool valueOf = false;

            //if (CommandType == CommandType.GetNextMessage)
            //{
            //    byte key = data[0];
            //    byte b2 = data[1];
            //    if (this.KeyIndex == key || key <= 0)
            //    {
            //        Log.w("CONNECT", "FC " + b2 + " [Actual " + key + "]");
            //        Boolean.valueOf(false);
            //        if (b2 == 0)
            //        {
            //            valueOf = Boolean.valueOf(m342a(ActivationDesistStatus.ActivationDesistStatusNoActivation));
            //        }
            //        else if (b2 == 1)
            //        {
            //            valueOf = Boolean.valueOf(m342a(ActivationDesistStatus.ActivationDesistStatusNoApp));
            //        }
            //        else if (b2 == 2)
            //        {
            //            valueOf = Boolean.valueOf(handleBalancePayload(data));
            //        }
            //        else if (b2 == 3)
            //        {
            //            valueOf = Boolean.valueOf(m384i(data));
            //        }
            //        else if (b2 == 4)
            //        {
            //            valueOf = Boolean.valueOf(m376g(data));
            //        }
            //        else if (b2 == 5)
            //        {
            //            valueOf = Boolean.valueOf(m380h(data));
            //        }
            //        else
            //        {
            //            Log.i("CONNECT", "Unknown FM " + b2);
            //            valueOf = true;
            //        }
            //        if (valueOf.booleanValue() && messageId != 0)
            //        {
            //            Log.i("CONNECT", "Should Delete " + messageId);
            //            post((BaseCommand)new DeleteMessageCommand(this.installationId, (short)0, messageId));
            //            return;
            //        }
            //        return;
            //    }
            //    Log.i("CONNECT", "PCF - NFM [Application = %i]");
            //}
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Process_Inbox(byte[] data)
        {
            ByteBuffer wrap = new ByteBuffer(data);
            short count = wrap.ReadInt16();

            logger.Log($"[INBOX] Inbox count = {count}");

            if (count > 0)
            {
                Debugger.Break();
                RequestNextMessage();
            }
        }


        /// <summary>
        ///  // Попросить устройство отправить еще необработанное (непрочитанное) сообщение
        ///  Попросить устройство ПОЛУЧИТЬ еще необработанное (непрочитанное) сообщение
        /// </summary>
        private void RequestNextMessage()
        {

            //for (byte index = 0; index < byte.MaxValue; index++)
            //{
            //    PostCommand(new GetNextMessageCommand(DEFAULT_APP_ID, index));
            //}

            ////return;

            PostCommand(new GetNextMessageCommand(this.AppId, 0));
            PostCommand(new GetNextMessageCommand(this.AppId, this.KeyIndex));

            if (IsRawMessagingAvailable)
            {
                PostCommand(new GetNextMessageCommand(DEFAULT_APP_ID, DEFAULT_KEY_INDEX));
                PostCommand(new GetNextMessageCommand(appId: this.AppId, this.KeyIndex));
            }
        }

        /// <summary>
        /// Попросить устройство отправить еще необработанный статус сообщения
        /// </summary>
        private void RequestMessageStatus()
        {
            PostCommand(new ActionCommand(this.AppId, 0, ActionRequestType.UpdateMessageStatus));
            PostCommand(new ActionCommand(this.AppId, this.KeyIndex, ActionRequestType.UpdateMessageStatus));

            if (IsRawMessagingAvailable)
                PostCommand(new ActionCommand(DEFAULT_APP_ID, DEFAULT_KEY_INDEX, ActionRequestType.UpdateMessageStatus));
        }



        //    /* renamed from: fl */
        //    public class C0147fl extends BaseCommand
        //    {
        //public C0147fl(long j, short s, short s2)
        //    {
        //        super(R7CommandType.R7CommandTypeSendMessage, j, s);
        //        this.messageId = s2;
        //        ArrayList arrayList = new ArrayList();
        //        arrayList.add(m221h());
        //        this.packets = arrayList;
        //    }

        //    /* renamed from: h */
        //    private byte[] m221h()
        //    {
        //        short s = (short)13;
        //        ByteBuffer allocate = ByteBuffer.allocate(20);
        //        allocate.putShort(s);
        //        allocate.put(getCommandType_BYTE());
        //        allocate.put(getInstallationId_BYTES());
        //        allocate.put((byte)0);
        //        allocate.putShort(this.messageId);
        //        allocate.put(getAppIdIndex_BYTE());
        //        allocate.put((byte)2);
        //        allocate.putShort(ChecksumHelper.checksum(allocate.array(), s));
        //        return allocate.array();
        //    }
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Process_MessageStatus(byte[] data)
        {
            var status = DeserializedStatus.Parse(data);


            if (status.MessageId != 0)
            {
                logger.Log($"[STATUS] Message status updated `{status.MessageId}` -> Transmitted");

                if (status.AppId == this.AppId && status.Key == this.KeyIndex)
                {
                    var args = new MessageStatusUpdatedEventArgs()
                    {
                        MessageId = status.MessageId.Value,
                        Status = MessageStatus.Transmitted,
                        Handled = false
                    };
                    _MessageStatusUpdated(this, args);

                    if (args.Handled)
                    {
                        PostCommand(new AcknowledgeMessageStatusCommand(status.MessageId.Value, status.AppId, status.Key));
                    }
                }
                else if (status.Key == DEFAULT_KEY_INDEX)
                {
                    var args = new MessageStatusUpdatedEventArgs()
                    {
                        MessageId = status.MessageId.Value,
                        Status = MessageStatus.Transmitted,
                        Handled = false
                    };
                    _MessageStatusUpdated(this, args);

                    if (args.Handled)
                    {
                        PostCommand(new AcknowledgeMessageStatusCommand(status.MessageId.Value, this.AppId, status.Key));
                    }
                }
                else if (status.Key == 0)
                {
                    var a = BitConverter.GetBytes(1099511627775);
                    //this.comms.onMessageStatusHandled(this.id, 1099511627775L, (short)-1);

                    PostCommand(new AcknowledgeMessageStatusCommand(status.MessageId.Value, this.AppId, DEFAULT_KEY_INDEX));

                    //if (this.f452ae != 0 && messageId == this.f452ae)
                    //{
                    //    handler.postDelayed(new C0078cx(this), 5000);
                    //}
                }
                else
                {
                    //Log.w("CONNECT", "THIS STATUS UPDATE IS NOT FOR ME");
                }

            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private string Process_Hardware(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Replace("\0", string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private string Process_Firmware(byte[] data)
        {
            return Encoding.UTF8.GetString(data).Replace("\0", string.Empty);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void Process_Location(byte[] data)
        {
            var location = LocationParser.Parse(data, ConnectedDevice.Firmware);
            ConnectedDevice.Location = location;
        }



        public enum InnerLockState : int
        {
            Unlocked = 0,
            Locked = 1,
            IncorrectPin = 2,
            Unknown = 3
        }



        /// <summary>
        /// Изменилось состояние блокировки устройства
        /// </summary>
        /// <param name="data"></param>
        private void Process_LockStatus(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer(data);

                InnerLockState innerState = (InnerLockState)buffer.ReadInt16();

                LockState state = LockState.Unknown;
                bool? incorrectPin = null;

                switch (innerState)
                {
                    case InnerLockState.IncorrectPin:
                        state = LockState.Locked;
                        incorrectPin = true;
                        break;

                    case InnerLockState.Locked:
                        state = LockState.Locked;
                        break;

                    case InnerLockState.Unknown:
                        state = LockState.Unknown;
                        break;

                    case InnerLockState.Unlocked:
                        state = LockState.Unlocked;
                        break;
                }

                ConnectedDevice.SetLockStatus(state, incorrectPin);
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                logger.Log(e);
            }
        }




        /// <summary>
        /// Изменилось значения батарейки
        /// </summary>
        /// <param name="data"></param>
        private void Process_Battery(byte[] data)
        {
            try
            {
                var buffer = new ByteBuffer(data);
                int? battery = buffer.ReadInt32();

                ///HACK: RockFLEET всегда возвращает 0. В нативном приложении батарейка для него вообще не отображается
                if (battery == 0)
                    battery = null;

                ConnectedDevice.Battery = battery;
            }
            catch (Exception e)
            {
#if DEBUG
                Debugger.Break();
#endif
                logger.Log(e);
            }
        }



        /// <summary>
        /// Ставим задачу в очередь на выполнение без ожидания завершения
        /// </summary>
        /// <param name="command"></param>
        internal void PostCommand(BaseCommand command, Action onFinish = null, Action<Exception> onError = null)
        {
            worker.Post(new CommandRunnable(this, command, logger, onFinish, onError));
        }


        /// <summary>
        /// Ставим задачу в очередь на выполнение и ожидаем ее завершения
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal async Task PostCommandAsync(BaseCommand command)
        {
            await Task.Run(() =>
            {
                AutoResetEvent r = new AutoResetEvent(false);
                Exception exception = null;

                PostCommand
                (
                    command,
                    onFinish: () =>
                    {
                        r.Set();
                    },
                    onError: (e) =>
                    {
                        exception = e;
                        r.Set();
                    }
                );

                r.WaitOne();


                if (exception != null)
                    throw new Exception("Exception occured", exception);
            });
        }


        public void Dispose()
        {
            this.bluetooth.BluetoothStateChanged -= Bluetooth_BluetoothStateChanged;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal async Task ProcessQueue()
        {
            while (queue.HasItems)
            {
                MessageChunk chunk = queue.Shift();

                var data = chunk.Payload;

                ///Согласно документации RockConnect Framework'а
                if (data.Length > 20)
                    throw new ArgumentOutOfRangeException("Transmittable package is greater than 20 bytes");


                await outbox.WriteAsync(data);


                if (chunk.CommandType == CommandType.SendFileSegment)
                {
                    throw new NotImplementedException();
                }
                else if (chunk.MessageId != 0)
                {
                    //MessageProgressUpdated(this, new MessageProgressEventArgs());

                    //if (chunk.Index == chunk.TotalCount - 1)
                    //MessageProgressCompleted(this, new MessageProgressEventArgs());

                }

            }
        }



        //public short SendMessageWithString(string text, short id)
        //{
        //    var iso = Encoding.GetEncoding("ISO-8859-1");
        //    var bytes = iso.GetBytes(text);

        //    return SendRawMessageWithDataAndIdentifier(bytes, id);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task SendRawMessageWithDataAndIdentifier(byte[] data, short messageId)
        {
            if (!IsConnected)
                throw new MessageSendingException("Device is not connected");

            if (!ConnectedDevice.IsSupported(DeviceCapability.RawCommands) || !IsRawMessagingAvailable)
                throw new MessageSendingException("Raw messaging unavailable or unsupported");

            if (data.Length > 338)
                throw new MessageSendingException("Message payload is too long");

            await PostCommandAsync(new SendMessageCommand(AppId, KeyIndex, messageId, data));
        }


        /// <summary>
        /// Устройство подключено?
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return ConnectedDevice.IsConnected;
            }
        }




    }



}
