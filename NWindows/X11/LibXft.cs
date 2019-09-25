namespace NWindows.X11
{
    using System;
    using System.Runtime.InteropServices;
    using Colormap = System.UInt64;
    using Display_ptr = System.IntPtr;
    using Drawable = System.UInt64;
    using FcBool = System.Int32;
    using FcChar32 = System.UInt32;
    using Visual_ptr = System.IntPtr;
    using XftColor_ptr = System.IntPtr;
    using XftDraw_ptr = System.IntPtr;
    using XftFont_ptr = System.IntPtr;

    internal static class LibXft
    {
        [DllImport("libXft.so.2")]
        public static extern XftFont_ptr XftFontOpenName(Display_ptr dpy, int screen, byte[] name);

        [DllImport("libXft.so.2")]
        public static extern void XftFontClose(Display_ptr display, XftFont_ptr font);

        [DllImport("libXft.so.2")]
        public static extern XftDraw_ptr XftDrawCreate(Display_ptr dpy, Drawable drawable, Visual_ptr visual, Colormap colormap);

        [DllImport("libXft.so.2")]
        public static extern bool XftDrawSetClipRectangles(XftDraw_ptr draw, int xOrigin, int yOrigin, XRectangle[] rects, int n);

        [DllImport("libXft.so.2")]
        public static extern void XftDrawDestroy(XftDraw_ptr draw);

        [DllImport("libXft.so.2")]
        public static extern int XftColorAllocValue(
            Display_ptr dpy,
            Visual_ptr visual,
            Colormap colormap,
            [MarshalAs(UnmanagedType.LPStruct)] XRenderColor color,
            XftColor_ptr xftColor
        );

        [DllImport("libXft.so.2")]
        public static extern void XftColorFree(Display_ptr dpy, Visual_ptr visual, Colormap colormap, XftColor_ptr xftColor);

        [DllImport("libXft.so.2")]
        public static extern FcBool XftCharExists(Display_ptr dpy, XftFont_ptr pub, FcChar32 ucs4);

        [DllImport("libXft.so.2")]
        public static extern void XftTextExtents32(
            Display_ptr dpy,
            XftFont_ptr font,
            IntPtr text,
            int len,
            out XGlyphInfo extents
        );

        [DllImport("libXft.so.2")]
        public static extern void XftDrawString32(
            XftDraw_ptr draw,
            XftColor_ptr color,
            XftFont_ptr pub,
            int x,
            int y,
            IntPtr text,
            int len
        );
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XftColor
    {
        public readonly ulong pixel;
        public readonly XRenderColor color;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XftFont
    {
        public readonly int ascent;
        public readonly int descent;
        public readonly int height;
        public readonly int max_advance_width;
        public readonly IntPtr charset;
        public readonly IntPtr pattern;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XGlyphInfo
    {
        public readonly ushort width;
        public readonly ushort height;
        public readonly short x;
        public readonly short y;
        public readonly short xOff;
        public readonly short yOff;
    }
}