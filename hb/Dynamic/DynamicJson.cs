using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hb.Dynamic
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:45:00
    /// description:
    /// </summary>
    public class DynamicJson
    {
        public static string SerializeObject(object value)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(value, Formatting.None, jsonSettings);
        }

        public static string SerializeObject(object value, string[] properties, bool retain = true)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            jsonSettings.ContractResolver = new LimitPropsContractResolver(properties, retain);
            return JsonConvert.SerializeObject(value, Formatting.None, jsonSettings);
        }

        public static T DeserializeObject<T>(string value, string timeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var iso = new IsoDateTimeConverter();
            iso.DateTimeFormat = timeFormat;
            return JsonConvert.DeserializeObject<T>(value, iso);
        }

        public static string SerializeXmlNode(System.Xml.XmlNode xml, bool omitRootObject)
        {
            return JsonConvert.SerializeXmlNode(xml, Formatting.Indented, omitRootObject);
        }

        public static string SerializeXNode(System.Xml.Linq.XObject node, bool omitRootObject)
        {
            return JsonConvert.SerializeXNode(node, Formatting.Indented, omitRootObject);
        }

    }

    public class LimitPropsContractResolver : DefaultContractResolver
    {
        private string[] props = null;
        private bool retain;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="props">传入的属性数组</param>
        /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
        public LimitPropsContractResolver(string[] props, bool retain)
        {
            //指定要序列化属性的清单
            this.props = props;
            this.retain = retain;
        }

        protected override IList<JsonProperty> CreateProperties(Type type,

        MemberSerialization memberSerialization)
        {
            IList<JsonProperty> list =
            base.CreateProperties(type, memberSerialization);
            //只保留清单有列出的属性
            return list.Where(p =>
            {
                if (retain)
                {
                    return props.Contains(p.PropertyName);
                }
                else
                {
                    return !props.Contains(p.PropertyName);
                }
            }).ToList();
        }
    }
}
