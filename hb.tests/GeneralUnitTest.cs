using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class GeneralUnitTest
    {
        [TestMethod]
        public void ProcessExecTest()
        {
            ProcessExec p = new ProcessExec((str) => {
                Console.WriteLine(str);
            });
            p.Execute("dir");
            bool isFinished = false;
            TaskPro.Wait(ref isFinished);
        }
    }
}
