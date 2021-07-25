using hb.LogServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 13:46:32
    /// description:
    /// </summary>
    public static class XProcess
    {
        public static Action<string> OutputAction;

        public static string Execute(string cmd, bool waitForExit = false)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";                  //设置要启动的应用程序
            p.StartInfo.UseShellExecute = false;               //设置是否使用操作系统shell启动进程
            p.StartInfo.RedirectStandardInput = true;          //指示应用程序是否从StandardInput流中读取
            p.StartInfo.RedirectStandardOutput = true;         //将应用程序的输入写入到StandardOutput流中
            p.StartInfo.RedirectStandardError = true;          //将应用程序的错误输出写入到StandardError流中
            p.StartInfo.CreateNoWindow = true;                 //是否在新窗口中启动进程
            string strOutput;
            try
            {
                p.OutputDataReceived += Proc_OutputDataReceived;
                p.Start();
                p.StandardInput.WriteLine(cmd);            //将CMD命令写入StandardInput流中
                p.StandardInput.WriteLine("exit");         //将 exit 命令写入StandardInput流中
                strOutput = p.StandardOutput.ReadToEnd();  //读取所有输出的流的所有字符
                if (waitForExit)
                {
                    p.WaitForExit();         //无限期等待，直至进程退出
                }
                p.Close();                   //释放进程，关闭进程
            }
            catch (Exception e)
            {
                strOutput = e.Message;
            }

            return strOutput;
        }

        private static void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                OutputAction?.Invoke(e.Data);
        }

        /// <summary>
        /// 可执行文件是否运行
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns>是否运行</returns>
        public static bool IsRun(this FileInfo file)
        {
            return file.Exists && Process.GetProcesses().Any(oth => string.Equals(oth.ProcessName, file.NameWithoutExt(), StringComparison.CurrentCultureIgnoreCase));
        }


        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="name">进程名</param>
        public static void Kill(string filename)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    try
                    {
                        if (!string.Equals(p.ProcessName, filename, StringComparison.CurrentCultureIgnoreCase))
                        {
                            continue;
                        }

                        p.Kill();
                        p.Close();
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

        /// <summary>
        /// 运行文件，当文件已经运行时调至前台
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="bringToFrontIfRun">运行时调至前台</param>
        public static Process RunExe(this FileInfo file, bool bringToFrontIfRun = true)
        {
            if (file == null || !file.Exists)
            {
                return null;
            }

            if (bringToFrontIfRun)
            {
                foreach (Process oth in Process.GetProcesses())
                {
                    if (!string.Equals(oth.ProcessName, file.Name.Replace(file.Extension, ""), StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    HWND hWnd = oth.MainWindowHandle;
                    ShowWindowAsync(hWnd, (int)ProcessWindowStyle.Maximized);
                    SetForegroundWindow(hWnd);
                    return oth;
                }
            }

            if (file.Directory != null)
            {
                Directory.SetCurrentDirectory(Path.GetFullPath(file.Directory.FullName));
            }

            return Process.Start(file.FullName);

        }


        /// <summary>
        /// 运行dos命令
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>System.String.</returns>
        public static string RunDosCmd(string command)
        {
            string str;
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                if (process == null) return string.Empty;
                using (StreamReader reader = process.StandardOutput)
                {
                    str = reader.ReadToEnd();
                }

                process.WaitForExit();
            }

            return str.Trim();
        }



        [DllImport("user32")]
        public static extern int ShowWindowAsync(HWND hwnd, int nCmdShow);

        [DllImport("user32")]
        public static extern int SetForegroundWindow(HWND hwnd);
    }
}
