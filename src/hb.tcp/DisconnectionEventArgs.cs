using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Event arguments for when a disconnection is encountered
    public class DisconnectionEventArgs : EventArgs
    {
        internal DisconnectionEventArgs(string ipPort, DisconnectReason reason)
        {
            IpPort = ipPort;
            Reason = reason;
        }

        /// <summary>
        /// The IP:port of the endpoint for which the disconnection was detected.
        /// </summary>
        public string IpPort { get; }

        /// <summary>
        /// The reason for the disconnection.
        /// </summary>
        public DisconnectReason Reason { get; }
    }
}
