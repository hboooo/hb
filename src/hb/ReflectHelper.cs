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
    /// date       :2022/4/17 1:41:56
    /// description:
    /// </summary>
    public static class ReflectHelper
    {
        /// <summary>
        /// 获取描述特性值
        /// </summary>
        /// <param name="valEnum"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum valEnum)
        {
            Type type = valEnum.GetType();
            MemberInfo[] memInfo = type.GetMember(valEnum.ToString());
            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (null != attrs && attrs.Length > 0)
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
            }
            return valEnum.ToString();
        }

    }
}
