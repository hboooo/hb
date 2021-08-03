using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace hbo.demo.console
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/4 0:26:38
    /// description:
    /// </summary>
    public class ManualResetEventDemo
    {
        public static void Start()
        {
            var signal = new ManualResetEvent(false);
            new Thread(() => {
                Console.WriteLine("Waiting for signal ...");
                signal.WaitOne();//阻塞当前线程
                signal.Dispose();
                Console.WriteLine("Go signal! ...");
                Console.ReadLine();
            }).Start();

            Thread.Sleep(3000);
            signal.Set();//打开了信号
        }
    }
}
