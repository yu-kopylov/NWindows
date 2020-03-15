using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32Application : INativeApplication
    {
        private const string WindowClassName = "DEFAULT";

        private readonly Dictionary<IntPtr, Win32Window> windows = new Dictionary<IntPtr, Win32Window>();

        private Win32Graphics graphics;
        private Win32ImageCodec imageCodec;
        private Win32Clipboard clipboard;

        public void Dispose()
        {
            graphics.Dispose();
        }

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
            catch (EntryPointNotFoundException)
            {
                return false;
            }
        }

        public void Init()
        {
            graphics = new Win32Graphics();
            imageCodec = new Win32ImageCodec();
            clipboard = new Win32Clipboard();
        }

        public void Run(INativeWindowStartupInfo startupInfo)
        {
            GdiplusStartupInput gdiPlusStartupInput = GdiplusStartupInput.CreateV1();
            GdiPlusAPI.CheckStatus(GdiPlusAPI.GdiplusStartup(out var gdiPlusToken, ref gdiPlusStartupInput, out _));
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
                    startupInfo.Title,
                    Win32WindowStyle.WS_OVERLAPPEDWINDOW,
                    CW_USEDEFAULT, CW_USEDEFAULT,
                    startupInfo.Width, startupInfo.Height,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    hInstance,
                    IntPtr.Zero
                );

                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to create a window.");
                }

                var window = new Win32Window(hwnd, startupInfo);
                windows.Add(hwnd, window);
                startupInfo.OnCreate(window);

                try
                {
                    Win32API.ShowWindow(hwnd, Win32ShowWindowCommand.SW_SHOWNORMAL);

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

                // todo: use separate finally for GDI+
                GdiPlusAPI.GdiplusShutdown(gdiPlusToken);
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, Win32MessageType uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (HandleWindowEvent(hwnd, uMsg, wParam, lParam))
            {
                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_SETCURSOR)
            {
                uint lParam32 = (uint) lParam.ToInt64();
                var hitTest = (Win32HitTestResult) (lParam32 & 0xFFFF);
                if (hitTest == Win32HitTestResult.HTCLIENT)
                {
                    IntPtr cursor = Win32API.LoadImageW(
                        IntPtr.Zero,
                        Win32SystemResources.IDC_ARROW,
                        Win32ImageType.IMAGE_CURSOR,
                        0, 0,
                        Win32LoadImageFlags.LR_DEFAULTSIZE | Win32LoadImageFlags.LR_SHARED
                    );
                    if (cursor != IntPtr.Zero)
                    {
                        Win32API.SetCursor(cursor);
                        return IntPtr.Zero;
                    }
                }
            }

            if (uMsg == Win32MessageType.WM_DESTROY)
            {
                windows.Remove(hwnd);
                Win32API.PostQuitMessage(0);
                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_MOUSEMOVE)
            {
                if (windows.TryGetValue(hwnd, out var window))
                {
                    uint wParam32 = (uint) wParam.ToInt64();
                    uint lParam32 = (uint) lParam.ToInt64();
                    int x = (short) (lParam32 & 0xFFFF);
                    int y = (short) ((lParam32 >> 16) & 0xFFFF);
                    window.StartupInfo.OnMouseMove(new Point(x, y));
                }

                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_CHAR)
            {
                if (windows.TryGetValue(hwnd, out var window))
                {
                    char c = (char) wParam.ToInt64();

                    if (!char.IsControl(c))
                    {
                        window.StartupInfo.OnTextInput(c.ToString());
                    }
                }

                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_PAINT)
            {
                PAINTSTRUCT ps = new PAINTSTRUCT();
                IntPtr hdc = Win32API.BeginPaint(hwnd, ref ps);
                try
                {
                    if (windows.TryGetValue(hwnd, out var window))
                    {
                        Rectangle area = new Rectangle(ps.rcPaint.left, ps.rcPaint.top, ps.rcPaint.Width, ps.rcPaint.Height);
                        using (Win32Bitmap bitmap = Win32Bitmap.Create(hdc, area.Width, area.Height))
                        {
                            using (Win32Canvas canvas = graphics.CreateCanvas(bitmap, new Point(-area.X, -area.Y)))
                            {
                                window.StartupInfo.OnPaint(canvas, area);
                            }

                            bitmap.CopyTo(hdc, area.X, area.Y);
                        }
                    }
                }
                finally
                {
                    Win32API.EndPaint(hwnd, ref ps);
                }

                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_SIZE)
            {
                if (windows.TryGetValue(hwnd, out var window))
                {
                    ulong lParam32 = (uint) lParam.ToInt64();
                    int width = (short) (lParam32 & 0xFFFF);
                    int height = (short) ((lParam32 >> 16) & 0xFFFF);
                    window.StartupInfo.OnResize(new Size(width, height));
                }

                return IntPtr.Zero;
            }

            return Win32API.DefWindowProcW(hwnd, uMsg, wParam, lParam);
        }

        private bool HandleWindowEvent(IntPtr hwnd, Win32MessageType uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (!windows.TryGetValue(hwnd, out var window))
            {
                return false;
            }

            uint wParam32 = (uint) wParam.ToInt64();
            uint lParam32 = (uint) lParam.ToInt64();

            return Win32EventHandler.HandleWindowEvent(window, uMsg, wParam32, lParam32);
        }

        public INativeGraphics Graphics
        {
            get { return graphics; }
        }

        public INativeImageCodec ImageCodec
        {
            get { return imageCodec; }
        }

        public INativeClipboard Clipboard
        {
            get { return clipboard; }
        }
    }
}