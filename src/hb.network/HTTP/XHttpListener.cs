using hb.LogServices;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:47:14
    /// description:
    /// </summary>
    public class XHttpListener : IDisposable
    {
        private readonly Router router;

        private readonly HttpListener listener = new HttpListener();

        public string Scheme { get; set; } = "http";

        /// <summary>
        /// +代表本机可能的IP
        /// 如localhost,127.0.0.1,192.168.1.X(本机IP)等
        /// 也可指定固定IP
        /// </summary>
        public string Hostname { get; set; } = "+";

        public int Port { get; set; } = 8666;

        public string BaseUrl
        {
            get { return BuildUri(); }
        }

        public bool IsListening
        {
            get { return listener.IsListening; }
        }

        /// <summary>
        /// 是否支持跨域
        /// </summary>
        public bool CORS
        {
            get
            {
                return router.CORS;
            }
            set
            {
                router.CORS = value;
            }
        }

        public XHttpListener()
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            router = new Router();
        }

        public XHttpListener(int port) : this()
        {
            Port = port;
        }


        public bool Run()
        {
            try
            {
                Logger.Info($"Start running, scheme:{Scheme}, host:{Hostname}, port:{Port}, CORS:{CORS}");
                router.GetAllRoutes().ToList().ForEach(path =>
                    {
                        string query = "";
                        if (path.Contains("?"))
                        {
                            query = path.Substring(path.IndexOf("?") + 1);
                            path = path.Substring(0, path.IndexOf("?"));
                        }
                        if (!path.EndsWith("/"))
                            path += "/";
                        string prefixes = BuildUri(path, query);
                        listener.Prefixes.Add(prefixes);
                        Logger.Info($"Add prefixes:{prefixes}");
                    });

                listener.Start();
                ThreadPool.QueueUserWorkItem(HttpListenerHandleAsync);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Listen port error", ex);
            }
            return false;
        }

        private void HttpListenerHandleAsync(object obj)
        {
            try
            {
                Logger.Info("Web server running...");
                while (listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(HttpListenerAction, listener.GetContext());
                }
            }
            catch (HttpListenerException ex)
            {
                Logger.InfoFormatted("Http listener throw an exception, message:{0}", ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void HttpListenerAction(object obj)
        {
            var ctx = obj as HttpListenerContext;
            if (ctx == null) return;
            try
            {
                Logger.InfoFormatted("Request:{0}://{1}:{2}{3}", ctx.Request.Url.Scheme, ctx.Request.Url.Host, ctx.Request.Url.Port, ctx.Request.Url.AbsolutePath);
                Func<HttpListenerRequest, byte[]> handler = router.FindHandler(ctx.Request);
                if (handler == null)
                {
                    Respond404(ctx);
                }
                else
                {
                    HandleRcvRequest(ctx, handler);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Handle request error, request:{ctx.Request.Url.AbsolutePath}", ex);
            }
            finally
            {
                ctx?.Response.OutputStream.Close();
            }
        }

        public void Stop()
        {
            try
            {
                Logger.Info("Web server stopped.");
                listener.Stop();
                listener.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Web server stop error", ex);
            }
        }

        private void HandleRcvRequest(HttpListenerContext ctx, Func<HttpListenerRequest, byte[]> handler)
        {
            try
            {
                byte[] response = handler(ctx.Request);
                Respond200(ctx, response);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormatted("Handle request:{0}://{1}:{2}{3}, statusCode:{4}, description:{5}",
                ctx.Request.Url.Scheme, ctx.Request.Url.Host, ctx.Request.Url.Port, ctx.Request.Url.AbsolutePath, 500, ex.Message);
                Logger.Error(ex);
                Respond500(ctx);
            }
        }

        /// <summary>
        /// 跨域处理
        /// </summary>
        /// <param name="ctx"></param>
        private void SetResponseHeader(HttpListenerContext ctx)
        {
            if (CORS)
            {
                ctx.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                ctx.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
                ctx.Response.AppendHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, DELETE, HEAD, PUT");
                ctx.Response.AppendHeader("Access-Control-Max-Age", "86400");
                ctx.Response.AppendHeader("Access-Control-Allow-Headers", "access-control-allow-origin, authority, content-type, version-info, X-Requested-With");
            }
        }

        public void Respond200(HttpListenerContext ctx, byte[] bytes)
        {
            int statusCode = 200;
            string desc = "The request was fulfilled.";
            Logger.ErrorFormatted("Handle request:{0}://{1}:{2}{3}, statusCode:{4}, description:{5}",
                ctx.Request.Url.Scheme, ctx.Request.Url.Host, ctx.Request.Url.Port, ctx.Request.Url.AbsolutePath, statusCode, desc);

            SetResponseHeader(ctx);
            ctx.Response.StatusCode = statusCode;
            ctx.Response.StatusDescription = desc;
            byte[] buf = bytes;
            ctx.Response.ContentLength64 = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
        }

        public void Respond404(HttpListenerContext ctx)
        {
            int statusCode = 404;
            string desc = "The server has not found anything matching the URI given.";
            Logger.ErrorFormatted("Handle request:{0}://{1}:{2}{3}, statusCode:{4}, description:{5}",
                ctx.Request.Url.Scheme, ctx.Request.Url.Host, ctx.Request.Url.Port, ctx.Request.Url.AbsolutePath, statusCode, desc);

            SetResponseHeader(ctx);
            ctx.Response.StatusCode = statusCode;
            ctx.Response.StatusDescription = desc;
        }

        public void Respond500(HttpListenerContext ctx)
        {
            int statusCode = 500;
            string desc = "The server encountered an unexpected condition which prevented it from fulfilling the request.";
            Logger.ErrorFormatted("Handle request:{0}://{1}:{2}{3}, statusCode:{4}, description:{5}",
                ctx.Request.Url.Scheme, ctx.Request.Url.Host, ctx.Request.Url.Port, ctx.Request.Url.AbsolutePath, statusCode, desc);

            SetResponseHeader(ctx);
            ctx.Response.StatusCode = statusCode;
            ctx.Response.StatusDescription = desc;
        }

        public void ServeStatic(DirectoryInfo directory, string path = "")
        {
            router.ServeStatic(directory, path);
        }

        private string BuildUri(string path = "", string query = "")
        {
            return new UriBuilder(Scheme, Hostname, Port, path, query).ToString();
        }

        public RequestHandlerRegistrator Get
        {
            get { return router.GetRegistrator(Method.GET); }
        }

        public RequestHandlerRegistrator Post
        {
            get { return router.GetRegistrator(Method.POST); }
        }

        public RequestHandlerRegistrator Put
        {
            get { return router.GetRegistrator(Method.PUT); }
        }

        public RequestHandlerRegistrator Delete
        {
            get { return router.GetRegistrator(Method.DELETE); }
        }

        public RequestHandlerRegistrator Options
        {
            get { return router.GetRegistrator(Method.OPTIONS); }
        }

        public RequestHandlerRegistrator Head
        {
            get { return router.GetRegistrator(Method.HEAD); }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (listener.IsListening)
                        Stop();
                }

                disposed = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
