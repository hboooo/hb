using System;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 1:42:25
    /// description:
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToStandard(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToCompact(this DateTime dt)
        {
            return dt.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLongStandard(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="isMinseconds">是否包含毫秒</param>
        /// <returns></returns>
        public static long ToTimeStamp(this DateTime dt, bool isMinseconds = false)
        {
            var ts = dt - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(isMinseconds ? ts.TotalMilliseconds : ts.TotalSeconds);
        }

        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="isMinseconds">是否包含毫秒</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timeStamp, bool isMinseconds = false)
        {
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return isMinseconds ? time.AddMilliseconds(timeStamp) : time.AddSeconds(timeStamp);
        }

    }
}
