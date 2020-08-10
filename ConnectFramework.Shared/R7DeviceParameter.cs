#if ANDROID
using Android.Runtime;
using UK.Rock7.Connect.Device;
#elif IOS
using Foundation;
#endif
using Iridium360.Connect.Framework;
using Iridium360.Connect.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iridium360.Connect.Framework.Util;


namespace ConnectFramework.Shared
{
    [DebuggerDisplay("{Id} -> {CachedValue}")]
    internal class R7DeviceParameter : BaseDeviceParameter
    {
        /// <summary>
        /// Возможные значения параметра
        /// </summary>
        public override List<Enum> Options
        {
            get
            {
#if ANDROID
                var options = new Android.Runtime.JavaDictionary<int, string>(source.Options.Handle, Android.Runtime.JniHandleOwnership.DoNotRegister).ToDictionary(t => t.Key, t => t.Value);
#elif IOS
                var keys = source.Options.Keys;
                var values = source.Options.Values;
                var options = new Dictionary<int, string>();

                for (int i = 0; i < keys.Length; i++)
                    options.Add((int)(NSNumber)keys[i], (NSString)values[i]);
#endif


                return options.Keys
                    .OrderBy(x => x)
                    .Select(x => (Enum)Enum.ToObject(type, x))
                    .Where(x => !x.HasAttribute<HiddenAttribute>())
                    .OrderBy(x => x.GetAttribute<OrderAttribute>()?.Value ?? int.MaxValue)
                    .ToList();
            }
        }


        private readonly DeviceParameter source;

        public R7DeviceParameter(R7ConnectFramework framework, R7Device device, DeviceParameter source) : base(framework, device, source.Identifier.ToR7().FromR7())
        {
            this.source = source;
        }

    }
}
