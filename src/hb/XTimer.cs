using hb.LogServices;
using System;
using System.Threading;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/23 1:19:08
    /// description:定时器
    /// </summary>
    public class XTimer : IDisposable
    {
        private readonly Action f_Action;

        private readonly Timer f_Timer = null;

        private ManualResetEvent f_WaitHandle = new ManualResetEvent(false);

        private int f_timespan = 0;

        private int f_LastTimespan = Timeout.Infinite;
        /// <summary>
        /// 任务定时器
        /// </summary>
        /// <param name="action">定时任务</param>
        /// <param name="delay">启动延时</param>
        /// <param name="timespan">启动执行时间间隔</param>
        public XTimer(Action action, int delay, int timespan)
        {
            f_Action = action;
            f_timespan = timespan;
            f_LastTimespan = f_timespan;

            f_Timer = new Timer(state =>
            {
                f_WaitHandle.Reset();
                this.f_Timer.Change(Timeout.Infinite, Timeout.Infinite);
                try
                {
                    f_Action.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.Error("Task timer action error", ex);
                }
                finally
                {
                    try
                    {
                        this.f_Timer.Change(delay, f_timespan);
                    }
                    catch
                    {
                    }
                }
            }, null, delay, f_timespan);
            Logger.Debug($"Start task timer, action:{f_Action.Method}, delay:{delay}, timespan:{f_timespan}");
        }

        /// <summary>
        /// 修改计时器触发事件时间间隔，单位毫秒
        /// </summary>
        /// <param name="timespan"></param>
        public void ChangeInterval(int timespan)
        {
            if (timespan < -1) return;
            f_timespan = timespan;
            f_LastTimespan = f_timespan;
        }

        /// <summary>
        /// 启动计时器
        /// </summary>
        public void Start()
        {
            f_timespan = f_LastTimespan;
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop()
        {
            f_timespan = Timeout.Infinite;
        }

        public void Dispose()
        {
            try
            {
                f_WaitHandle.Close();
                f_Timer?.Dispose();
                Logger.Debug($"Dispose task timer action, action:{f_Action.Method}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
