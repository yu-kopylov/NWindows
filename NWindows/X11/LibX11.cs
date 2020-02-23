namespace NWindows.X11
{
    using System;
    using System.Runtime.InteropServices;
    using Atom = System.UInt64;
    using Bool = System.Int32;
    using Status = System.Int32;
    using Colormap = System.UInt64;
    using Cursor = System.UInt64;
    using Drawable = System.UInt64;
    using Font = System.UInt64;
    using GC = System.UInt64;
    using KeySym = System.UInt64;
    using Pixmap = System.UInt64;
    using Time = System.UInt64;
    using VisualID = System.UInt64;
    using DisplayPtr = System.IntPtr;
    using VisualPtr = System.IntPtr;
    using Window = System.UInt64;
    using XComposeStatusPtr = System.IntPtr;
    using XImagePtr = System.IntPtr;

    internal static class LibX11
    {
        [DllImport("libX11.so.6")]
        public static extern Status XInitThreads();

        [DllImport("libX11.so.6")]
        public static extern int XFree(IntPtr data);

        [DllImport("libX11.so.6")]
        public static extern DisplayPtr XOpenDisplay(string display_name);

        [DllImport("libX11.so.6")]
        public static extern int XCloseDisplay(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern int XDefaultScreen(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern Window XDefaultRootWindow(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern VisualPtr XDefaultVisual(DisplayPtr display, int screen_number);

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

        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        public static extern int XDestroyWindow(DisplayPtr display, Window w);

        [DllImport("libX11.so.6", CharSet = CharSet.Ansi)]
        public static extern Atom XInternAtom(DisplayPtr display, string atom_name, Bool only_if_exists);

        [DllImport("libX11.so.6")]
        private static extern IntPtr XGetAtomName(DisplayPtr display, Atom atom);

        public static string GetAtomName(DisplayPtr display, Atom atom)
        {
            IntPtr namePtr = XGetAtomName(display, atom);

            if (namePtr == IntPtr.Zero)
            {
                return null;
            }

            string name = Marshal.PtrToStringAnsi(namePtr);
            XFree(namePtr);
            return name;
        }

        [DllImport("libX11.so.6")]
        public static extern int XChangeProperty(
            DisplayPtr display,
            Window w,
            Atom property,
            Atom type,
            XChangePropertyFormat format,
            XChangePropertyMode mode,
            byte[] data,
            int nelements
        );

        [DllImport("libX11.so.6")]
        public static extern int XChangeProperty(
            DisplayPtr display,
            Window w,
            Atom property,
            Atom type,
            XChangePropertyFormat format,
            XChangePropertyMode mode,
            ulong[] data,
            int nelements
        );

        [DllImport("libX11.so.6")]
        public static extern int XGetWindowProperty(
            DisplayPtr display,
            Window w,
            Atom property,
            long long_offset,
            long long_length,
            Bool delete,
            Atom req_type,
            out Atom actual_type_return,
            out int actual_format_return,
            out ulong nitems_return,
            out ulong bytes_after_return,
            out IntPtr prop_return
        );

        [DllImport("libX11.so.6")]
        public static extern Status XSetWMProtocols(DisplayPtr display, Window w, Atom[] protocols, int count);

        [DllImport("libX11.so.6")]
        public static extern int XMapWindow(DisplayPtr display, Window w);

        [DllImport("libX11.so.6")]
        public static extern int XFlush(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern int XNextEvent(DisplayPtr display, out XEvent event_return);

        [DllImport("libX11.so.6")]
        public static extern int XPeekEvent(DisplayPtr display, out XEvent event_return);

        [DllImport("libX11.so.6")]
        public static extern int XPending(DisplayPtr display);

        [DllImport("libX11.so.6")]
        public static extern Status XSendEvent(DisplayPtr display, Window w, Bool propagate, XEventMask event_mask, ref XEvent event_send);

        [DllImport("libX11.so.6")]
        public static extern int XUngrabPointer(DisplayPtr display, Time time);

        [DllImport("libX11.so.6")]
        public static extern XImagePtr XCreateImage(
            DisplayPtr display,
            VisualPtr visual,
            uint depth,
            XImageFormat format,
            int offset,
            IntPtr data,
            uint width,
            uint height,
            int bitmap_pad,
            int bytes_per_line
        );

        [DllImport("libX11.so.6")]
        public static extern int XDestroyImage(XImagePtr ximage);

        [DllImport("libX11.so.6")]
        public static extern XImagePtr XGetSubImage(
            DisplayPtr display,
            Drawable d,
            int x,
            int y,
            uint width,
            uint height,
            ulong plane_mask,
            XImageFormat format,
            XImagePtr dest_image,
            int dest_x,
            int dest_y
        );

        [DllImport("libX11.so.6")]
        public static extern int XPutImage(
            DisplayPtr display,
            Drawable d,
            GC gc,
            XImagePtr image,
            int src_x,
            int src_y,
            int dest_x,
            int dest_y,
            uint width,
            uint height
        );

        [DllImport("libX11.so.6")]
        public static extern GC XCreateGC(
            DisplayPtr display,
            Drawable d,
            ulong valuemask,
            [In] ref XGCValues values
        );

        [DllImport("libX11.so.6")]
        public static extern int XFreeGC(DisplayPtr display, GC gc);

        [DllImport("libX11.so.6")]
        public static extern Pixmap XCreatePixmap(
            DisplayPtr display,
            Drawable d,
            uint width,
            uint height,
            uint depth
        );

        [DllImport("libX11.so.6")]
        public static extern Pixmap XCreateBitmapFromData(
            DisplayPtr display,
            Drawable d,
            IntPtr data,
            uint width,
            uint height
        );

        [DllImport("libX11.so.6")]
        public static extern int XFreePixmap(DisplayPtr display, Pixmap pixmap);

        [DllImport("libX11.so.6")]
        public static extern KeySym XLookupKeysym([In] ref XKeyEvent key_event, int index);

        [DllImport("libX11.so.6")]
        public static extern int XLookupString(
            [In] ref XKeyEvent event_struct,
            byte[] buffer_return,
            int bytes_buffer,
            out KeySym keysym_return,
            XComposeStatusPtr status_in_out
        );

        [DllImport("libX11.so.6")]
        public static extern int XSetSelectionOwner(
            DisplayPtr display,
            Atom selection,
            Window owner,
            Time time
        );

        [DllImport("libX11.so.6")]
        public static extern int XConvertSelection(
            DisplayPtr display,
            Atom selection,
            Atom target,
            Atom property,
            Window requestor,
            Time time
        );
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

    internal enum XEventType
    {
        KeyPress = 2,
        KeyRelease = 3,
        ButtonPress = 4,
        ButtonRelease = 5,
        MotionNotify = 6,
        EnterNotify = 7,
        LeaveNotify = 8,
        FocusIn = 9,
        FocusOut = 10,
        KeymapNotify = 11,
        Expose = 12,
        GraphicsExpose = 13,
        NoExpose = 14,
        VisibilityNotify = 15,
        CreateNotify = 16,
        DestroyNotify = 17,
        UnmapNotify = 18,
        MapNotify = 19,
        MapRequest = 20,
        ReparentNotify = 21,
        ConfigureNotify = 22,
        ConfigureRequest = 23,
        GravityNotify = 24,
        ResizeRequest = 25,
        CirculateNotify = 26,
        CirculateRequest = 27,
        PropertyNotify = 28,
        SelectionClear = 29,
        SelectionRequest = 30,
        SelectionNotify = 31,
        ColormapNotify = 32,
        ClientMessage = 33,
        MappingNotify = 34,
        GenericEvent = 35
    }

    [StructLayout(LayoutKind.Explicit, Size = 24 * 8)]
    internal struct XEvent
    {
        // todo: should event size be 96 or 192 (24*4 or 24 *8)
        [FieldOffset(0)] public XEventType type;
        [FieldOffset(0)] public XButtonEvent ButtonEvent;
        [FieldOffset(0)] public XClientMessageEvent ClientMessageEvent;
        [FieldOffset(0)] public XConfigureEvent ConfigureEvent;
        [FieldOffset(0)] public XExposeEvent ExposeEvent;
        [FieldOffset(0)] public XFocusChangeEvent FocusChangeEvent;
        [FieldOffset(0)] public XKeyEvent KeyEvent;
        [FieldOffset(0)] public XMotionEvent MotionEvent;
        [FieldOffset(0)] public XPropertyEvent PropertyEvent;
        [FieldOffset(0)] public XSelectionClearEvent SelectionClearEvent;
        [FieldOffset(0)] public XSelectionEvent SelectionEvent;
        [FieldOffset(0)] public XSelectionRequestEvent SelectionRequestEvent;

        public static XEvent CreateClientMessage(Window window, Atom messageType)
        {
            var res = new XEvent();
            res.type = XEventType.ClientMessage;
            res.ClientMessageEvent.window = window;
            res.ClientMessageEvent.format = 32;
            res.ClientMessageEvent.message_type = messageType;
            return res;
        }

        public static XEvent CreateExpose(int x, int y, int width, int height)
        {
            var res = new XEvent();
            res.type = XEventType.Expose;
            res.ExposeEvent.x = x;
            res.ExposeEvent.y = y;
            res.ExposeEvent.width = width;
            res.ExposeEvent.height = height;
            return res;
        }

        public static XEvent CreateSelectionNotify(Window requestor, Atom selection, Atom target, Atom property, ulong time)
        {
            var res = new XEvent();
            res.type = XEventType.SelectionNotify;
            res.SelectionEvent.requestor = requestor;
            res.SelectionEvent.selection = selection;
            res.SelectionEvent.target = target;
            res.SelectionEvent.property = property;
            res.SelectionEvent.time = time;
            return res;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XButtonEvent
    {
        public readonly int type; /* ButtonPress or ButtonRelease */
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window; /* ``event'' window it is reported relative to */
        public readonly Window root; /* root window that the event occurred on */
        public readonly Window subwindow; /* child window */
        public readonly Time time; /* milliseconds */
        public readonly int x, y; /* pointer x, y coordinates in event window */
        public readonly int x_root, y_root; /* coordinates relative to root */
        public readonly uint state; /* key or button mask */
        public readonly uint button; /* detail */
        public readonly Bool same_screen; /* same screen flag */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XClientMessageEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public Window window;
        public Atom message_type;

        public int format;

        // todo: how much data are really available (20 bytes or 5*8 = 40 bytes)?
        public readonly ulong data0;
        public readonly ulong data1;
        public readonly ulong data2;
        public readonly ulong data3;
        public readonly ulong data4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XExposeEvent
    {
        public int type;
        public ulong serial; /* # of last request processed by server */
        public Bool send_event; /* true if this came from a SendEvent request */
        public DisplayPtr display; /* Display the event was read from */
        public Window window;
        public int x, y;
        public int width, height;
        public int count; /* if non-zero, at least this many more */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XFocusChangeEvent
    {
        public readonly int type; /* FocusIn or FocusOut */
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window; /* window of event */

        public readonly FocusNotifyMode mode; /* NotifyNormal, NotifyWhileGrabbed,
				       NotifyGrab, NotifyUngrab */

        public readonly FocusNotifyDetail detail;
        /*
         * NotifyAncestor, NotifyVirtual, NotifyInferior,
         * NotifyNonlinear,NotifyNonlinearVirtual, NotifyPointer,
         * NotifyPointerRoot, NotifyDetailNone
         */
    }

    internal enum FocusNotifyMode
    {
        NotifyNormal = 0,
        NotifyGrab = 1,
        NotifyUngrab = 2,
        NotifyWhileGrabbed = 3
    }

    internal enum FocusNotifyDetail
    {
        NotifyAncestor = 0,
        NotifyVirtual = 1,
        NotifyInferior = 2,
        NotifyNonlinear = 3,
        NotifyNonlinearVirtual = 4,
        NotifyPointer = 5,
        NotifyPointerRoot = 6,
        NotifyDetailNone = 7
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XKeyEvent
    {
        public readonly int type; /* KeyPress or KeyRelease */
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window; /* ``event'' window it is reported relative to */
        public readonly Window root; /* root window that the event occurred on */
        public readonly Window subwindow; /* child window */
        public readonly Time time; /* milliseconds */
        public readonly int x, y; /* pointer x, y coordinates in event window */
        public readonly int x_root, y_root; /* coordinates relative to root */
        public readonly uint state; /* key or button mask */
        public readonly uint keycode; /* detail */
        public readonly Bool same_screen; /* same screen flag */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XMotionEvent
    {
        public readonly int type; /* MotionNotify */
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window; /* ``event'' window reported relative to */
        public readonly Window root; /* root window that the event occurred on */
        public readonly Window subwindow; /* child window */
        public readonly Time time; /* milliseconds */
        public readonly int x, y; /* pointer x, y coordinates in event window */
        public readonly int x_root, y_root; /* coordinates relative to root */
        public readonly uint state; /* key or button mask */
        public readonly char is_hint; /* detail */
        public readonly Bool same_screen; /* same screen flag */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XPropertyEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window;
        public readonly Atom atom;
        public readonly Time time;
        public readonly int state; /* NewValue, Deleted */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XSelectionClearEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window window;
        public readonly Atom selection;
        public readonly Time time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XSelectionEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public Window requestor;
        public Atom selection;
        public Atom target;
        public Atom property; /* ATOM or None */
        public Time time;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XSelectionRequestEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window owner;
        public readonly Window requestor;
        public readonly Atom selection;
        public readonly Atom target;
        public readonly Atom property;
        public readonly Time time;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XConfigureEvent
    {
        public readonly int type;
        public readonly ulong serial; /* # of last request processed by server */
        public readonly Bool send_event; /* true if this came from a SendEvent request */
        public readonly DisplayPtr display; /* Display the event was read from */
        public readonly Window eventWindow;
        public readonly Window window;
        public readonly int x, y;
        public readonly int width, height;
        public readonly int border_width;
        public readonly Window above;
        public readonly Bool override_redirect;
    }

    internal enum XChangePropertyFormat
    {
        Byte = 8,
        Atom = 32
    }

    internal enum XChangePropertyMode
    {
        PropModeReplace = 0,
        PropModePrepend = 1,
        PropModeAppend = 2
    }

    internal enum XImageFormat
    {
        XYBitmap = 0,
        XYPixmap = 1,
        ZPixmap = 2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XGCValues
    {
        public int function; /* logical operation */
        public ulong plane_mask; /* plane mask */
        public ulong foreground; /* foreground pixel */
        public ulong background; /* background pixel */
        public int line_width; /* line width (in pixels) */
        public int line_style; /* LineSolid, LineOnOffDash, LineDoubleDash */
        public int cap_style; /* CapNotLast, CapButt, CapRound, CapProjecting */
        public int join_style; /* JoinMiter, JoinRound, JoinBevel */
        public int fill_style; /* FillSolid, FillTiled, FillStippled FillOpaqueStippled*/
        public int fill_rule; /* EvenOddRule, WindingRule */
        public int arc_mode; /* ArcChord, ArcPieSlice */
        public Pixmap tile; /* tile pixmap for tiling operations */
        public Pixmap stipple; /* stipple 1 plane pixmap for stippling */
        public int ts_x_origin; /* offset for tile or stipple operations */
        public int ts_y_origin;
        public Font font; /* default text font for text operations */
        public int subwindow_mode; /* ClipByChildren, IncludeInferiors */
        public Bool graphics_exposures; /* boolean, should exposures be generated */
        public int clip_x_origin; /* origin for clipping */
        public int clip_y_origin;
        public Pixmap clip_mask; /* bitmap clipping; other calls for rects */
        public int dash_offset; /* patterned/dashed line information */
        public char dashes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XRectangle
    {
        public readonly short x, y;
        public readonly ushort width, height;

        public XRectangle(int x, int y, int width, int height)
        {
            this.x = ToShort(x);
            this.y = ToShort(y);
            this.width = ToUShort(width);
            this.height = ToUShort(height);
        }

        private static short ToShort(int val)
        {
            if (val < short.MinValue)
            {
                return short.MinValue;
            }

            if (val > short.MaxValue)
            {
                return short.MaxValue;
            }

            return (short) val;
        }

        private static ushort ToUShort(int val)
        {
            if (val < ushort.MinValue)
            {
                return ushort.MinValue;
            }

            if (val > ushort.MaxValue)
            {
                return ushort.MaxValue;
            }

            return (ushort) val;
        }
    }
}