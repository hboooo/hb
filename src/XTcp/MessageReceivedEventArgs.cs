﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XTcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Event arguments for when a message is received
    public class MessageReceivedEventArgs : EventArgs
    {
        internal MessageReceivedEventArgs(string ipPort, Dictionary<object, object> metadata, byte[] data)
        {
            IpPort = ipPort;
            Metadata = metadata;
            Data = data;
        }

        /// <summary>
        /// The IP:port of the endpoint.
        /// </summary>
        public string IpPort { get; }

        /// <summary>
        /// The metadata received from the endpoint.
        /// </summary>
        public Dictionary<object, object> Metadata { get; }

        /// <summary>
        /// The data received from the endpoint.
        /// </summary>
        public byte[] Data { get; }
    }
}
