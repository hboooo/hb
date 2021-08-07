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
    /// date       :2021/8/7 23:57:41
    /// description:
    /// </summary>
    public class ReflectionHelper
    {
        /// <summary>
        /// 获取实现了指定接口类型的基类实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static T[] GetImplementObjects<T>(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            return assembly.GetExportedTypes().Where(c =>
            {
                if (c.IsClass && !c.IsAbstract)
                {
                    var interfaces = c.GetInterfaces();
                    if (interfaces != null) return interfaces.Contains(typeof(T));
                }
                return false;
            }).Select(c => (T)c.GetConstructor(new Type[0]).Invoke(new object[0])).ToArray();
        }
    }
}
