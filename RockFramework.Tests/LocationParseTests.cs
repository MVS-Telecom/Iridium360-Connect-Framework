using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Rock.Helpers;

namespace RockFramework.Tests
{
    [TestClass]
    public class LocationParseTests
    {
        [TestMethod]
        public void Parse()
        {
            var location1 = LocationParser.Parse("6F1AFAA99234CB31770000140774365500000000".ToByteArray(), "YB3 03.6.10");
            var location2 = LocationParser.Parse("6F56C4297C6B095931E0002C10DC745400000000".ToByteArray(), "TS 01.06.09");
            var location3 = LocationParser.Parse("06036F35D2298182287D816000040BA9243B0E1F".ToByteArray(), "YB3 03.6.10");
            
        }
    }
}
