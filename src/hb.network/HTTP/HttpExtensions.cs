using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:51:49
    /// description:
    /// </summary>
    public static class HttpExtensions
    {
        public static Method GetHttpMethod(this HttpListenerRequest request)
        {
            return (Method)Enum.Parse(typeof(Method), request.HttpMethod, ignoreCase: true);
        }

        public static string GetBody(this HttpListenerRequest request)
        {
            try
            {
                if (request.HasEntityBody)
                {
                    using (System.IO.Stream body = request.InputStream)
                    {
                        using (var reader = new System.IO.StreamReader(body, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return "";
        }

        public static Dictionary<string, string> GetHeaders(this HttpListenerRequest request)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            try
            {
                foreach (var item in request.Headers.Keys)
                {
                    headers.Add(item.ToString(), request.Headers[item.ToString()]);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return headers;
        }

        public static Dictionary<string, string> GetQueryString(this HttpListenerRequest request)
        {
            Dictionary<string, string> queryString = new Dictionary<string, string>();
            try
            {
                foreach (var item in request.QueryString.AllKeys)
                {
                    queryString.Add(item, request.QueryString[item]);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return queryString;
        }
    }
}
