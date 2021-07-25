using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:18:58
    /// description:MD5摘要算法
    /// </summary>
    public static class MD5Utils
    {
        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileMD5(string fileName, MD5Output output = MD5Output.Base64)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(fileName);
            }
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(fileName);
            }

            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] bytes = md5.ComputeHash(file);
                return GetMD5Output(bytes, output);
            }
        }

        /// <summary>
        /// 计算byte[]的MD5值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetBytesMD5(this byte[] bytes, MD5Output output = MD5Output.Base64)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] datas = md5.ComputeHash(bytes);
            return GetMD5Output(datas, output);
        }

        /// <summary>
        /// 计算string的MD5值
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string GetStringMD5(this string @this, MD5Output output = MD5Output.Base64)
        {
            return @this.GetStringMD5(Encoding.Default, output);
        }

        /// <summary>
        /// 计算string的MD5值
        /// </summary>
        /// <param name="this">指定string的编码格式</param>
        /// <returns></returns>
        public static string GetStringMD5(this string @this, Encoding encode, MD5Output output = MD5Output.Base64)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            byte[] bytes = encode.GetBytes(@this);
            return GetBytesMD5(bytes, output);
        }

        private static string GetMD5Output(byte[] bytes, MD5Output output)
        {
            string result;
            switch (output)
            {
                case MD5Output.Base64:
                    result = Convert.ToBase64String(bytes);
                    break;
                case MD5Output.Hex:
                    result = bytes.ToHexString();
                    break;
                default:
                    result = Convert.ToBase64String(bytes);
                    break;
            }
            return result;
        }
    }

    /// <summary>
    /// MD5输出字符串格式
    /// </summary>
    public enum MD5Output
    {
        /// <summary>
        /// base64 string 
        /// </summary>
        Base64,
        /// <summary>
        /// hex string
        /// </summary>
        Hex
    }
}
