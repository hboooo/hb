using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace hb.wpf.Hook
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 14:39:24
    /// description:
    /// </summary>
    public class MouseHook : XHook
    {
        #region MouseEventType Enum

        private enum MouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove
        }

        #endregion MouseEventType Enum

        #region Events

        public event HookMouseEventHandler MouseDown;

        public event HookMouseEventHandler MouseUp;

        public event HookMouseEventHandler MouseMove;

        public event HookMouseEventHandler MouseWheel;

        public event HookMouseEventHandler Click;

        public event HookMouseEventHandler DoubleClick;

        #endregion Events

        #region Constructor

        public MouseHook()
        {
            _hookType = WH_MOUSE_LL;
        }

        #endregion Constructor

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode > -1 && (MouseDown != null || MouseUp != null || MouseMove != null))
            {
                MouseLLHookStruct mouseHookStruct =
                    (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                MouseButton button = GetButton(wParam);
                MouseEventType eventType = GetEventType(wParam);

                HookMouseEventArgs e = new HookMouseEventArgs(
                    button,
                    eventType == MouseEventType.DoubleClick ? 2 : 1,
                    mouseHookStruct.pt.x,
                    mouseHookStruct.pt.y,
                    eventType == MouseEventType.MouseWheel ? (short)((mouseHookStruct.mouseData >> 16) & 0xffff) : 0);

                // Prevent multiple Right Click events (this probably happens for popup menus)
                if (button == MouseButton.Right && mouseHookStruct.flags != 0)
                {
                    eventType = MouseEventType.None;
                }

                switch (eventType)
                {
                    case MouseEventType.MouseDown:
                        MouseDown?.Invoke(this, e);
                        break;

                    case MouseEventType.MouseUp:
                        Click?.Invoke(this, e);
                        MouseUp?.Invoke(this, e);
                        break;

                    case MouseEventType.DoubleClick:
                        DoubleClick?.Invoke(this, e);
                        break;

                    case MouseEventType.MouseWheel:
                        MouseWheel?.Invoke(this, e);
                        break;

                    case MouseEventType.MouseMove:
                        MouseMove?.Invoke(this, e);
                        break;
                }
            }

            return CallNextHookEx(_handleToHook, nCode, wParam, lParam);
        }

        private MouseButton GetButton(int wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                    return MouseButton.Left;

                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                    return MouseButton.Right;

                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                    return MouseButton.Middle;

                default:
                    return MouseButton.XButton1;
            }
        }

        private MouseEventType GetEventType(int wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return MouseEventType.MouseDown;

                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return MouseEventType.MouseUp;

                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;

                case WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;

                case WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;

                default:
                    return MouseEventType.None;
            }
        }

        public delegate void HookMouseEventHandler(object sender, HookMouseEventArgs e);

        #endregion Methods
    }

    public class HookMouseEventArgs : EventArgs
    {
        public HookMouseEventArgs(MouseButton mouseButton, int clicks, int x, int y, int delta)
        {
            MouseButton = mouseButton;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        public MouseButton MouseButton { get; set; }

        public int Clicks { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Delta { get; set; }
    }
}
