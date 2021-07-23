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
        public static string GetMD5File(string fileName)
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
                byte[] output = md5.ComputeHash(file);
                return BytesToHexString(output);
            }
        }

        /// <summary>
        /// 计算byte[]的MD5值
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetMd5Bytes(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(bytes);
            return BytesToHexString(output);
        }

        /// <summary>
        /// 计算string的MD5值
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string GetMd5String(this string @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            return @this.GetMd5String(Encoding.Default);
        }

        /// <summary>
        /// 计算string的MD5值
        /// </summary>
        /// <param name="this">指定string的编码格式</param>
        /// <returns></returns>
        public static string GetMd5String(this string @this, Encoding encode)
        {
            if (@this == null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            byte[] bytes = encode.GetBytes(@this);
            return GetMd5Bytes(bytes);
        }

        /// <summary>
        /// byte[]转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
