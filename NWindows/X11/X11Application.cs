using System;
using System.Drawing;
using System.Text;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Application : INativeApplication
    {
        public const int RequiredColorDepth = 32;
        public const int RequiredBitsPerChannel = 8;

        private IntPtr display;

        private X11Graphics graphics;
        private GdkPixBufImageCodec imageCodec;
        private X11Clipboard clipboard;

        private Rectangle? invalidatedArea;
        private bool paintQueued;

        private ulong WM_PROTOCOLS;
        private ulong WM_DELETE_WINDOW;
        private ulong XA_NWINDOWS_PAINT_COMPLETE;

        public void Dispose()
        {
            clipboard.Stop();
            graphics.Dispose();
            LibX11.XCloseDisplay(display);
        }

        public static bool IsAvailable()
        {
            try
            {
                LibX11.XInitThreads();
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
            LibX11.XInitThreads();
            display = LibX11.XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                throw new InvalidOperationException("Cannot open display.");
            }

            WM_PROTOCOLS = LibX11.XInternAtom(display, "WM_PROTOCOLS", 0);
            WM_DELETE_WINDOW = LibX11.XInternAtom(display, "WM_DELETE_WINDOW", 0);
            XA_NWINDOWS_PAINT_COMPLETE = LibX11.XInternAtom(display, "NWINDOWS_PAINT_COMPLETE", 0);

            graphics = X11Graphics.Create(display);
            imageCodec = new GdkPixBufImageCodec(display, graphics.Visual, graphics.RootWindow);
            clipboard = X11Clipboard.Create();
        }

        public void Run(INativeWindowStartupInfo window)
        {
            XSetWindowAttributes attr = new XSetWindowAttributes();
            attr.border_pixel = 0;
            attr.bit_gravity = 1 /*NorthWestGravity	*/;
            attr.event_mask = XEventMask.ExposureMask |
                              XEventMask.ButtonPressMask |
                              XEventMask.ButtonReleaseMask |
                              XEventMask.FocusChangeMask |
                              XEventMask.KeyPressMask |
                              XEventMask.KeyReleaseMask |
                              XEventMask.PointerMotionMask |
                              XEventMask.StructureNotifyMask;
            attr.colormap = graphics.ColorMap;

            ulong windowId = LibX11.XCreateWindow
            (
                display,
                graphics.RootWindow,
                0, 0,
                (uint) window.Width, (uint) window.Height,
                1,
                graphics.PictFormat.depth,
                WindowClass.InputOutput,
                graphics.Visual,
                XSetWindowAttributeMask.CWBorderPixel |
                XSetWindowAttributeMask.CWBitGravity |
                XSetWindowAttributeMask.CWEventMask |
                XSetWindowAttributeMask.CWColormap,
                ref attr
            );

            X11Window nativeWindow = new X11Window(display, windowId, Invalidate);
            window.OnCreate(nativeWindow);
            nativeWindow.SetTitle(window.Title);

            var protocols = new[] {WM_DELETE_WINDOW};
            LibX11.XSetWMProtocols(display, windowId, protocols, protocols.Length);

            LibX11.XMapWindow(display, windowId);
            LibX11.XFlush(display);

            byte[] textBuffer = new byte[32];
            bool autoRepeat = false;

            bool windowClosed = false;

            while (!windowClosed)
            {
                LibX11.XNextEvent(display, out XEvent evt);
                if (evt.type == XEventType.Expose)
                {
                    var rect = new Rectangle(evt.ExposeEvent.x, evt.ExposeEvent.y, evt.ExposeEvent.width, evt.ExposeEvent.height);
                    Invalidate(rect);
                }
                else if (evt.type == XEventType.MotionNotify)
                {
                    window.OnMouseMove(new Point(evt.MotionEvent.x, evt.MotionEvent.y));
                }
                else if (evt.type == XEventType.ConfigureNotify)
                {
                    Size clientArea = new Size(evt.ConfigureEvent.width, evt.ConfigureEvent.height);
                    window.OnResize(clientArea);
                }
                else if (evt.type == XEventType.ButtonPress)
                {
                    window.OnMouseButtonDown(
                        GetMouseButton(evt.ButtonEvent.button),
                        new Point(evt.ButtonEvent.x, evt.ButtonEvent.y),
                        GetModifierKey(evt.ButtonEvent.state)
                    );
                }
                else if (evt.type == XEventType.ButtonRelease)
                {
                    window.OnMouseButtonUp(
                        GetMouseButton(evt.ButtonEvent.button),
                        new Point(evt.ButtonEvent.x, evt.ButtonEvent.y)
                    );
                }
                else if (evt.type == XEventType.KeyPress)
                {
                    // XLookupString returns no more than the requested number of characters, but it also writes a zero-byte after them
                    int charCount = LibX11.XLookupString(ref evt.KeyEvent, textBuffer, textBuffer.Length - 1, out var keySym, IntPtr.Zero);
                    var keyCode = X11KeyMap.GetKeyCode(keySym);
                    if (keyCode == NKeyCode.Unknown)
                    {
                        keySym = LibX11.XLookupKeysym(ref evt.KeyEvent, 0);
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
                        LibX11.XLookupString(ref evt.KeyEvent, textBuffer, textBuffer.Length - 1, out var keySym, IntPtr.Zero);
                        var keyCode = X11KeyMap.GetKeyCode(keySym);
                        if (keyCode == NKeyCode.Unknown)
                        {
                            keySym = LibX11.XLookupKeysym(ref evt.KeyEvent, 0);
                            keyCode = X11KeyMap.GetKeyCode(keySym);
                        }

                        window.OnKeyUp(keyCode);
                    }
                }
                else if (evt.type == XEventType.FocusIn)
                {
                    if (IsWindowActivationEvent(evt.FocusChangeEvent.mode, evt.FocusChangeEvent.detail))
                    {
                        window.OnActivated();
                    }
                }
                else if (evt.type == XEventType.FocusOut)
                {
                    if (IsWindowActivationEvent(evt.FocusChangeEvent.mode, evt.FocusChangeEvent.detail))
                    {
                        window.OnDeactivated();
                    }
                }
                else if (evt.type == XEventType.ClientMessage)
                {
                    var cevt = evt.ClientMessageEvent;
                    if (cevt.message_type == WM_PROTOCOLS && cevt.data0 == WM_DELETE_WINDOW)
                    {
                        windowClosed = true;
                    }
                    else if (cevt.message_type == XA_NWINDOWS_PAINT_COMPLETE)
                    {
                        paintQueued = false;
                    }
                }

                if (invalidatedArea != null && !paintQueued)
                {
                    Rectangle area = invalidatedArea.Value;
                    invalidatedArea = null;

                    using (X11Image image = graphics.CreateImage(area.Width, area.Height))
                    {
                        using (X11Canvas canvas = graphics.CreateCanvas(image.PixmapId))
                        {
                            canvas.SetOrigin(area.X, area.Y);
                            window.OnPaint(canvas, area);
                        }

                        using (X11Canvas canvas = graphics.CreateCanvas(windowId))
                        {
                            canvas.DrawImage(image, area.X, area.Y);
                        }
                    }

                    paintQueued = true;
                    var message = XEvent.CreateClientMessage(windowId, XA_NWINDOWS_PAINT_COMPLETE);
                    LibX11.XSendEvent(display, windowId, 0, XEventMask.NoEventMask, ref message);
                }
            }
        }

        private void Invalidate(Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                invalidatedArea = invalidatedArea == null ? rect : Rectangle.Union(invalidatedArea.Value, rect);
            }
        }

        private static bool IsWindowActivationEvent(FocusNotifyMode mode, FocusNotifyDetail detail)
        {
            bool isWindowActivation = true;

            isWindowActivation &= mode == FocusNotifyMode.NotifyNormal ||
                                  mode == FocusNotifyMode.NotifyWhileGrabbed;

            isWindowActivation &= detail == FocusNotifyDetail.NotifyAncestor ||
                                  detail == FocusNotifyDetail.NotifyVirtual ||
                                  detail == FocusNotifyDetail.NotifyNonlinear ||
                                  detail == FocusNotifyDetail.NotifyNonlinearVirtual;

            return isWindowActivation;
        }

        private NMouseButton GetMouseButton(uint button)
        {
            if (button == 1)
            {
                return NMouseButton.Left;
            }

            if (button == 2)
            {
                return NMouseButton.Middle;
            }

            if (button == 3)
            {
                return NMouseButton.Right;
            }

            if (button == 8)
            {
                return NMouseButton.X1;
            }

            if (button == 9)
            {
                return NMouseButton.X2;
            }

            return NMouseButton.Unknown;
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