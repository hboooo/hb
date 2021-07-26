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
    /// date       :2021/7/25 16:39:55
    /// description:
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值描述信息
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
