using hb.LogServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:50:18
    /// description:
    /// </summary>
    public class Router
    {
        /// <summary>
        /// 是否支持跨域
        /// </summary>
        internal bool CORS { get; set; } = false;

        public Router()
        {
            registrators = new List<RequestHandlerRegistrator>(); ;
            servedStatic = new Dictionary<string, DirectoryInfo>();
        }

        private readonly List<RequestHandlerRegistrator> registrators;

        private readonly Dictionary<string, DirectoryInfo> servedStatic;

        public RequestHandlerRegistrator GetRegistrator(Method method)
        {
            var res = registrators.FirstOrDefault(r => r.Method == method);
            if (res == null)
            {
                res = new RequestHandlerRegistrator(method);
                registrators.Add(res);
            }
            return res;
        }

        public IEnumerable<string> GetAllRoutes()
        {
            return registrators
                .SelectMany(r => r.Handlers.Keys)
                .Union(servedStatic.Keys);
        }

        public Func<HttpListenerRequest, byte[]> FindHandler(HttpListenerRequest request)
        {
            Func<HttpListenerRequest, byte[]> res = null;

            var requestMethod = request.GetHttpMethod();
            //如果支持跨域则自动处理options
            if (requestMethod == Method.OPTIONS && CORS)
            {
                res = _ => new byte[0];
            }

            if (res == null)
            {
                var registrator = registrators.FirstOrDefault(r => r.Method == requestMethod);
                Logger.DebugFormatted("{0}, {1}", requestMethod, request.Url.AbsolutePath);
                if (registrator != null)
                {
                    res = registrator.Handlers
                        .Where(kv => kv.Key == request.Url.AbsolutePath)
                        .Select(kv => kv.Value)
                        .FirstOrDefault();
                }

                //静态资源
                if (res == null && requestMethod == Method.GET)
                {
                    string staticMatch = servedStatic.Keys.FirstOrDefault(k => request.Url.AbsolutePath.StartsWith(k));
                    if (staticMatch != null)
                    {
                        string fileRelPath = request.Url.AbsolutePath.Substring(staticMatch.Length);
                        if (fileRelPath == "" || fileRelPath == "index")
                            fileRelPath = "index.html";
                        string fileAbsPath = Path.Combine(servedStatic[staticMatch].FullName, fileRelPath);
                        if (File.Exists(fileAbsPath))
                        {
                            try
                            {
                                res = _ => File.ReadAllBytes(fileAbsPath);
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// 静态资源访问
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="path"></param>
        public void ServeStatic(DirectoryInfo directory, string path = "")
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (servedStatic.ContainsKey(path))
                throw new ArgumentException("Directory is already served statically at path " + path, "path");

            path = path.Trim();
            if (path == "")
                path = "/";
            else
            {
                if (!path.StartsWith("/"))
                    path = "/" + path;
                if (!path.EndsWith("/"))
                    path += "/";
            }

            servedStatic[path] = directory;
        }
    }
}
