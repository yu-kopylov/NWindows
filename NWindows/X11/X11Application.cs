using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace NWindows.X11
{
    internal class X11Application
    {
        private IntPtr display;
        private int defaultScreen;
        private ulong defaultRootWindow;
        private IntPtr pictFormatPtr;
        private XRenderPictFormat pictFormat;
        private XVisualInfo visualInfo;
        private ulong colormap;

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

        public void Run(BasicWindow window)
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

            XSetWindowAttributes attr = new XSetWindowAttributes();
            attr.border_pixel = 0;
            attr.event_mask = XEventMask.ExposureMask | XEventMask.ButtonPressMask | XEventMask.PointerMotionMask;
            attr.colormap = colormap;

            ulong windowId = LibX11.XCreateWindow
            (
                display,
                defaultRootWindow,
                0, 0,
                (uint) window.Width, (uint) window.Height,
                10,
                pictFormat.depth,
                WindowClass.InputOutput,
                visualInfo.visual,
                XSetWindowAttributeMask.CWBorderPixel |
                XSetWindowAttributeMask.CWEventMask |
                XSetWindowAttributeMask.CWColormap,
                ref attr
            );

            window.NativeWindow = new X11Window(display, windowId);
            window.NativeWindow.SetTitle(window.Title);

            LibX11.XMapWindow(display, windowId);
            LibX11.XFlush(display);

            while (true)
            {
                LibX11.XNextEvent(display, out XEvent evt);
                if (evt.type == XEventType.Expose)
                {
                    var rect = new Rectangle(evt.ExposeEvent.x, evt.ExposeEvent.y, evt.ExposeEvent.width, evt.ExposeEvent.height);
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
                        window.Paint(canvas, rect);
                    }
                }
                else if (evt.type == XEventType.MotionNotify)
                {
                    window.OnMouseMove(new Point(evt.MotionEvent.x, evt.MotionEvent.y));
                }
            }

            //todo: close window
            //todo: free colormap?

            LibX11.XCloseDisplay(display);
        }
    }
}