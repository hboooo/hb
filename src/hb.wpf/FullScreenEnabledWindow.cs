using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace hb.wpf
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 16:17:53
    /// description:wpf窗体全屏
    /// </summary>
    public class FullScreenEnabledWindow : Window
    {
        public bool FullScreen
        {
            get { return (bool)GetValue(FullScreenProperty); }
            set { SetValue(FullScreenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullScreen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullScreenProperty =
            DependencyProperty.Register("FullScreen", typeof(bool), typeof(FullScreenEnabledWindow), new PropertyMetadata(false));


        WindowState previousWindowState = WindowState.Maximized;
        double oldLeft, oldTop, oldWidth, oldHeight;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FullScreenProperty)
            {
                if ((bool)e.NewValue)
                {
                    if (WindowState == WindowState.Normal || WindowState == WindowState.Maximized)
                        previousWindowState = WindowState;
                    oldLeft = Left;
                    oldTop = Top;
                    oldWidth = Width;
                    oldHeight = Height;

                    WindowInteropHelper interop = new WindowInteropHelper(this);
                    interop.EnsureHandle();
                    Screen screen = Screen.FromHandle(interop.Handle);

                    Rect bounds = screen.Bounds.ToWpf().TransformFromDevice(this);

                    ResizeMode = ResizeMode.NoResize;
                    Left = bounds.Left;
                    Top = bounds.Top;
                    Width = bounds.Width;
                    Height = bounds.Height;
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                }
                else
                {
                    ClearValue(WindowStyleProperty);
                    ClearValue(ResizeModeProperty);
                    ClearValue(MaxWidthProperty);
                    ClearValue(MaxHeightProperty);
                    WindowState = previousWindowState;
                    Left = oldLeft;
                    Top = oldTop;
                    Width = oldWidth;
                    Height = oldHeight;
                }
            }
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F12)
            {
                this.FullScreen = !this.FullScreen;
            }
        }
    }

    public static class WinFormsExtensions
    {

        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }

        public static System.Windows.Point ToWpf(this System.Drawing.Point p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }

        public static System.Windows.Size ToWpf(this System.Drawing.Size s)
        {
            return new System.Windows.Size(s.Width, s.Height);
        }

        public static System.Windows.Rect ToWpf(this System.Drawing.Rectangle rect)
        {
            return new System.Windows.Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
        }
    }

}
