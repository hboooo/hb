using System;
using System.Collections.Generic;
using System.Text;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Event arguments for when an exception is encountered
    public class ExceptionEventArgs
    {
        internal ExceptionEventArgs(Exception e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            Exception = e;
            Json = XSerialization.SerializeJson(e, true);
        }

        /// <summary>
        /// Exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// JSON representation of the exception.
        /// </summary>
        public string Json { get; }
    }
}
