using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 1:33:55
    /// description:
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        internal ExceptionEventArgs(Exception e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            Exception = e;
        }

        /// <summary>
        /// Exception.
        /// </summary>
        public Exception Exception { get; }

    }
}
