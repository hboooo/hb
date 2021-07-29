using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 1:52:49
    /// description:
    /// </summary>
    public class Utils
    {
        internal static void BytesToStream(byte[] data, int start, out int contentLength, out Stream stream)
        {
            contentLength = 0;
            stream = new MemoryStream(new byte[0]);

            if (data != null && data.Length > 0)
            {
                contentLength = (data.Length - start);
                stream = new MemoryStream();
                stream.Write(data, start, contentLength);
                stream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
