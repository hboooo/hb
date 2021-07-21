using System;

namespace hb.network.FTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:35:39
    /// description:
    /// </summary>
    public class FileStruct
    {
        public string Origin { get; set; }
        public string[] OriginArr { get; set; }
        public string Flags { get; set; }
        /// <summary>
        /// 所有者
        /// </summary>
        public string Owner { get; set; }
        public string Group { get; set; }
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// 文件或目录更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 文件或目录名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件大小(目录始终为0)
        /// </summary>
        public int Size { get; set; }
    }

    /// <summary>
    /// 文件列表格式
    /// </summary>
    public enum EFileListFormat
    {
        /// <summary>
        /// Unix文件格式
        /// </summary>
        UnixFormat,
        /// <summary>
        /// Window文件格式
        /// </summary>
        WindowsFormat,
        /// <summary>
        /// 未知格式
        /// </summary>
        Unknown
    }
}
