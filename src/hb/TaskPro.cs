using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// <summary>
        /// 多线程处理
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="list">待处理数据列表</param>
        /// <param name="action">数据处理方法</param>
        /// <param name="threadCount">线程数量</param>
        /// <param name="waitFlag">是否等待执行结束</param>
        public static void RunTask<T>(List<T> list, Action<T> action, int threadCount = 5, bool waitFlag = true)
        {
            ConcurrentQueue<T> queue = new ConcurrentQueue<T>(list);
            Task[] tasks = new Task[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (queue.TryDequeue(out T t))
                    {
                        action(t);
                    }
                });
            }
            if (waitFlag)
            {
                Task.WaitAll(tasks);
            }
        }

        /// <summary>
        /// 多线程处理数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="list">待处理数据列表</param>
        /// <param name="func">数据处理方法</param>
        /// <param name="threadCount">线程数量</param>
        /// <returns></returns>
        public static List<T> RunTask<T>(List<T> list, Func<T, T> func, int threadCount = 5)
        {
            var result = new List<T>();
            ConcurrentQueue<T> queue = new ConcurrentQueue<T>(list);
            Task<List<T>>[] tasks = new Task<List<T>>[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var taskResultList = new List<T>();
                    while (queue.TryDequeue(out T t))
                    {
                        taskResultList.Add(func(t));
                    }
                    return taskResultList;
                });
            }
            Task.WaitAll(tasks);
            for (int i = 0; i < threadCount; i++)
            {
                result.AddRange(tasks[i].Result);
            }
            return result;
        }

        /// <summary>
        /// 线程延迟调用处理
        /// </summary>
        /// <param name="dueTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 线程等待
        /// </summary>
        /// <param name="isFinished"></param>
        /// <param name="timeout"></param>
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
