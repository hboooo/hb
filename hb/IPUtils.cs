using System;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:18:58
    /// description:
    /// </summary>
    public class IPUtils
    {
        /// <summary>
        /// 获取MAC地址，仅支持单网卡
        /// </summary>
        /// <returns></returns>
        public static string GetMacAddr()
        {
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection managements = managementClass.GetInstances();
            foreach (ManagementObject managementObject in managements)
            {
                if (managementObject["IPEnabled"].ToString() == "True")
                    return managementObject["MacAddress"].ToString();
            }
            return null;
        }

        /// <summary>
        /// 获取本机IP地址，仅支持单网卡
        /// </summary>
        /// <returns></returns>
        public static string GetLocalHost()
        {
            IPAddress[] addrs = Dns.GetHostAddresses(Environment.MachineName);
            foreach (IPAddress addr in addrs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return addr.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 串口是否存在
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static bool ComExist(string com)
        {
            if (string.IsNullOrEmpty(com))
            {
                throw new ArgumentNullException(nameof(com));
            }

            string[] coms = SerialPort.GetPortNames();
            var res = coms.Where(c => string.Compare(c, com, true) == 0);
            if (res.Count() > 0)
            {
                return true;
            }
            return false;
        }

    }
}
