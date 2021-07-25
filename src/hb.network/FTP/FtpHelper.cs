using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace hb.network.FTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:36:54
    /// description:FTP操作
    /// </summary>
    public class FtpHelper
    {
        #region 属性
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获取或设置密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置FTP服务器地址
        /// </summary>
        public Uri Uri { get; set; }
        /// <summary>
        /// 获取或者是读取文件、目录列表时所使用的编码，默认为UTF-8 
        /// </summary>
        public Encoding Encode { get; set; }

        #endregion

        #region 构造函数
        public FtpHelper(Uri uri, string username, string password, string encode = "utf-8")
        {
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this.UserName = username ?? throw new ArgumentNullException(nameof(username));
            this.Password = password ?? throw new ArgumentNullException(nameof(password));
            this.Encode = Encoding.GetEncoding(encode);
        }

        public FtpHelper(Uri uri, string encode = "utf-8") : this(uri, null, null, encode)
        {

        }

        #endregion

        #region 建立连接

        /// <summary>
        /// 建立FTP链接,返回请求对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="method">操作命令(WebRequestMethods.Ftp)</param>
        /// <returns></returns>
        private FtpWebRequest CreateRequest(Uri uri, string method)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            if (!string.IsNullOrEmpty(UserName))
            {
                request.Credentials = new NetworkCredential(this.UserName, this.Password);//指定登录ftp服务器的用户名和密码。
            }
            request.KeepAlive = false;    //指定连接是应该关闭还是在请求完成之后关闭，默认为true
            request.UsePassive = true;    //指定使用被动模式，默认为true
            request.UseBinary = true;     //指示服务器要传输的是二进制数据.false,指示数据为文本。默认值为true
            request.EnableSsl = false;    //如果控制和数据传输是加密的,则为true.否则为false.默认值为 false
            request.Method = method;
            return request;
        }

        /// <summary>
        /// 建立FTP链接,返回响应对象
        /// </summary>
        /// <param name="uri">FTP地址</param>
        /// <param name="method">操作命令(WebRequestMethods.Ftp)</param>
        /// <returns></returns>
        private FtpWebResponse CreateResponse(Uri uri, string method)
        {
            FtpWebRequest request = CreateRequest(uri, method);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            return response;
        }

        private void CreateResponseAsync(Uri uri, string method, Action<FtpWebResponse> action, Action<Exception> errAction = null)
        {
            try
            {
                FtpWebRequest request = CreateRequest(uri, method);
                request.BeginGetResponse((obj) =>
                {
                    try
                    {
                        FtpWebRequest req = (FtpWebRequest)obj.AsyncState;
                        var response = (FtpWebResponse)req.EndGetResponse(obj);
                        action?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        errAction?.Invoke(ex);
                    }
                }, request);
            }
            catch (WebException ex)
            {
                errAction?.Invoke(ex);
            }
            catch (Exception ex)
            {
                errAction?.Invoke(ex);
            }
        }
        #endregion

        #region 下载文件

        /// <summary>
        /// 从FTP服务器下载文件
        /// </summary>
        /// <param name="remoteFile">远程完整文件名</param>
        /// <param name="localFile">本地带有完整路径的文件名</param>
        public bool DownloadFile(string remoteFile, string localFile)
        {
            try
            {
                string localDirector = Path.GetDirectoryName(localFile);
                CheckCreateDir(localDirector, true);
                FtpWebResponse response = CreateResponse(new Uri(this.Uri.ToString() + remoteFile), WebRequestMethods.Ftp.DownloadFile);
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int bytesCount = 0;
                        while ((bytesCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, bytesCount);
                        }
                        using (FileStream fs = new FileStream(localFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            fs.Write(ms.ToArray(), 0, (int)ms.Length);
                        }
                    }
                }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DownloadFileAsync(string remoteFilePath, string localFilePath, Action<bool> action = null)
        {
            try
            {
                string localDirector = Path.GetDirectoryName(localFilePath);
                CheckCreateDir(localDirector, true);
                CreateResponseAsync(new Uri(this.Uri.ToString() + remoteFilePath), WebRequestMethods.Ftp.DownloadFile, (ftpWebResponse) =>
                {
                    if (ftpWebResponse != null)
                    {
                        using (Stream stream = ftpWebResponse.GetResponseStream())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] buffer = new byte[1024];
                                int bytesCount = 0;
                                while ((bytesCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    ms.Write(buffer, 0, bytesCount);
                                }
                                using (FileStream fs = new FileStream(localFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                                {
                                    fs.Write(ms.ToArray(), 0, (int)ms.Length);
                                }
                            }
                        }
                        ftpWebResponse.Close();
                    }
                    action?.Invoke(true);
                });
            }
            catch (WebException ex)
            {
                Logger.Error(ex);
                action?.Invoke(false);
            }
            catch (System.Exception ex)
            {
                Logger.Error(ex);
                action?.Invoke(false);
            }
        }

        #endregion

        #region 上传文件

        public delegate void FtpUploadProgress(int value);
        public event FtpUploadProgress FtpUploadProgressEvent;

        public delegate void FtpUploadFinish();
        public event FtpUploadFinish FtpUploadFinishEvent;

        /// <summary>
        /// 上传文件接口
        /// </summary>
        /// <param name="localFilePath">本地带有完整路径的文件名</param>
        /// <param name="remoteFilePath">要在FTP服务器上面保存完整文件名</param>
        /// <returns></returns>
        public void UploadFile(string localFilePath, string remoteFilePath)
        {
            UploadFile(localFilePath, remoteFilePath, false);
        }

        /// <summary>
        /// 上传文件到FTP服务器,若文件已存在自动覆盖
        /// </summary>
        /// <param name="localFilePath">本地带有完整路径的文件名</param>
        /// <param name="remoteFilePath">要在FTP服务器上面保存完整文件名</param>
        /// <param name="autoCreateDirectory">是否自动递归创建文件目录</param>
        /// <returns></returns>
        public void UploadFile(string localFilePath, string remoteFilePath, bool autoCreateDirectory)
        {
            long current = 0;
            long total = 1;
            int percent = 0;
            int temp = 0;
            try
            {
                //自动递归创建目录
                if (autoCreateDirectory)
                {
                    if (!CreateDirectory(Path.GetDirectoryName(remoteFilePath)))
                    {
                        //递归创建目录失败，返回異常
                        throw new FileNotFoundException(string.Format("創建目錄失敗:{0}!", localFilePath));
                    }
                }
                FileInfo fileInf = new FileInfo(localFilePath);
                if (!fileInf.Exists)
                {
                    throw new FileNotFoundException(string.Format("本地文件不存在:{0}!", localFilePath));
                }

                FtpWebRequest request = CreateRequest(new Uri(this.Uri + remoteFilePath), WebRequestMethods.Ftp.UploadFile);
                request.ContentLength = fileInf.Length;
                total = fileInf.Length;
                using (FileStream fs = fileInf.OpenRead())
                {
                    using (Stream stream = request.GetRequestStream())
                    {
                        int contentLen = 0;
                        byte[] buff = new byte[1024];
                        while ((contentLen = fs.Read(buff, 0, buff.Length)) > 0)
                        {
                            stream.Write(buff, 0, contentLen);
                            //产生进度条数据
                            current += contentLen;
                            temp = Int32.Parse((100 * current / total).ToString());
                            if (temp != percent)
                            {
                                FtpUploadProgressEvent?.Invoke(percent);
                                percent = temp;
                            }
                        }
                        FtpUploadFinishEvent?.Invoke();
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region 递归创建目录
        /// <summary>
        /// 递归创建目录，在创建目录前不进行目录是否已存在检测
        /// </summary>
        /// <param name="remoteDirectory"></param>
        public bool CreateDirectory(string remoteDirectory)
        {
            return CreateDirectory(remoteDirectory, false);
        }

        /// <summary>
        /// 在FTP服务器递归创建目录
        /// </summary>
        /// <param name="remoteDirectory">要创建的目录</param>
        /// <param name="autoCheckExist">创建目录前是否进行目录是否存在检测</param>
        /// <returns></returns>
        public bool CreateDirectory(string remoteDirectory, bool autoCheckExist)
        {
            try
            {
                string parentDirector = "/";
                if (!string.IsNullOrEmpty(remoteDirectory))
                {
                    remoteDirectory = remoteDirectory.Replace("\\", "/");
                    string[] directors = remoteDirectory.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string director in directors)
                    {
                        if (!parentDirector.EndsWith("/")) parentDirector += "/";
                        if (autoCheckExist)
                        {
                            if (!DirectoryExist(parentDirector, director))
                                CreateResponse(new Uri(this.Uri + parentDirector + director), WebRequestMethods.Ftp.MakeDirectory);
                        }
                        else
                        {
                            try
                            {
                                CreateResponse(new Uri(this.Uri + parentDirector + director), WebRequestMethods.Ftp.MakeDirectory);
                            }
                            catch (WebException ex)
                            {
                                throw ex;
                            }
                        }
                        parentDirector += director;
                    }
                }
                return true;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检测指定目录下是否存在指定的目录名
        /// </summary>
        /// <param name="parentDirector"></param>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        private bool DirectoryExist(string parentDirector, string directoryName)
        {
            List<FileStruct> list = GetFileAndDirectoryList(parentDirector);
            foreach (FileStruct fstruct in list)
            {
                if (fstruct.IsDirectory && fstruct.Name == directoryName)
                {
                    return true;
                }
            }
            return false;
        }


        #endregion

        #region 目录、文件列表
        /// <summary>
        /// 获取FTP服务器上指定目录下的所有文件和目录
        /// 若获取的中文文件、目录名优乱码现象
        /// 请调用this.Encode进行文件编码设置，默认为UTF-8，一般改为GB2312就能正确识别
        /// </summary>
        /// <param name="direcotry"></param>
        /// <returns></returns>
        public List<FileStruct> GetFileAndDirectoryList(string direcotry)
        {
            try
            {
                List<FileStruct> list = new List<FileStruct>();
                string str = null;
                FtpWebResponse response = CreateResponse(new Uri(this.Uri.ToString() + direcotry), WebRequestMethods.Ftp.ListDirectoryDetails);
                Stream stream = response.GetResponseStream();

                using (StreamReader sr = new StreamReader(stream, this.Encode))
                {
                    str = sr.ReadToEnd();
                }
                string[] fileList = str.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                EFileListFormat format = JudgeFileListFormat(fileList);
                if (!string.IsNullOrEmpty(str) && format != EFileListFormat.Unknown)
                {
                    list = ParseFileStruct(fileList, format);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解析文件列表信息返回文件列表
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="format">文件列表格式</param>
        /// <returns></returns>
        private List<FileStruct> ParseFileStruct(string[] fileList, EFileListFormat format)
        {
            List<FileStruct> list = new List<FileStruct>();
            if (format == EFileListFormat.UnixFormat)
            {
                foreach (string info in fileList)
                {
                    FileStruct fstuct = new FileStruct();
                    fstuct.Origin = info.Trim();
                    fstuct.OriginArr = fstuct.Origin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fstuct.OriginArr.Length == 9)
                    {
                        fstuct.Flags = fstuct.OriginArr[0];
                        fstuct.IsDirectory = (fstuct.Flags[0] == 'd');
                        fstuct.Owner = fstuct.OriginArr[2];
                        fstuct.Group = fstuct.OriginArr[3];
                        fstuct.Size = Convert.ToInt32(fstuct.OriginArr[4]);
                        if (fstuct.OriginArr[7].Contains(":"))
                        {
                            fstuct.OriginArr[7] = DateTime.Now.Year + " " + fstuct.OriginArr[7];
                        }
                        fstuct.UpdateTime = DateTime.Parse(string.Format("{0} {1} {2}", fstuct.OriginArr[5], fstuct.OriginArr[6], fstuct.OriginArr[7]));
                        fstuct.Name = fstuct.OriginArr[8];
                        if (fstuct.Name != "." && fstuct.Name != "..")
                        {
                            list.Add(fstuct);
                        }
                    }

                }
            }
            else if (format == EFileListFormat.WindowsFormat)
            {
                foreach (string info in fileList)
                {
                    FileStruct fstuct = new FileStruct();
                    fstuct.Origin = info.Trim();
                    fstuct.OriginArr = fstuct.Origin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fstuct.OriginArr.Length == 4)
                    {
                        DateTimeFormatInfo usDate = new CultureInfo("en-US", false).DateTimeFormat;
                        usDate.ShortTimePattern = "t";
                        fstuct.UpdateTime = DateTime.Parse(fstuct.OriginArr[0] + " " + fstuct.OriginArr[1], usDate);

                        fstuct.IsDirectory = (fstuct.OriginArr[2] == "<DIR>");
                        if (!fstuct.IsDirectory)
                        {
                            fstuct.Size = Convert.ToInt32(fstuct.OriginArr[2]);
                        }
                        fstuct.Name = fstuct.OriginArr[3];
                        if (fstuct.Name != "." && fstuct.Name != "..")
                        {
                            list.Add(fstuct);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="fileList">文件信息列表</param>
        /// <returns></returns>
        private EFileListFormat JudgeFileListFormat(string[] fileList)
        {
            foreach (string str in fileList)
            {
                if (str.Length > 10 && Regex.IsMatch(str.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                {
                    return EFileListFormat.UnixFormat;
                }
                else if (str.Length > 8 && Regex.IsMatch(str.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                {
                    return EFileListFormat.WindowsFormat;
                }
            }
            return EFileListFormat.Unknown;
        }

        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FileStruct f = new FileStruct();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.UpdateTime = DateTime.Parse(dateStr + " " + timeStr, myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);   // true);
                processstr = strs[1];
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }

        /// <summary>
        /// 检测目录并创建
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCreateDir"></param>
        /// <returns></returns>
        private bool CheckCreateDir(string path, bool isCreateDir = false)
        {
            if (!Directory.Exists(path))
            {
                if (isCreateDir) Directory.CreateDirectory(path);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        private void DeleteFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            try
            {
                if (File.Exists(filename))
                {
                    FileInfo fileInfo = new FileInfo(filename);
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) > 0)
                        fileInfo.Attributes ^= FileAttributes.ReadOnly;
                    fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除指定目录中的所有文件
        /// </summary>
        /// <param name="path"></param>
        private void DeleteFiles(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (var item in files)
                {
                    DeleteFile(item);
                }
            }
        }
        #endregion

    }
}
