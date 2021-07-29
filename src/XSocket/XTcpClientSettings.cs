using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 0:57:55
    /// description:
    /// </summary>
    public class XTcpClientSettings
    {
        public Action<Severity, string> Logger = null;


        private int _ConnectTimeoutSeconds = 5;
        private int _StreamBufferSize = 65536;

        public int ConnectTimeoutSeconds
        {
            get
            {
                return _ConnectTimeoutSeconds;
            }
            set
            {
                if (value < 1) throw new ArgumentException("ConnectTimeoutSeconds must be greater than zero.");
                _ConnectTimeoutSeconds = value;
            }
        }

        public int StreamBufferSize
        {
            get
            {
                return _StreamBufferSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("Read stream buffer size must be greater than zero.");
                _StreamBufferSize = value;
            }
        }
    }
}
