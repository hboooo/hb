using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XSocket
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/29 1:09:40
    /// description:
    /// </summary>
    public enum Severity
    {
        /// <summary>
        /// Debug messages.
        /// </summary>
        Debug = 0,
        /// <summary>
        /// Informational messages.
        /// </summary>
        Info = 1,
        /// <summary>
        /// Warning messages.
        /// </summary>
        Warn = 2,
        /// <summary>
        /// Error messages.
        /// </summary>
        Error = 3,
        /// <summary>
        /// Alert messages.
        /// </summary>
        Alert = 4,
        /// <summary>
        /// Critical messages.
        /// </summary>
        Critical = 5,
        /// <summary>
        /// Emergency messages.
        /// </summary>
        Emergency = 6
    }
}
