using System;
using System.Collections.Generic;
using System.Text;

namespace Iridium360.Connect.Framework.Implementations
{
    internal class DeviceAccessoryParameter
    {
        private GprsParameter parameter;
        private IFramework framework;
        private string f477b;
        private string f478c;

        public DeviceAccessoryParameter(IFramework framework, GprsParameter parameter)
        {
            this.framework = framework;
            this.parameter = parameter;
        }

        public string getAbout()
        {
            return this.f478c;
        }

        public GprsParameter getIndex()
        {
            return this.parameter;
        }

        public string getLabel()
        {
            return this.f477b;
        }

        public void update(byte[] value)
        {
            throw new NotImplementedException();
            //#if RELEASE
            //            Rock.updateGprsConfig(this.parameter, value);
            //#endif
        }
    }
}
