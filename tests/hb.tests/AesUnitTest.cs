using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Cryptography;

namespace hb.tests
{
    [TestClass]
    public class AesUnitTest
    {
        [TestMethod]
        public void AesTest1()
        {
            string original = "爱丽丝打开flask对方立刻洒家地方";
            using (Aes aes = Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = AESHelper.EncryptToBytes(original, aes.Key, aes.IV);

                // Decrypt the bytes to a string.
                string roundtrip = AESHelper.DecryptFromBytes(encrypted, aes.Key, aes.IV);

                //Display the original data and the decrypted data.
                Assert.AreEqual(original, roundtrip);
            }
        }

        [TestMethod]
        public void AesTest2()
        {
            string original = "爱丽丝打开flask对方立刻洒家地方";
            using (Aes aes = Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                string encrypted = AESHelper.Encrypt(original, aes.Key, aes.IV);

                // Decrypt the bytes to a string.
                string roundtrip = AESHelper.Decrypt(encrypted, aes.Key, aes.IV);

                //Display the original data and the decrypted data.
                Assert.AreEqual(original, roundtrip);
            }
        }

        [TestMethod]
        public void AesTest3()
        {
            string original = "爱丽丝打开flask对方立刻洒家地方";

            // Encrypt the string to an array of bytes.
            string encrypted = AESHelper.Encrypt(original, "a", "b");

            // Decrypt the bytes to a string.
            string roundtrip = AESHelper.Decrypt(encrypted, "a", "b");

            //Display the original data and the decrypted data.
            Assert.AreEqual(original, roundtrip);

        }
    }
}
