#if ANDROID || IPHONE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iridium360.Connect.Framework;
using Iridium360.Connect.Framework.Exceptions;
using Iridium360.Connect.Framework.Helpers;

#if IOS
using Foundation;
using ConnectFramework;
#else
using UK.Rock7.Connect.Device;
#endif

namespace ConnectFramework.Shared
{
    internal class R7Device : IDevice
    {
        public event EventHandler<ParameterChangedEventArgs> ParameterChanged = delegate { };
        public event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged = delegate { };
        public event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated = delegate { };
        public event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated = delegate { };
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
        public event EventHandler<EventArgs> DeviceInfoUpdated = delegate { };


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Guid Id => framework.deviceId;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name => source?.Name;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Serial => RockstarHelper.GetSerialFromName(Name);


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DeviceType? DeviceType => RockstarHelper.GetTypeByName(Name);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="location"></param>
        internal void SetLocation(Location location)
        {
            Location = location;

            LocationUpdated(this, new LocationUpdatedEventArgs()
            {
                Location = location
            });
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public List<Iridium360.Connect.Framework.DeviceParameter> Parameters { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Firmware => source?.Version;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Hardware => null;

        private LockState lockStatus = LockState.Unknown;

        /// <summary>
        /// 
        /// </summary>
        public bool? IncorrectPin { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        public LockState LockStatus
        {
            get
            {
                return lockStatus;
            }
        }

        internal void SetLockStatus(LockState state, bool? incorrectPin = null)
        {
            var @new = state;
            var @old = lockStatus;

            this.IncorrectPin = incorrectPin;

            //if (@old != @new || incorrectPin == true)
            {
                lockStatus = @new;

                DeviceLockStatusUpdated(this, new LockStatusUpdatedEventArgs()
                {
                    IncorrectPin = incorrectPin,
                    New = @new,
                    Old = @old
                });
            }
        }

        private uint? battery = null;

        public uint? Battery
        {
            get
            {
                return battery;
            }
            set
            {
                ///HACK: RockFLEET всегда возвращает 0. В нативном приложении батарейка для него вообще не отображается
                if (value == 0)
                    value = null;

                if (battery != value)
                {
                    battery = value;
                    BatteryUpdated(this, new BatteryUpdatedEventArgs()
                    {
                        Value = battery
                    });
                }

            }
        }

        private DeviceState state;
        private R7ConnectFramework framework;
        private ConnectDevice source => framework.comms.CurrentDevice;


        public R7Device(R7ConnectFramework framework)
        {
            this.framework = framework;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Task Beep()
        {
            return framework.Beep();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Task FactoryReset()
        {
            return framework.FactoryReset();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestAlert()
        {
            return framework.RequestAlert();
        }

        /// <summary>
        /// 
        /// </summary>
        public DeviceState State
        {
            get
            {
#if DEBUG && ANDROID
                bool a = state == DeviceState.Connected;
                bool b = framework.comms.IsConnected().BooleanValue();

                if (a != b)
                    Console.WriteLine("!! STATE != STATE !!!");
#endif

                return state;
            }
        }

        internal void SetState(DeviceState state)
        {
            if (this.state != state)
            {
                this.state = state;

                if (state == DeviceState.Connected)
                {
                    Parameters = source.Parameters()
                        .Select(x =>
                        {
                            try
                            {
                                var id = x.Identifier.ToR7().FromR7();
                                return new Iridium360.Connect.Framework.DeviceParameter(framework, this, id);
                            }
                            catch
                            {
                                return null;
                            }
                        })
                        .Where(x => x != null)
                        .ToList();

                    DeviceInfoUpdated(this, new EventArgs());
                }
                else
                {
                    Parameters = null;
                    SetLockStatus(LockState.Unknown);
                }


                ConnectionChanged(this, new DeviceConnectionChangedEventArgs()
                {
                    ConnectedDevice = this,
                    State = this.state,
                });
            }
        }



        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void RequestBattery()
        {
            framework.RequestBattery();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Task RequestMailboxCheck()
        {
            return framework.RequestMailboxCheck();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task Reconnect()
        {
            return framework.Reconnect(throwOnError: true);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task UpdateAllParameters()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestNewLocation()
        {
            return framework.RequestNewLocation();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
#if ANDROID
        public void OnParameterChanged(UK.Rock7.Connect.Device.DeviceParameter p)
#elif IOS
        public void OnParameterChanged(DeviceParameter p)
#endif
        {
            var id = p.Identifier.ToR7().FromR7().EnumToInt();
            var parameter = Parameters.SingleOrDefault(x => (int)x.Id == (int)id);

            if (parameter == null)
                return;

            try
            {
                bool changed = parameter.UpdateCachedValue(new byte[] { (byte)p.CachedValue });

                if (changed)
                {
                    ParameterChanged(this, new ParameterChangedEventArgs()
                    {
                        Parameter = parameter
                    });
                }

            }
            catch (DeviceIsLockedException e)
            {
                SetLockStatus(LockState.Locked);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SaveDeviceParameter(Parameter parameter, Enum value)
        {
            return framework.SaveDeviceParameter(parameter, value);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task UpdateParameters(List<Parameter> ids)
        {
            return framework.UpdateParameters(ids);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        public Task Unlock(short pin)
        {
            return framework.Unlock(pin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Location> UpdateLocationFromDevice()
        {
            return framework.UpdateLocationFromDevice();
        }
    }


}

#endif
