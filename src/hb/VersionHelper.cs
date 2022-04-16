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
    /// date       :2022/4/17 1:47:11
    /// description:
    /// </summary>
    internal class VersionHelper
    {
        /// <summary>
        /// 获取当前发布版本号
        /// </summary>
        /// <returns></returns>
        public static Version GetMainVersion()
        {
            try
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// 获取内部修订版本号
        /// </summary>
        /// <returns></returns>
        public static string GetReversion()
        {
            try
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                    if (attributes.Length > 0)
                    {
                        AssemblyInformationalVersionAttribute attribute = (AssemblyInformationalVersionAttribute)attributes[0];
                        if (attribute != null)
                        {
                            return attribute.InformationalVersion;
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
    }
}
