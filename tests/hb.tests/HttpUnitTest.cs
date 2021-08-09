using hb.network.HTTP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class HttpUnitTest
    {
        [TestMethod]
        public void ExecuteGet()
        {
            var ret = Rest.Create(Method.GET).SetUrl("http://www.baidu.com")
                .Execute()
                .TakeString();
            Assert.IsNotNull(ret);
        }

        [TestMethod]
        public void ExecuteGetQuery()
        {
            var ret = Rest.Create(Method.GET)
                .SetBaseUrl("http://www.baidu.com")
                .AddQueryParameter("wd", "双色球")
                .Execute()
                .TakeHttpCode();
            Assert.IsTrue(ret == System.Net.HttpStatusCode.OK);
        }
    }
}
