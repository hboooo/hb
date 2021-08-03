using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace hbo.demo.console
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/4 0:24:23
    /// description:
    /// </summary>
    public class MutiThreadDemo
    {
        private static object ConsoleSyncObj = new object();

        public static void Start(string[] args)
        {
            string[] urls = args;
            if (args.Length == 0)
            {
                urls = new string[]
                {
                    "http://www.cnblogs.com/cuiyansong/",
                    "http://www.baidu.com",
                    "http://www.cnblogs.com/",
                };
            }

            Task[] tasks = new Task[urls.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = DisplayPageSizeAsync(urls[i], i);
            }

            while (!Task.WaitAll(tasks, 10))
            {
                DisplayProgress(tasks);
            }
            Console.SetCursorPosition(0, urls.Length);
            Console.Read();
        }

        private static void DisplayProgress(Task[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted)
                {
                    DisplayProgress((WebRequestState)tasks[i].AsyncState);
                }
            }
        }

        private static void DisplayProgress(WebRequestState state)
        {
            lock (ConsoleSyncObj)
            {
                int left = state.ConsoleColumn;
                int top = state.ConsoleLine;
                if (left > Console.BufferWidth - int.MaxValue.ToString().Length)
                {
                    left = state.Url.Length;
                    Console.SetCursorPosition(left, top);
                    Console.Write("".PadRight(Console.BufferWidth - state.Url.Length));
                    state.ConsoleColumn = left;
                }
                Write(state, ".");
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] magintudes = new string[] { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(1024, magintudes.Length);

            return string.Format("{1:##.##} {0}", magintudes.FirstOrDefault(mag => bytes > (max /= 1024)) ?? "0 Bytes", (decimal)bytes / (decimal)max).Trim();
        }

        private static Task<System.Net.WebResponse> DisplayPageSizeAsync(string url, int i)
        {
            var webRequest = System.Net.WebRequest.Create(url);
            var requestState = new WebRequestState(webRequest, i);

            Write(requestState, url + "  ");

            return Task<System.Net.WebResponse>.Factory.FromAsync(webRequest.BeginGetResponse, GetResponseAsyncCompleted, requestState);
        }

        private static WebResponse GetResponseAsyncCompleted(IAsyncResult asyncResult)
        {
            WebRequestState completedState = (WebRequestState)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)completedState.WebRequest.EndGetResponse(asyncResult);

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                int length = reader.ReadToEnd().Length;
                Write(completedState, FormatBytes(length));
            }
            return response;
        }

        private static void Write(WebRequestState completedState, string text)
        {
            lock (ConsoleSyncObj)
            {
                Console.SetCursorPosition(completedState.ConsoleColumn, completedState.ConsoleLine);
                Console.Write(text);
                completedState.ConsoleColumn += text.Length;
            }
        }

        private class WebRequestState
        {
            public System.Net.WebRequest WebRequest { get; private set; }

            public int ConsoleLine { get; set; }

            public int ConsoleColumn { get; set; }

            public string Url
            {
                get
                {
                    return WebRequest.RequestUri.ToString();
                }
            }

            public WebRequestState(System.Net.WebRequest request)
            {
                WebRequest = request;
            }

            public WebRequestState(System.Net.WebRequest request, int line)
            {
                WebRequest = request;
                ConsoleLine = line;
                ConsoleColumn = 0;
            }
        }
    }
}
