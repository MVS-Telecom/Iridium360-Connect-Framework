using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iridium360.Connect.Framework.Helpers;

namespace Iridium360.Connect.Framework.Fakes
{
    internal class FakeDevice : IFoundDevice
    {
        public object Native => null;

        public Guid Id { get; set; }

        public string Mac => Id.ToBluetoothAddress();

        public string Name { get; set; }

        public bool IsConnected => true;

        public DeviceType? DeviceType => RockstarHelper.GetTypeBySerial(Serial);

        public string Serial => RockstarHelper.GetSerialFromName(Name);

        public FakeDevice(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public async Task<List<IGattService>> GetServicesAsync()
        {
            await Task.Delay(500);

            return new List<IGattService>()
            {
                new FakeGatt()
            };
        }

        public void Dispose()
        {

        }
    }
}
