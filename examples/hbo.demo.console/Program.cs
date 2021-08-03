using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hbo.demo.console
{
    class Program
    {

        static void Main(string[] args)
        {
            MutiThreadDemo.Start(args);
            ManualResetEventDemo.Start();
        }
    }
}
