using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hb.wpf.Controls
{
    /// <summary>
    /// author     :habo
    /// date       :2022/4/17 1:58:40
    /// description:
    /// </summary>
    internal class BackgroundWoringImageButton:Button
    {
        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImgSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(BackgroundWoringImageButton), new PropertyMetadata(null));


        public double ImageSize
        {
            get { return (double)GetValue(ImageSizeProperty); }
            set { SetValue(ImageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(double), typeof(BackgroundWoringImageButton), new PropertyMetadata(18.0));


        public Visibility ImageVisibility
        {
            get { return (Visibility)GetValue(ImageVisibilityProperty); }
            set { SetValue(ImageVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageVisibilityProperty =
            DependencyProperty.Register("ImageVisibility", typeof(Visibility), typeof(BackgroundWoringImageButton), new PropertyMetadata(Visibility.Hidden));


        public bool IsWorking
        {
            get { return (bool)GetValue(IsWorkingProperty); }
            set { SetValue(IsWorkingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWorking.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWorkingProperty =
            DependencyProperty.Register("IsWorking", typeof(bool), typeof(BackgroundWoringImageButton), new PropertyMetadata(false, (obj, e) =>
            {
                BackgroundWoringImageButton backgroundWorkingButton = obj as BackgroundWoringImageButton;
                if (backgroundWorkingButton == null) return;

                if (Convert.ToBoolean(e.NewValue) == true)
                {
                    backgroundWorkingButton.IsEnabled = false;
                    backgroundWorkingButton.ImageVisibility = Visibility.Visible;
                    backgroundWorkingButton.BeginStoryboard();
                }
                else
                {
                    backgroundWorkingButton.IsEnabled = true;
                    backgroundWorkingButton.ImageVisibility = Visibility.Hidden;
                    backgroundWorkingButton.StopStoryboard();
                }
            }));


        private Storyboard f_Storyboard;
        private Image f_Image;

        private void BeginStoryboard()
        {
            if (f_Image == null)
                f_Image = this.Template.FindName("image", this) as Image;

            if (f_Storyboard == null)
                f_Storyboard = this.FindResource("buttonImageRotate") as Storyboard;

            if (f_Storyboard != null && f_Image != null)
                f_Storyboard.Begin(f_Image, HandoffBehavior.SnapshotAndReplace, true);
        }

        private void StopStoryboard()
        {
            if (f_Storyboard != null && f_Image != null)
                f_Storyboard.Stop(f_Image);
        }
    }
}
