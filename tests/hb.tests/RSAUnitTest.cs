using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    /// <summary>
    /// 1.服务器生成公钥-私钥
    /// 2.发送公钥给客户端
    /// 3. a. 客户端用公钥加密内容发送到服务端，服务端用私钥解密
    ///    b. 服务端用私钥签名发送内容给客户端，客户端用公钥验证签名
    /// </summary>
    [TestClass]
    public class RSAUnitTest
    {
        [TestMethod]
        public void RSATest1()
        {
            string xmlKeys;
            string xmlPublicKeys;
            RSAHelper.CreateKey(out xmlKeys, out xmlPublicKeys);

            // a
            string original = "爱丽丝打开flask对方立刻洒家地方";
            string encrypted = RSAHelper.Encrypt(xmlPublicKeys, original);
            string roundtrip = RSAHelper.Decrypt(xmlKeys, encrypted);

            Assert.AreEqual(original, roundtrip);
        }

        [TestMethod]
        public void RSATest2()
        {
            string xmlKeys;
            string xmlPublicKeys;
            RSAHelper.CreateKey(out xmlKeys, out xmlPublicKeys);

            // b
            string signData = "hashcode test";
            string signDataMD5 = signData.GetStringMD5();
            string sign = RSAHelper.SignatureFormatterToString(xmlKeys, signDataMD5);
            bool ret = RSAHelper.SignatureDeformatter(xmlPublicKeys, signDataMD5, sign);
            Assert.IsTrue(ret);
        }
    }
}
