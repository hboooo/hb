using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 1:39:16
    /// description:
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        internal ConnectionEventArgs(string ipPort)
        {
            IpPort = ipPort;
        }


        public string IpPort { get; }
    }
}
