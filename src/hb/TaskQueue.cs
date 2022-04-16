using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Threading;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/23 1:17:00
    /// description:生产消费队列
    /// </summary>
    public class TaskQueue<T> : IDisposable
    {
        private object f_Locker = new object();

        private Thread[] f_Threads;

        private int f_ThreadCount;

        private Queue<T> f_TaskQueue = new Queue<T>();

        private Action<T> f_Action;

        private Action f_ComplateAction;

        public const int Threads = 2;

        /// <summary>
        /// 初始化一个生产消费队列
        /// 队列中启动的线程个数 2
        /// </summary>
        /// <param name="action">消费者方法</param>
        public TaskQueue(Action<T> action) : this(action, Threads)
        {

        }

        /// <summary>
        /// 初始化一个生产消费队列
        /// </summary>
        /// <param name="action">消费者方法</param>
        /// <param name="threadCount">队列中启动的线程个数</param>
        public TaskQueue(Action<T> action, int threadCount) : this(action, null, threadCount)
        {

        }

        /// <summary>
        /// 初始化一个生产消费队列
        /// </summary>
        /// <param name="action">消费者方法</param>
        /// <param name="complateAction">所有线程完成后回调</param>
        /// <param name="threadCount">队列中启动的线程个数</param>
        public TaskQueue(Action<T> action, Action complateAction, int threadCount)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (threadCount < 1)
            {
                throw new ArgumentException("threadCount must greater than 0");
            }

            f_Action = action;
            f_ComplateAction = complateAction;
            f_ThreadCount = threadCount;
            Start();
        }

        private void Start()
        {
            f_Threads = new Thread[f_ThreadCount];

            for (int i = 0; i < f_ThreadCount; i++)
            {
                (f_Threads[i] = new Thread(Consume)).Start();
            }
            Logger.Debug($"TaskQueue started, action:{f_Action.Method} thread count:{f_ThreadCount} ");
        }

        /// <summary>
        /// 添加一个即将消费的对象
        /// </summary>
        /// <param name="task"></param>
        public void EnqueueTask(T task)
        {
            lock (f_Locker)
            {
                f_TaskQueue.Enqueue(task);
                Monitor.PulseAll(f_Locker);
            }
        }

        /// <summary>
        /// 消费队列中的对象
        /// </summary>
        private void Consume()
        {
            while (true)
            {
                T task;
                lock (f_Locker)
                {
                    while (f_TaskQueue.Count == 0) Monitor.Wait(f_Locker);
                    task = f_TaskQueue.Dequeue();
                }
                if (task == null || task.Equals(default(T))) return;
                try
                {
                    f_Action?.Invoke(task);
                }
                catch (Exception ex)
                {
                    Logger.Error("TaskQueue action consume error", ex);
                }
                Thread.Sleep(0);
            }
        }

        /// <summary>
        /// 释放当前队列
        /// 在所有任务执行完成后关闭所有消费者线程。
        /// </summary>
        public void Dispose()
        {
            foreach (var thread in f_Threads)
            {
                EnqueueTask(default(T));
            }
            foreach (var thread in f_Threads)
            {
                thread.Join();
            }
            try
            {
                f_ComplateAction?.Invoke();
            }
            catch (Exception ex)
            {
                Logger.Error("TaskQueue complate action invoke error", ex);
            }
            Logger.Debug($"TaskQueue disposed, action:{f_Action.Method}");
        }
    }
}
