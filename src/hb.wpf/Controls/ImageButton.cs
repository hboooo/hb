using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hb.wpf.Controls
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/10 1:49:22
    /// description:
    /// </summary>
    public class ImageButton : Button
    {

        public ImageSource DisplayImage
        {
            get { return (ImageSource)GetValue(DisplayImageProperty); }
            set { SetValue(DisplayImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayImageProperty =
            DependencyProperty.Register("DisplayImage", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));


        public double DisplayImageWidth
        {
            get { return (double)GetValue(DisplayImageWidthProperty); }
            set { SetValue(DisplayImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayImageWidthProperty =
            DependencyProperty.Register("DisplayImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(20.0));


        public double DisplayImageHeight
        {
            get { return (double)GetValue(DisplayImageHeightProperty); }
            set { SetValue(DisplayImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayImageHeightProperty =
            DependencyProperty.Register("DisplayImageHeight", typeof(double), typeof(ImageButton), new PropertyMetadata(20.0));


        public ImagePosition Position
        {
            get { return (ImagePosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(ImagePosition), typeof(ImageButton), new PropertyMetadata(ImagePosition.Left));


        public bool IsExecuting
        {
            get { return (bool)GetValue(IsExecutingProperty); }
            set { SetValue(IsExecutingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsExecuting.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExecutingProperty =
            DependencyProperty.Register("IsExecuting", typeof(bool), typeof(ImageButton), new PropertyMetadata(false, (obj, e) =>
            {
                ImageButton imageButton = obj as ImageButton;
                if (imageButton == null) return;

                if (Convert.ToBoolean(e.NewValue) == true)
                {
                    imageButton.BeginStoryboard();
                }
                else
                {
                    imageButton.StopStoryboard();
                }
            }));

        private Storyboard f_Storyboard;
        private Image f_Image;

        private void BeginStoryboard()
        {
            if (f_Image == null)
                f_Image = this.Template.FindName("image", this) as Image;

            if (f_Storyboard == null)
                f_Storyboard = this.FindResource("rotateStoryboard") as Storyboard;

            if (f_Storyboard != null && f_Image != null)
                f_Storyboard.Begin(f_Image, HandoffBehavior.SnapshotAndReplace, true);
        }

        private void StopStoryboard()
        {
            if (f_Storyboard != null && f_Image != null)
                f_Storyboard.Stop(f_Image);
        }

    }

    public enum ImagePosition
    {
        Left,
        Right
    }
}
