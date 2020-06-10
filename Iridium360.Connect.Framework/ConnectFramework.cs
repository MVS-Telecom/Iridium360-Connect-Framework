using Rock;
using Rock.Util;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;

namespace Iridium360.Connect.Framework
{
    public static class ConnectFramework
    {
        public static IFramework GetInstance(IStorage storage, ILogger logger)
        {
#if ANDROID || IPHONE
            return global::ConnectFramework.Shared.R7ConnectFramework.GetInstance(storage, logger);
#else
            throw new NotSupportedException();
#endif
        }
    }
}
