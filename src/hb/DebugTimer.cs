using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace hb
{
    /// <summary>
    /// 时间统计
    /// </summary>
    public class DebugTimer
    {
        [ThreadStatic]
        static Stack<Stopwatch> _stopWatches;

        [Conditional("DEBUG")]
        public static void Start()
        {
            if (_stopWatches == null)
            {
                _stopWatches = new Stack<Stopwatch>();
            }
            _stopWatches.Push(Stopwatch.StartNew());
        }

        [Conditional("DEBUG")]
        public static void Stop(string desc)
        {
            Stopwatch watch = _stopWatches.Pop();
            watch.Stop();
            //TODO: 日志输出
        }

        /// <summary>
        /// Starts a new stopwatch and stops it when the returned object is disposed.
        /// 启动一个定时器，当对象释放时结束定时器
        /// </summary>
        /// <returns>
        /// 返回停止计时器对象(in debug builds); or null (in release builds).
        /// </returns>
        public static IDisposable Time(string text)
        {
#if DEBUG
            Stopwatch watch = Stopwatch.StartNew();
            return new CallbackOnDispose(
                delegate
                {
                    watch.Stop();
                    //TODO:日志输出
                    //LoggingService.Debug("[" + text + "] took " + (watch.ElapsedMilliseconds) + " ms");
                }
            );
#else
			return null;
#endif
        }
    }
}
