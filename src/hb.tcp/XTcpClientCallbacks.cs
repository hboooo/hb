using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:X TCP client callbacks
    public class XTcpClientCallbacks
    {
        #region Public-Members

        /// <summary>
        /// Function called when authentication is requested from the server.  Expects the 16-byte preshared key.
        /// </summary>
        public Func<string> AuthenticationRequested = null;

        /// <summary>
        /// Callback to invoke when receiving a synchronous request that demands a response.
        /// </summary>
        public Func<XRequest, XResponse> SyncRequestReceived
        {
            get
            {
                return _SyncRequestReceived;
            }
            set
            {
                _SyncRequestReceived = value;
            }
        }

        #endregion

        #region Private-Members

        private Func<XRequest, XResponse> _SyncRequestReceived = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public XTcpClientCallbacks()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Internal-Methods

        internal string HandleAuthenticationRequested()
        {
            string ret = null;

            if (AuthenticationRequested != null)
            {
                try
                {
                    ret = AuthenticationRequested();
                }
                catch (Exception)
                {

                }
            }

            return ret;
        }

        internal XResponse HandleSyncRequestReceived(XRequest req)
        {
            XResponse ret = null;

            if (SyncRequestReceived != null)
            {
                try
                {
                    ret = SyncRequestReceived(req);
                }
                catch (Exception)
                {

                }
            }

            return ret;
        }

        #endregion

        #region Private-Methods
         
        #endregion
    }
}
