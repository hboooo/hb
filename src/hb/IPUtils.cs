using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;

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
        /// A类: 10.0.0.0-10.255.255.255
        /// </summary>
        static private long ipABegin, ipAEnd;
        /// <summary>
        /// B类: 172.16.0.0-172.31.255.255   
        /// </summary>
        static private long ipBBegin, ipBEnd;
        /// <summary>
        /// C类: 192.168.0.0-192.168.255.255
        /// </summary>
        static private long ipCBegin, ipCEnd;

        static IPUtils()
        {
            ipABegin = IpToNumber("10.0.0.0");
            ipAEnd = IpToNumber("10.255.255.255");

            ipBBegin = IpToNumber("172.16.0.0");
            ipBEnd = IpToNumber("172.31.255.255");

            ipCBegin = IpToNumber("192.168.0.0");
            ipCEnd = IpToNumber("192.168.255.255");
        }

        public static long IpToNumber(string ipAddress)
        {
            return IpToNumber(IPAddress.Parse(ipAddress));
        }

        public static long IpToNumber(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            return bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
        }

        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsIntranet(string ipAddress)
        {
            return IsIntranet(IpToNumber(ipAddress));
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsIntranet(IPAddress ipAddress)
        {
            return IsIntranet(IpToNumber(ipAddress));
        }
        /// <summary>
        /// true表示为内网IP
        /// </summary>
        /// <param name="longIP"></param>
        /// <returns></returns>
        public static bool IsIntranet(long longIP)
        {
            return ((longIP >= ipABegin) && (longIP <= ipAEnd) ||
                    (longIP >= ipBBegin) && (longIP <= ipBEnd) ||
                    (longIP >= ipCBegin) && (longIP <= ipCEnd));
        }
        /// <summary>
        /// 获取本机内网IP
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIntranetIP()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (var child in list)
            {
                if (IsIntranet(child)) return child;
            }

            return null;
        }
        /// <summary>
        /// 获取本机内网IP列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalIntranetIPList()
        {
            var list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            var result = new List<IPAddress>();
            foreach (var child in list)
            {
                if (IsIntranet(child)) result.Add(child);
            }

            return result;
        }

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
        /// 判断IP是否是私有地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsPrivateIP(this IPAddress ip)
        {
            //if (IPAddress.IsLoopback(ip)) return true;
            //ip = ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
            //byte[] bytes = ip.GetAddressBytes();
            //return ip.AddressFamily switch
            //{
            //    AddressFamily.InterNetwork when bytes[0] == 10 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 169 && bytes[1] == 254 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 172 && bytes[1] == 16 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 88 && bytes[2] == 99 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 192 && bytes[1] == 168 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 18 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 198 && bytes[1] == 51 && bytes[2] == 100 => true,
            //    AddressFamily.InterNetwork when bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113 => true,
            //    AddressFamily.InterNetworkV6 when ip.IsIPv6Teredo || ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("::") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("64:ff9b::") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("100::") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001::") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001:2") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2001:db8:") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("2002:") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fc") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fd") => true,
            //    AddressFamily.InterNetworkV6 when ip.ToString().StartsWith("fe") => true,
            //    AddressFamily.InterNetworkV6 when bytes[0] == 255 => true,
            //    _ => false
            //};
            return false;
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
