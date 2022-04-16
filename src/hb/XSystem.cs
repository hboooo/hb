using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 15:40:42
    /// description:
    /// </summary>
    public static class XSystem
    {

        /// <summary>
        /// Byte To KB
        /// </summary>
        /// <param name="byteValue">Byte value</param>
        /// <returns>KB</returns>
        public static long ToKB(this long byteValue)
        {
            return (long)(byteValue / 1024.0);
        }

        /// <summary>
        /// Byte To MB
        /// </summary>
        /// <param name="byteValue">Byte value</param>
        /// <returns>MB</returns>
        public static long ToMB(this long byteValue)
        {
            return (long)(byteValue / 1024.0 / 1024.0);
        }

        /// <summary>
        /// Byte To GB
        /// </summary>
        /// <param name="byteValue">Byte value</param>
        /// <returns>GB</returns>
        public static long ToGB(this long byteValue)
        {
            return (long)(byteValue / 1024.0 / 1024.0 / 1024.0);
        }

        /// <summary>
        /// Byte To TB
        /// </summary>
        /// <param name="byteValue">Byte value</param>
        /// <returns>TB</returns>
        public static long ToTB(this long byteValue)
        {
            return (long)(byteValue / 1024.0 / 1024.0 / 1024.0 / 1024.0);
        }

        /// <summary>
        /// 获取当前进程占用内存大小(单位：MB)
        /// </summary>
        /// <returns>当前进程占用内存大小(单位：MB)</returns>
        public static double ProcessRamUsed()
        {
            return Process.GetCurrentProcess().WorkingSet64.ToMB();
        }

        /// <summary>
        /// 根据文件名或者磁盘名获取磁盘信息
        /// </summary>
        /// <param name="diskName">文件名或者磁盘名（取第一个字符）</param>
        /// <returns>磁盘信息</returns>
        public static DriveInfo Disk(string diskName)
        {
            if (string.IsNullOrEmpty(diskName))
            {
                return null;
            }

            diskName = (diskName[0] + ":\\").ToUpper();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                if (drive.Name.ToUpper() == diskName)
                {
                    return drive;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据文件名或者磁盘名判断磁盘是否存在
        /// </summary>
        /// <param name="diskName">文件名或者磁盘名（取第一个字符）</param>
        /// <returns>磁盘是否存在</returns>
        public static bool DiskExist(this string diskName)
        {
            return Disk(diskName) != null;
        }

        /// <summary>
        /// CPU使用率(单位：%)
        /// </summary>
        /// <returns>CPU使用率(单位：%)</returns>
        public static float CpuUsed()
        {
            const string CategoryName = "Processor";
            const string CounterName = "% Processor Time";
            const string InstanceName = "_Total";
            var pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);
            return pc.NextValue();
        }

        public static string GetOperatingSystemInfo()
        {
            string caption = string.Empty;
            string version = string.Empty;
            try
            {
                var query = "SELECT * FROM Win32_OperatingSystem";
                var searcher = new ManagementObjectSearcher(query);
                var info = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                caption = info.Properties["Caption"].Value.ToString();
                version = info.Properties["Version"].Value.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return $"{caption}({version})";
        }
    }
}
