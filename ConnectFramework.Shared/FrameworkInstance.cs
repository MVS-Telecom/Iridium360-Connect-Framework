using Iridium360.Connect.Framework.Messaging;
using Iridium360.Connect.Framework.Messaging.Storage;
using System;
using System.Linq;

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
        public static IFrameworkProxy GetInstance(IStorage storage, ILogger logger, Lazy<IBluetoothHelper> bluetoothHelper)
        {
#if ANDROID || IPHONE
            return new FrameworkProxy(
                global::ConnectFramework.Shared.R7ConnectFramework.GetInstance(storage, logger, bluetoothHelper),
                logger,
                new RealmPacketBuffer(),
                storage);
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
        public static IFrameworkProxy GetInstance_EXPERIMENTAL(IStorage storage, ILogger logger, IBluetooth bluetooth)
        {
            return new FrameworkProxy(
                new global::Iridium360.Connect.Framework.Implementations.FrameworkInstance__EXPERIMENTAL(bluetooth, storage, logger: logger),
                logger,
                new RealmPacketBuffer(),
                storage);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IFrameworkProxy GetInstance_FAKE(IStorage storage, ILogger logger)
        {
            return new FrameworkProxy(
                new global::Iridium360.Connect.Framework.Fakes.FrameworkInstance_FAKE(storage),
                logger,
                new RealmPacketBuffer(),
                storage);
        }
    }
}
