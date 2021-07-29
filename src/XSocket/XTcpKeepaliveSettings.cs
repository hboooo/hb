using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 1:00:56
    /// description:
    /// </summary>
    public class XTcpKeepaliveSettings
    {
        public bool EnableTcpKeepAlives = true;

        public int TcpKeepAliveInterval
        {
            get
            {
                return _TcpKeepAliveInterval;
            }
            set
            {
                if (value < 1) throw new ArgumentException("TcpKeepAliveInterval must be greater than zero.");
                _TcpKeepAliveInterval = value;
            }
        }

        public int TcpKeepAliveTime
        {
            get
            {
                return _TcpKeepAliveTime;
            }
            set
            {
                if (value < 1) throw new ArgumentException("TcpKeepAliveTime must be greater than zero.");
                _TcpKeepAliveTime = value;
            }
        }

        public int TcpKeepAliveRetryCount
        {
            get
            {
                return _TcpKeepAliveRetryCount;
            }
            set
            {
                if (value < 1) throw new ArgumentException("TcpKeepAliveRetryCount must be greater than zero.");
                _TcpKeepAliveRetryCount = value;
            }
        }

        private int _TcpKeepAliveInterval = 5;
        private int _TcpKeepAliveTime = 5;
        private int _TcpKeepAliveRetryCount = 5;
    }
}
