namespace NWindows.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using ATOM = System.UInt16;
    using BOOL = System.Int32;
    using BYTE = System.Byte;
    using COLORREF = System.UInt32;
    using DWORD = System.UInt32;
    using LONG = System.Int32;
    using UINT = System.UInt32;
    using HBRUSH = System.IntPtr;
    using HCURSOR = System.IntPtr;
    using HDC = System.IntPtr;
    using HGDIOBJ = System.IntPtr;
    using HICON = System.IntPtr;
    using HINSTANCE = System.IntPtr;
    using HMENU = System.IntPtr;
    using HMODULE = System.IntPtr;
    using HPEN = System.IntPtr;
    using HWND = System.IntPtr;
    using LPARAM = System.IntPtr;
    using LPVOID = System.IntPtr;
    using LRESULT = System.IntPtr;
    using WPARAM = System.IntPtr;

    internal static class Win32API
    {
        [DllImport("Kernel32.dll")]
        public static extern HMODULE GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("User32.dll")]
        public static extern ATOM RegisterClassExW(ref WNDCLASSEXW arg1);

        [DllImport("User32.dll")]
        public static extern BOOL UnregisterClassW([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, HINSTANCE hInstance);

        [DllImport("User32.dll")]
        public static extern HWND CreateWindowExW(
            DWORD dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            Win32WindowStyle dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            HWND hWndParent,
            HMENU hMenu,
            HINSTANCE hInstance,
            LPVOID lpParam
        );

        [DllImport("User32.dll")]
        public static extern BOOL DestroyWindow(HWND hWnd);

        [DllImport("User32.dll")]
        public static extern BOOL ShowWindow(HWND hWnd, Win32ShowWindowCommand nCmdShow);

        [DllImport("User32.dll")]
        public static extern LRESULT DefWindowProcW(
            HWND hWnd,
            Win32MessageType uMsg,
            WPARAM wParam,
            LPARAM lParam
        );

        [DllImport("User32.dll")]
        public static extern BOOL GetMessageW(
            ref MSG lpMsg,
            HWND hWnd,
            UINT wMsgFilterMin,
            UINT wMsgFilterMax
        );

        [DllImport("User32.dll")]
        public static extern BOOL TranslateMessage(ref MSG lpMsg);

        [DllImport("User32.dll")]
        public static extern LRESULT DispatchMessageW(ref MSG lpMsg);

        [DllImport("User32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [DllImport("User32.dll")]
        public static extern HDC BeginPaint(HWND hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("User32.dll")]
        public static extern BOOL EndPaint(HWND hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("User32.dll")]
        public static extern int FillRect(HDC hDC, ref RECT lprc, HBRUSH hbr);
    }

    internal static class Gdi32API
    {
        [DllImport("Gdi32.dll")]
        public static extern HBRUSH CreateSolidBrush(COLORREF color);

        [DllImport("Gdi32.dll")]
        public static extern HPEN CreatePen(GdiPenStyle iStyle, int cWidth, COLORREF color);

        [DllImport("Gdi32.dll")]
        public static extern BOOL DeleteObject(HGDIOBJ ho);

        [DllImport("Gdi32.dll")]
        public static extern HGDIOBJ GetStockObject(GdiStockObjectType i);

        [DllImport("Gdi32.dll")]
        public static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ h);

        [DllImport("Gdi32.dll")]
        public static extern BOOL Rectangle(HDC hdc, int left, int top, int right, int bottom);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WNDCLASSEXW
    {
        public UINT cbSize;
        public UINT style;
        public WNDPROC lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public HINSTANCE hInstance;
        public HICON hIcon;
        public HCURSOR hCursor;
        public HBRUSH hbrBackground;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszClassName;

        public HICON hIconSm;
    }

    internal delegate LRESULT WNDPROC(HWND hwnd, Win32MessageType uMsg, WPARAM wParam, LPARAM lParam);

    [Flags]
    internal enum Win32WindowStyle : uint
    {
        WS_CAPTION = 0x00C00000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_OVERLAPPED = 0x0000000,
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000
    }

    internal enum Win32ShowWindowCommand
    {
        SW_SHOW = 5,
        SW_SHOWDEFAULT = 10,
        SW_SHOWNORMAL = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public readonly HWND hwnd;
        public readonly UINT message;
        public readonly WPARAM wParam;
        public readonly LPARAM lParam;
        public readonly DWORD time;
        public readonly POINT pt;
        public readonly DWORD lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public readonly LONG x;
        public readonly LONG y;
    }

    internal enum Win32MessageType : uint
    {
        WM_DESTROY = 0x0002,
        WM_PAINT = 0x000F
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PAINTSTRUCT
    {
        public HDC hdc;
        public BOOL fErase;
        public RECT rcPaint;
        public BOOL fRestore;
        public BOOL fIncUpdate;
        public ulong rgbReserved1;
        public ulong rgbReserved2;
        public ulong rgbReserved3;
        public ulong rgbReserved4;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public LONG left;
        public LONG top;
        public LONG right;
        public LONG bottom;
    }

    internal enum GdiPenStyle
    {
        PS_SOLID = 0,
        PS_DASH = 1,
        PS_DOT = 2,
        PS_DASHDOT = 3,
        PS_DASHDOTDOT = 4,
        PS_NULL = 5,
        PS_INSIDEFRAME = 6
    }

    internal enum GdiStockObjectType
    {
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8
    }
}