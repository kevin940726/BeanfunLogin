using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BeanfunLogin
{
    public static class WindowsAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            uint Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam            // 参数2
        );
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            uint Msg,            // 消息ID
            int wParam,         // 参数1
            IntPtr lParam            // 参数2
        );
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,
            byte bScan,
            int dwFlags,
            int dwExtraInfo
        );
    }
}

