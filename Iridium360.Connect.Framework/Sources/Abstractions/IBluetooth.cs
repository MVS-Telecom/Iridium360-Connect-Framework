using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework
{

    public class BluetoothStateChangedEventArgs : EventArgs
    {
        public bool IsEnabled { get; set; }
    }

    public class ScanResultsEventArgs : EventArgs
    {
        public List<IFoundDevice> FoundDevices { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IFoundDevice : IBluetoothDevice
    {
        /// <summary>
        /// 
        /// </summary>
        DeviceType? DeviceType { get; }

        /// <summary>
        /// 
        /// </summary>
        string Serial { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IBluetoothDevice : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        object Native { get; }

        /// <summary>
        /// Id устройства (то же что и MAC)
        /// </summary>
        Guid Id { get; }
 
        /// <summary>
        /// MAC-адрес устройства
        /// </summary>
        string Mac { get; }

        /// <summary>
        /// Имя устройства
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Физическое подключение к устройству
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Получить GATT сервисы
        /// </summary>
        /// <returns></returns>
        Task<List<IGattService>> GetServicesAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IGattService
    {
        Guid Id { get; }
        Task<List<IGattCharacteristic>> GetCharacteristicsAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValueUpdatedEventArgs : EventArgs
    {
        public IGattCharacteristic Characteristic { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IGattCharacteristic
    {
        /// <summary>
        /// Значение обновилось
        /// </summary>
        event EventHandler<ValueUpdatedEventArgs> ValueUpdated;

        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Пользовательское имя (для отладки)
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Текущее значение
        /// </summary>
        byte[] Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task StartUpdatesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        Task WriteAsync(byte[] bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IBluetooth : IDisposable, IBluetoothHelper
    {
        event EventHandler<BluetoothStateChangedEventArgs> BluetoothStateChanged;
        event EventHandler<ScanResultsEventArgs> ScanResults;
        event EventHandler<EventArgs> ScanTimeout;
        event EventHandler<EventArgs> DeviceConnectionLost;

        /// <summary>
        /// Смартфон поддерживает Bluetoth Low Energy?
        /// </summary>
        bool IsLeSupported { get; }

        /// <summary>
        /// 
        /// </summary>
        Task<bool> TurnOn(bool force = true);

        /// <summary>
        /// Начать поиск
        /// </summary>
        void StartLeScan();

        /// <summary>
        /// Остановить поиск
        /// </summary>
        void StopLeScan();


        /// <summary>
        /// Подключиться к устройству
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task ConnectToDeviceAsync(IBluetoothDevice device);

        /// <summary>
        /// Подключиться к устройству по id (по MACу)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IBluetoothDevice> ConnectToDeviceAsync(Guid id);

        /// <summary>
        /// Отключиться от подключенного устройства
        /// </summary>
        /// <returns></returns>
        Task DisconnectFromDeviceAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DisconnectFromDeviceAsync(Guid id);
    }
}
