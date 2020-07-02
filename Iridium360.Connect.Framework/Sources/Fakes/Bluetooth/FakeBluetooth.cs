using Iridium360.Connect.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Iridium360.Connect.Framework.Fakes
{
    public class FakeBluetooth : IBluetooth
    {
        public event EventHandler<BluetoothStateChangedEventArgs> BluetoothStateChanged = delegate { };
        public event EventHandler<ScanResultsEventArgs> ScanResults = delegate { };
        public event EventHandler<EventArgs> ScanTimeout = delegate { };
        public event EventHandler<EventArgs> DeviceConnectionLost = delegate { };

        public bool IsOn => true;


        public Task ConnectToDeviceAsync(IBluetoothDevice device)
        {
            if (device is FakeDevice)
                return ConnectToDeviceAsync(device.Id);

            throw new NotImplementedException();
        }


        public async Task<IBluetoothDevice> ConnectToDeviceAsync(Guid id)
        {
            await Task.Delay(1000);

            //throw new Exception();

            if (id == FAKE_ROCKSTAR.Id)
                return FAKE_ROCKSTAR;

            if (id == FAKE_ROCKFLEET.Id)
                return FAKE_ROCKFLEET;

            if (id == FAKE_ROCKSTAR2.Id)
                return FAKE_ROCKSTAR2;


            throw new NotImplementedException();
        }


        private FakeDevice FAKE_ROCKSTAR = new FakeDevice(Guid.Parse("00000000-0000-0000-0000-000780a3f079"), "RockSTAR 20975 ᴰᴱᴹᴼ");
        private FakeDevice FAKE_ROCKSTAR2 = new FakeDevice(Guid.Parse("00000000-0000-0000-0000-000780a3f079"), "RockSTAR 29999 ᴰᴱᴹᴼ");
        private FakeDevice FAKE_ROCKFLEET = new FakeDevice(Guid.Parse("00000000-0000-0000-0000-000780a3f078"), "RockFLEET 50309 ᴰᴱᴹᴼ");


        public void StartLeScan()
        {
            Task.Run(async () =>
            {
                await Task.Delay(3000);

                ScanResults(this, new ScanResultsEventArgs()
                {
                    FoundDevices = new List<IBluetoothDevice>()
                    {
                        FAKE_ROCKSTAR
                    }
                });


                await Task.Delay(4000);

                ScanResults(this, new ScanResultsEventArgs()
                {
                    FoundDevices = new List<IBluetoothDevice>()
                    {
                        FAKE_ROCKSTAR,
                        FAKE_ROCKFLEET,
                        FAKE_ROCKSTAR2
                    }
                });

            });
        }

        public void StopLeScan()
        {

        }

        public Task<bool> TurnOn(bool force = true)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public Task DisconnectFromDeviceAsync()
        {
            return Task.Run(async () =>
            {
                await Task.Delay(3000);
            });
        }

        public void Dispose()
        {

        }

        public Task DisconnectFromDeviceAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
