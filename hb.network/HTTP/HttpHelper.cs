using System;
using System.IO;
using System.Net;

namespace hb.network.HTTP
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/20 1:58:17
    /// description:
    /// </summary>
    public class HttpHelper
    {

        /// <summary>
        /// 下載文件
        /// </summary>
        /// <param name="uri">http url</param>
        /// <param name="file">下載到本地的目錄</param>
        /// <returns></returns>
        public static bool HttpDownload(string uri, string file)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] data = new byte[1024];
                        int size = 0;
                        while ((size = responseStream.Read(data, 0, data.Length)) > 0)
                        {
                            ms.Write(data, 0, size);
                        }
                        using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                        {
                            fs.Write(ms.ToArray(), 0, (int)ms.Length);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// 下载文件
        /// 异步
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="action"></param>
        public static void HttpDownloadAsync(string url, string file, Action<bool> action = null)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.BeginGetResponse((obj) =>
                {
                    try
                    {
                        HttpWebRequest req = (HttpWebRequest)obj.AsyncState;
                        var response = req.EndGetResponse(obj);
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] data = new byte[1024];
                                int size = 0;
                                while ((size = responseStream.Read(data, 0, data.Length)) > 0)
                                {
                                    ms.Write(data, 0, size);
                                }
                                using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                                {
                                    fs.Write(ms.ToArray(), 0, (int)ms.Length);
                                }
                            }
                        }
                        action?.Invoke(true);
                    }
                    catch (Exception ex)
                    {
                        action?.Invoke(false);
                    }
                }, request);
            }
            catch (Exception ex)
            {
                action?.Invoke(false);
            }
        }

    }
}
