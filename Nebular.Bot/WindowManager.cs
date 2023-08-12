using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Nebular.Core
{
    public class WindowManager
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);



        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
            public static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;
        private const uint INPUT_HARDWARE = 2;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_CHAR = 0x0102;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;

        private IntPtr hwnd;

        public WindowManager(int pid)
        {
            hwnd = IntPtr.Zero;
            EnumWindows(new EnumWindowsProc((hWnd, lParam) =>
            {
                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);
                if (windowPid == pid)
                {
                    if (hwnd == IntPtr.Zero)
                    {
                        hwnd = hWnd;
                        return false;
                    }
                }
                return true;
            }), IntPtr.Zero);

            if (hwnd == IntPtr.Zero)
            {
                throw new Exception("La fenêtre n'a pas été trouvée");
            }
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 750, 595, SWP_NOMOVE | SWP_NOZORDER);
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;

        private SemaphoreSlim ClickSemaphore = new SemaphoreSlim(1, 1);
        public void Click(int x, int y)
        {
            lock (this)
            {
                Console.WriteLine("POS: "+ x + ", " + y);
                this.ActivateWindow();
                int lParam = MakeLParam(x, y);
                SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
                SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
                Task.Delay(7).Wait();
                SendMessage(hwnd, WM_LBUTTONUP, 0, lParam);
                Task.Delay(100).Wait();
                lParam = MakeLParam(0, 0);
                SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
                Task.Delay(40).Wait();
            }
        }

        public void PostMessageClick(int x, int y)
        {
            lock (this)
            {
                int lParam = MakeLParam(x, y);
                SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
                Task.Delay(7).Wait();
                SendMessage(hwnd, WM_LBUTTONUP, 0, lParam);
            }
        }

        public void Move(int x, int y)
        {
            lock (this)
            {
                this.ActivateWindow();
                int lParam = MakeLParam(x, y);
                SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
                Task.Delay(10).Wait();
            }
        }

        internal void MouseDown(int x, int y)
        {
            ActivateWindow();
            int lParam = MakeLParam(x, y);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
            SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
        }

        internal void MouseUp(int x, int y)
        {
            ActivateWindow();
            int lParam = MakeLParam(x, y);
            SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
            SendMessage(hwnd, WM_LBUTTONUP, 0, lParam);
        }

        public void RightClick(int x, int y)
        {
            ActivateWindow();
            int lParam = MakeLParam(x, y);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
            SendMessage(hwnd, WM_RBUTTONDOWN, 1, lParam);
            Task.Delay(50).Wait();
            lParam = MakeLParam(0, 0);
            SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
        }


        public void DoubleClick(int x, int y)
        {
            lock (this)
            {
                this.ActivateWindow();
                int lParam = MakeLParam(x, y);
                SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
                SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
                SendMessage(hwnd, WM_LBUTTONUP, 0, lParam);
                SendMessage(hwnd, WM_LBUTTONDOWN, 1, lParam);
                SendMessage(hwnd, WM_LBUTTONUP, 0, lParam);
                Task.Delay(100).Wait();
                lParam = MakeLParam(0, 0);
                SendMessage(hwnd, WM_MOUSEMOVE, 0, lParam);
                Task.Delay(40).Wait();
            }
        }

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private const uint MAPVK_VK_TO_VSC = 0x00;



        private void pressUp(ushort vkKey)
        {
            ActivateWindow();
            uint scanCode = MapVirtualKey(vkKey, MAPVK_VK_TO_VSC);
            int lParam = (1 << 0) | // repeat count
                         ((int)scanCode << 16) | // scan code
                         (0 << 24) | // extended key
                         (0 << 29) | // context code
                         (0 << 30) | // previous key state
                         (0 << 31); // transition state
            PostMessage(hwnd, WM_KEYUP, vkKey, lParam);
        }

        private void pressDown(ushort vkKey)
        {
            ActivateWindow();
            uint scanCode = MapVirtualKey(vkKey, MAPVK_VK_TO_VSC);
            int lParam = (1 << 0) | // repeat count
                         ((int)scanCode << 16) | // scan code
                         (0 << 24) | // extended key
                         (0 << 29) | // context code
                         (0 << 30) | // previous key state
                         (0 << 31); // transition state
            PostMessage(hwnd, WM_KEYDOWN, vkKey, lParam);
        }


        public void Press(VirtualKeys key)
        {
            PressDown(key);
            Task.Delay(20).Wait();
            PressUp(key);
        }
        public void PressDown(VirtualKeys key) => pressDown((ushort)key);
        public void PressUp(VirtualKeys key) => pressUp((ushort)key);



        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int WM_NCACTIVATE = 0x0086;
        private const int WM_ACTIVATE = 0x0006;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_SETFOCUS = 0x0007;


        public void ActivateWindow()
        {
            PostMessage(this.hwnd, WM_WINDOWPOSCHANGING, 0, 0);
            PostMessage(this.hwnd, WM_NCACTIVATE, 1, 0);
            PostMessage(this.hwnd, WM_ACTIVATE, 1, 0);
            PostMessage(this.hwnd, WM_IME_SETCONTEXT, 1, -1);
            PostMessage(this.hwnd, WM_IME_NOTIFY, 2, 0);
            PostMessage(this.hwnd, WM_SETFOCUS, 0, 0);
        }

        public void PressEnter()
        {
            this.Press(VirtualKeys.ENTER);
        }

        private int MakeLParam(int LoWord, int HiWord)
        {
            return ((HiWord << 16) | (LoWord & 0xffff));
        }


        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);
        private ushort VkKeyScan(string key)
        {
            return (ushort)VkKeyScan(key[0]);
        }


    }
}
