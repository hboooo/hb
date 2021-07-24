using hb.LogServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 18:50:19
    /// description:对象序列化
    /// </summary>
    public class BytesSerialize
    {

        public static byte[] Serialize<T>(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, model);
                return ms.GetBuffer();
            }
        }


        public static T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object obj = formatter.Deserialize(ms);
                return (T)Convert.ChangeType(obj, typeof(T));
            }
        }

    }
}
