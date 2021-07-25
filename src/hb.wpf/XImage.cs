using hb.LogServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace hb.wpf
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:27:26
    /// description:
    /// </summary>
    public class XImage
    {
        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool Save(BitmapImage image, string destFile)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            if (string.IsNullOrEmpty(destFile))
            {
                throw new ArgumentNullException(nameof(destFile));
            }

            try
            {
                string fullPath = Path.GetFullPath(destFile);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var fs = new FileStream(destFile, FileMode.Create))
                {
                    encoder.Save(fs);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static bool Save(byte[] bytes, string destFile)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(bytes));
            }
            if (string.IsNullOrEmpty(destFile))
            {
                throw new ArgumentNullException(nameof(destFile));
            }

            try
            {
                string fullPath = Path.GetFullPath(destFile);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                using (FileStream fs = new FileStream(destFile, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                    fs.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public static BitmapImage CreateImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            try
            {
                var bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static BitmapImage CreateImage(string filename)
        {
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return null;
            }
            try
            {
                byte[] imageBytes = null;
                using (FileStream fs = File.OpenRead(filename))
                {
                    imageBytes = new byte[(int)fs.Length];
                    fs.Read(imageBytes, 0, imageBytes.Length);
                }
                return CreateImage(imageBytes);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static string ImageToBase64(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            try
            {
                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }

        public static string ImageToBase64(string imageFile)
        {
            if (string.IsNullOrEmpty(imageFile))
            {
                return null;
            }
            if (!File.Exists(imageFile))
            {
                return null;
            }
            try
            {
                using (FileStream fs = File.OpenRead(imageFile))
                {
                    byte[] imageBytes = new byte[(int)fs.Length];
                    fs.Read(imageBytes, 0, imageBytes.Length);
                    return ImageToBase64(imageBytes);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return null;
        }
    }
}
