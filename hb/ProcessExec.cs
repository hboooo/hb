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
        private Process proc = null;

        private Action<string> action;

        public ProcessExec()
        {
            proc = new Process();
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
    }
}
