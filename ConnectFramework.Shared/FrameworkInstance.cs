using System;

namespace Iridium360.Connect.Framework
{
    public static class FrameworkInstance
    {
        public static IFramework GetInstance(IStorage storage, ILogger logger, IBluetoothHelper bluetoothHelper)
        {
#if ANDROID || IPHONE
            return global::ConnectFramework.Shared.R7ConnectFramework.GetInstance(storage, logger, bluetoothHelper);
#else
            throw new NotSupportedException();
#endif
        }
    }
}
