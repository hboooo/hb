using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 15:22:47
    /// description:
    /// </summary>
    public static class XFile
    {
        /// <summary>
        /// 文件信息
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>结果</returns>
        public static FileInfo FileInfo(this string filename)
        {
            return File.Exists(filename) ? new FileInfo(filename) : null;
        }

        /// <summary>
        /// 获取无后缀的文件名
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <returns>文件名</returns>
        public static string NameWithoutExt(this FileInfo file)
        {
            return file == null ? string.Empty : file.Name.Replace(file.Extension, "");
        }


        /// <summary>
        /// 尝试删除文件
        /// </summary>
        /// <param name="file">文件名</param>
        /// <returns>结果</returns>
        public static bool TryDelete(string file)
        {
            return file.FileInfo().TryDelete();
        }

        /// <summary>
        /// 尝试删除文件
        /// </summary>
        /// <param name="fi">文件名</param>
        /// <returns>结果</returns>
        public static bool TryDelete(this FileInfo fi)
        {
            try
            {
                File.Delete(fi.FullName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="dest">目标目录</param>
        public static void Copy(string source, string dest)
        {
            Copy(new DirectoryInfo(source), new DirectoryInfo(dest));
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="dest">目标目录</param>
        public static void Copy(DirectoryInfo source, DirectoryInfo dest)
        {
            if (dest.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!dest.Exists)
            {
                dest.Create();
            }

            FileInfo[] files = source.GetFiles();

            foreach (FileInfo file in files)
            {
                File.Copy(file.FullName, Path.Combine(dest.FullName, file.Name), true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                Copy(dir.FullName, Path.Combine(dest.FullName, dir.Name));
            }
        }

    }
}
