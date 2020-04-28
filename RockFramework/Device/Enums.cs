using System;
using System.Collections.Generic;
using System.Text;

namespace Rock
{
    public class GattCharacteristicAttribute : Attribute
    {
        public string Value { get; set; }

        public GattCharacteristicAttribute(string uuid)
        {
            this.Value = uuid;
        }
    }

    /// <summary>
    /// Параметр для для чтения (изменять нельзя)
    /// </summary>
    public class ReadonlyAttribute : Attribute { }

    /// <summary>
    /// Описание параметра
    /// </summary>
    public class DescriptionAttribute : Attribute
    {
        public string Value { get; set; }
        public DescriptionAttribute(string description)
        {
            this.Value = description;
        }
    }


    /// <summary>
    /// Возможные значения параметра
    /// </summary>
    public class ValuesAttribute : Attribute
    {
        public Type Value { get; set; }
        public ValuesAttribute(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException("Specified type is not enum");

            this.Value = type;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GroupAttribute : Attribute
    {
        public Group Key { get; set; }
        public int Order { get; set; }

        public GroupAttribute(Group key, int order = int.MaxValue)
        {
            this.Key = key;
            this.Order = order;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class OrderAttribute : Attribute
    {
        public int Value { get; set; }
        public OrderAttribute(int value)
        {
            this.Value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValueAttribute : Attribute
    {
        public int Value { get; set; }
        public ValueAttribute(int value)
        {
            this.Value = value;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class EventAttribute : Attribute { }

    /// <summary>
    /// Значение этого параметра не нужно очищать при отключении от устройства
    /// </summary>
    public class PersistedAttribute : Attribute { }


    /// <summary>
    /// 
    /// </summary>
    public class HiddenAttribute : Attribute { }


    /// <summary>
    /// 
    /// </summary>
    public class CheckAttribute : Attribute { }

    /// <summary>
    /// 
    /// </summary>
    public class SwitchAttribute : Attribute { }

    /// <summary>
    /// 
    /// </summary>
    public class ListAttribute : Attribute { }


    /// <summary>
    /// 
    /// </summary>
    public class TranslationAttribute : Attribute
    {
        public string Key { get; set; }

        public TranslationAttribute(string key)
        {
            this.Key = key;
        }
    }


    public enum Group
    {
        Other,
        [Translation("$=group_tracking$$")]
        Tracking,

        [Translation("$=group_tracking_advanced$$")]
        TrackingAdvanced,

        [Translation("$=group_alerts$$")]
        Alerts,

        [Translation("$=group_external_accessory$$")]
        ExternalAccessory,

        [Translation("$=group_screen$$")]
        Screen,

        [Translation("$=group_mailbox$$")]
        Mailbox,
    }


    public enum DeviceCapability
    {
        InstallUpdateRestart,
        Geofence,
        FirmwareRestart,
        ChangePin,
        ExternalPower,
        RawCommands,
        SerialAPI,
        ActivitySense,
        ExternalPowerAvailability,
        ExternalPowerConfigReport,
        MobWatcher,
        Notify,
        GenericAlerts,
        Gprs,
        Maximet,
        RockBoard,
        SerialDump,
        UnlockedFirmwareUpdate,
        MessageAlignment,
        MaximetGmx200,
        InputSensitivity,
        Wave,
        Polyfence,
        FriendlyName,
        GprsConfig,
        RevisedFrequency,
        VWTP3,
        ContextualReporting,
        ActivationMode,
        TransmissionFormat,
        FastCellular,
        RevisedPositionFormat,
        DaliaFPSO
    }


    public enum Parameter
    {
        AlertsTimerStatus,
        AlertsTimerTimeout,
        AlertsBluetoothLossStatus,

        [GattCharacteristic("c0afcd64-feae-4176-9b76-5e5d74b925be")]
        [Values(typeof(AlertsCollisionDuration))]
        [Group(Group.Alerts)]
        AlertsCollisionDuration,

        [GattCharacteristic("7be1ee43-9987-4dde-8963-e98d876febb8")]
        [Values(typeof(AlertsCollisionStatus))]
        [Group(Group.Alerts)]
        AlertsCollisionStatus,

        [GattCharacteristic("61a0023b-ae58-423c-9d23-baf193cd3a7a")]
        [Values(typeof(AlertsCollisionThreshold))]
        [Group(Group.Alerts)]
        AlertsCollisionThreshold,

        AlertsDeadmanStatus,
        AlertsDeadmanTimeout,

        [GattCharacteristic("8df1fbd9-0fd6-4835-a7e9-5efc4f1db1ff")]
        [Values(typeof(AlertsGeofenceDistance))]
        [Group(Group.Alerts)]
        AlertsGeofenceDistance,

        [GattCharacteristic("73f2a956-e57c-4353-0000-000000000000")]
        [Group(Group.Alerts)]
        AlertsGeofenceCentre,

        [GattCharacteristic("fd2a51ee-1589-41c3-8694-c35c314b3fc1")]
        [Values(typeof(AlertsGeofenceCheckFrequency))]
        [Group(Group.Alerts)]
        AlertsGeofenceCheckFrequency,

        [GattCharacteristic("73f2a956-e57c-4353-b3ae-41cb65f8bdbe")]
        [Values(typeof(AlertsGeofenceStatus))]
        [Group(Group.Alerts)]
        AlertsGeofenceStatus,

        [GattCharacteristic("0bc89e74-5feb-4ab6-acbf-4e2e1b506893")]
        [Values(typeof(AlertsPowerLossStatus))]
        [Group(Group.Alerts)]
        AlertsPowerLossStatus,

        [GattCharacteristic("02206f9d-f501-41dd-b76f-7e85d3b5633c")]
        [Values(typeof(AlertsTemperatureCheckFrequency))]
        [Group(Group.Alerts)]
        AlertsTemperatureCheckFrequency,

        [GattCharacteristic("04d9e48b-80a4-45d4-bfc4-9d2b8f80a257")]
        [Values(typeof(AlertsHotTemperature))]
        [Group(Group.Alerts)]
        AlertsHotTemperature,

        [GattCharacteristic("ff86be94-0f05-429c-9e95-26c4369637b0")]
        [Values(typeof(AlertsColdTemperature))]
        [Group(Group.Alerts)]
        AlertsColdTemperature,

        [GattCharacteristic("629fc918-d6fe-4193-b5c3-cff4a8121dd2")]
        [Values(typeof(AlertsTemperatureStatus))]
        [Group(Group.Alerts)]
        AlertsTemperatureStatus,

        [GattCharacteristic("c231c3a2-dddf-4cc0-af13-f57d1e7deda4")]
        [Values(typeof(ExternalBaudRate))]
        ExternalBaudRate,

        [GattCharacteristic("694edc49-8e35-48b0-84ad-ef2a20814589")]
        [Values(typeof(ExternalStatus))]
        ExternalStatus,

        [GattCharacteristic("671ea868-389f-406f-bae9-5a5f6fd8d858")]
        [Values(typeof(ExternalSampleRate))]
        ExternalSampleRate,
        FrameworkBatteryLevel,
        FrameworkInboxCount,
        FrameworkOutboxConfirm,
        FrameworkSerialRX,
        FrameworkSerialTX,
        FrameworkLocation,

        [GattCharacteristic("58d4761a-ebcf-417d-85c9-b2ba5b4fb1d4")]
        [Values(typeof(GPSEarlyWakeup))]
        GPSEarlyWakeup,

        [GattCharacteristic("9cd89e19-3108-4cba-8b80-d7cfe100fe47")]
        [Values(typeof(GPSMode))]
        GPSMode,

        [GattCharacteristic("2ba1ee0c-1637-46cd-adee-5724eeb39fc2")]
        [Values(typeof(GPSFixesbeforeaccept))]
        GPSFixesBeforeAccept,

        [GattCharacteristic("bd7e15c7-4389-4834-9568-bf9997c6ccaa")]
        [Values(typeof(GPSHotStatus))]
        GPSHotStatus,

        [GattCharacteristic("9d1ef19c-4e96-4e1a-8e74-691eae8be429")]
        [Values(typeof(GpxLoggingPeriod))]
        GpxLoggingPeriod,

        [GattCharacteristic("9524c783-5fed-415f-ba7a-bf29cba1aea1")]
        [Values(typeof(GpxLoggingStatus))]
        GpxLoggingStatus,

        [GattCharacteristic("679b8ff1-7c36-49b4-9d16-8b7703eed2bf")]
        [Values(typeof(MailboxCheckFrequency))]
        [Translation("$=mailbox_check_frequency$$")]
        [Group(Group.Mailbox, 1)]
        [List]
        MailboxCheckFrequency,

        [GattCharacteristic("40a40e43-8ac5-42a4-ae1c-7f822194a671")]
        [Values(typeof(MailboxCheckStatus))]
        [Translation("$=mailbox_check_status$$")]
        [Group(Group.Mailbox, 0)]
        [Switch]
        MailboxCheckStatus,

        PrivateLogging,
        PrivateLogo,
        PrivateUserMode,
        ScreenAccessPin,
        ScreenNonActivityThreshold,
        ScreenScreenBrightness,
        ScreenTimeout,
        ScreenStealthStatus,
        SystemEncryptionStatus,

        [GattCharacteristic("22d272a2-17e8-4d92-8ce4-fd8ba8265a33")]
        [Values(typeof(TrackingActivitySenseStatus))]
        [Group(Group.Tracking)]
        [Hidden]
        TrackingActivitySenseStatus,

        [GattCharacteristic("8d1ef3d1-1d99-4145-bf63-06d1a2f9c8d4")]
        [Values(typeof(TrackingActivitySenseThreshold))]
        [Group(Group.Tracking)]
        [Hidden]
        TrackingActivitySenseThreshold,

        [GattCharacteristic("afe410ab-9478-4ef7-b4c9-119421894ec1")]
        [Values(typeof(TrackingActivitySenseLowThreshold))]
        [Group(Group.Tracking)]
        [Hidden]
        TrackingActivitySenseLowThreshold,

        [GattCharacteristic("8d1ef3d1-1d99-4145-bf63-06d1a2f9c8d4")]
        [Values(typeof(TrackingActivitySenseHighThreshold))]
        [Group(Group.Tracking)]
        [Hidden]
        TrackingActivitySenseHighThreshold,

        [GattCharacteristic("0220ca25-888e-42dc-9d31-59c1d648c95c")]
        [Values(typeof(TrackingBurstFixPeriod))]
        [Group(Group.Tracking, 2)]
        [Translation("$=tracking_burst_fix_period$$")]
        [Persisted]
        [List]
        TrackingBurstFixPeriod,

        [GattCharacteristic("d4a405a8-8af4-44c1-997d-f04ab29b4faf")]
        [Values(typeof(TrackingBurstTransmitPeriod))]
        [Group(Group.Tracking, 3)]
        [Translation("$=tracking_burst_transmit_period$$")]
        [Persisted]
        [List]
        TrackingBurstTransmitPeriod,

        [GattCharacteristic("42d1b719-d17e-41e4-9023-ef6e502f8be7")]
        [Values(typeof(TrackingFrequency))]
        [Translation("$=tracking_frequency$$")]
        [Group(Group.Tracking, 1)]
        [Persisted]
        [List]
        TrackingFrequency,

        [GattCharacteristic("639e1d0d-121d-43cb-94cf-aedba3907919")]
        [Values(typeof(TrackingStatus))]
        [Translation("$=tracking_status$$")]
        [Group(Group.Tracking, 0)]
        [Persisted]
        [Switch]
        TrackingStatus,


        BatteryLevel,


        [GattCharacteristic("9c6edb37-11e7-4055-9fc7-ee27d10d6848")]
        [Values(typeof(GpsStatus))]
        [Readonly]
        GpsStatus,

        [GattCharacteristic("0063046d-51f7-4c5b-be3a-37fce82a5500")]
        [Values(typeof(IridiumStatus))]
        [Readonly]
        IridiumStatus,

        [GattCharacteristic("811ce202-883e-4aea-af13-420353c05f7e")]
        [Event]
        [Readonly]
        [Translation("$=power_status$$")]
        PowerStatus,

        [GattCharacteristic("2c000f67-6710-4ede-be23-3389b9ea3a8e")]
        [Values(typeof(ExternalMobWatcher))]
        ExternalMobWatcher,

        [GattCharacteristic("77a0c07d-0e16-4f61-8b38-42d19e7aed7d")]
        [Values(typeof(ExternalPowerAvailiability))]
        [Group(Group.ExternalAccessory)]
        ExternalPowerAvailability,

        [GattCharacteristic("e4ea5871-3ed2-4988-9bbb-6934238bdbe5")]
        [Values(typeof(AlertsNotify))]
        Notify,

        [GattCharacteristic("afe410ab-9478-4ef7-b4c9-119421894ec1")]
        [Values(typeof(GprsStrategy))]
        GprsStrategy,

        [GattCharacteristic("64f40917-ae1d-42e8-95da-86688aab5d42")]
        [Readonly]
        [Values(typeof(GprsStatus))]
        GprsStatus,

        [GattCharacteristic("1db4e95c-e99e-4d73-9336-01d86def270a")]
        [Readonly]
        GprsSignal,

        [GattCharacteristic("edb28027-b017-41b9-b5e1-67ce7fa94c0c")]
        [Readonly]
        GprsMsisdn,

        [GattCharacteristic("05ee3364-3c87-4890-97eb-bb2ab1b90a93")]
        [Readonly]
        GprsSim,

        [GattCharacteristic("56450e84-01aa-4958-adfb-50a159c07588")]
        [Readonly]
        GprsLocation,

        [GattCharacteristic("c231c3a2-dddf-4cc0-af13-f57d1e7deda4")]
        [Values(typeof(InputSensitivity))]
        InputSensitivity,

        [GattCharacteristic("6c0d60e3-0be7-4363-9f29-f33f4f40603d")]
        [Values(typeof(CellularFrequency))]
        CellularFrequency,

        [GattCharacteristic("be021d8d-45ab-4b41-9dc0-4a6610d0884a")]
        [Values(typeof(CellularBurstFixPeriod))]
        CellularBurstFixPeriod,

        [GattCharacteristic("68b17606-00e2-4e58-affe-d6f4970d59ea")]
        [Values(typeof(CellularBurstTransmitPeriod))]
        CellularBurstTransmitPeriod,

        [GattCharacteristic("f36ecb94-0926-4add-ae8d-bdefa87cadab")]
        [Values(typeof(DistressFrequency))]
        DistressFrequency,

        [GattCharacteristic("250926a5-d2de-45ea-802d-51c330255e60")]
        [Values(typeof(DistressBurstFixPeriod))]
        [Group(Group.Tracking)]
        [List]
        DistressBurstFixPeriod,

        [GattCharacteristic("22d5dca5-344d-4297-a373-1c7065a38bc8")]
        [Values(typeof(DistressBurstTransmitPeriod))]
        [Group(Group.Tracking)]
        [List]
        DistressBurstTransmitPeriod,

        [GattCharacteristic("03639c2c-291c-452b-a54a-07277a0fa95d")]
        [Values(typeof(TransmissionFormat))]
        TransmissionFormat
    }



    public enum TrackingStatus
    {
        Off,
        On
    }


    public enum GpsStatus
    {
        Inactive,
        Active,
    }


    public enum IridiumStatus
    {
        Inactive,
        Active,
    }

    public enum TrackingFrequency
    {
        [Order(1)] [Value(5)] [Translation("$=continuous$$")] FrequencyContinuous,
        [Order(7)] [Value(5 * 60)] [Translation("$=time_5min$$")] Frequency5min,
        [Order(10)] [Value(10 * 60)] [Translation("$=time_10min$$")] Frequency10min,
        [Order(12)] [Value(15 * 60)] [Translation("$=time_15min$$")] Frequency15min,
        [Order(13)] [Value(20 * 60)] [Translation("$=time_20min$$")] Frequency20min,
        [Order(14)] [Value(30 * 60)] [Translation("$=time_30min$$")] Frequency30min,
        [Order(15)] [Value(60 * 60)] [Translation("$=time_60min$$")] Frequency60min,
        [Order(16)] [Value(90 * 60)] [Translation("$=time_90min$$")] Frequency90min,
        [Order(17)] [Value(120 * 60)] [Translation("$=time_120min$$")] Frequency120min,
        [Order(18)] [Value(180 * 60)] [Translation("$=time_180min$$")] Frequency180min,
        [Order(19)] [Value(240 * 60)] [Translation("$=time_240min$$")] Frequency240min,
        [Order(20)] [Value(350 * 60)] [Translation("$=time_360min$$")] Frequency360min,
        [Order(21)] [Value(480 * 60)] [Translation("$=time_480min$$")] Frequency480min,
        [Order(22)] [Value(720 * 60)] [Translation("$=time_720min$$")] Frequency720min,
        [Order(23)] [Value(-1)] [Translation("$=burst$$")] FrequencyBurst,
        [Order(3)] [Value(1 * 60)] [Translation("$=time_1min$$")] Frequency1min,
        [Order(4)] [Value(2 * 60)] [Translation("$=time_2min$$")] Frequency2min,
        [Order(5)] [Value(3 * 60)] [Translation("$=time_3min$$")] Frequency3min,
        [Order(6)] [Value(4 * 60)] [Translation("$=time_4min$$")] Frequency4min,
        [Order(23)] [Value(1440 * 60)] [Translation("$=time_1440min$$")] Frequency1440min,
        [Order(8)] [Value(6 * 60)] [Translation("$=time_6min$$")] Frequency6min,
        [Order(9)] [Value(8 * 60)] [Translation("$=time_8min$$")] Frequency8min,
        [Order(11)] [Value(12 * 60)] [Translation("$=time_12min$$")] Frequency12min,
        [Order(1)] [Value(15)] [Translation("$=time_15sec$$")] Frequency15sec,
        [Order(2)] [Value(30)] [Translation("$=time_30sec$$")] Frequency30sec
    }

    public enum AlertsBluetoothLossStatus
    {
        AlertsBluetoothLossStatusOff,
        AlertsBluetoothLossStatusOn
    }


    public enum AlertsColdTemperature
    {
        AlertsColdTemperatureMinus40,
        AlertsColdTemperatureMinus35,
        AlertsColdTemperatureMinus30,
        AlertsColdTemperatureMinus25,
        AlertsColdTemperatureMinus20,
        AlertsColdTemperatureMinus15,
        AlertsColdTemperatureMinus10,
        AlertsColdTemperatureMinus5,
        AlertsColdTemperatureZero,
        AlertsColdTemperaturePlus5,
        AlertsColdTemperaturePlus10,
        AlertsColdTemperaturePlus15,
        AlertsColdTemperaturePlus20,
        AlertsColdTemperaturePlus25,
        AlertsColdTemperaturePlus30,
        AlertsColdTemperaturePlus35,
        AlertsColdTemperaturePlus40,
        AlertsColdTemperaturePlus45,
        AlertsColdTemperaturePlus50
    }

    public enum AlertsCollisionDuration
    {
        AlertsCollisionDuration1ms,
        AlertsCollisionDuration2ms,
        AlertsCollisionDuration5ms,
        AlertsCollisionDuration10ms,
        AlertsCollisionDuration20ms
    }


    public enum AlertsCollisionStatus
    {
        AlertsCollisionStatusOff,
        AlertsCollisionStatusOn
    }


    public enum AlertsCollisionThreshold
    {
        AlertsCollisionThreshold1g,
        AlertsCollisionThreshold2g,
        AlertsCollisionThreshold4g,
        AlertsCollisionThreshold8g,
        AlertsCollisionThreshold12g,
        AlertsCollisionThreshold16g
    }


    public enum AlertsDeadmanStatus
    {
        AlertsDeadmanStatusOff,
        AlertsDeadmanStatusOn
    }

    public enum AlertsDeadmanTimeout
    {
        AlertsDeadmanTimeout5min,
        AlertsDeadmanTimeout10min,
        AlertsDeadmanTimeout15min,
        AlertsDeadmanTimeout30min,
        AlertsDeadmanTimeout1hour,
        AlertsDeadmanTimeout2hour,
        AlertsDeadmanTimeout4hour,
        AlertsDeadmanTimeoutUnknown
    }

    public enum AlertsGeofenceCentre
    {
    }

    public enum AlertsGeofenceCheckFrequency
    {
        AlertsGeofenceCheckFrequency1min,
        AlertsGeofenceCheckFrequency2min,
        AlertsGeofenceCheckFrequency3min,
        AlertsGeofenceCheckFrequency5min,
        AlertsGeofenceCheckFrequency10min,
        AlertsGeofenceCheckFrequency15min,
        AlertsGeofenceCheckFrequency30min,
        AlertsGeofenceCheckFrequencyUnknown
    }


    public enum AlertsGeofenceDistance
    {
        AlertsGeofenceDistance25m,
        AlertsGeofenceDistance50m,
        AlertsGeofenceDistance100m,
        AlertsGeofenceDistance250m,
        AlertsGeofenceDistance1km,
        AlertsGeofenceDistance2km,
        AlertsGeofenceDistance3km
    }

    public enum AlertsGeofenceStatus
    {
        AlertsGeofenceStatusOff,
        AlertsGeofenceStatusOn,
        AlertsGeofenceStatusOnPolyfence
    }

    public enum AlertsHotTemperature
    {
        AlertsHotTemperatureMinus40,
        AlertsHotTemperatureMinus35,
        AlertsHotTemperatureMinus30,
        AlertsHotTemperatureMinus25,
        AlertsHotTemperatureMinus20,
        AlertsHotTemperatureMinus15,
        AlertsHotTemperatureMinus10,
        AlertsHotTemperatureMinus5,
        AlertsHotTemperatureZero,
        AlertsHotTemperaturePlus5,
        AlertsHotTemperaturePlus10,
        AlertsHotTemperaturePlus15,
        AlertsHotTemperaturePlus20,
        AlertsHotTemperaturePlus25,
        AlertsHotTemperaturePlus30,
        AlertsHotTemperaturePlus35,
        AlertsHotTemperaturePlus40,
        AlertsHotTemperaturePlus45,
        AlertsHotTemperaturePlus50
    }


    public enum AlertsNotify
    {
        AlertsNotifyNone,
        AlertsNotifyAudible,
        AlertsNotifyVisual,
        AlertsNotifyBoth
    }


    public enum AlertsPowerLossStatus
    {
        AlertsPowerLossStatusOff,
        AlertsPowerLossStatusOn
    }


    public enum AlertsTemperatureCheckFrequency
    {
        AlertsTemperatureCheckFrequency1min,
        AlertsTemperatureCheckFrequency2min,
        AlertsTemperatureCheckFrequency3min,
        AlertsTemperatureCheckFrequency5min,
        AlertsTemperatureCheckFrequency10min,
        AlertsTemperatureCheckFrequency15min,
        AlertsTemperatureCheckFrequency30min,
        AlertsTemperatureCheckFrequencyUnknown
    }


    public enum AlertsTemperatureStatus
    {
        AlertsTemperatureStatusOff,
        AlertsTemperatureStatusOn
    }

    public enum AlertsTimerStatus
    {
        AlertsTimerStatusOff,
        AlertsTimerStatusOn
    }

    public enum AlertsTimerTimeout
    {
        AlertsTimerTimeout5min,
        AlertsTimerTimeout10min,
        AlertsTimerTimeout15min,
        AlertsTimerTimeout30min,
        AlertsTimerTimeout1hour,
        AlertsTimerTimeout2hour,
        AlertsTimerTimeout4hour,
        AlertsTimerTimeoutUnknown
    }


    public enum CellularBurstFixPeriod
    {
        [Value(5)] [Translation("$=time_5sec$$")] CellularBurstFixPeriod5sec,
        [Value(10)] [Translation("$=time_10sec$$")] CellularBurstFixPeriod10sec,
        [Value(15)] [Translation("$=time_15sec$$")] CellularBurstFixPeriod15sec,
        [Value(20)] [Translation("$=time_20sec$$")] CellularBurstFixPeriod20sec,
        [Value(30)] [Translation("$=time_30sec$$")] CellularBurstFixPeriod30sec,
        [Value(1 * 60)] [Translation("$=time_1min$$")] CellularBurstFixPeriod1min,
        [Value(2 * 60)] [Translation("$=time_2min$$")] CellularBurstFixPeriod2min,
        [Value(5 * 60)] [Translation("$=time_5min$$")] CellularBurstFixPeriod5min,
        [Value(10 * 60)] [Translation("$=time_10min$$")] CellularBurstFixPeriod10min,
        [Value(15 * 60)] [Translation("$=time_15min$$")] CellularBurstFixPeriod15min,
        [Value(20 * 60)] [Translation("$=time_20min$$")] CellularBurstFixPeriod20min
    }

    public enum CellularBurstTransmitPeriod
    {
        [Translation("$=time_1min$$")] CellularBurstTransmitPeriod1min,
        [Translation("$=time_2min$$")] CellularBurstTransmitPeriod2min,
        [Translation("$=time_5min$$")] CellularBurstTransmitPeriod5min,
        [Translation("$=time_10min$$")] CellularBurstTransmitPeriod10min,
        [Translation("$=time_15min$$")] CellularBurstTransmitPeriod15min,
        [Translation("$=time_30min$$")] CellularBurstTransmitPeriod30min,
        [Translation("$=time_60min$$")] CellularBurstTransmitPeriod60min
    }


    public enum CellularFrequency
    {
        CellularFrequencyContinuous,
        CellularFrequency5min,
        CellularFrequency10min,
        CellularFrequency15min,
        CellularFrequency20min,
        CellularFrequency30min,
        CellularFrequency60min,
        CellularFrequency90min,
        CellularFrequency120min,
        CellularFrequency180min,
        CellularFrequency240min,
        CellularFrequency360min,
        CellularFrequency480min,
        CellularFrequency720min,
        CellularFrequencyBurst,
        CellularFrequency1min,
        CellularFrequency2min,
        CellularFrequency3min,
        CellularFrequency4min,
        CellularFrequency1440min,
        CellularFrequency6min,
        CellularFrequency8min,
        CellularFrequency12min,
        CellularFrequency15sec,
        CellularFrequency30sec
    }


    public enum DistressBurstFixPeriod
    {
        DistressBurstFixPeriod5sec,
        DistressBurstFixPeriod10sec,
        DistressBurstFixPeriod15sec,
        DistressBurstFixPeriod20sec,
        DistressBurstFixPeriod30sec,
        DistressBurstFixPeriod1min,
        DistressBurstFixPeriod2min,
        DistressBurstFixPeriod5min,
        DistressBurstFixPeriod10min,
        DistressBurstFixPeriod15min,
        DistressBurstFixPeriod20min
    }


    public enum DistressBurstTransmitPeriod
    {
        DistressBurstTransmitPeriod1min,
        DistressBurstTransmitPeriod2min,
        DistressBurstTransmitPeriod5min,
        DistressBurstTransmitPeriod10min,
        DistressBurstTransmitPeriod15min,
        DistressBurstTransmitPeriod30min,
        DistressBurstTransmitPeriod60min
    }

    public enum DistressFrequency
    {
        DistressFrequencyContinuous,
        DistressFrequency5min,
        DistressFrequency10min,
        DistressFrequency15min,
        DistressFrequency20min,
        DistressFrequency30min,
        DistressFrequency60min,
        DistressFrequency90min,
        DistressFrequency120min,
        DistressFrequency180min,
        DistressFrequency240min,
        DistressFrequency360min,
        DistressFrequency480min,
        DistressFrequency720min,
        DistressFrequencyBurst,
        DistressFrequency1min,
        DistressFrequency2min,
        DistressFrequency3min,
        DistressFrequency4min,
        DistressFrequency1440min,
        DistressFrequency6min,
        DistressFrequency8min,
        DistressFrequency12min,
        DistressFrequency15sec,
        DistressFrequency30sec
    }



    public enum ExternalBaudRate
    {
        ExternalBaudRate4800,
        ExternalBaudRate9600,
        ExternalBaudRate19200,
        ExternalBaudRate38400,
        ExternalBaudRate57600,
        ExternalBaudRate115200
    }


    public enum ExternalMobWatcher
    {
        ExternalMobWatcherOff,
        ExternalMobWatcherOn
    }

    public enum ExternalPowerAvailiability
    {
        ExternalPowerAvailiabilityUnlimited,
        ExternalPowerAvailiabilityLimited,
        ExternalPowerAvailiabilityUnlimitedActivate
    }

    public enum ExternalSampleRate
    {
        ExternalSampleRate5sec,
        ExternalSampleRate10sec,
        ExternalSampleRate20sec,
        ExternalSampleRate40sec,
        ExternalSampleRate60sec
    }


    public enum ExternalStatus
    {
        ExternalStatusOff,
        ExternalStatusNMEA,
        ExternalStatusHydrosphere,
        ExternalStatusSerialApi,
        ExternalStatusMaximet,
        ExternalStatusMaximetGmx200,
        ExternalStatusWave,
        ExternalStatusVWTP3,
        DeviceCapabilityTypeDaliaFPSO
    }

    public enum GprsStatus
    {
        GprsStatusGsm,
        GprsStatusGsmCompact,
        GprsStatusUmts,
        GprsStatusGsmEdge,
        GprsStatusUmtsHsdpa,
        GprsStatusUmtsHsupa,
        GprsStatusUmtsHsdpaHsupa,
        GprsStatusLte
    }


    public enum GprsStrategy
    {
        GprsStrategyNever,
        GprsStrategyAlways,
        GprsStrategyPreferred
    }

    public enum GPSEarlyWakeup
    {
        GPSEarlyWakeup20sec,
        GPSEarlyWakeup40sec,
        GPSEarlyWakeup60sec,
        GPSEarlyWakeup120sec,
        GPSEarlyWakeup180sec,
        GPSEarlyWakeup240sec
    }

    public enum GPSFixesbeforeaccept
    {
        GPSFixesbeforeaccept1Fix,
        GPSFixesbeforeaccept5Fix,
        GPSFixesbeforeaccept10Fix,
        GPSFixesbeforeaccept20Fix,
        GPSFixesbeforeacceptUnknown
    }

    public enum GPSHotStatus
    {
        GPSHotStatusOff,
        GPSHotStatusOn
    }

    public enum GPSMode
    {
        GPSMode2D,
        GPSMode3D,
        GPSModeUnknown
    }

    public enum GpxLoggingPeriod
    {
        GpxLoggingPeriod1Sec,
        GpxLoggingPeriod5Sec,
        GpxLoggingPeriod10Sec,
        GpxLoggingPeriod30Sec,
        GpxLoggingPeriod1Min,
        GpxLoggingPeriod5Min,
        GpxLoggingPeriod10Min,
        GpxLoggingPeriod15Min,
        GpxLoggingPeriod20Min,
        GpxLoggingPeriod30Min,
        GpxLoggingPeriod60Min,
        GpxLoggingPeriodUnknown
    }

    public enum GpxLoggingStatus
    {
        GpxLoggingStatusOff,
        GpxLoggingStatusOn
    }


    public enum InputSensitivity
    {
        InputSensitivityFast0123,
        InputSensitivityFast012,
        InputSensitivityFast01,
        InputSensitivityFast0,
        InputSensitivityAllSlow
    }

    [Translation("$=mailbox_check_frequency$$")]
    public enum MailboxCheckFrequency
    {
        [Translation("$=time_5min$$")] Frequency5min,
        [Translation("$=time_10min$$")] Frequency10min,
        [Translation("$=time_15min$$")] Frequency15min,
        [Translation("$=time_20min$$")] Frequency20min,
        [Translation("$=time_30min$$")] Frequency30min,
        [Translation("$=time_60min$$")] Frequency60min,
        [Translation("$=time_90min$$")] Frequency90min,
        [Translation("$=time_120min$$")] Frequency120min,
        [Translation("$=time_180min$$")] Frequency180min,
        [Translation("$=time_240min$$")] Frequency240min,
        [Translation("$=time_360min$$")] Frequency360min,
        [Translation("$=time_480min$$")] Frequency480min,
        [Translation("$=time_720min$$")] Frequency720min
    }

    [Translation("$=mailbox_check_status$$")]
    public enum MailboxCheckStatus
    {
        [Translation("$=off$$")] Off,
        [Translation("$=on$$")] On
    }

    public enum ScreenLockStatus
    {
        Off,
        On
    }


    public enum ScreenNonActivityThreshold
    {
        [Translation("$=time_10sec$$")] Threshold10sec,
        [Translation("$=time_20sec$$")] Threshold20sec,
        [Translation("$=time_30sec$$")] Threshold30sec,
        [Translation("$=time_1min$$")] Threshold1min
    }

    [Translation("$=screen_brightness$$")]
    public enum ScreenScreenBrightness
    {
        [Translation("$=value_25$$")] Brightness25,
        [Translation("$=value_50$$")] Brightness50,
        [Translation("$=value_75$$")] Brightness75,
        [Translation("$=value_100$$")] Brightness100
    }

    public enum ScreenStealthStatus
    {
        Off,
        On
    }

    [Translation("$=screen_timeout$$")]
    public enum ScreenTimeout
    {
        [Translation("$=time_10sec$$")] Timeout10sec,
        [Translation("$=time_20sec$$")] Timeout20sec,
        [Translation("$=time_30sec$$")] Timeout30sec,
        [Translation("$=time_1min$$")] Timeout1min
    }

    public enum SystemEncryptionStatus
    {
        SystemEncryptionStatusOff,
        SystemEncryptionStatusOn
    }

    public enum TrackingActivitySenseHighThreshold
    {
        [Translation("$=value_z$$")] HighThresholdZ,
        [Translation("$=value_1$$")] HighThreshold1,
        [Translation("$=value_2$$")] HighThreshold2,
        [Translation("$=value_3$$")] HighThreshold3,
        [Translation("$=value_4$$")] HighThreshold4,
        [Translation("$=value_5$$")] HighThreshold5,
        [Translation("$=value_6$$")] HighThreshold6,
        [Translation("$=value_7$$")] HighThreshold7,
        [Translation("$=value_8$$")] HighThreshold8,
        [Translation("$=value_9$$")] HighThreshold9,
        [Translation("$=value_10$$")] HighThreshold10,
        [Translation("$=value_11$$")] HighThreshold11,
        [Translation("$=value_12$$")] HighThreshold12,
        [Translation("$=value_13$$")] HighThreshold13,
        [Translation("$=value_14$$")] HighThreshold14,
        [Translation("$=value_15$$")] HighThreshold15,
        [Translation("$=value_16$$")] HighThreshold16,
        [Translation("$=value_17$$")] HighThreshold17,
        [Translation("$=value_18$$")] HighThreshold18,
        [Translation("$=value_19$$")] HighThreshold19,
        [Translation("$=value_20$$")] HighThreshold20,
        [Translation("$=value_21$$")] HighThreshold21,
        [Translation("$=value_22$$")] HighThreshold22,
        [Translation("$=value_23$$")] HighThreshold23,
        [Translation("$=value_24$$")] HighThreshold24,
        [Translation("$=value_25$$")] HighThreshold25,
        [Translation("$=value_26$$")] HighThreshold26,
        [Translation("$=value_27$$")] HighThreshold27,
        [Translation("$=value_28$$")] HighThreshold28,
        [Translation("$=value_29$$")] HighThreshold29,
        [Translation("$=value_30$$")] HighThreshold30,
        [Translation("$=value_31$$")] HighThreshold31,
        [Translation("$=value_32$$")] HighThreshold32
    }


    public enum TrackingActivitySenseLowThreshold
    {
        [Translation("$=value_z$$")] LowThresholdZ,
        [Translation("$=value_1$$")] LowThreshold1,
        [Translation("$=value_2$$")] LowThreshold2,
        [Translation("$=value_3$$")] LowThreshold3,
        [Translation("$=value_4$$")] LowThreshold4,
        [Translation("$=value_5$$")] LowThreshold5,
        [Translation("$=value_6$$")] LowThreshold6,
        [Translation("$=value_7$$")] LowThreshold7,
        [Translation("$=value_8$$")] LowThreshold8,
        [Translation("$=value_9$$")] LowThreshold9,
        [Translation("$=value_10$$")] LowThreshold10,
        [Translation("$=value_11$$")] LowThreshold11,
        [Translation("$=value_12$$")] LowThreshold12,
        [Translation("$=value_13$$")] LowThreshold13,
        [Translation("$=value_14$$")] LowThreshold14,
        [Translation("$=value_15$$")] LowThreshold15,
        [Translation("$=value_16$$")] LowThreshold16,
        [Translation("$=value_17$$")] LowThreshold17,
        [Translation("$=value_18$$")] LowThreshold18,
        [Translation("$=value_19$$")] LowThreshold19,
        [Translation("$=value_20$$")] LowThreshold20,
        [Translation("$=value_21$$")] LowThreshold21,
        [Translation("$=value_22$$")] LowThreshold22,
        [Translation("$=value_23$$")] LowThreshold23,
        [Translation("$=value_24$$")] LowThreshold24,
        [Translation("$=value_25$$")] LowThreshold25,
        [Translation("$=value_26$$")] LowThreshold26,
        [Translation("$=value_27$$")] LowThreshold27,
        [Translation("$=value_28$$")] LowThreshold28,
        [Translation("$=value_29$$")] LowThreshold29,
        [Translation("$=value_30$$")] LowThreshold30,
        [Translation("$=value_31$$")] LowThreshold31,
        [Translation("$=value_32$$")] LowThreshold32
    }


    [Translation("$=tracking_activity_sense_status$$")]
    public enum TrackingActivitySenseStatus
    {
        [Translation("$=off$$")] Off,
        [Translation("$=on$$")] On,
        [Translation("$=bump$$")] Bump,
        [Translation("$=sog$$")] Sog,
        [Translation("$=bump_and_sog$$")] BumpAndSog
    }

    [Translation("$=tracking_activity_sense_threshold$$")]
    public enum TrackingActivitySenseThreshold
    {
        [Translation("$=value_18$$")] Threshold18,
        [Translation("$=value_19$$")] Threshold19,
        [Translation("$=value_20$$")] Threshold20,
        [Translation("$=value_21$$")] Threshold21,
        [Translation("$=value_22$$")] Threshold22,
        [Translation("$=value_23$$")] Threshold23,
        [Translation("$=value_24$$")] Threshold24,
        [Translation("$=value_25$$")] Threshold25,
        [Translation("$=value_26$$")] Threshold26,
        [Translation("$=value_27$$")] Threshold27,
        [Translation("$=value_28$$")] Threshold28,
        [Translation("$=value_29$$")] Threshold29,
        [Translation("$=value_30$$")] Threshold30,
        [Translation("$=value_31$$")] Threshold31,
        [Translation("$=value_32$$")] Threshold32,
        [Translation("$=value_33$$")] Threshold33,
        [Translation("$=value_34$$")] Threshold34,
        [Translation("$=value_35$$")] Threshold35,
        [Translation("$=value_36$$")] Threshold36,
        [Translation("$=value_37$$")] Threshold37,
        [Translation("$=value_38$$")] Threshold38,
        [Translation("$=value_39$$")] Threshold39,
        [Translation("$=value_40$$")] Threshold40,
        [Translation("$=value_41$$")] Threshold41,
        [Translation("$=value_42$$")] Threshold42,
        [Translation("$=value_43$$")] Threshold43,
        [Translation("$=value_44$$")] Threshold44,
        [Translation("$=value_45$$")] Threshold45,
        [Translation("$=value_46$$")] Threshold46,
        [Translation("$=value_47$$")] Threshold47,
        [Translation("$=value_48$$")] Threshold48,
        [Translation("$=value_49$$")] Threshold49,
        [Translation("$=value_50$$")] Threshold50,
        [Translation("$=value_51$$")] Threshold51,
        [Translation("$=value_52$$")] Threshold52,
        [Translation("$=value_53$$")] Threshold53,
        [Translation("$=value_54$$")] Threshold54
    }


    [Translation("$=tracking_burst_fix_period$$")]
    public enum TrackingBurstFixPeriod
    {
        [Value(5)] [Translation("$=time_5sec$$")] Period5sec,
        [Value(10)] [Translation("$=time_10sec$$")] Period10sec,
        [Value(15)] [Translation("$=time_15sec$$")] Period15sec,
        [Value(20)] [Translation("$=time_20sec$$")] Period20sec,
        [Value(30)] [Translation("$=time_30sec$$")] Period30sec,
        [Value(1 * 60)] [Translation("$=time_1min$$")] Period1min,
        [Value(2 * 60)] [Translation("$=time_2min$$")] Period2min,
        [Value(5 * 60)] [Translation("$=time_5min$$")] Period5min,
        [Value(10 * 60)] [Translation("$=time_10min$$")] Period10min,
        [Value(15 * 60)] [Translation("$=time_15min$$")] Period15min,
        [Value(20 * 60)] [Translation("$=time_20min$$")] Period20min
    }

    [Translation("$=tracking_burst_transmit_period$$")]
    public enum TrackingBurstTransmitPeriod
    {
        [Value(1 * 60)] [Translation("$=time_1min$$")] TrackingBurstTransmitPeriod1min,
        [Value(2 * 60)] [Translation("$=time_2min$$")] TrackingBurstTransmitPeriod2min,
        [Value(5 * 60)] [Translation("$=time_5min$$")] TrackingBurstTransmitPeriod5min,
        [Value(10 * 60)] [Translation("$=time_10min$$")] TrackingBurstTransmitPeriod10min,
        [Value(15 * 60)] [Translation("$=time_15min$$")] TrackingBurstTransmitPeriod15min,
        [Value(30 * 60)] [Translation("$=time_30min$$")] TrackingBurstTransmitPeriod30min,
        [Value(60 * 60)] [Translation("$=time_60min$$")] TrackingBurstTransmitPeriod60min
    }


    public enum TransmissionFormat
    {
        TransmissionFormatStandard,
        TransmissionFormatCompact,
        TransmissionFormatAes
    }


    public enum GprsParameter
    {
        GprsParameterApnName,
        GprsParameterEndpointAddress1,
        GprsParameterEndpointAddress2,
        GprsParameterEndpointAddress3,
        GprsParameterEndpointPort1,
        GprsParameterEndpointPort2,
        GprsParameterEndpointPort3,
        GprsParameterApnUsername,
        GprsParameterApnPassword
    }


    public enum ActivationState
    {
        Unknown,
        Pending,
        Activating,
        Activated,
        Error
    }


    public enum GenericAlertType
    {
        TypeA,
        TypeB,
        TypeC,
        TypeD,
        TypeE,
        TypeF,
        TypeG,
        TypeH,
        TypeI,
        TypeJ,
        TypeK,
        TypeL,
        TypeM,
        TypeN,
        TypeO,
        TypeP,
        TypeQ,
        TypeR,
        TypeS,
        TypeT,
        TypeU,
        TypeV,
        TypeW,
        TypeX,
        TypeY,
        TypeZ
    }
}
