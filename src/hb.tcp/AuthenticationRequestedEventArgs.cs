using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Event arguments for when authentication is requested from a client
    public class AuthenticationRequestedEventArgs
    {
        internal AuthenticationRequestedEventArgs(string ipPort)
        {
            IpPort = ipPort;
        }

        /// <summary>
        /// The IP:port of the client.
        /// </summary>
        public string IpPort { get; }
    }
}
