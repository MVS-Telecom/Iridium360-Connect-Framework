using Iridium360.Connect.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class __DeviceParameter : BaseDeviceParameter
    {
        public __DeviceParameter(IFramework framework, IDevice device, Parameter id) : base(framework, device, id)
        {
            __options = Enum
                .GetValues(type)
                .OfType<Enum>()
                .Where(x => !x.HasAttribute<HiddenAttribute>())
                .OrderBy(x => x.GetAttribute<OrderAttribute>()?.Value ?? int.MaxValue)
                .ToList();
        }


        private List<Enum> __options;
        private byte? __cachedValue;


        public override List<Enum> Options => __options;
        protected override byte? cachedValue => __cachedValue;



        internal void removeValueOption(Enum option)
        {
            var a = __options.FirstOrDefault(x => x.Equals(option));
            __options.Remove(a);
        }


        internal override bool UpdateCachedValue(byte[] value)
        {
            __cachedValue = value[0];
            return base.UpdateCachedValue(value);
        }
    }
}
