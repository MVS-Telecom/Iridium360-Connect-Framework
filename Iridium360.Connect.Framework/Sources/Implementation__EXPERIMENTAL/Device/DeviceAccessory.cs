using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class DeviceAccessory
    {
        public readonly List<DeviceAccessoryParameter> f475a;

        public DeviceAccessory(List<DeviceAccessoryParameter> parameters)
        {
            this.f475a = parameters;
        }
    }
}
