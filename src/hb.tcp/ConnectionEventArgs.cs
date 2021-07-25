using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Event arguments for when a connection is established
    public class ConnectionEventArgs : EventArgs
    {
        internal ConnectionEventArgs(string ipPort)
        {
            IpPort = ipPort;
        }

        /// <summary>
        /// The IP:port of the endpoint to which the connection was established.
        /// </summary>
        public string IpPort { get; }
    }
}
