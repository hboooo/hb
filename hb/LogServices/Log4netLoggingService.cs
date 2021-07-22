using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace hb.LogServices
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/22 20:11:40
    /// description:log4net日志输出
    /// </summary>
    public sealed class Log4netLoggingService : ILoggingService
    {
        ILog log;

        public Log4netLoggingService()
        {
            //从app.config中加载配置
            //XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

            //从当前动态库中加载配置 
            string config = $"{ Assembly.GetExecutingAssembly().GetName().Name}.dll.config";
            XmlConfigurator.ConfigureAndWatch(new FileInfo(config));
        }

        public void Debug(object message)
        {
            Logging.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            Logging.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Error(object message)
        {
            Logging.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            Logging.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            Logging.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Fatal(object message)
        {
            Logging.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            Logging.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            Logging.FatalFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Info(object message)
        {
            Logging.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            Logging.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Warn(object message)
        {
            Logging.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            Logging.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            Logging.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }

        private ILog Logging
        {
            get
            {
                Type type = GetMethodInvokeType();
                if (type != null)
                    log = LogManager.GetLogger(type);
                else
                    log = LogManager.GetLogger(typeof(Log4netLoggingService));
                return log;
            }
        }

        /// <summary>
        /// 获取Logger的上一级调用堆栈
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
