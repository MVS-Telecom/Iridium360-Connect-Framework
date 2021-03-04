#if ANDROID || IPHONE

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Iridium360.Connect.Framework;
using Iridium360.Connect.Framework.Helpers;
using System.Diagnostics;

#if IOS
using Foundation;
#else
using UK.Rock7.Connect.Device.R7generic;
#endif


namespace ConnectFramework.Shared
{
    internal static class Extensions
    {
#if IOS
        public static R7GenericDeviceParameter ToR7(this nuint parameter)
        {
            return (R7GenericDeviceParameter)(int)parameter;
        }
#endif

        public static R7GenericDeviceParameter ToR7(this int parameter)
        {
#if ANDROID
            return R7GenericDeviceParameter.Values().SingleOrDefault(x => x.Ordinal() == parameter);
#elif IOS
            return (R7GenericDeviceParameter)parameter;
#endif
        }

        public static R7GenericDeviceParameter ToR7(this Parameter parameter)
        {
            try
            {
#if ANDROID
                return R7GenericDeviceParameter.Values().SingleOrDefault(x => x.Ordinal() == (int)parameter);
#elif IOS
                return (R7GenericDeviceParameter)Enum.Parse(typeof(R7GenericDeviceParameter), parameter.ToString());
#endif
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw;
            }
        }

        public static Parameter FromR7(this R7GenericDeviceParameter parameter)
        {
            try
            {
#if ANDROID
                if (!Enum.IsDefined(typeof(Parameter), (int)parameter.Ordinal()))
                    throw new Exception($"Specified value {parameter.Ordinal()} not defined in enum `{nameof(Parameter)}`");

                return (Parameter)(int)parameter.Ordinal();
#elif IOS
                return (Parameter)Enum.Parse(typeof(Parameter), parameter.ToString(), ignoreCase: true);
#endif
            }
            catch (Exception e)
            {
                ///Не нашелся параметр в "нашем" энаме
                throw;
            }
        }


#if ANDROID
        public static int EnumToInt(this Java.Lang.Enum value)
        {
            return value.Ordinal();
        }
        public static int EnumToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
#elif IOS
        public static nuint EnumToInt(this Enum value)
        {
            return (nuint)Convert.ToInt32(value);
        }
#endif
    }


    internal class R7BluetoothDevice : IFoundDevice
    {
        public object Native => throw new NotSupportedException();
        public Guid Id { get; private set; }
        public string Mac { get; private set; }
        public string Name { get; private set; }
        public bool IsConnected => throw new NotSupportedException();
        public DeviceType? DeviceType => RockstarHelper.GetTypeByName(Name);
        public string Serial => RockstarHelper.GetSerialFromName(Name);
        public string Source { get; private set; }


        public R7BluetoothDevice(string id, string name, string source)
        {
#if ANDROID
            this.Id = Guid.Parse($"00000000-0000-0000-0000-{id.Replace(":", "")}");
            this.Mac = id;
#elif IOS
            if (id.Length != Guid.Empty.ToString().Length)
                id = $"00000000-0000-0000-0000-{id.Replace(":", "")}";

            this.Id = Guid.Parse(id);
            this.Mac = null;
#endif
            this.Name = name;
            this.Source = source;
        }


        public void Dispose()
        {

        }

        public Task<List<IGattService>> GetServicesAsync()
        {
            throw new NotSupportedException();
        }
    }


#if IOS
    /// <summary>
    /// 
    /// </summary>
    internal class R : R7DeviceResponseDelegate
    {
        private R7ConnectFramework framework;

        public R(R7ConnectFramework framework)
        {
            this.framework = framework;
        }

        public override void DeviceConnected(ConnectDevice device, R7ActivationState activated, bool locked)
        {
            this.framework.DeviceConnected(device, activated, locked);
        }

        public override void DeviceDisconnected()
        {
            this.framework.DeviceDisconnected();
        }

        public override void DeviceReady()
        {
            this.framework.DeviceReady();
        }

        public override void DeviceLockStatusUpdated(R7LockState state)
        {
            this.framework.DeviceLockStatusUpdated(state);
        }

        public override void DeviceParameterUpdated(DeviceParameter parameter)
        {
            this.framework.DeviceParameterUpdated(parameter);
        }

        public override void DeviceBatteryUpdated(nuint battery, NSDate timestamp)
        {
            this.framework.DeviceBatteryUpdated(battery, timestamp);
        }

        public override void DeviceError(R7DeviceError error)
        {
            this.framework.DeviceError(error);
        }

        public override void DeviceStateChanged(R7ConnectionState stateTo, R7ConnectionState stateFrom)
        {
            this.framework.DeviceStateChanged(stateTo, stateFrom);
        }

        public override void LocationUpdated(CoreLocation.CLLocation location)
        {
            this.framework.LocationUpdated(location);
        }

        public override void DeviceCommandReceived(R7CommandType command)
        {
            this.framework.DeviceCommandReceived(command);
        }


        public override void DeviceStatusUpdated(nuint field, nuint value)
        {
            this.framework.DeviceStatusUpdated(field, value);
        }
    }



    internal class M : R7DeviceMessagingDelegate
    {
        private R7ConnectFramework framework;

        public M(R7ConnectFramework framework)
        {
            this.framework = framework;
        }

        public override void InboxUpdated(nuint messages)
        {
            this.framework.InboxUpdated(messages);
        }

        public override void MessageProgressCompleted(nuint messageId)
        {
            this.framework.MessageProgressCompleted(messageId);
        }

        public override bool MessageReceived(ushort messageId, NSData data)
        {
            return this.framework.MessageReceived(messageId, data);
        }

        public override bool MessageStatusUpdated(ushort messageId, R7MessageStatus status)
        {
            return this.framework.MessageStatusUpdated(messageId, status);
        }

        public override void MessageProgressUpdated(nuint messageId, nuint part, nint totalParts)
        {
            this.framework.MessageProgressUpdated(messageId, part, totalParts);
        }
    }

    internal class D : R7DeviceDiscoveryDelegate
    {
        private R7ConnectFramework framework;

        public D(R7ConnectFramework framework)
        {
            this.framework = framework;
        }

        public override void DiscoveryStarted()
        {
            this.framework.DiscoveryStarted();
        }

        public override void DiscoveryFoundDevice(NSUuid deviceIdentifier, string deviceName)
        {
            this.framework.DiscoveryFoundDevice(deviceIdentifier, deviceName);
        }

        public override void DiscoveryStopped()
        {
            this.framework.DiscoveryStopped();
        }

        public override void DiscoveryUpdatedDevice(NSUuid deviceIdentifier, string deviceName, NSNumber rssi)
        {
            this.framework.DiscoveryUpdatedDevice(deviceIdentifier, deviceName, rssi);
        }
    }

#endif
}


#endif
