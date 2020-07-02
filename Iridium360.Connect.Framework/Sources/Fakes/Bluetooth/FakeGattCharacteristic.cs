using Iridium360.Connect.Framework.Helpers;
using Iridium360.Connect.Framework.Util;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Fakes
{
    internal abstract class FakeGattCharacteristic : IGattCharacteristic
    {
        public event EventHandler<ValueUpdatedEventArgs> ValueUpdated = delegate { };

        public abstract Guid Id { get; }

        public string Name { get; set; }

        public byte[] Value { get; set; } = new byte[] { 0 };

        public Task StartUpdatesAsync() { return Task.Run(() => { }); }

        public abstract Task WriteAsync(byte[] bytes);
        public abstract Task<byte[]> ReadAsync();

        public virtual async void FakeReadAndNotify()
        {
            await ReadAsync();
            FakeNotify();
        }

        public virtual void FakeNotify()
        {
            ValueUpdated(this, new ValueUpdatedEventArgs()
            {
                Characteristic = this
            });
        }
    }



    internal class FakeIndicatorGattCharacterictics : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("d0701859-7e41-47b1-af19-fb305f98ab51");

        public override Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeInboxGattCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("4de3e821-2f25-4da0-b696-d06f81f46a52");


        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(200);
            return new byte[] { 0 };
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeCommonGattCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("a84c417a-3380-4a4b-a885-f926a647bb3c");

        public override Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeBatteryGattCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("8c0a3f8b-fccb-482a-8406-e6ad57b324f4");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(1000);
            Value = new byte[] { 81 };
            return Value;
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeFirmwareGattCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(1000);
            //Value = Encoding.UTF8.GetBytes("YB3 03.09.10");
            //Value = Encoding.UTF8.GetBytes("TS 01.06.09");
            Value = Encoding.UTF8.GetBytes("YB3 03.06.10");
            return Value;
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeHardwareGattCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("b266cf58-8aa9-4491-a3e6-4876b4ee6efc");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(1000);
            Value = Encoding.UTF8.GetBytes("88b266cf584876b4ee6efc");
            return Value;
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }



    internal class FakeLockStatusCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("049e8f08-78a3-443a-9517-58dab1ce721d");

        public bool unlocked = false;

        public override async Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }



    internal class FakeRawMessagingCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("c7de1b97-882d-4c52-9a79-8f87bd9a9a4f");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(1000);
            Value = new byte[] { 1 };
            return Value;
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeOutboxCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("cf17c69c-9d80-4ffb-8fa3-f81a83170cef");
        private FakeGatt gatt;

        public FakeOutboxCharacteristic(FakeGatt gatt)
        {
            this.gatt = gatt;
        }

        public override Task<byte[]> ReadAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }


    internal class FakeLocationCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("d6f3af9a-cea4-4220-a7ee-8eced1534af3");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(1000);
            //Value = "06036F35D2298182287D816000040BA9243B0E1F".ToByteArray();
            //Value = "6F56C4297C6B095931E0002C10DC745400000000".ToByteArray();
            Value = "6F1AFAA99234CB31770000140774365500000000".ToByteArray();
            FakeNotify();
            return Value;
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }

    internal class FakeMessageStatusCharacteristic : FakeGattCharacteristic
    {
        public override Guid Id => Guid.Parse("a7a6f930-1ad0-4a68-9d85-1228fc3e5c19");

        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(100);
            return new byte[] { 0 };
        }

        public override Task WriteAsync(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }



    internal class Fake_DeviceParameterCharacterictic : FakeGattCharacteristic
    {
        readonly Guid id;
        public override Guid Id => id;

        public Fake_DeviceParameterCharacterictic(Parameter parameter, Enum initValue = null) : base()
        {
            var value = initValue ?? Enum.GetValues(parameter.GetAttribute<ValuesAttribute>().Value).Cast<Enum>().ToList().FirstOrDefault();
            id = Guid.Parse(parameter.GetAttribute<GattCharacteristicAttribute>().Value);

            Value = new byte[] { (byte)Convert.ToInt16(value) };
        }


        public override async Task<byte[]> ReadAsync()
        {
            await Task.Delay(100);
            FakeNotify();
            return Value;
        }

        public override async Task WriteAsync(byte[] bytes)
        {
            await Task.Delay(100);
            Value = bytes;
        }
    }

}
