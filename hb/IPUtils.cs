using System;
using System.IO.Ports;
using System.Linq;
using System.Management;

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
        /// 获取本地Mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 检查串口是否存在
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static bool IsComExist(string com)
        {
            try
            {
                string[] comPorts = SerialPort.GetPortNames();
                if (comPorts != null && comPorts.Length > 0)
                {
                    var query = comPorts.Where(c => string.Compare(c, com) == 0);
                    if (query.Count() > 0)
                        return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// 獲取本機IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                System.Net.IPAddress[] _IPList = System.Net.Dns.GetHostAddresses(Environment.MachineName);
                for (int i = 0; i != _IPList.Length; i++)
                {
                    if (_IPList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return _IPList[i].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
    }
}
