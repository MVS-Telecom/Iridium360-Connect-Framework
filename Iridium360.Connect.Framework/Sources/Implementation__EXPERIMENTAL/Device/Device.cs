using Iridium360.Connect.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class Device : IDevice
    {
        public event EventHandler<ParameterChangedEventArgs> ParameterChanged = delegate { };
        public event EventHandler<DeviceConnectionChangedEventArgs> ConnectionChanged = delegate { };
        public event EventHandler<LockStatusUpdatedEventArgs> DeviceLockStatusUpdated = delegate { };
        public event EventHandler<BatteryUpdatedEventArgs> BatteryUpdated = delegate { };
        public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };
        public event EventHandler<EventArgs> DeviceInfoUpdated = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public Guid Id => framework.deviceMac;


        /// <summary>
        /// Серийный номер
        /// </summary>
        public string Serial => Helpers.RockstarHelper.GetSerialFromName(name);

        /// <summary>
        /// 
        /// </summary>
        public DeviceType? DeviceType => Helpers.RockstarHelper.GetTypeByName(name);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="capability"></param>
        /// <returns></returns>
        public bool IsSupported(DeviceCapability capability)
        {
            return DeviceHelper.IsCapabilitySupported(Firmware, capability);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsYB()
        {
            return DeviceHelper.IsYB(Firmware);
        }


        private Location location;

        /// <summary>
        /// Координаты устройства
        /// </summary>
        public Location Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                LocationUpdated(this, new LocationUpdatedEventArgs()
                {
                    Location = location
                });
            }
        }



        public List<IDeviceParameter> Parameters { get; private set; } = new List<IDeviceParameter>();


        private uint? battery;

        /// <summary>
        /// Заряд батарейки
        /// </summary>
        public uint? Battery
        {
            get
            {
                return battery;
            }
            internal set
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



        private LockState lockStatus = LockState.Unknown;

        /// <summary>
        /// 
        /// </summary>
        public bool? IncorrectPin { get; private set; }

        /// <summary>
        /// Статус залоченности устройства
        /// </summary>
        public LockState LockStatus
        {
            get
            {
                return lockStatus;
            }
        }



        internal void SetLockStatus(LockState value, bool? incorrectPin = null)
        {
            IncorrectPin = incorrectPin;

            if (lockStatus != value || incorrectPin == true)
            {
                var old = lockStatus;
                lockStatus = value;

                ConsoleLogger.WriteLine($"[LOCK-STATE] {old} -> {lockStatus}");

                DeviceLockStatusUpdated(this, new LockStatusUpdatedEventArgs()
                {
                    IncorrectPin = incorrectPin,
                    Old = old,
                    New = value
                });
            }
        }



        private DeviceState state = DeviceState.Disconnected;

        /// <summary>
        /// Состояние подключения телефона к устройству
        /// </summary>
        internal void SetState(DeviceState state)
        {
            if (this.state != state)
            {
                this.state = state;

                ConnectionChanged(this, new DeviceConnectionChangedEventArgs()
                {
                    ConnectedDevice = this,
                    State = state
                });


                if (state != DeviceState.Connected)
                {
                    Battery = null;
                }

                if (state == DeviceState.Disconnected)
                {
                    SetLockStatus(LockState.Unknown);

                    ///Сбрасываем некоторые параметры устройства при отключении
                    Parameters
                        .Where(x => !x.IsPersisted)
                        .ToList()
                        .ForEach(x => OnParameterUpdated(x.GattId, null));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DeviceState State
        {
            get
            {
                return state;
            }
        }


        /// <summary>
        /// Устройство подключено?
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return state == DeviceState.Connected;
            }
        }


        /// <summary>
        /// Версия прошивки
        /// </summary>
        public virtual string Firmware
        {
            get
            {
                return firmware;
            }
            internal set
            {
                firmware = value;
                OnFirmwareChanged(value);
                DeviceInfoUpdated(this, new EventArgs());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Hardware
        {
            get
            {
                return hardware;
            }
            internal set
            {
                hardware = value;
                DeviceInfoUpdated(this, new EventArgs());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            internal set
            {
                name = value;
                DeviceInfoUpdated(this, new EventArgs());
            }
        }




        protected FrameworkInstance__EXPERIMENTAL framework;
        protected string AppId => framework.AppId;
        protected short Key => framework.KeyIndex;



        private string name;
        private string firmware;
        private string hardware;
        private DeviceAccessory accessory;


        public Device(FrameworkInstance__EXPERIMENTAL framework)
        {
            this.framework = framework;
        }


        private void OnFirmwareChanged(string firmware)
        {
            __DeviceParameter deviceParameter = new __DeviceParameter(framework, this, Parameter.GpsStatus);
            Parameters.Add(deviceParameter);

            __DeviceParameter deviceParameter2 = new __DeviceParameter(framework, this, Parameter.IridiumStatus);
            Parameters.Add(deviceParameter2);

            __DeviceParameter deviceParameter3 = new __DeviceParameter(framework, this, Parameter.PowerStatus);
            Parameters.Add(deviceParameter3);

            if (this.IsSupported(DeviceCapability.ActivitySense))
            {
                __DeviceParameter deviceParameter4 = new __DeviceParameter(framework, this, Parameter.TrackingActivitySenseStatus);
                Parameters.Add(deviceParameter4);
                __DeviceParameter deviceParameter5 = new __DeviceParameter(framework, this, Parameter.TrackingActivitySenseThreshold);
                Parameters.Add(deviceParameter5);
            }
            else
            {
                __DeviceParameter deviceParameter6 = new __DeviceParameter(framework, this, Parameter.TrackingActivitySenseStatus);
                Parameters.Add(deviceParameter6);
                __DeviceParameter deviceParameter7 = new __DeviceParameter(framework, this, Parameter.TrackingActivitySenseLowThreshold);
                Parameters.Add(deviceParameter7);
                __DeviceParameter deviceParameter8 = new __DeviceParameter(framework, this, Parameter.TrackingActivitySenseHighThreshold);
                Parameters.Add(deviceParameter8);
            }
            __DeviceParameter deviceParameter9 = new __DeviceParameter(framework, this, Parameter.TrackingBurstFixPeriod);
            if (!this.IsSupported(DeviceCapability.RevisedFrequency))
            {
                deviceParameter9.removeValueOption(TrackingBurstFixPeriod.Period20min);
            }
            Parameters.Add(deviceParameter9);
            __DeviceParameter deviceParameter10 = new __DeviceParameter(framework, this, Parameter.TrackingBurstTransmitPeriod);
            Parameters.Add(deviceParameter10);
            __DeviceParameter deviceParameter11 = new __DeviceParameter(framework, this, Parameter.TrackingStatus);
            Parameters.Add(deviceParameter11);
            __DeviceParameter deviceParameter12 = new __DeviceParameter(framework, this, Parameter.TrackingFrequency);

#if RELEASE
            //DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeContextualReporting) ? context.getText(R.string.satellite_frequency).toString() : context.getText(R.string.frequency).toString());
#endif

            if (!this.IsSupported(DeviceCapability.FastCellular))
            {
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency15sec);
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency30sec);
            }
            if (!this.IsSupported(DeviceCapability.RevisedFrequency))
            {
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency1min);
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency2min);
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency3min);
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency4min);
            }
            if (!this.IsSupported(DeviceCapability.ContextualReporting))
            {
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency6min);
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency8min);
            }
            if (!this.IsSupported(DeviceCapability.ContextualReporting))
            {
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency12min);
            }
            if (!this.IsSupported(DeviceCapability.RevisedFrequency))
            {
                deviceParameter12.removeValueOption(TrackingFrequency.Frequency1440min);
            }

            Parameters.Add(deviceParameter12);
            if (this.IsSupported(DeviceCapability.ContextualReporting))
            {
                __DeviceParameter deviceParameter13 = new __DeviceParameter(framework, this, Parameter.DistressFrequency);
                if (!this.IsSupported(DeviceCapability.FastCellular))
                {
                    deviceParameter13.removeValueOption(DistressFrequency.DistressFrequency15sec);
                    deviceParameter13.removeValueOption(DistressFrequency.DistressFrequency30sec);
                }
                Parameters.Add(deviceParameter13);
                __DeviceParameter deviceParameter14 = new __DeviceParameter(framework, this, Parameter.DistressBurstFixPeriod);
                Parameters.Add(deviceParameter14);
                __DeviceParameter deviceParameter15 = new __DeviceParameter(framework, this, Parameter.DistressBurstTransmitPeriod);
                Parameters.Add(deviceParameter15);

                if (framework.IsGprsAttached)
                {
                    __DeviceParameter deviceParameter16 = new __DeviceParameter(framework, this, Parameter.CellularFrequency);
                    if (!this.IsSupported(DeviceCapability.FastCellular))
                    {
                        deviceParameter16.removeValueOption(CellularFrequency.CellularFrequency15sec);
                        deviceParameter16.removeValueOption(CellularFrequency.CellularFrequency30sec);
                    }
                    Parameters.Add(deviceParameter16);
                    __DeviceParameter deviceParameter17 = new __DeviceParameter(framework, this, Parameter.CellularBurstFixPeriod);
                    Parameters.Add(deviceParameter17);
                    __DeviceParameter deviceParameter18 = new __DeviceParameter(framework, this, Parameter.CellularBurstTransmitPeriod);
                    Parameters.Add(deviceParameter18);
                }
            }
            if (this.IsSupported(DeviceCapability.Notify))
            {
                __DeviceParameter deviceParameter19 = new __DeviceParameter(framework, this, Parameter.Notify);
                if (!this.IsYB())
                {
                    deviceParameter19.removeValueOption(AlertsNotify.AlertsNotifyVisual);
                    deviceParameter19.removeValueOption(AlertsNotify.AlertsNotifyBoth);
                }
                Parameters.Add(deviceParameter19);
            }
            __DeviceParameter deviceParameter20 = new __DeviceParameter(framework, this, Parameter.AlertsCollisionDuration);
            Parameters.Add(deviceParameter20);
            __DeviceParameter deviceParameter21 = new __DeviceParameter(framework, this, Parameter.AlertsCollisionStatus);
            Parameters.Add(deviceParameter21);
            __DeviceParameter deviceParameter22 = new __DeviceParameter(framework, this, Parameter.AlertsCollisionThreshold);
            Parameters.Add(deviceParameter22);
            __DeviceParameter deviceParameter23 = new __DeviceParameter(framework, this, Parameter.AlertsGeofenceDistance);
            Parameters.Add(deviceParameter23);
            __DeviceParameter deviceParameter24 = new __DeviceParameter(framework, this, Parameter.AlertsGeofenceCentre);
            Parameters.Add(deviceParameter24);
            __DeviceParameter deviceParameter25 = new __DeviceParameter(framework, this, Parameter.AlertsGeofenceCheckFrequency);
            Parameters.Add(deviceParameter25);
            __DeviceParameter deviceParameter26 = new __DeviceParameter(framework, this, Parameter.AlertsGeofenceStatus);
            if (!this.IsSupported(DeviceCapability.Polyfence))
            {
                deviceParameter26.removeValueOption(AlertsGeofenceStatus.AlertsGeofenceStatusOnPolyfence);
            }

            Parameters.Add(deviceParameter26);
            __DeviceParameter deviceParameter27 = new __DeviceParameter(framework, this, Parameter.AlertsPowerLossStatus);
            Parameters.Add(deviceParameter27);
            __DeviceParameter deviceParameter28 = new __DeviceParameter(framework, this, Parameter.AlertsTemperatureCheckFrequency);
            Parameters.Add(deviceParameter28);
            __DeviceParameter deviceParameter29 = new __DeviceParameter(framework, this, Parameter.AlertsHotTemperature);
            Parameters.Add(deviceParameter29);
            __DeviceParameter deviceParameter30 = new __DeviceParameter(framework, this, Parameter.AlertsColdTemperature);
            Parameters.Add(deviceParameter30);
            __DeviceParameter deviceParameter31 = new __DeviceParameter(framework, this, Parameter.AlertsTemperatureStatus);
            Parameters.Add(deviceParameter31);

            if (this.IsSupported(DeviceCapability.ExternalPowerAvailability))
            {
                __DeviceParameter deviceParameter32 = new __DeviceParameter(framework, this, Parameter.ExternalPowerAvailability);
                if (!this.IsSupported(DeviceCapability.ActivationMode))
                {
                    deviceParameter32.removeValueOption(ExternalPowerAvailiability.ExternalPowerAvailiabilityUnlimitedActivate);
                }
                Parameters.Add(deviceParameter32);
            }

            //#if RELEASE
            //            if (!context.getPackageName().contains("iridium360"))
            //            {
            //                if (DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeMobWatcher))
            //                {
            //                    __DeviceParameter deviceParameter33 = new __DeviceParameter(Parameter.ExternalMobWatcher);
            //                    parameters.Add(deviceParameter33);
            //                    this.f497h.Add(deviceParameter33);
            //                }
            //                __DeviceParameter deviceParameter34 = new __DeviceParameter(Parameter.ExternalStatus);

            //                if (!DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeMaximet))
            //                {
            //                    deviceParameter34.removeValueOption(ExternalStatus.ExternalStatusMaximet);
            //                }
            //                if (!DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeMaximetGmx200))
            //                {
            //                    deviceParameter34.removeValueOption(ExternalStatus.ExternalStatusMaximetGmx200);
            //                }
            //                if (!DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeWave))
            //                {
            //                    deviceParameter34.removeValueOption(ExternalStatus.ExternalStatusWave);
            //                }
            //                if (!DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeVWTP3))
            //                {
            //                    deviceParameter34.removeValueOption(ExternalStatus.ExternalStatusVWTP3);
            //                }
            //                if (!DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeDaliaFPSO))
            //                {
            //                    deviceParameter34.removeValueOption(DeviceCapability.DeviceCapabilityTypeDaliaFPSO);
            //                }
            //                parameters.Add(deviceParameter34);
            //                this.f497h.Add(deviceParameter34);
            //                __DeviceParameter deviceParameter35 = new __DeviceParameter(Parameter.ExternalSampleRate);
            //                parameters.Add(deviceParameter35);
            //                this.f497h.Add(deviceParameter35);
            //                __DeviceParameter deviceParameter36 = new __DeviceParameter(Parameter.ExternalBaudRate);
            //                parameters.Add(deviceParameter36);
            //                this.f497h.Add(deviceParameter36);
            //                if (DeviceHelper.IsCapabilitySupported(this, DeviceCapability.DeviceCapabilityTypeInputSensitivity))
            //                {
            //                    __DeviceParameter deviceParameter37 = new __DeviceParameter(Parameter.InputSensitivity);
            //                    parameters.Add(deviceParameter37);
            //                    this.f497h.Add(deviceParameter37);
            //                }
            //            }
            //#endif

            if (this.IsSupported(DeviceCapability.Gprs) && framework.IsGprsAttached)
            {
                __DeviceParameter deviceParameter38 = new __DeviceParameter(framework, this, Parameter.GprsStrategy);
                Parameters.Add(deviceParameter38);
                __DeviceParameter deviceParameter39 = new __DeviceParameter(framework, this, Parameter.GprsMsisdn);
                Parameters.Add(deviceParameter39);
                __DeviceParameter deviceParameter40 = new __DeviceParameter(framework, this, Parameter.GprsSim);
                Parameters.Add(deviceParameter40);
                __DeviceParameter deviceParameter41 = new __DeviceParameter(framework, this, Parameter.GprsSignal);
                Parameters.Add(deviceParameter41);
                __DeviceParameter deviceParameter42 = new __DeviceParameter(framework, this, Parameter.GprsStatus);
                Parameters.Add(deviceParameter42);
                __DeviceParameter deviceParameter43 = new __DeviceParameter(framework, this, Parameter.GprsLocation);
                Parameters.Add(deviceParameter43);
            }

            __DeviceParameter deviceParameter44 = new __DeviceParameter(framework, this, Parameter.GPSEarlyWakeup);
            Parameters.Add(deviceParameter44);
            __DeviceParameter deviceParameter45 = new __DeviceParameter(framework, this, Parameter.GPSMode);
            Parameters.Add(deviceParameter45);
            __DeviceParameter deviceParameter46 = new __DeviceParameter(framework, this, Parameter.GPSFixesBeforeAccept);
            Parameters.Add(deviceParameter46);
            __DeviceParameter deviceParameter47 = new __DeviceParameter(framework, this, Parameter.GPSHotStatus);
            Parameters.Add(deviceParameter47);
            __DeviceParameter deviceParameter48 = new __DeviceParameter(framework, this, Parameter.GpxLoggingPeriod);
            Parameters.Add(deviceParameter48);
            __DeviceParameter deviceParameter49 = new __DeviceParameter(framework, this, Parameter.GpxLoggingStatus);
            Parameters.Add(deviceParameter49);
            __DeviceParameter deviceParameter50 = new __DeviceParameter(framework, this, Parameter.MailboxCheckFrequency);
            Parameters.Add(deviceParameter50);
            __DeviceParameter deviceParameter51 = new __DeviceParameter(framework, this, Parameter.MailboxCheckStatus);
            Parameters.Add(deviceParameter51);

            if (framework.IsGprsAttached && this.IsSupported(DeviceCapability.GprsConfig))
            {
                List<DeviceAccessoryParameter> a = new List<DeviceAccessoryParameter>()
                {
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterApnName),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterApnUsername),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterApnPassword),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointAddress1),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointPort1),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointAddress2),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointPort2),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointAddress3),
                    new DeviceAccessoryParameter(framework, GprsParameter.GprsParameterEndpointPort3),
                };

                this.accessory = new DeviceAccessory(a);
            }

            if (this.IsSupported(DeviceCapability.TransmissionFormat))
            {
                __DeviceParameter deviceParameter52 = new __DeviceParameter(framework, this, Parameter.TransmissionFormat);
                Parameters.Add(deviceParameter52);
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterId"></param>
        /// <returns></returns>
        public async Task UpdateParameter(Parameter parameterId)
        {
            IDeviceParameter parameter = GetParameter(parameterId);

            if (parameter == null)
                throw new NullReferenceException($"Unknown device parameter `{parameterId}`");

            if (!parameter.IsAvailable)
                throw new ParameterUnavailableException(parameterId);

            await this.framework.ReadDeviceParameter(parameter.GattId, checkSuccess: true);

            ConsoleLogger.WriteLine($"Read finish -> [{parameterId}]");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task UpdateParameters(List<Parameter> parameters)
        {
            foreach (var p in parameters)
                await UpdateParameter(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task UpdateAllParameters()
        {
            foreach (var p in this.Parameters.Select(x => x.Id))
                await UpdateParameter(p);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveDeviceParameter(Parameter parameterId, Enum value)
        {
            var parameter = GetParameter(parameterId);

            if (parameter == null)
                throw new NullReferenceException("Unknown device parameter");

            if (!parameter.IsAvailable)
                throw new InvalidOperationException("This parameter is not available (GATT characterictic wasn't found)");

            int[] bytes = { Convert.ToInt32(value) };


            ///Отправляем значение на устройство
            ///
            await this.framework.WriteDeviceParameter(
                parameter.GattId,
                bytes,
                ///ЭТО ВАЖНО! Убеждаемся, что новое значение успешно принято устройством
                checkSuccess: true);


            ///Обновляем кэшированное значение, если новое значение успешно сохранилось 
            ((BaseDeviceParameter)parameter).UpdateCachedValue(bytes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IDeviceParameter GetParameter(Parameter id)
        {
            return this.Parameters.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatt"></param>
        /// <returns></returns>
        internal IDeviceParameter GetParameter(Guid gatt)
        {
            return this.Parameters.FirstOrDefault(x => x.GattId == gatt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifiers"></param>
        internal void Discovered(List<Guid> identifiers)
        {
            this.Parameters.ForEach(p =>
            {
                ((__DeviceParameter)p).IsAvailable = identifiers.Contains(p.GattId);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gatt"></param>
        /// <param name="data"></param>
        internal void OnParameterUpdated(Guid gatt, int[] data)
        {
            IDeviceParameter parameter = GetParameter(gatt);

            if (parameter == null)
                throw new NullReferenceException("Unknown device parameter");

            try
            {
                bool changed = ((BaseDeviceParameter)parameter).UpdateCachedValue(data);

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
        /// <returns></returns>
        internal List<Guid> GetParameterCharacteristics()
        {
            return Parameters
                .Select(x => x.GattId)
                .ToList();
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
        /// <inheritdoc/>
        /// </summary>
        public Task RequestBattery()
        {
            return framework.RequestBattery();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Task RequestNewLocation()
        {
            return framework.RequestNewLocation();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public Task<Location> UpdateLocationFromDevice()
        {
            return framework.UpdateLocationFromDevice();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task RequestMailboxCheck()
        {
            return framework.RequestMailboxCheck();
        }
    }
}
