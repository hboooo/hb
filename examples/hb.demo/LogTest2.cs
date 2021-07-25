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
    /// date       :2021/7/22 21:26:40
    /// description:
    /// </summary>
    public class LogTest2
    {
        public void Output()
        {
            Logger.Debug("LogTest2 message");
            Logger.Error("LogTest2 message");
        }
    }
}
