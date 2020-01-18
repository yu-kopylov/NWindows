using System.Drawing;

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
    using SHORT = System.Int16;
    using UINT = System.UInt32;
    using UINT32 = System.UInt32;
    using WORD = System.UInt16;
    using GpBrush = System.IntPtr;
    using GpFont = System.IntPtr;
    using GpFontCollection = System.IntPtr;
    using GpFontFamily = System.IntPtr;
    using GpGraphics = System.IntPtr;
    using GpImage = System.IntPtr;
    using GpSolidFill = System.IntPtr;
    using GpStringFormat = System.IntPtr;
    using HANDLE = System.IntPtr;
    using HBITMAP = System.IntPtr;
    using HBRUSH = System.IntPtr;
    using HCURSOR = System.IntPtr;
    using HDC = System.IntPtr;
    using HFONT = System.IntPtr;
    using HGDIOBJ = System.IntPtr;
    using HGLOBAL = System.IntPtr;
    using HICON = System.IntPtr;
    using HINSTANCE = System.IntPtr;
    using HMENU = System.IntPtr;
    using HMODULE = System.IntPtr;
    using HPEN = System.IntPtr;
    using HWND = System.IntPtr;
    using HRGN = System.IntPtr;
    using LPARAM = System.IntPtr;
    using LPVOID = System.IntPtr;
    using LRESULT = System.IntPtr;
    using SIZE_T = System.IntPtr;
    using ULONG_PTR = System.IntPtr;
    using WPARAM = System.IntPtr;

    internal static class Win32API
    {
        [DllImport("Kernel32.dll")]
        public static extern HMODULE GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        [DllImport("Kernel32.dll")]
        public static extern HGLOBAL GlobalAlloc(GlobalAllocFlags uFlags, SIZE_T dwBytes);

        [DllImport("Kernel32.dll")]
        public static extern HGLOBAL GlobalFree(HGLOBAL hMem);

        [DllImport("Kernel32.dll")]
        public static extern LPVOID GlobalLock(HGLOBAL hMem);

        [DllImport("Kernel32.dll")]
        public static extern BOOL GlobalUnlock(HGLOBAL hMem);

        [DllImport("User32.dll")]
        private static extern HDC GetDC(HWND hWnd);

        public static HDC GetDCChecked(HWND hWnd)
        {
            HDC hdc = GetDC(hWnd);
            if (hdc == IntPtr.Zero)
            {
                throw new InvalidOperationException($"{nameof(GetDC)} failed. Window handle: 0x{hWnd.ToString("X16")}.");
            }

            return hdc;
        }

        [DllImport("User32.dll")]
        private static extern int ReleaseDC(HWND hWnd, HDC hDC);

        public static void ReleaseDCChecked(HWND hWnd, HDC hDC)
        {
            int res = ReleaseDC(hWnd, hDC);
            if (res != 1)
            {
                throw new InvalidOperationException($"{nameof(ReleaseDC)} failed. Window handle: 0x{hWnd.ToString("X16")}. HDC: 0x{hDC.ToString("X16")}.");
            }
        }

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
        public static extern SHORT GetKeyState(W32VirtualKey nVirtKey);

        [DllImport("User32.dll")]
        public static extern HWND SetCapture(HWND hWnd);

        [DllImport("User32.dll")]
        public static extern BOOL ReleaseCapture();

        [DllImport("User32.dll")]
        public static extern HDC BeginPaint(HWND hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("User32.dll")]
        public static extern BOOL EndPaint(HWND hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("User32.dll")]
        public static extern HANDLE LoadImageW(
            HINSTANCE hInst,
            IntPtr name,
            Win32ImageType type,
            int cx,
            int cy,
            Win32LoadImageFlags fuLoad
        );

        [DllImport("User32.dll")]
        public static extern HCURSOR SetCursor(HCURSOR hCursor);

        [DllImport("User32.dll")]
        public static extern BOOL InvalidateRect(HWND hWnd, ref RECT lpRect, BOOL bErase);

        [DllImport("User32.dll")]
        public static extern int FillRect(HDC hDC, ref RECT lprc, HBRUSH hbr);

        [DllImport("User32.dll")]
        public static extern int DrawTextW(
            HDC hdc,
            [MarshalAs(UnmanagedType.LPWStr)] string lpchText,
            int cchText,
            [In] ref RECT lprc,
            UINT format
        );

        [DllImport("User32.dll")]
        public static extern int DrawTextExW(
            HDC hdc,
            [MarshalAs(UnmanagedType.LPWStr)] string lpchText,
            int cchText,
            ref RECT lprc,
            UINT format,
            ref DRAWTEXTPARAMS lpdtp
        );

        [DllImport("User32.dll")]
        public static extern UINT MapVirtualKeyW(UINT uCode, VirtualKeyMapType uMapType);

        [DllImport("User32.dll")]
        public static extern BOOL OpenClipboard(HWND hWndNewOwner);

        [DllImport("User32.dll")]
        public static extern BOOL CloseClipboard();

        [DllImport("User32.dll")]
        public static extern BOOL EmptyClipboard();

        [DllImport("User32.dll")]
        public static extern int GetPriorityClipboardFormat(Win32ClipboardFormat[] paFormatPriorityList, int cFormats);

        [DllImport("User32.dll")]
        public static extern HANDLE GetClipboardData(Win32ClipboardFormat uFormat);

        [DllImport("User32.dll")]
        public static extern HANDLE SetClipboardData(Win32ClipboardFormat uFormat, HANDLE hMem);
    }

    internal static class Gdi32API
    {
        private static readonly HGDIOBJ HGDI_ERROR = new IntPtr(0xFFFFFFFFL);
        private static readonly COLORREF CLR_INVALID = 0xFFFFFFFF;

        [DllImport("Gdi32.dll")]
        private static extern HDC CreateCompatibleDC(HDC hdc);

        public static HDC CreateCompatibleDCChecked(HDC hdc)
        {
            HDC newHdc = CreateCompatibleDC(hdc);
            if (newHdc == IntPtr.Zero)
            {
                throw new InvalidOperationException($"{nameof(CreateCompatibleDC)} failed. Original device context: 0x{hdc.ToString("X16")}.");
            }

            return newHdc;
        }

        [DllImport("Gdi32.dll")]
        public static extern BOOL DeleteDC(HDC hdc);

        [DllImport("Gdi32.dll")]
        private static extern HBITMAP CreateDIBSection(
            HDC hdc,
            ref BITMAPINFO pbmi,
            UINT usage,
            out IntPtr ppvBits,
            HANDLE hSection,
            DWORD offset
        );

        public static HBITMAP CreateDIBSectionChecked
        (
            HDC hdc,
            BITMAPINFO pbmi,
            out IntPtr ppvBits
        )
        {
            HBITMAP res = CreateDIBSection(hdc, ref pbmi, 0, out ppvBits, IntPtr.Zero, 0);

            if (res == IntPtr.Zero)
            {
                throw new InvalidOperationException($"{nameof(CreateDIBSection)} failed. Width: {pbmi.bmiHeader.biWidth}, Height: {pbmi.bmiHeader.biHeight}.");
            }

            return res;
        }

        [DllImport("Gdi32.dll")]
        private static extern HBITMAP CreateCompatibleBitmap(
            HDC hdc,
            int cx,
            int cy
        );

        public static HBITMAP CreateCompatibleBitmapChecked(
            HDC hdc,
            int cx,
            int cy
        )
        {
            HBITMAP res = CreateCompatibleBitmap(hdc, cx, cy);

            if (res == IntPtr.Zero)
            {
                throw new InvalidOperationException($"{nameof(CreateCompatibleBitmap)} failed. Width: {cx}, Height: {cy}.");
            }

            return res;
        }

        [DllImport("Gdi32.dll")]
        private static extern Gdi32BackgroundMode SetBkMode(HDC hdc, Gdi32BackgroundMode mode);

        public static void SetBkModeChecked(HDC hdc, Gdi32BackgroundMode mode)
        {
            Gdi32BackgroundMode oldMode = SetBkMode(hdc, mode);
            // todo: is it worth checking?
            if (oldMode == Gdi32BackgroundMode.ERROR)
            {
                throw new InvalidOperationException($"{nameof(SetBkMode)} failed. Mode: {mode}.");
            }
        }

        [DllImport("Gdi32.dll")]
        public static extern int SelectClipRgn(HDC hdc, HRGN hrgn);

        [DllImport("Gdi32.dll")]
        public static extern HBRUSH CreateSolidBrush(COLORREF color);

        [DllImport("Gdi32.dll")]
        public static extern HPEN CreatePen(GdiPenStyle iStyle, int cWidth, COLORREF color);

        [DllImport("Gdi32.dll")]
        public static extern BOOL DeleteObject(HGDIOBJ ho);

        public static void SafeDeleteObject(IntPtr obj)
        {
            if (obj != IntPtr.Zero)
            {
                DeleteObject(obj);
            }
        }

        [DllImport("Gdi32.dll")]
        public static extern HGDIOBJ GetStockObject(GdiStockObjectType i);

        [DllImport("Gdi32.dll")]
        public static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ h);

        public static HGDIOBJ SelectObjectChecked(HDC hdc, HGDIOBJ h)
        {
            IntPtr oldObject = SelectObject(hdc, h);
            // todo: is it worth checking?
            if (oldObject == HGDI_ERROR)
            {
                throw new InvalidOperationException($"{nameof(SelectObject)} failed. Object: 0x{h.ToString("X16")}.");
            }

            return oldObject;
        }

        [DllImport("Gdi32.dll")]
        public static extern BOOL BitBlt(
            HDC hdc,
            int x,
            int y,
            int cx,
            int cy,
            HDC hdcSrc,
            int x1,
            int y1,
            GDI32RasterOperation rop
        );

        [DllImport("Gdi32.dll")]
        public static extern BOOL GdiAlphaBlend(
            HDC hdcDest,
            int xoriginDest,
            int yoriginDest,
            int wDest,
            int hDest,
            HDC hdcSrc,
            int xoriginSrc,
            int yoriginSrc,
            int wSrc,
            int hSrc,
            BLENDFUNCTION ftn
        );

        [DllImport("Gdi32.dll")]
        public static extern BOOL Rectangle(HDC hdc, int left, int top, int right, int bottom);

        [DllImport("Gdi32.dll")]
        private static extern HRGN CreateRectRgn(int x1, int y1, int x2, int y2);

        public static HRGN CreateRectRgnChecked(int x1, int y1, int x2, int y2)
        {
            HRGN region = CreateRectRgn(x1, y1, x2, y2);
            // todo: is it worth checking?
            if (region == IntPtr.Zero)
            {
                throw new InvalidOperationException($"{nameof(CreateRectRgn)} failed. X1: {x1}, Y1: {y1}, X2: {x2}, Y2: {y2}.");
            }

            return region;
        }

        [DllImport("Gdi32.dll")]
        public static extern BOOL SetRectRgn(HRGN hrgn, int left, int top, int right, int bottom);

        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern BOOL PaintRgn(HDC hdc, HRGN hrgn);

        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern BOOL FillRgn(HDC hdc, HRGN hrgn, HBRUSH hbr);

        [DllImport("Gdi32.dll")]
        private static extern COLORREF SetTextColor(HDC hdc, COLORREF color);

        public static void SetTextColorChecked(HDC hdc, COLORREF color)
        {
            COLORREF oldColor = SetTextColor(hdc, color);
            // todo: is it worth checking?
            if (oldColor == CLR_INVALID)
            {
                throw new InvalidOperationException($"{nameof(SetTextColor)} failed. Color: {color:X8}.");
            }
        }

        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode)]
        public static extern HFONT CreateFontW(
            int cHeight,
            int cWidth,
            int cEscapement,
            int cOrientation,
            int cWeight,
            DWORD bItalic,
            DWORD bUnderline,
            DWORD bStrikeOut,
            DWORD iCharSet,
            DWORD iOutPrecision,
            DWORD iClipPrecision,
            DWORD iQuality,
            DWORD iPitchAndFamily,
            string pszFaceName
        );

        [DllImport("Gdi32.dll")]
        public static extern BOOL SetTextJustification(
            HDC hdc,
            int extra,
            int count
        );

        [DllImport("Gdi32.dll")]
        public static extern BOOL GetTextExtentPoint32W(
            HDC hdc,
            [MarshalAs(UnmanagedType.LPWStr)] string lpString,
            int len,
            out SIZE psizl
        );

        [DllImport("Gdi32.dll")]
        public static extern BOOL TextOutW(
            HDC hdc,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpString,
            int stringLen
        );

        public static uint ToCOLORREF(Color color)
        {
            return (uint) (color.B << 16 | color.G << 8 | color.R);
        }
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

    [Flags]
    internal enum GlobalAllocFlags : UINT
    {
        GMEM_MOVEABLE = 0x0002
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
        WM_SIZE = 0x0005,
        WM_ACTIVATE = 0x0006,
        WM_PAINT = 0x000F,
        WM_SETCURSOR = 0x0020,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_CHAR = 0x0102,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C
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

    internal enum Gdi32BackgroundMode
    {
        ERROR = 0,
        TRANSPARENT = 1,
        OPAQUE = 2
    }

    internal enum W32VirtualKey
    {
        VK_LBUTTON = 0x01,
        VK_RBUTTON = 0x02,
        VK_MBUTTON = 0x04,
        VK_XBUTTON1 = 0x05,
        VK_XBUTTON2 = 0x06,
        VK_SHIFT = 0x10,
        VK_CONTROL = 0x11,
        VK_MENU = 0x12
    }

    internal enum VirtualKeyMapType : UINT
    {
        MAPVK_VK_TO_CHAR = 2,
        MAPVK_VK_TO_VSC = 0,
        MAPVK_VSC_TO_VK = 1,
        MAPVK_VSC_TO_VK_EX = 3
    }

    internal enum Win32ClipboardFormat : UINT
    {
        CF_UNICODETEXT = 13
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

    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFO
    {
        public readonly BITMAPINFOHEADER bmiHeader;
        public readonly RGBQUAD bmiColors;

        public BITMAPINFO(int width, int height)
        {
            bmiHeader = new BITMAPINFOHEADER(width, height);
            bmiColors = new RGBQUAD();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BITMAPINFOHEADER
    {
        private const uint BI_RGB = 0;

        public readonly DWORD biSize;
        public readonly LONG biWidth;
        public readonly LONG biHeight;
        public readonly WORD biPlanes;
        public readonly WORD biBitCount;
        public readonly DWORD biCompression;
        public readonly DWORD biSizeImage;
        public readonly LONG biXPelsPerMeter;
        public readonly LONG biYPelsPerMeter;
        public readonly DWORD biClrUsed;
        public readonly DWORD biClrImportant;

        public BITMAPINFOHEADER(int width, int height)
        {
            biSize = (uint) Marshal.SizeOf<BITMAPINFOHEADER>();
            biWidth = width;
            biHeight = -height;
            biPlanes = 1;
            biBitCount = 32;
            biCompression = BI_RGB;
            biSizeImage = 0;
            biXPelsPerMeter = 0;
            biYPelsPerMeter = 0;
            biClrUsed = 0;
            biClrImportant = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RGBQUAD
    {
        public readonly BYTE rgbBlue;
        public readonly BYTE rgbGreen;
        public readonly BYTE rgbRed;
        public readonly BYTE rgbReserved;
    }

    internal enum GDI32RasterOperation : uint
    {
        SRCCOPY = 0x00CC0020
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BLENDFUNCTION
    {
        private const byte AC_SRC_OVER = 0;
        private const byte AC_SRC_ALPHA = 1;

        public readonly BYTE BlendOp;
        public readonly BYTE BlendFlags;
        public readonly BYTE SourceConstantAlpha;
        public readonly BYTE AlphaFormat;

        private BLENDFUNCTION(byte blendOp, byte blendFlags, byte sourceConstantAlpha, byte alphaFormat)
        {
            BlendOp = blendOp;
            BlendFlags = blendFlags;
            SourceConstantAlpha = sourceConstantAlpha;
            AlphaFormat = alphaFormat;
        }

        public static BLENDFUNCTION ConstantAlpha(byte alpha)
        {
            return new BLENDFUNCTION(AC_SRC_OVER, 0, alpha, 0);
        }

        public static BLENDFUNCTION SourceAlpha()
        {
            return new BLENDFUNCTION(AC_SRC_OVER, 0, 255, AC_SRC_ALPHA);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        public readonly LONG cx;
        public readonly LONG cy;

        public SIZE(int cx, int cy) : this()
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DRAWTEXTPARAMS
    {
        public UINT cbSize;
        public int iTabLength;
        public int iLeftMargin;
        public int iRightMargin;
        public UINT uiLengthDrawn;

        public DRAWTEXTPARAMS(int tabLength)
        {
            cbSize = (uint) Marshal.SizeOf<DRAWTEXTPARAMS>();
            iTabLength = tabLength;
            iLeftMargin = 0;
            iRightMargin = 0;
            uiLengthDrawn = 0;
        }
    }

    internal enum Win32HitTestResult
    {
        HTERROR = -2,
        HTTRANSPARENT = -1,
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTCAPTION = 2,
        HTSYSMENU = 3,
        HTGROWBOX = 4,
        HTMENU = 5,
        HTHSCROLL = 6,
        HTVSCROLL = 7,
        HTMINBUTTON = 8,
        HTMAXBUTTON = 9,
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
        HTBORDER = 18,
        HTOBJECT = 19,
        HTCLOSE = 20,
        HTHELP = 21
    }

    internal enum Win32ImageType : UINT
    {
        IMAGE_BITMAP = 0,
        IMAGE_ICON = 1,
        IMAGE_CURSOR = 2
    }

    [Flags]
    internal enum Win32LoadImageFlags : UINT
    {
        LR_DEFAULTSIZE = 0x00000040,
        LR_SHARED = 0x00008000
    }

    internal static class Win32SystemResources
    {
        public static readonly IntPtr IDC_ARROW = (IntPtr) 32512;
        public static readonly IntPtr IDC_IBEAM = (IntPtr) 32513;
    }
}