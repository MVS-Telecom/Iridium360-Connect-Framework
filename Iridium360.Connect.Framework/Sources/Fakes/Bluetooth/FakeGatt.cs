using Iridium360.Connect.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Fakes
{
    internal class FakeGatt : IGattService
    {
        public Guid Id { get; private set; }

        public FakeGattCharacteristic Battery;
        public FakeGattCharacteristic Firmware;
        public FakeGattCharacteristic Hardware;
        public FakeGattCharacteristic LockStatus;
        public FakeGattCharacteristic MessageStatus;
        public FakeGattCharacteristic RawMessaging;
        public FakeGattCharacteristic Outbox;
        public FakeGattCharacteristic Location;
        public FakeGattCharacteristic Indicator;
        public FakeGattCharacteristic Common;
        public FakeGattCharacteristic Inbox;

        public FakeGatt()
        {
            Id = Guid.NewGuid();

            Inbox = new FakeInboxGattCharacteristic();
            Common = new FakeCommonGattCharacteristic();
            Indicator = new FakeIndicatorGattCharacterictics();
            Battery = new FakeBatteryGattCharacteristic();
            Firmware = new FakeFirmwareGattCharacteristic();
            Hardware = new FakeHardwareGattCharacteristic();
            LockStatus = new FakeLockStatusCharacteristic();
            MessageStatus = new FakeMessageStatusCharacteristic();
            RawMessaging = new FakeRawMessagingCharacteristic();
            Outbox = new FakeOutboxCharacteristic(this);
            Location = new FakeLocationCharacteristic();
        }

        public async Task<List<IGattCharacteristic>> GetCharacteristicsAsync()
        {
            await Task.Delay(100);

            return new List<IGattCharacteristic>()
            {
                Inbox,
                Common,
                Indicator,
                Battery,
                Firmware,
                Hardware,
                LockStatus,
                MessageStatus,
                RawMessaging,
                Outbox,
                Location,


                new Fake_DeviceParameterCharacterictic(Parameter.TrackingStatus, TrackingStatus.On),
                new Fake_DeviceParameterCharacterictic(Parameter.TrackingFrequency, TrackingFrequency.Frequency15min),
                new Fake_DeviceParameterCharacterictic(Parameter.TrackingBurstFixPeriod, TrackingBurstFixPeriod.Period15min),
                new Fake_DeviceParameterCharacterictic(Parameter.TrackingBurstTransmitPeriod, TrackingBurstTransmitPeriod.TrackingBurstTransmitPeriod60min),
                new Fake_DeviceParameterCharacterictic(Parameter.TrackingActivitySenseStatus),
                new Fake_DeviceParameterCharacterictic(Parameter.TrackingActivitySenseThreshold),
                new Fake_DeviceParameterCharacterictic(Parameter.IridiumStatus, IridiumStatus.Active),
                new Fake_DeviceParameterCharacterictic(Parameter.GpsStatus, GpsStatus.Active),
                new Fake_DeviceParameterCharacterictic(Parameter.MailboxCheckStatus, MailboxCheckStatus.On),
                new Fake_DeviceParameterCharacterictic(Parameter.MailboxCheckFrequency, MailboxCheckFrequency.Frequency60min),
            };
        }
    }
}
