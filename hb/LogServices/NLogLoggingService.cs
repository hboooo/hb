using System;
using System.Diagnostics;

namespace hb.LogServices
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/22 21:53:51
    /// description:NLog日志输出
    /// </summary>
    public class NLogLoggingService : ILoggingService
    {
        static NLog.Logger log;

        public NLogLoggingService()
        {

        }

        public void Debug(object message)
        {
            Logging.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            Logging.Debug(format, args);
        }

        public void Error(object message)
        {
            Logging.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            Logging.Error(exception, message.ToString());
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            Logging.Error(format, args);
        }

        public void Fatal(object message)
        {
            Logging.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Logging.Fatal(exception, message.ToString());
        }

        public void FatalFormatted(string format, params object[] args)
        {
            Logging.Fatal(format, args);
        }

        public void Info(object message)
        {
            Logging.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            Logging.Info(format, args);
        }

        public void Warn(object message)
        {
            Logging.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            Logging.Warn(exception, message.ToString());
        }

        public void WarnFormatted(string format, params object[] args)
        {
            Logging.Warn(format, args);
        }

        /// <summary>
        /// 当前日志调用者
        /// </summary>
        private NLog.Logger Logging
        {
            get
            {
                Type type = GetMethodInvokeType();
                if (type != null)
                    log = NLog.LogManager.GetLogger(type.FullName);
                else
                    log = NLog.LogManager.GetLogger(typeof(NLogLoggingService).FullName);
                return log;
            }
        }

        /// <summary>
        /// 获取NLogLoggingService的上一级调用堆栈
        /// </summary>
        /// <returns></returns>
        private Type GetMethodInvokeType()
        {
            StackTrace trace = new StackTrace();
            StackFrame[] frames = trace.GetFrames();
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].GetMethod().DeclaringType == typeof(Logger))
                {
                    return frames[i + 1].GetMethod().DeclaringType;
                }
            }
            return null;
        }

        public bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return log.IsWarnEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return log.IsFatalEnabled;
            }
        }
    }
}
