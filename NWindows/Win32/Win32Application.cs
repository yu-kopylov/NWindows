﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace NWindows.Win32
{
    public class Win32Application
    {
        private const string WindowClassName = "DEFAULT";

        private readonly Dictionary<IntPtr, BasicWindow> windows = new Dictionary<IntPtr, BasicWindow>();

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

        public void Run(BasicWindow window)
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
                    window.Title,
                    Win32WindowStyle.WS_OVERLAPPEDWINDOW,
                    CW_USEDEFAULT, CW_USEDEFAULT,
                    window.Width, window.Height,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    hInstance,
                    IntPtr.Zero
                );

                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Failed to create a window.");
                }

                window.NativeWindow = new Win32Window(hwnd);
                windows.Add(hwnd, window);

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
            if (uMsg == Win32MessageType.WM_DESTROY)
            {
                windows.Remove(hwnd);
                Win32API.PostQuitMessage(0);
                return IntPtr.Zero;
            }

            if (uMsg == Win32MessageType.WM_MOUSEMOVE)
            {
                ulong lParam32 = (uint) lParam.ToInt64();
                int x = (short) (lParam32 & 0xFFFF);
                int y = (short) ((lParam32 >> 16) & 0xFFFF);
                if (windows.TryGetValue(hwnd, out var window))
                {
                    window.OnMouseMove(new Point(x, y));
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
                        using (Win32Canvas canvas = new Win32Canvas(hdc))
                        {
                            // todo: check that width and height are exact and aligned with other API
                            Rectangle area = new Rectangle(ps.rcPaint.left, ps.rcPaint.top, ps.rcPaint.Width, ps.rcPaint.Height);
                            window.Paint(canvas, area);
                        }
                    }
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