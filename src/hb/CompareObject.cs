using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2022/4/17 1:39:01
    /// description:
    /// </summary>
    internal class CompareObject
    {
        /// <summary>
        /// 获取对象不同值的属性及变更的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="current"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public static List<ConstrastInfo> Constrast<T>(T original, T current, Dictionary<string, string> include = null) where T : class
        {
            List<ConstrastInfo> constrasts = new List<ConstrastInfo>();
            try
            {
                PropertyInfo[] propertyInfos = original.GetType().GetProperties();

                foreach (var property in propertyInfos)
                {
                    if (include != null && include.ContainsKey(property.Name))
                    {
                        var value1 = property.GetValue(original, null);
                        var value2 = current.GetType().GetProperty(property.Name).GetValue(current, null);
                        string s1 = value1 != null ? value1.ToString() : "";
                        string s2 = value2 != null ? value2.ToString() : "";
                        if (s1 != s2)
                        {
                            constrasts.Add(new ConstrastInfo()
                            {
                                FieldPropertyName = property.Name,
                                FieldName = include[property.Name],
                                DataType = property.PropertyType,
                                OriginValue = value1,
                                CurrentValue = value2
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return constrasts;
        }
    }


    internal struct ConstrastInfo
    {
        public string FieldPropertyName { get; set; }
        public string FieldName { get; set; }
        public Type DataType { get; set; }
        public object OriginValue { get; set; }
        public object CurrentValue { get; set; }
    }
}
