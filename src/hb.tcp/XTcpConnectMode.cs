using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace hb.tcp
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/26 18:50:19
    /// description:Mode
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum XTcpConnectMode
    {
        /// <summary>
        /// Tcp.
        /// </summary>
        [EnumMember(Value = "Tcp")]
        Tcp = 0,
        /// <summary>
        /// Ssl.
        /// </summary>
        [EnumMember(Value = "Ssl")]
        Ssl = 1
    }
}