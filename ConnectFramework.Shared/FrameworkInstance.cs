using System;

namespace Iridium360.Connect.Framework
{
    public static partial class FrameworkInstance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="logger"></param>
        /// <param name="bluetoothHelper"></param>
        /// <returns></returns>
        public static IFramework GetInstance(IStorage storage, ILogger logger, IBluetoothHelper bluetoothHelper)
        {
#if ANDROID || IPHONE
            return global::ConnectFramework.Shared.R7ConnectFramework.GetInstance(storage, logger, bluetoothHelper);
#else
            throw new NotSupportedException();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="logger"></param>
        /// <param name="bluetooth"></param>
        /// <returns></returns>
        [Obsolete("Use this framework implementaion only for experimental purposes! NOT FOR PRODUCTION")]
        public static IFramework GetInstance_EXPERIMENTAL(IStorage storage, ILogger logger, IBluetooth bluetooth)
        {
            return new global::Iridium360.Connect.Framework.Implementations.FrameworkInstance__EXPERIMENTAL(bluetooth, storage, logger: logger);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFramework GetInstance_FAKE()
        {
            return new global::Iridium360.Connect.Framework.Fakes.FrameworkInstance_FAKE();
        }
    }
}
