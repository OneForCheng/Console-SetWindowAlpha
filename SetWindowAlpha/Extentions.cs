using System;
using System.Runtime.InteropServices;

namespace SetWindowAlpha
{
    public static class Extentions
    {
        #region 设置窗口透明度
        private const int GwlExstyle = -20;
        private const int WsExLayered = 0x80000;
        private const int LwaAlpha = 2;

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        /// <summary>
        /// 设置窗口透明度
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="alpha"></param>
        public static bool SetWindowAlpha(this IntPtr handle, byte alpha)
        {
            SetWindowLong(handle, GwlExstyle, GetWindowLong(handle, GwlExstyle) | WsExLayered);
            return SetLayeredWindowAttributes(handle, 0, alpha, LwaAlpha);
        }

        #endregion

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

    }
}
