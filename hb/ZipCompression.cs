using hb.LogServices;
using System;
using System.IO;
using System.IO.Compression;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 1:38:08
    /// description:解压缩
    /// </summary>
    public class ZipCompression
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] bytes)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    using (var gzip = new GZipStream(memory, CompressionMode.Compress))
                    {
                        gzip.Write(bytes, 0, bytes.Length);
                    }
                    return memory.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
        
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] bytes)
        {
            try
            {
                using (var memory = new MemoryStream(bytes))
                {
                    using (var gzip = new GZipStream(memory, CompressionMode.Decompress))
                    {
                        using (var result = new MemoryStream())
                        {
                            gzip.CopyTo(result);
                            return result.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
    }
}
