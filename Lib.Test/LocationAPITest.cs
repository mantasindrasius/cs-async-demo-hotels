using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lib.Test
{
    [TestClass]
    public class LocationAPITest
    {
        [TestMethod]
        public void GetLocation()
        {
            var api = new LocationAPI();

            var result = api.GetLocation("Vilnius");

            Assert.AreEqual(0, result.Latitude); 
        }
    }
}
