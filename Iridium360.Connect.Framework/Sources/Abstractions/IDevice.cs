using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework
{
    public class LockStatusUpdatedEventArgs : EventArgs
    {
        public LockState Old { get; set; }
        public LockState New { get; set; }
        public bool? IncorrectPin { get; set; }
    }

    public class BatteryUpdatedEventArgs : EventArgs
    {
        public uint? Value { get; set; }
    }

    public class DeviceConnectionChangedEventArgs : EventArgs
    {
        public IDevice ConnectedDevice { get; set; }
        public DeviceState State { get; set; }

    }

    public class ParameterChangedEventArgs : EventArgs
    {
        public DeviceParameter Parameter { get; set; }
    }


    public class LocationUpdatedEventArgs : EventArgs
    {
        public Location Location { get; set; }
    }



    /// <summary>
    /// Тип устройства
    /// </summary>
    public enum DeviceType : int
    {
        RockStar = 0,
        RockFleet = 1,
        RockAir = 2
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IDevice
    {
        event EventHandler<ParameterChangedEventArgs> ParameterChanged;
        event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged;
        event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated;
        event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated;
        event EventHandler<LocationUpdatedEventArgs> LocationUpdated;
        event EventHandler<EventArgs> DeviceInfoUpdated;


        /// <summary>
        /// Id блютуз устройства (на Android то статический MAC-адрес, на iOS - рандомный guid (в пределах жизни приложения?))
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Имя блютуз устройства
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        string Serial { get; }

        /// <summary>
        /// 
        /// </summary>
        DeviceType? DeviceType { get; }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        Location Location { get; }

        /// <summary>
        /// 
        /// </summary>
        List<DeviceParameter> Parameters { get; }

        /// <summary>
        /// 
        /// </summary>
        string Firmware { get; }

        /// <summary>
        /// 
        /// </summary>
        uint? Battery { get; }

        /// <summary>
        /// 
        /// </summary>
        DeviceState State { get; }

        /// <summary>
        /// 
        /// </summary>
        LockState LockStatus { get; }

        /// <summary>
        /// 
        /// </summary>
        bool? IncorrectPin { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SaveDeviceParameter(Parameter parameter, Enum value);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task UpdateAllParameters();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task UpdateParameters(List<Parameter> ids);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        Task Unlock(short pin);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task Beep();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task FactoryReset();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task RequestAlert();

        /// <summary>
        /// 
        /// </summary>
        void RequestBattery();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task RequestNewLocation();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<Location> UpdateLocationFromDevice();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task RequestMailboxCheck();
    }


    /// <summary>
    /// 
    /// </summary>
    public enum DeviceState
    {
        [Translation("$=device_disconnected$$")]
        Disconnected,

        [Translation("$=device_connecting$$")]
        Connecting,

        [Translation("$=device_connected$$")]
        Connected
    }
}
