using System;

namespace hb.LogServices
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/22 20:25:48
    /// description:日志系统
    /// </summary>
    public sealed class Logger
    {

        private static ILoggingService _logger;

        /// <summary>
        /// log4net日志模块名称
        /// </summary>
        public static readonly string LOG4NET = "log4net";

        /// <summary>
        /// NLog日志模块名称
        /// </summary>
        public static readonly string NLOG = "nlog";

        /// <summary>
        /// 初始化日志模块
        /// </summary>
        /// <param name="type">日志模块名称</param>
        public static void Initialize(string type = null)
        {
            if (string.IsNullOrEmpty(type) || string.Compare(type, LOG4NET, true) == 0)
                _logger = new Log4netLoggingService();
            else if (string.Compare(type, NLOG, true) == 0)
                _logger = new NLogLoggingService();
            else
                throw new NotSupportedException(type);
        }

        public static void Debug(object message)
        {
            _logger.Debug(message);
        }

        public static void DebugFormatted(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public static void Error(object message)
        {
            _logger.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        public static void ErrorFormatted(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public static void Fatal(object message)
        {
            _logger.Fatal(message);
        }

        public static void Fatal(object message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        public static void FatalFormatted(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public static void Info(object message)
        {
            _logger.Info(message);
        }

        public static void InfoFormatted(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public static void Warn(object message)
        {
            _logger.Warn(message);
        }

        public static void Warn(object message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        public static void WarnFormatted(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }
    }
}
