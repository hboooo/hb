using System.Runtime.InteropServices;
using System.Text;

namespace hb.wpf.Native
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/25 12:52:51
    /// description:
    /// </summary>
    public static class NativeMethods
    {
        internal enum MapType : uint
        {
            MapvkVkToVsc = 0x0,
            MapvkVscToVk = 0x1,
            MapvkVkToChar = 0x2,
            MapvkVscToVkEx = 0x3,
        }

        [DllImport("user32.dll")]
        internal static extern int ToUnicode(uint wVirtKey,
                                             uint wScanCode,
                                             byte[] lpKeyState,
                                             [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
                                             int cchBuff,
                                             uint wFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, MapType uMapType);
    }
}
