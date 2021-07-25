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
            string value1 = MD5Utils.GetFileMD5(file);

            string test = "";
            string value2 = MD5Utils.GetStringMD5(test);

            Assert.IsTrue(value1 == value2);
        }

        [TestMethod]
        public void MD5bytes()
        {
            //说明：先创建空文件d:/a.txt
            string file = "d:/a.txt";
            string value1 = MD5Utils.GetFileMD5(file);

            byte[] bytes = new byte[0];
            string value2 = MD5Utils.GetBytesMD5(bytes);

            Assert.IsTrue(value1 == value2);
        }

        [TestMethod]
        public void MD5Output()
        {
            string test = "md5 output type";
            string value1 = MD5Utils.GetStringMD5(test);
            string value2 = MD5Utils.GetStringMD5(test, hb.MD5Output.Hex);
            Console.WriteLine(value1);
            Console.WriteLine(value2);

        }
    }
}
