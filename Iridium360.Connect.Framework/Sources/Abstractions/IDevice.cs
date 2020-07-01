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


    public interface IDevice
    {
        event EventHandler<ParameterChangedEventArgs> ParameterChanged;
        event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged;
        event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated;
        event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated;
        event EventHandler<LocationUpdatedEventArgs> LocationUpdated;
        event EventHandler<EventArgs> DeviceInfoUpdated;

        Location Location { get; }
        List<DeviceParameter> Parameters { get; }
        string Firmware { get; }
        //string Hardware { get; }
        uint? Battery { get; }


        DeviceState State { get; }
        LockState LockStatus { get; }
        bool? IncorrectPin { get; }


        Task SaveDeviceParameter(Parameter parameter, Enum value);
        Task UpdateAllParameters();
        Task UpdateParameters(List<Parameter> ids);


        Task Unlock(short pin);


        Task Beep();
        Task FactoryReset();
        Task RequestAlert();
        void RequestBattery();
        Task RequestNewLocation();
        Task<Location> UpdateLocationFromDevice();
        Task RequestMailboxCheck();
    }

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
