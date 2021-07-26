using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XTcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/27 0:52:31
    /// description:Stream写入和读取扩展
    /// </summary>
    public static class StreamExtensions
    {
        public static byte ReadByte(this Stream stream)
        {
            return (byte)stream.ReadByte();
        }

    }
}
