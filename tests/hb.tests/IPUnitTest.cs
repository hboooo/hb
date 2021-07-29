using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace hb.tests
{
    [TestClass]
    public class IPUnitTest
    {
        [TestMethod]
        public void GetMacAddr()
        {
            string mac = IPUtils.GetMacAddr();
            Debug.WriteLine($"mac:{mac}");
            Assert.IsNotNull(mac);
        }

        [TestMethod]
        public void GetLocalHost()
        {
            var host = IPUtils.GetLocalIntranetIP();
            Debug.WriteLine($"host:{host}");
            Assert.IsNotNull(host);
        }

        [TestMethod]
        public void ComExist()
        {
            Assert.IsFalse(IPUtils.ComExist("COMAAA"));
        }
    }
}
