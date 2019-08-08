using System;
using System.Runtime.InteropServices;

namespace NWindows.Win32
{
    public class Win32Application
    {
        private const string WindowClassName = "DEFAULT";

        public static bool IsAvailable()
        {
            try
            {
                Win32API.GetModuleHandleW(null);
                // todo: check UI capabilities
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }

        public void Run(Window window)
        {
            IntPtr hInstance = Win32API.GetModuleHandleW(null);

            var windowClass = new WNDCLASSEXW();
            windowClass.cbSize = (uint) Marshal.SizeOf<WNDCLASSEXW>();
            windowClass.lpfnWndProc = WindowProc;
            windowClass.hInstance = hInstance;
            windowClass.lpszClassName = WindowClassName;
            var atom = Win32API.RegisterClassExW(ref windowClass);
            if (atom == 0)
            {
                throw new InvalidOperationException("Failed to register window class.");
            }

            try
            {
                const int CW_USEDEFAULT = unchecked((int) 0x80000000);
                IntPtr hwnd = Win32API.CreateWindowExW(
                    0,
                    WindowClassName,
                    window.Title,
                    Win32WindowStyle.WS_OVERLAPPEDWINDOW,
                    CW_USEDEFAULT, CW_USEDEFAULT,
                    600, 400,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    hInstance,
                    IntPtr.Zero
                );

                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to create a window.");
                }

                Win32API.ShowWindow(hwnd, Win32ShowWindowCommand.SW_SHOWNORMAL);

                try
                {
                    MSG msg = new MSG();
                    int mRet;
                    while ((mRet = Win32API.GetMessageW(ref msg, IntPtr.Zero, 0, 0)) != 0)
                    {
                        if (mRet == -1)
                        {
                            throw new InvalidOperationException("Failed to retrieve a message.");
                        }

                        Win32API.TranslateMessage(ref msg);
                        Win32API.DispatchMessageW(ref msg);
                    }
                }
                finally
                {
                    // usually window will be destroyed before this point, unless error occurs
                    if (Win32API.DestroyWindow(hwnd) == 0)
                    {
                        // todo: log warning ?
                    }
                }
            }
            finally
            {
                if (Win32API.UnregisterClassW(WindowClassName, hInstance) == 0)
                {
                    // todo: log warning
                }
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, Win32MessageType uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (uMsg == Win32MessageType.WM_DESTROY)
            {
                Win32API.PostQuitMessage(0);
                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_PAINT)
            {
                PAINTSTRUCT ps = new PAINTSTRUCT();
                IntPtr hdc = Win32API.BeginPaint(hwnd, ref ps);
                try
                {
                    // todo: window.Paint();
                    Win32API.FillRect(hdc, ref ps.rcPaint, new IntPtr(5 /*COLOR_WINDOW*/));
                    RECT rect = new RECT {left = 10, top = 10, right = 200, bottom = 100};
                    Win32API.FillRect(hdc, ref rect, new IntPtr(6 /*COLOR_WINDOWFRAME*/));
                }
                finally
                {
                    Win32API.EndPaint(hwnd, ref ps);
                }

                return IntPtr.Zero;
            }

            return Win32API.DefWindowProcW(hwnd, uMsg, wParam, lParam);
        }
    }
}