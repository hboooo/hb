using hb.Dynamic;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 2:02:12
    /// description:
    /// </summary>
    public class Rest
    {
        private string _url;
        private IRestClient _restClient;
        private IRestRequest _request;
        private IRestResponse _response;

        private Dictionary<string, object> _queryParams;
        private Dictionary<string, object> _bodyParams;

        private Rest(Method method = Method.POST)
        {
            _restClient = new RestClient();
            _request = new RestRequest((RestSharp.Method)method);
        }
        public static Rest Create(Method method)
        {
            return new Rest(method);
        }

        public Rest GetRest()
        {
            return this;
        }

        public Rest SetUrl(string url)
        {
            this._url = url;
            _restClient.BaseUrl = new Uri(this._url);
            return this;
        }

        public Rest AddHeader(string name, string value)
        {
            _request.AddHeader(name, value);
            return this;
        }

        public Rest AddUrlSegment(string key, string value)
        {
            _request.AddUrlSegment(key, value);
            return this;
        }

        public Rest AddQueryParameter(string key, object value)
        {
            if (_queryParams == null)
            {
                _queryParams = new Dictionary<string, object>();
            }
            _queryParams[key] = value;
            return this;
        }

        public Rest AddBodyParameter(string key, object value)
        {
            if (_bodyParams == null)
            {
                _bodyParams = new Dictionary<string, object>();
            }
            _bodyParams[key] = value;
            return this;
        }

        public Rest AddQueryParameter(object jsonObject)
        {
            string json = DynamicJson.SerializeObject(jsonObject);
            AddQueryParameter(json);
            return this;
        }
        public Rest AddBodyParameter(object jsonObject)
        {
            string json = DynamicJson.SerializeObject(jsonObject);
            AddBodyParameter(json);
            return this;
        }

        public Rest AddQueryParameter(string json)
        {
            var dic = DynamicJson.DeserializeObject<Dictionary<string, object>>(json);
            if (_queryParams != null)
            {
                foreach (var key in dic.Keys)
                {
                    this._queryParams[key] = dic[key];
                }
            }
            else
            {
                this._queryParams = dic;
            }
            return this;
        }
        public Rest AddBodyParameter(string json)
        {
            var dic = DynamicJson.DeserializeObject<Dictionary<string, object>>(json);
            if (_bodyParams != null)
            {
                foreach (var key in dic.Keys)
                {
                    this._bodyParams[key] = dic[key];
                }
            }
            else
            {
                this._bodyParams = dic;
            }
            return this;
        }

        public Rest AddBodyJson(object jsonObject)
        {
            _request.AddJsonBody(jsonObject);
            return this;
        }

        public Rest Execute()
        {
            PrepareRequest();
            Func<IRestRequest, IRestResponse> action = new Func<IRestRequest, IRestResponse>(_restClient.Execute);
            IAsyncResult result = action.BeginInvoke(_request, null, null);
            this._response = action.EndInvoke(result);

            Exception ex = PrepareAsyncResponse();
            if (ex != null) throw ex;
            return this;
        }

        private void PrepareRequest()
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

        private Exception PrepareAsyncResponse()
        {
            if (this._response.StatusCode != HttpStatusCode.OK)
            {
                var respStatus = 0;
                dynamic respContent = default(dynamic);
                try
                {
                    respStatus = (int)this._response.StatusCode;
                    respContent = DynamicJson.DeserializeObject<object>(_response.Content);
                }
                catch (Exception ex)
                {

                }
            }
            return null;
        }

        public Rest GetContext(out IRestClient rc, out IRestResponse res, out IRestRequest req)
        {
            rc = this._restClient;
            res = this._response;
            req = this._request;
            return this;
        }

        public Rest GetContext(out IRestResponse res, out IRestRequest req)
        {
            res = this._response;
            req = this._request;
            return this;
        }

        public Rest GetContext(out IRestResponse res)
        {
            res = this._response;
            return this;
        }

        public Rest GetContext(out IRestRequest req)
        {
            req = this._request;
            return this;
        }

        public int GetResponseStatus()
        {
            return (int)this._response.StatusCode;
        }

        public int ToInt()
        {
            return (int)ParseResponseContentStruct<int>();
        }

        public long ToLong()
        {
            return (long)ParseResponseContentStruct<long>();
        }

        public float ToFloat()
        {
            return (float)ParseResponseContentStruct<float>();
        }

        public double ToFloat(Func<IRestResponse, double> func)
        {
            return (double)ParseResponseContentStruct<double>();
        }

        public Boolean ToBoolean()
        {
            return (Boolean)ParseResponseContentStruct<Boolean>();
        }

        public T To<T>() where T : class
        {
            return (T)ParseResponseContent<T>();
        }
        public T ToStruct<T>() where T : struct
        {
            return (T)ParseResponseContentStruct<T>();
        }

        private object ParseResponseContent<T>() where T : class
        {
            if (typeof(T) == typeof(string))
            {
                return _response.Content;
            }
            return DynamicJson.DeserializeObject<T>(_response.Content);
        }
        private object ParseResponseContentStruct<T>() where T : struct
        {
            return DynamicJson.DeserializeObject<T>(_response.Content);
        }
    }


    //
    // 摘要:
    //     HTTP method to use when making requests
    public enum Method
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7
    }

}
