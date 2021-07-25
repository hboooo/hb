using hb.LogServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:50:18
    /// description:
    /// </summary>
    public class Router
    {
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

        public Func<HttpListenerRequest, string> FindHandler(HttpListenerRequest request)
        {
            Func<HttpListenerRequest, string> res = null;

            var requestMethod = request.GetHttpMethod();
            var registrator = registrators.FirstOrDefault(r => r.Method == requestMethod);
            Logger.DebugFormatted("{0}, {1}", requestMethod, request.Url.AbsolutePath);
            if (registrator != null)
            {
                res = registrator.Handlers
                    .Where(kv => kv.Key == request.Url.AbsolutePath)
                    .Select(kv => kv.Value)
                    .FirstOrDefault();
            }
            return res;
        }

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
