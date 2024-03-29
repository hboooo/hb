﻿using Emgu.CV;
using hb.LogServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace hb.demo
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/3 23:59:41
    /// description:
    /// </summary>
    public class RtspRealPlayManager : IDisposable
    {
        private VideoCapture f_VideoCapture;
        //private PictureBox f_PictureBox;      //Winform使用PictureBox
        private Image f_Image;                  //Wpf使用Image
        private object obj = new object();
        private Mat f_OriginalFrame = new Mat();

        private string f_RtspStream = string.Empty;

        private void InitRtsp()
        {
            try
            {
                f_VideoCapture = new VideoCapture(f_RtspStream);
                f_VideoCapture.ImageGrabbed -= VideoCapture_ImageGrabbed;
                f_VideoCapture.ImageGrabbed += VideoCapture_ImageGrabbed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void VideoCapture_ImageGrabbed(object sender, EventArgs e)
        {
            //if (f_PictureBox == null)
            //    return;

            if (f_Image == null)
                return;

            if (f_VideoCapture == null)
                return;

            try
            {
                lock (obj)
                {
                    if (!f_VideoCapture.Retrieve(f_OriginalFrame))
                    {
                        f_OriginalFrame.Dispose();
                        return;
                    }
                    if (f_OriginalFrame.IsEmpty)
                        return;

                    //Winform
                    //f_PictureBox.Invoke(new Action(() =>
                    //{
                    //    f_PictureBox.Image = f_TargetFrame.Bitmap;
                    //}));

                    //Wpf
                    f_Image.Dispatcher.Invoke(new Action(() =>
                    {
                        f_Image.Source = ToBitmapSource(f_OriginalFrame);
                    }));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public bool Start(IntPtr playerHandle, string rtspStream)
        {
            if (IntPtr.Zero.Equals(playerHandle))
            {
                return false;
            };

            PictureBox picture = System.Windows.Forms.Control.FromHandle(playerHandle) as PictureBox;
            if (picture == null)
            {
                return false;
            }

            f_RtspStream = rtspStream ?? throw new ArgumentNullException("rtspStream");
            //f_PictureBox = picture;

            try
            {
                f_VideoCapture?.Stop();
                f_VideoCapture?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            try
            {
                InitRtsp();

                //f_PictureBox = picture;
                f_VideoCapture.Start(new CaptureExceptionHandler());
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

        public bool Start(PictureBox picture, string rtspStream)
        {
            if (string.Compare(f_RtspStream, rtspStream, true) != 0)
            {
                f_RtspStream = rtspStream ?? throw new ArgumentNullException("rtspStream");
                try
                {
                    f_VideoCapture?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                InitRtsp();
            }
            //f_PictureBox = picture;
            f_VideoCapture.Start(new CaptureExceptionHandler());
            return true;
        }

        public bool Start(Image image, string rtspStream)
        {
            if (string.Compare(f_RtspStream, rtspStream, true) != 0)
            {
                f_RtspStream = rtspStream ?? throw new ArgumentNullException("rtspStream");
                try
                {
                    f_VideoCapture?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                InitRtsp();
            }
            f_Image = image;
            f_VideoCapture.Start(new CaptureExceptionHandler());
            return true;
        }

        /// <summary>
        /// Delete a GDI object
        /// </summary>
        /// <param name="o">The poniter to the GDI object to be deleted</param>
        /// <returns></returns>
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public bool Stop()
        {
            f_VideoCapture?.Stop();
            return true;
        }

        public void Dispose()
        {
            f_VideoCapture?.Dispose();
            f_OriginalFrame?.Dispose();
            f_OriginalFrame = null;
        }

    }

    public class CaptureExceptionHandler : ExceptionHandler
    {
        public override bool HandleException(Exception exception)
        {
            //忽略错误
            return true;
        }
    }
}
