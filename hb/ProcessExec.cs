using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 13:46:32
    /// description:
    /// </summary>
    public class ProcessExec
    {
        private Action<string> action;

        public ProcessExec()
        {

        }

        public ProcessExec(Action<string> outputAction) : this()
        {
            action = outputAction ?? throw new ArgumentNullException(nameof(outputAction));
        }

        public void Execute(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentNullException(nameof(cmd));
            }
            try
            {
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.OutputDataReceived += Proc_OutputDataReceived;
                proc.Start();
                StreamWriter cmdWriter = proc.StandardInput;
                proc.BeginOutputReadLine();
                cmdWriter.WriteLine(cmd);
                cmdWriter.Close();
                proc.WaitForExit();
                proc.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                action?.Invoke(e.Data);
        }

        /// <summary>
        /// 进程是否存在
        /// </summary>
        /// <param name="name">进程名称</param>
        /// <returns></returns>
        public static bool Exist(string name)
        {
            try
            {
                var p = Process.GetProcessesByName(name);
                if (p.Length > 0) return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Start(string name, bool waitForExit = false, string args = "")
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = name;
                p.StartInfo.Arguments = args;
                p.StartInfo.UseShellExecute = true;
                p.Start();
                if (waitForExit)
                {
                    p.WaitForExit();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="name">进程名</param>
        public static void Kill(string name)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(name);
                foreach (Process p in processes)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
