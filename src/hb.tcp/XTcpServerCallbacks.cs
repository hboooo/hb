using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:X TCP server callbacks
    public class XTcpServerCallbacks
    {
        #region Public-Members

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
        public XTcpServerCallbacks()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Internal-Methods

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
