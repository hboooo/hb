using System;
using System.Security.Cryptography;
using System.Text;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 15:20:33
    /// description:RSA加密
    /// RSA方式加密 
    /// KEY必须是XML的形式,返回的是字符串 
    /// 该加密方式有长度限制
    /// </summary>
    public class RSAHelper
    {
        /// <summary>
        /// RSA产生密钥
        /// </summary>
        /// <param name="xmlKeys"></param>
        /// <param name="xmlPublicKey"></param>
        public static void CreateKey(out string xmlKeys, out string xmlPublicKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                xmlKeys = rsa.ToXmlString(true);
                xmlPublicKey = rsa.ToXmlString(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// RSA的加密函数
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns></returns>
        public static string Encrypt(string xmlPublicKey, string encryptString)
        {
            byte[] encryptBytes = Encoding.UTF8.GetBytes(encryptString);
            return Encrypt(xmlPublicKey, encryptBytes);
        }

        /// <summary>
        /// RSA的加密函数 
        /// </summary>
        /// <param name="xmlPublicKey">公钥</param>
        /// <param name="encryptBytes">待加密的字节数组</param>
        /// <returns></returns>
        public static string Encrypt(string xmlPublicKey, byte[] encryptBytes)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            byte[] cypherTextBArray = rsa.Encrypt(encryptBytes, false);
            return Convert.ToBase64String(cypherTextBArray);
        }

        /// <summary>
        /// RSA的解密函数
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns></returns>
        public static string Decrypt(string xmlPrivateKey, string decryptString)
        {
            byte[] plainTextBArray = Convert.FromBase64String(decryptString);
            return Decrypt(xmlPrivateKey, plainTextBArray);
        }

        /// <summary>
        /// RSA的解密函数 
        /// </summary>
        /// <param name="xmlPrivateKey">私钥</param>
        /// <param name="decryptBytes">待解密的字节数组</param>
        /// <returns></returns>
        public static string Decrypt(string xmlPrivateKey, byte[] decryptBytes)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            byte[] dypherTextBArray = rsa.Decrypt(decryptBytes, false);
            return Encoding.UTF8.GetString(dypherTextBArray);
        }


        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="hashbyteSignature">待签名Hash描述</param>
        /// <returns></returns>
        public static byte[] SignatureFormatter(string strPrivateKey, byte[] hashbyteSignature)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(strPrivateKey);
            RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(RSA);
            //设置签名的算法为MD5 
            RSAFormatter.SetHashAlgorithm("MD5");
            //执行签名 
            return RSAFormatter.CreateSignature(hashbyteSignature);
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strPrivateKey"></param>
        /// <param name="strHashbyteSignature"></param>
        /// <returns></returns>
        public static byte[] SignatureFormatter(string strPrivateKey, string strHashbyteSignature)
        {
            byte[] hashbyteSignature = Convert.FromBase64String(strHashbyteSignature);
            return SignatureFormatter(strPrivateKey, hashbyteSignature);
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strPrivateKey">私钥</param>
        /// <param name="hashbyteSignature">待签名Hash描述</param>
        /// <returns></returns>
        public static string SignatureFormatterToString(string strPrivateKey, byte[] hashbyteSignature)
        {
            return Convert.ToBase64String(SignatureFormatter(strPrivateKey, hashbyteSignature));
        }

        /// <summary>
        /// RSA签名
        /// </summary>
        /// <param name="strPrivateKey"></param>
        /// <param name="strHashbyteSignature"></param>
        /// <returns></returns>
        public static string SignatureFormatterToString(string strPrivateKey, string strHashbyteSignature)
        {
            return Convert.ToBase64String(SignatureFormatter(strPrivateKey, strHashbyteSignature));
        }

        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="hashbyteDeformatter">Hash描述</param>
        /// <param name="deformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strPublicKey, byte[] hashbyteDeformatter, byte[] deformatterData)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(strPublicKey);
            RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(RSA);
            //指定解密的时候HASH算法为MD5 
            RSADeformatter.SetHashAlgorithm("MD5");
            if (RSADeformatter.VerifySignature(hashbyteDeformatter, deformatterData))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="strHashbyteDeformatter">Hash描述</param>
        /// <param name="deformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strPublicKey, string strHashbyteDeformatter, byte[] deformatterData)
        {
            byte[] hashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);
            return SignatureDeformatter(strPublicKey, hashbyteDeformatter, deformatterData);
        }

        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="hashbyteDeformatter">Hash描述</param>
        /// <param name="strDeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strPublicKey, byte[] hashbyteDeformatter, string strDeformatterData)
        {
            byte[] deformatterData = Convert.FromBase64String(strDeformatterData);
            return SignatureDeformatter(strPublicKey, hashbyteDeformatter, deformatterData);
        }

        /// <summary>
        /// RSA签名验证
        /// </summary>
        /// <param name="strPublicKey">公钥</param>
        /// <param name="strHashbyteDeformatter">Hash描述</param>
        /// <param name="strDeformatterData">签名后的结果</param>
        /// <returns></returns>
        public static bool SignatureDeformatter(string strPublicKey, string strHashbyteDeformatter, string strDeformatterData)
        {
            byte[] hashbyteDeformatter = Convert.FromBase64String(strHashbyteDeformatter);
            byte[] deformatterData = Convert.FromBase64String(strDeformatterData);
            return SignatureDeformatter(strPublicKey, hashbyteDeformatter, deformatterData);
        }

    }
}
