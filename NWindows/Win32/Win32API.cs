namespace NWindows.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using ATOM = System.UInt16;
    using ARGB = System.Int32;
    using BOOL = System.Int32;
    using BYTE = System.Byte;
    using COLORREF = System.UInt32;
    using DWORD = System.UInt32;
    using INT = System.Int32;
    using LONG = System.Int32;
    using UINT = System.UInt32;
    using UINT32 = System.UInt32;
    using GpBrush = System.IntPtr;
    using GpFont = System.IntPtr;
    using GpFontCollection = System.IntPtr;
    using GpFontFamily = System.IntPtr;
    using GpGraphics = System.IntPtr;
    using GpImage = System.IntPtr;
    using GpSolidFill = System.IntPtr;
    using GpStringFormat = System.IntPtr;
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
    using ULONG_PTR = System.IntPtr;
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
        public static extern BOOL SetWindowTextW(HWND hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpString);

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

    internal static class GdiPlusAPI
    {
        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdiplusStartup(out ULONG_PTR token, ref GdiplusStartupInput input, out GdiplusStartupOutput output);

        [DllImport("Gdiplus.dll")]
        public static extern void GdiplusShutdown(ULONG_PTR token);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipCreateFromHDC(HDC hdc, out GpGraphics graphics);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipCreateFromHWND(HWND hwnd, out GpGraphics graphics);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipDeleteGraphics(GpGraphics graphics);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipCreateSolidFill(ARGB color, out GpSolidFill brush);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipDeleteBrush(GpBrush brush);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipFillRectangleI(GpGraphics graphics, GpBrush brush, INT x, INT y, INT width, INT height);

        [DllImport("Gdiplus.dll", CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipCreateFontFamilyFromName(string name, GpFontCollection fontCollection, out GpFontFamily fontFamily);

        [DllImport("Gdiplus.dll", CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDeleteFontFamily(GpFontFamily fontFamily);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipCreateFont(GpFontFamily fontFamily, float emSize, FontStyle style, Unit unit, out GpFont font);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipDeleteFont(GpFont font);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipStringFormatGetGenericDefault(out GpStringFormat format);

        [DllImport("Gdiplus.dll")]
        public static extern GpStatus GdipDeleteStringFormat(GpStringFormat format);

        [DllImport("Gdiplus.dll", CharSet = CharSet.Unicode)]
        public static extern GpStatus GdipDrawString(
            GpGraphics graphics,
            string text,
            INT length,
            GpFont font,
            ref RectF layoutRect,
            GpStringFormat stringFormat,
            GpBrush brush
        );

        public static void CheckStatus(GpStatus status)
        {
            if (status != GpStatus.Ok)
            {
                throw new InvalidOperationException($"GDI+ error: {status}.");
            }
        }

        public static FontStyle GetFontStyle(FontConfig font)
        {
            FontStyle style = FontStyle.FontStyleRegular;
            if (font.IsBold)
            {
                style |= FontStyle.FontStyleBold;
            }

            if (font.IsItalic)
            {
                style |= FontStyle.FontStyleItalic;
            }

            if (font.IsUnderline)
            {
                style |= FontStyle.FontStyleUnderline;
            }

            if (font.IsStrikeout)
            {
                style |= FontStyle.FontStyleStrikeout;
            }

            return style;
        }
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

        public LONG Width => right - left;
        public LONG Height => bottom - top;
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

    internal enum GpStatus
    {
        Ok,
        GenericError,
        InvalidParameter,
        OutOfMemory,
        ObjectBusy,
        InsufficientBuffer,
        NotImplemented,
        Win32Error,
        WrongState,
        Aborted,
        FileNotFound,
        ValueOverflow,
        AccessDenied,
        UnknownImageFormat,
        FontFamilyNotFound,
        FontStyleNotFound,
        NotTrueTypeFont,
        UnsupportedGdiplusVersion,
        GdiplusNotInitialized,
        PropertyNotFound,
        PropertyNotSupported,
        ProfileNotFound
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GdiplusStartupInput
    {
        private UINT32 GdiplusVersion;
        private readonly IntPtr DebugEventCallback;
        private readonly BOOL SuppressBackgroundThread;
        private readonly BOOL SuppressExternalCodecs;

        delegate void DebugEventProc(BOOL suppressBackgroundThread, BOOL suppressExternalCodecs);

        public static GdiplusStartupInput CreateV1()
        {
            return new GdiplusStartupInput {GdiplusVersion = 1};
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GdiplusStartupOutput
    {
        private readonly IntPtr NotificationHook;
        private readonly IntPtr NotificationUnhook;
    }

    [Flags]
    internal enum FontStyle
    {
        FontStyleRegular = 0,
        FontStyleBold = 1,
        FontStyleItalic = 2,
        FontStyleBoldItalic = 3,
        FontStyleUnderline = 4,
        FontStyleStrikeout = 8
    }

    internal enum Unit
    {
        UnitWorld = 0,
        UnitDisplay = 1,
        UnitPixel = 2,
        UnitPoint = 3,
        UnitInch = 4,
        UnitDocument = 5,
        UnitMillimeter = 6
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RectF
    {
        private readonly float x;
        private readonly float y;
        private readonly float width;
        private readonly float height;

        public RectF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}