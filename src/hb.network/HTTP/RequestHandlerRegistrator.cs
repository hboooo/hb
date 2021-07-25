using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:48:15
    /// description:
    /// </summary>
    public class RequestHandlerRegistrator
    {
        public RequestHandlerRegistrator(Method method)
        {
            httpMethod = method;
        }

        public Method Method
        {
            get { return httpMethod; }
        }
        private readonly Method httpMethod;

        public Dictionary<string, Func<HttpListenerRequest, string>> Handlers
        {
            get { return handlers; }
        }
        private readonly Dictionary<string, Func<HttpListenerRequest, string>> handlers = new Dictionary<string, Func<HttpListenerRequest, string>>();

        public Func<HttpListenerRequest, string> this[string path]
        {
            get { return handlers[path]; }
            set { handlers[path] = value; }
        }
    }
}
