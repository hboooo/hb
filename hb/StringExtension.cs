using System;
using System.Text;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/21 1:51:24
    /// description:字符串扩展
    /// </summary>
    public static class StringExtension
    {
        public static string ToHexString(this string @this, Encoding encode)
        {
            if (string.IsNullOrEmpty(@this))
            {
                throw new ArgumentNullException("@this");
            }
            return ToHexString(encode.GetBytes(@this));
        }

        public static string ToHexString(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
