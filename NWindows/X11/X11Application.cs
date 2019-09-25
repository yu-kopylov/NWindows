using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Application : INativeApplication
    {
        public const int RequiredColorDepth = 32;
        public const int RequiredBitsPerChannel = 8;

        private IntPtr display;
        private int defaultScreen;
        private ulong defaultRootWindow;
        private IntPtr pictFormatPtr;
        private XRenderPictFormat pictFormat;
        private XVisualInfo visualInfo;
        private ulong colormap;

        private Rectangle? pendingRedraw;

        public static bool IsAvailable()
        {
            try
            {
                IntPtr display = LibX11.XOpenDisplay(null);
                if (display == IntPtr.Zero)
                {
                    return false;
                }

                LibX11.XCloseDisplay(display);
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
            display = LibX11.XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot open display.");
            }

            defaultScreen = LibX11.XDefaultScreen(display);
            defaultRootWindow = LibX11.XDefaultRootWindow(display);

            //todo: is is safe to keep pictFormatPtr after reading it into XRenderPictFormat
            pictFormatPtr = LibXRender.XRenderFindStandardFormat(display, StandardPictFormat.PictStandardARGB32);

            if (pictFormatPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Display does not support 32-bit ARGB.");
            }

            pictFormat = Marshal.PtrToStructure<XRenderPictFormat>(pictFormatPtr);

            int status = LibX11.XMatchVisualInfo
            (
                display,
                defaultScreen,
                pictFormat.depth,
                VisualClass.TrueColor,
                out visualInfo
            );
            if (status == 0)
            {
                throw new InvalidOperationException($"TrueColor visual with depth {pictFormat.depth} does not exist.");
            }

            colormap = LibX11.XCreateColormap
            (
                display,
                defaultRootWindow,
                visualInfo.visual,
                CreateColormapOption.AllocNone
            );
        }

        public void Run(INativeWindowStartupInfo window)
        {
            XSetWindowAttributes attr = new XSetWindowAttributes();
            attr.border_pixel = 0;
            attr.event_mask = XEventMask.ExposureMask |
                              XEventMask.ButtonPressMask |
                              XEventMask.KeyPressMask |
                              XEventMask.KeyReleaseMask |
                              XEventMask.PointerMotionMask |
                              XEventMask.StructureNotifyMask;
            attr.colormap = colormap;

            ulong windowId = LibX11.XCreateWindow
            (
                display,
                defaultRootWindow,
                0, 0,
                (uint) window.Width, (uint) window.Height,
                1,
                pictFormat.depth,
                WindowClass.InputOutput,
                visualInfo.visual,
                XSetWindowAttributeMask.CWBorderPixel |
                XSetWindowAttributeMask.CWEventMask |
                XSetWindowAttributeMask.CWColormap,
                ref attr
            );

            X11Window nativeWindow = new X11Window(display, windowId);
            window.OnCreate(nativeWindow);
            nativeWindow.SetTitle(window.Title);

            LibX11.XMapWindow(display, windowId);
            LibX11.XFlush(display);

            byte[] textBuffer = new byte[32];
            bool autoRepeat = false;

            while (true)
            {
                LibX11.XNextEvent(display, out XEvent evt);
                if (evt.type == XEventType.Expose)
                {
                    // System.Console.WriteLine($"Expose: {evt.ExposeEvent.x} x {evt.ExposeEvent.y} .. {evt.ExposeEvent.width} x {evt.ExposeEvent.height}");
                    var rect = new Rectangle(evt.ExposeEvent.x, evt.ExposeEvent.y, evt.ExposeEvent.width, evt.ExposeEvent.height);
                    if (pendingRedraw == null)
                    {
                        pendingRedraw = rect;
                    }
                    else
                    {
                        int left = Math.Min(pendingRedraw.Value.Left, rect.Left);
                        int top = Math.Min(pendingRedraw.Value.Top, rect.Top);
                        int right = Math.Max(pendingRedraw.Value.Right, rect.Right);
                        int bottom = Math.Max(pendingRedraw.Value.Bottom, rect.Bottom);
                        pendingRedraw = new Rectangle(left, top, right - left, bottom - top);
                    }
                }
                else if (evt.type == XEventType.MotionNotify)
                {
                    // System.Console.WriteLine($"MotionNotify: {evt.MotionEvent.x} x {evt.MotionEvent.y}");
                    window.OnMouseMove(new Point(evt.MotionEvent.x, evt.MotionEvent.y));
                }
                else if (evt.type == XEventType.ConfigureNotify)
                {
                    // System.Console.WriteLine($"ConfigureNotify: {evt.ConfigureEvent.width} x {evt.ConfigureEvent.height}");
                    Size clientArea = new Size(evt.ConfigureEvent.width, evt.ConfigureEvent.height);
                    window.OnResize(clientArea);
                }
                else if (evt.type == XEventType.KeyPress)
                {
                    // XLookupString returns no more than the requested number of characters, but it also writes a zero-byte after them
                    int charCount = LibX11.XLookupString(evt.KeyEvent, textBuffer, textBuffer.Length - 1, out var keySym, IntPtr.Zero);
                    var keyCode = X11KeyMap.GetKeyCode(keySym);
                    if (keyCode == NKeyCode.Unknown)
                    {
                        keySym = LibX11.XLookupKeysym(evt.KeyEvent, 0);
                        keyCode = X11KeyMap.GetKeyCode(keySym);
                    }

                    window.OnKeyDown(keyCode, GetModifierKey(evt.KeyEvent.state), autoRepeat);
                    autoRepeat = false;

                    if (charCount > 0)
                    {
                        string text = Encoding.UTF8.GetString(textBuffer, 0, charCount);
                        StringBuilder sb = new StringBuilder(text.Length);
                        foreach (char c in text)
                        {
                            if (!char.IsControl(c))
                            {
                                sb.Append(c);
                            }
                        }

                        if (sb.Length > 0)
                        {
                            window.OnTextInput(sb.ToString());
                        }
                    }
                }
                else if (evt.type == XEventType.KeyRelease)
                {
                    if (LibX11.XPending(display) > 0)
                    {
                        LibX11.XPeekEvent(display, out XEvent nextEvent);
                        autoRepeat =
                            nextEvent.type == XEventType.KeyPress &&
                            nextEvent.KeyEvent.time == evt.KeyEvent.time &&
                            nextEvent.KeyEvent.keycode == evt.KeyEvent.keycode;
                    }

                    if (!autoRepeat)
                    {
                        // XLookupString returns no more than the requested number of characters, but it also writes a zero-byte after them
                        LibX11.XLookupString(evt.KeyEvent, textBuffer, textBuffer.Length - 1, out var keySym, IntPtr.Zero);
                        var keyCode = X11KeyMap.GetKeyCode(keySym);
                        if (keyCode == NKeyCode.Unknown)
                        {
                            keySym = LibX11.XLookupKeysym(evt.KeyEvent, 0);
                            keyCode = X11KeyMap.GetKeyCode(keySym);
                        }

                        window.OnKeyUp(keyCode);
                    }
                }

                if (pendingRedraw != null && LibX11.XPending(display) == 0)
                {
                    using (X11ObjectCache objectCache = new X11ObjectCache(display, defaultScreen))
                    using (X11Canvas canvas = X11Canvas.CreateForWindow(
                        display,
                        defaultScreen,
                        objectCache,
                        visualInfo.visual,
                        colormap,
                        pictFormatPtr,
                        windowId
                    ))
                    {
                        window.OnPaint(canvas, pendingRedraw.Value);
                    }

                    pendingRedraw = null;
                }
            }

            //todo: close window
            //todo: free colormap?

            LibX11.XCloseDisplay(display);
        }

        private NModifierKey GetModifierKey(uint state)
        {
            NModifierKey modifierKey = NModifierKey.None;

            if ((state & (1 << 0)) != 0)
            {
                modifierKey |= NModifierKey.Shift;
            }

            if ((state & (1 << 2)) != 0)
            {
                modifierKey |= NModifierKey.Control;
            }

            if ((state & (1 << 3)) != 0)
            {
                modifierKey |= NModifierKey.Alt;
            }

            return modifierKey;
        }

        public INativeImageCodec CreateImageCodec()
        {
            return new GdkPixBufImageCodec(display, visualInfo.visual);
        }
    }
}