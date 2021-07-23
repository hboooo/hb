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
                throw new ArgumentNullException(nameof(@this));
            }
            return ToHexString(encode.GetBytes(@this));
        }

        public static string ToHexString(this byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
		/// Hexadecimal string to an byte array.
		/// </summary>
		/// <param name="hex">The hex string.</param>
		/// <returns>An byte array.</returns>
		public static byte[] HexToByte(this string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


    }
}
