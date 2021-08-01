using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace hb.wpf
{
    /// <summary>
    /// author     :habo
    /// date       :2021/8/1 7:33:42
    /// description:AreoWindow
    /// </summary>
    public class AreoWindow : Window
    {
        public static DependencyProperty TitleForegroundProperty = DependencyProperty.Register("TitleForeground", typeof(SolidColorBrush), typeof(AreoWindow), new PropertyMetadata(Brushes.Black));

        static AreoWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AreoWindow), new FrameworkPropertyMetadata(typeof(AreoWindow)));
        }

        public SolidColorBrush TitleForeground
        {
            get
            {
                return (SolidColorBrush)GetValue(TitleForegroundProperty);
            }
            set
            {
                SetValue(TitleForegroundProperty, value);
            }
        }

        public AreoWindow()
        {
            InitializeCommands();
            this.Loaded += (a, b) => AreoWindowHelper.AreoWindow(this);
        }

        private void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
        }

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            //SystemCommands.CloseWindow(this);
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            try
            {
                // Load window placement details for previous application session from application settings
                // Note - if window was closed on a monitor that is now disconnected from the computer,
                //        SetWindowPlacement will place the window onto a visible monitor.
                WindowPlacement wp = new WindowPlacement();//Settings.Default.WindowPlacement;
                wp.length = Marshal.SizeOf(typeof(WindowPlacement));
                wp.flags = 0;
                wp.showCmd = (wp.showCmd == WindowPlacementHelper.SwShowminimized ? WindowPlacementHelper.SwShownormal : wp.showCmd);
                var hwnd = new WindowInteropHelper(this).Handle;
                WindowPlacementHelper.SetWindowPlacement(hwnd, ref wp);
            }
            catch
            {
                // ignored
            }

        }

        // WARNING - Not fired when Application.SessionEnding is fired
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // Persist window placement details to application settings
            WindowPlacement wp;
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowPlacementHelper.GetWindowPlacement(hwnd, out wp);
            //Settings.Default.WindowPlacement = wp;
            //Settings.Default.Save();
        }
    }

    public struct ACCENTPOLICY
    {
        public int nAccentState;
        public int nFlags;
        public int nColor;
        public int nAnimationId;
    }

    public struct WINCOMPATTRDATA
    {
        public int nAttribute;
        public IntPtr pData;
        public int ulDataSize;
    }

    public class AreoWindowHelper
    {
        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WINCOMPATTRDATA data);

        private const int ACCENT_ENABLE_BLURBEHIND = 3;
        private const int WCA_ACCENT_POLICY = 19;

        public static void AreoWindow(System.Windows.Window window)
        {
            var winhelp = new WindowInteropHelper(window);

            ACCENTPOLICY policy_Blur = new ACCENTPOLICY();
            policy_Blur.nAccentState = ACCENT_ENABLE_BLURBEHIND;
            policy_Blur.nFlags = 0;
            policy_Blur.nColor = 0;
            policy_Blur.nAnimationId = 0;

            WINCOMPATTRDATA wINCOMPATTRDATA = new WINCOMPATTRDATA();
            wINCOMPATTRDATA.nAttribute = WCA_ACCENT_POLICY;
            IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(policy_Blur));
            Marshal.StructureToPtr(policy_Blur, pData, false);
            wINCOMPATTRDATA.pData = pData;
            wINCOMPATTRDATA.ulDataSize = Marshal.SizeOf(policy_Blur);

            SetWindowCompositionAttribute(winhelp.Handle, ref wINCOMPATTRDATA);

            Marshal.FreeHGlobal(pData);
        }
    }

    public class WindowPlacementHelper
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WindowPlacement lpwndpl);

        public const int SwShownormal = 1;
        public const int SwShowminimized = 2;
    }

    public partial class AreaWindowEvent
    {
        private DateTime lastTime = DateTime.MinValue;
        private const int MouseDoubleClickTimeSpan = 500;

        public void Icon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var ts = DateTime.Now - lastTime;

            if (ts.TotalMilliseconds < MouseDoubleClickTimeSpan)
            {
                System.Windows.Application.Current.Shutdown(0);
            }
            else
            {
                var element = e.OriginalSource as FrameworkElement;
                if (element == null)
                    return;

                var window = FindAncestorWindow(element);

                var point = window.WindowState == WindowState.Maximized ? new System.Windows.Point(0, element.ActualHeight)
                    : new System.Windows.Point(window.Left + window.BorderThickness.Left, element.ActualHeight + window.Top + window.BorderThickness.Top);
                point = element.TransformToAncestor(window).Transform(point);
                SystemCommands.ShowSystemMenu(window, point);
            }

            lastTime = DateTime.Now;
        }

        private Window FindAncestorWindow(UIElement uIElement)
        {
            var window = LogicalTreeHelper.GetParent(uIElement);
            DependencyObject temp = null;

            while (window != null)
            {
                temp = window;
                window = System.Windows.Media.VisualTreeHelper.GetParent(window);
            }

            return temp as Window;
        }
    }
}
