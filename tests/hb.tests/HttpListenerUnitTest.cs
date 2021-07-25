using hb.LogServices;
using hb.network.HTTP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class HttpListenerUnitTest
    {
        [TestMethod]
        public void HttpListenerTest()
        {
            Logger.Initialize(Logger.LOG4NET);

            XHttpListener xl = new XHttpListener();
            xl.Port = 8866;
            xl.Hostname = "+";
            xl.Get["/api/v1/test1"] = (r) =>
            {
                return "test1";
            };
            xl.Post["/api/v1/test2"] = (r) =>
            {
                var query = r.GetQueryString();
                var body = r.GetBody();
                return "test2";
            };
            bool ret = xl.Run();
            Console.WriteLine($"listen port {ret}");
            if (ret)
            {
                XTask.Delay(60 * 1000);
            }
        }
    }
}
