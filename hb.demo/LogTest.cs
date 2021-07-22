using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb.demo
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/22 21:18:40
    /// description:
    /// </summary>
    public class LogTest
    {
        public void Output()
        {
            Logger.Debug("LogTest message");
            Test();
        }

        public void Test()
        {
            LogTest2 l = new LogTest2();
            l.Output();
        }
    }
}
