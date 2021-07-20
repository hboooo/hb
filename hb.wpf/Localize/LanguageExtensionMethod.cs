using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb.wpf.Localize
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:26:01
    /// description:
    /// </summary>
    public static class LanguageExtensionMethod
    {
        public static string GetLanguage(this string wesFlowName)
        {
            return LanguageService.GetLanguages(wesFlowName);
        }

        public static string GetLanguage(this Enum wesName)
        {
            string language = LanguageService.GetLanguages(wesName.GetType().FullName + "." + wesName.GetType().GetField(wesName.ToString()).Name);
            if (string.IsNullOrEmpty(language)) language = wesName.GetType().GetField(wesName.ToString()).Name;
            return language;
        }
    }
}
