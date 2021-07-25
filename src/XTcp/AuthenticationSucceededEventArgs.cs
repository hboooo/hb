using System;
using System.Collections.Generic;
using System.Text;

namespace XTcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:
    public class AuthenticationSucceededEventArgs
    {
        internal AuthenticationSucceededEventArgs(string ipPort)
        {
            IpPort = ipPort;
        }

        /// <summary>
        /// The IP:port of the client.
        /// </summary>
        public string IpPort { get; }
    }
}
