using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class MD5UnitTest
    {
        [TestMethod]
        public void MD5Str()
        {
            //说明：先创建空文件d:/a.txt
            string file = "d:/a.txt";
            string value1 = MD5Utils.GetMD5File(file);

            string test = "";
            string value2 = MD5Utils.GetMd5String(test);

            Assert.IsTrue(value1 == value2);
        }

        [TestMethod]
        public void MD5bytes()
        {
            //说明：先创建空文件d:/a.txt
            string file = "d:/a.txt";
            string value1 = MD5Utils.GetMD5File(file);

            byte[] bytes = null;
            string value2 = MD5Utils.GetMd5Bytes(bytes);

            Assert.IsTrue(value1 == value2);
        }
    }
}
