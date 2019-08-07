using System;
using System.Runtime.InteropServices;
using Bool = System.Int32;
using Status = System.Int32;
using Colormap = System.UInt64;
using Cursor = System.UInt64;
using Pixmap = System.UInt64;
using VisualID = System.UInt64;
using Window = System.UInt64;
using DisplayPtr = System.IntPtr;
using VisualPtr = System.IntPtr;

namespace NWindows.X11
{
    internal static class LibX11
    {
        [DllImport("libX11.so.6")]
        public static extern DisplayPtr XOpenDisplay(string display_name);

        [DllImport("libX11.so.6")]
        public static extern int XCloseDisplay(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern int XDefaultScreen(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern Window XDefaultRootWindow(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern Status XMatchVisualInfo
        (
            DisplayPtr display,
            int screen,
            int depth,
            VisualClass visualClass,
            out XVisualInfo vinfo_return
        );

        [DllImport("libX11.so.6")]
        public static extern Colormap XCreateColormap(
            DisplayPtr display,
            Window w,
            VisualPtr visual,
            CreateColormapOption alloc
        );

        [DllImport("libX11.so.6")]
        public static extern Window XCreateWindow(
            DisplayPtr display,
            Window parent,
            int x, int y,
            uint width,
            uint height,
            uint border_width,
            int depth,
            WindowClass windowClass,
            VisualPtr visual,
            XSetWindowAttributeMask valuemask,
            ref XSetWindowAttributes attributes
        );

        [DllImport("libX11.so.6")]
        public static extern int XMapWindow(DisplayPtr display, Window w);

        [DllImport("libX11.so.6")]
        public static extern int XFlush(DisplayPtr display);
    }

    internal enum VisualClass
    {
        StaticGray = 0,
        GrayScale = 1,
        StaticColor = 2,
        PseudoColor = 3,
        TrueColor = 4,
        DirectColor = 5
    }

    internal enum CreateColormapOption
    {
        AllocNone = 0,
        AllocAll = 1
    }

    internal enum WindowClass
    {
        CopyFromParent = 0,
        InputOutput = 1,
        InputOnly = 2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XVisualInfo
    {
        public readonly VisualPtr visual;
        public readonly VisualID visualid;
        public readonly int screen;
        public readonly uint depth;
        public readonly VisualClass visualClass;

        public readonly ulong red_mask;
        public readonly ulong green_mask;
        public readonly ulong blue_mask;
        public readonly int colormap_size;
        public readonly int bits_per_rgb;
    }

    [Flags]
    internal enum XSetWindowAttributeMask : ulong
    {
        CWBackPixmap = 1L << 0,
        CWBackPixel = 1L << 1,
        CWBorderPixmap = 1L << 2,
        CWBorderPixel = 1L << 3,
        CWBitGravity = 1L << 4,
        CWWinGravity = 1L << 5,
        CWBackingStore = 1L << 6,
        CWBackingPlanes = 1L << 7,
        CWBackingPixel = 1L << 8,
        CWOverrideRedirect = 1L << 9,
        CWSaveUnder = 1L << 10,
        CWEventMask = 1L << 11,
        CWDontPropagate = 1L << 12,
        CWColormap = 1L << 13,
        CWCursor = 1L << 14
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XSetWindowAttributes
    {
        public Pixmap background_pixmap; /* background, None, or ParentRelative */
        public ulong background_pixel; /* background pixel */
        public Pixmap border_pixmap; /* border of the window or CopyFromParent */
        public ulong border_pixel; /* border pixel value */
        public int bit_gravity; /* one of bit gravity values */
        public int win_gravity; /* one of the window gravity values */
        public int backing_store; /* NotUseful, WhenMapped, Always */
        public ulong backing_planes; /* planes to be preserved if possible */
        public ulong backing_pixel; /* value to use in restoring planes */
        public Bool save_under; /* should bits under be saved? (popups) */
        public XEventMask event_mask; /* set of events that should be saved */
        public XEventMask do_not_propagate_mask; /* set of events that should not propagate */
        public Bool override_redirect; /* boolean value for override_redirect */
        public Colormap colormap; /* color map to be associated with window */
        public Cursor cursor; /* cursor to be displayed (or None) */
    }

    [Flags]
    internal enum XEventMask : long
    {
        NoEventMask = 0L,
        KeyPressMask = 1L << 0,
        KeyReleaseMask = 1L << 1,
        ButtonPressMask = 1L << 2,
        ButtonReleaseMask = 1L << 3,
        EnterWindowMask = 1L << 4,
        LeaveWindowMask = 1L << 5,
        PointerMotionMask = 1L << 6,
        PointerMotionHintMask = 1L << 7,
        Button1MotionMask = 1L << 8,
        Button2MotionMask = 1L << 9,
        Button3MotionMask = 1L << 10,
        Button4MotionMask = 1L << 11,
        Button5MotionMask = 1L << 12,
        ButtonMotionMask = 1L << 13,
        KeymapStateMask = 1L << 14,
        ExposureMask = 1L << 15,
        VisibilityChangeMask = 1L << 16,
        StructureNotifyMask = 1L << 17,
        ResizeRedirectMask = 1L << 18,
        SubstructureNotifyMask = 1L << 19,
        SubstructureRedirectMask = 1L << 20,
        FocusChangeMask = 1L << 21,
        PropertyChangeMask = 1L << 22,
        ColormapChangeMask = 1L << 23,
        OwnerGrabButtonMask = 1L << 24
    }
}