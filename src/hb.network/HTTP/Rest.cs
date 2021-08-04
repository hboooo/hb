using hb.Dynamic;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 2:02:12
    /// description:http requests
    /// </summary>
    public class Rest
    {
        private string _url;
        private static volatile string _host;
        private string _path;
        private IRestClient _restClient;
        private IRestRequest _request;
        private IRestResponse _response;

        private Dictionary<string, object> _queryParams;
        private Dictionary<string, object> _bodyParams;

        private Rest(Method method, string contentType)
        {
            _restClient = new RestClient();
            _request = new RestRequest((RestSharp.Method)method);
            if (string.IsNullOrEmpty(contentType))
                _request.AddHeader("Content-Type", "application/json; charset=utf-8");
        }

        /// <summary>
        /// 创建一个http请求
        /// </summary>
        /// <param name="method">HTTP Method</param>
        /// <returns></returns>
        public static Rest Create(Method method, string contentType = null)
        {
            return new Rest(method, contentType);
        }

        /// <summary>
        /// 超时，单位秒
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Rest SetTimeout(int second)
        {
            _request.Timeout = second * 1000;
            return this;
        }

        /// <summary>
        /// 设置http请求地址url
        /// 例如 https://github.com/search?q=microsoft
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Rest SetUrl(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
            _restClient.BaseUrl = new Uri(_url);
            return this;
        }

        /// <summary>
        /// 设置http请求host，与path一起使用
        /// 例如 https://github.com 与path构建为https://github.com/search
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public Rest SetHost(string host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            return this;
        }

        /// <summary>
        /// 设置http请求path，与host一起使用
        /// 例如 /search 与host构建为https://github.com/search
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Rest SetPath(string path)
        {
            if (string.IsNullOrEmpty(_host)) throw new ArgumentNullException("host", "host must be set first");
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _restClient.BaseUrl = new Uri(_host + _path);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, HttpHeader) overload
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rest AddHeader(string name, string value)
        {
            _request.AddHeader(name, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, Cookie) overload
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rest AddCookies(string name, string value)
        {
            _request.AddCookie(name, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, UrlSegment) overload
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rest AddUrlSegment(string key, string value)
        {
            _request.AddUrlSegment(key, value);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, QueryString) overload
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rest AddQueryParameter(string key, object value)
        {
            if (_queryParams == null)
            {
                _queryParams = new Dictionary<string, object>();
            }
            _queryParams[key] = value;
            return this;
        }

        /// <summary>
        /// Instructs RestSharp to send a given object in the request body, serialized as JSON.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Rest AddBodyParameter(string key, object value)
        {
            if (_bodyParams == null)
            {
                _bodyParams = new Dictionary<string, object>();
            }
            _bodyParams[key] = value;
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, QueryString) overload
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public Rest AddQueryParameter(object jsonObject)
        {
            string json = DynamicJson.SerializeObject(jsonObject);
            AddQueryParameter(json);
            return this;
        }

        /// <summary>
        /// Instructs RestSharp to send a given object in the request body, serialized as JSON.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public Rest AddBodyParameter(object jsonObject)
        {
            string json = DynamicJson.SerializeObject(jsonObject);
            AddBodyParameter(json);
            return this;
        }

        /// <summary>
        /// Shortcut to AddParameter(name, value, QueryString) overload
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Rest AddQueryParameter(string json)
        {
            var dic = DynamicJson.DeserializeObject<Dictionary<string, object>>(json);
            if (_queryParams != null)
            {
                foreach (var key in dic.Keys)
                {
                    _queryParams[key] = dic[key];
                }
            }
            else
            {
                _queryParams = dic;
            }
            return this;
        }

        /// <summary>
        /// Instructs RestSharp to send a given object in the request body, serialized as JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Rest AddBodyParameter(string json)
        {
            var dic = DynamicJson.DeserializeObject<Dictionary<string, object>>(json);
            if (_bodyParams != null)
            {
                foreach (var key in dic.Keys)
                {
                    _bodyParams[key] = dic[key];
                }
            }
            else
            {
                _bodyParams = dic;
            }
            return this;
        }

        /// <summary>
        /// Instructs RestSharp to send a given object in the request body, serialized as JSON.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public Rest AddBodyJson(object jsonObject)
        {
            _request.AddJsonBody(jsonObject);
            return this;
        }

        /// <summary>
        /// Instructs RestSharp to send a given object in the request body, serialized as
        /// JSON. Allows specifying a custom content type. Usually, this method is used to
        /// support PATCH requests that require application/json-patch+json content type.
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public Rest AddBodyJson(object jsonObject, string contentType)
        {
            _request.AddJsonBody(jsonObject, contentType);
            return this;
        }

        public Rest AddFile(string name, string path, string contentType = null)
        {
            _request.AddFile(name, path, contentType);
            return this;
        }

        private void PreparePostData()
        {
            if (_queryParams != null)
            {
                foreach (var key in _queryParams.Keys)
                {
                    _request.AddQueryParameter(key, _queryParams[key] == null ? null : _queryParams[key].ToString());
                }
            }

            if (_bodyParams != null)
            {
                _request.AddJsonBody(_bodyParams);
            }

        }

        /// <summary>
        /// 开始发起请求
        /// </summary>
        /// <returns></returns>
        public Rest Execute()
        {
            PreparePostData();
            Func<IRestRequest, IRestResponse> action = new Func<IRestRequest, IRestResponse>(_restClient.Execute);
            IAsyncResult result = action.BeginInvoke(_request, null, null);
            _response = action.EndInvoke(result);
            return this;
        }

        /// <summary>
        /// 开始发起异步请求
        /// </summary>
        /// <param name="callback"></param>
        public void ExecuteAsync(Action<Rest> callback)
        {
            PreparePostData();
            Func<IRestRequest, IRestResponse> action = new Func<IRestRequest, IRestResponse>(_restClient.Execute);
            IAsyncResult result = action.BeginInvoke(_request, (asyncResult) =>
            {
                Rest rest = (Rest)asyncResult.AsyncState;
                rest._response = action.EndInvoke(asyncResult);
                callback.Invoke(rest);
            }, this);
        }

        public bool DownloadFile(string filename)
        {
            bool downloaded = false;
            using (var writer = new FileStream(filename, FileMode.Create))
            {
                _request.ResponseWriter = (responseSteam) => responseSteam.CopyTo(writer);
                Func<IRestRequest, byte[]> action = new Func<IRestRequest, byte[]>(_restClient.DownloadData);
                IAsyncResult result = action.BeginInvoke(_request, null, null);
                var res = action.EndInvoke(result);
                downloaded = writer.Length > 0 ? true : false;
            }
            if (!downloaded)
            {
                DeleteFile(filename);
            }
            return downloaded;
        }

        private void DeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="res"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public Rest GetContext(out IRestClient rc, out IRestResponse res, out IRestRequest req)
        {
            rc = _restClient;
            res = _response;
            req = _request;
            return this;
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <param name="res"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public Rest GetContext(out IRestResponse res, out IRestRequest req)
        {
            res = _response;
            req = _request;
            return this;
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public Rest GetContext(out IRestResponse res)
        {
            res = _response;
            return this;
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Rest GetContext(out IRestRequest req)
        {
            req = _request;
            return this;
        }

        /// <summary>
        /// 获取当前请求对象
        /// </summary>
        /// <returns></returns>
        public Rest TakeRest()
        {
            return this;
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <returns></returns>
        public Tuple<IRestClient, IRestRequest, IRestResponse> TakeClient()
        {
            return new Tuple<IRestClient, IRestRequest, IRestResponse>(_restClient, _request, _response);
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <returns></returns>
        public Tuple<IRestRequest, IRestResponse> TakeRequests()
        {
            return new Tuple<IRestRequest, IRestResponse>(_request, _response);
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <returns></returns>
        public IRestResponse TakeResponse()
        {
            return _response;
        }

        /// <summary>
        /// 获取当前上下文
        /// </summary>
        /// <returns></returns>
        public IRestRequest TakeRequest()
        {
            return _request;
        }

        /// <summary>
        /// 获取当前http请求返回状态码
        /// </summary>
        /// <returns></returns>
        public HttpStatusCode TakeHttpCode()
        {
            return _response.StatusCode;
        }

        public int TakeInt()
        {
            return (int)ParseResponseContentStruct<int>();
        }

        public long TakeLong()
        {
            return (long)ParseResponseContentStruct<long>();
        }

        public float TakeFloat()
        {
            return (float)ParseResponseContentStruct<float>();
        }

        public bool TakeBoolean()
        {
            return (bool)ParseResponseContentStruct<bool>();
        }

        public string TakeString()
        {
            return _response.Content;
        }

        public dynamic TakeDynamic()
        {
            return Take<dynamic>();
        }

        public T Take<T>() where T : class
        {
            return (T)ParseResponseContent<T>();
        }
        public T TakeStruct<T>() where T : struct
        {
            return (T)ParseResponseContentStruct<T>();
        }

        private object ParseResponseContent<T>() where T : class
        {
            if (string.IsNullOrEmpty(_response.Content))
                return null;

            if (typeof(T) == typeof(string))
            {
                return _response.Content;
            }
            return DynamicJson.DeserializeObject<T>(_response.Content);
        }

        private object ParseResponseContentStruct<T>() where T : struct
        {
            if (string.IsNullOrEmpty(_response.Content))
                return null;

            return DynamicJson.DeserializeObject<T>(_response.Content);
        }

    }

}
