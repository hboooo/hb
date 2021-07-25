﻿using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Settings for X TCP client
    public class XTcpClientSettings
    {
        #region Public-Members

        /// <summary>
        /// Buffer size to use when reading input and output streams.  Default is 65536.  Value must be greater than zero.
        /// </summary>
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

        /// <summary>
        /// Maximum content length for streams that are proxied through a MemoryStream.
        /// If the content length exceeds this value, the underlying DataStream will be passed in the StreamReceived event.
        /// Value must be greater than zero.
        /// </summary>
        public int MaxProxiedStreamSize
        {
            get
            {
                return _MaxProxiedStreamSize;
            }
            set
            {
                if (value < 1) throw new ArgumentException("MaxProxiedStreamSize must be greater than zero.");
                _MaxProxiedStreamSize = value;
            }
        }

        /// <summary>
        /// Enable or disable message debugging.  Requires `Logger` to be set.
        /// WARNING: Setting this value to true will emit a large number of log messages with a large amount of data.
        /// </summary>
        public bool DebugMessages = false;

        /// <summary>
        /// Method to invoke when sending a log message.
        /// </summary>
        public Action<Severity, string> Logger = null;

        /// <summary>
        /// Enable acceptance of SSL certificates from clients that cannot be validated.
        /// </summary>
        public bool AcceptInvalidCertificates = true;

        /// <summary>
        /// Require mutual authentication between SSL clients and this server.
        /// </summary>
        public bool MutuallyAuthenticate = false;

        /// <summary>
        /// Preshared key that must be consistent between clients and this server.
        /// </summary>
        public string PresharedKey = null;

        /// <summary>
        /// For X TCP client, the number of seconds to wait before timing out a connection attempt.  Default is 5 seconds.  Value must be greater than zero.
        /// </summary>
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

        /// <summary>
        /// Local TCP port.  
        /// Set to '0' to have the underlying TcpClient implementation automatically assign.
        /// Value must be 0, or, 1024 or greater.
        /// </summary>
        public int LocalPort
        {
            get
            {
                return _LocalPort;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Valid values for LocalPort are 0, 1024-65535.");
                }
                else if (value > 0 && value < 1024)
                {
                    throw new ArgumentException("Valid values for LocalPort are 0, 1024-65535."); 
                }
                else if (value > 65535)
                {
                    throw new ArgumentException("Valid values for LocalPort are 0, 1024-65535.");
                }

                _LocalPort = value;
            }
        }

        #endregion

        #region Private-Members

        private int _StreamBufferSize = 65536;
        private int _MaxProxiedStreamSize = 67108864;

        private int _ConnectTimeoutSeconds = 5;
        private int _LocalPort = 0;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public XTcpClientSettings()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
