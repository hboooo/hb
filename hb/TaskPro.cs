using System;
using System.Threading;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/23 1:15:58
    /// description:Task帮助类
    /// </summary>
    public class TaskPro
    {
        public static Task Delay(int dueTime)
        {
            if (dueTime < -1) throw new ArgumentOutOfRangeException("dueTime");

            Timer timer = null;
            var source = new TaskCompletionSource<bool>();
            timer = new Timer(_ =>
            {
                using (timer) source.TrySetResult(true);
            }, null, dueTime, Timeout.Infinite);

            return source.Task;
        }

        public static void Wait(ref bool isFinished, int timeout = 1000)
        {
            int sleepspan = 10;
            int i = 0;
            while (isFinished == false && (i + 1) * sleepspan < timeout)
            {
                Thread.Sleep(sleepspan);
                i++;
            }
        }
    }
}
