using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Clipboard : INativeClipboard
    {
        private const int OperationTimeout = 10000;

        private readonly IntPtr display;
        private readonly ulong windowId;
        private readonly ulong XA_CLIPBOARD;
        private readonly ulong XA_UTF8_STRING;
        private readonly ulong XA_NWINDOWS_CONVERTED_CLIPBOARD;

        // todo: make this class disposable
        private readonly object selectionConversionLock = new object();
        private readonly AutoResetEvent selectionConvertedEvent = new AutoResetEvent(false);

        private string convertedText;

        private X11Clipboard(IntPtr display, ulong windowId)
        {
            this.display = display;
            this.windowId = windowId;

            XA_CLIPBOARD = LibX11.XInternAtom(display, "CLIPBOARD", 0);
            XA_UTF8_STRING = LibX11.XInternAtom(display, "UTF8_STRING", 0);
            XA_NWINDOWS_CONVERTED_CLIPBOARD = LibX11.XInternAtom(display, "NWINDOWS_CONVERTED_CLIPBOARD", 0);
        }

        public static X11Clipboard Create()
        {
            IntPtr display = IntPtr.Zero;
            ulong windowId = 0;

            try
            {
                display = LibX11.XOpenDisplay(null);
                if (display == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Cannot open display.");
                }

                ulong rootWindow = LibX11.XDefaultRootWindow(display);
                int defaultScreen = LibX11.XDefaultScreen(display);
                IntPtr defaultVisual = LibX11.XDefaultVisual(display, defaultScreen);

                XSetWindowAttributes attr = new XSetWindowAttributes();
                windowId = LibX11.XCreateWindow
                (
                    display,
                    rootWindow,
                    0, 0,
                    10, 10,
                    0,
                    0,
                    WindowClass.InputOnly,
                    defaultVisual,
                    0,
                    ref attr
                );

                LibX11.XFlush(display);

                X11Clipboard clipboard = new X11Clipboard(display, windowId);
                Thread thread = new Thread(clipboard.Run);
                thread.IsBackground = true;
                thread.Start();
                windowId = 0;
                display = IntPtr.Zero;
                return clipboard;
            }
            finally
            {
                if (windowId != 0)
                {
                    LibX11.XDestroyWindow(display, windowId);
                    LibX11.XFlush(display);
                }

                if (display != IntPtr.Zero)
                {
                    LibX11.XCloseDisplay(display);
                }
            }
        }

        private void Run()
        {
            // todo: stop correctly
            while (true)
            {
                LibX11.XNextEvent(display, out XEvent evt);
                // todo: log and handle exceptions
                if (evt.type == XEventType.SelectionNotify)
                {
                    lock (selectionConversionLock)
                    {
                        if (evt.SelectionEvent.property == XA_NWINDOWS_CONVERTED_CLIPBOARD)
                        {
                            byte[] data = ReadWindowProperty(display, windowId, XA_NWINDOWS_CONVERTED_CLIPBOARD);
                            convertedText = Encoding.UTF8.GetString(data);
                        }
                        else
                        {
                            convertedText = null;
                        }

                        selectionConvertedEvent.Set();
                    }
                }
            }
        }

        public bool GetText(out string text)
        {
            lock (selectionConversionLock)
            {
                selectionConvertedEvent.Reset();

                LibX11.XConvertSelection
                (
                    display,
                    XA_CLIPBOARD,
                    XA_UTF8_STRING,
                    XA_NWINDOWS_CONVERTED_CLIPBOARD,
                    windowId,
                    0 /* CurrentTime */
                );

                LibX11.XFlush(display);
            }

            if (!selectionConvertedEvent.WaitOne(OperationTimeout))
            {
                text = null;
                return false;
            }

            lock (selectionConversionLock)
            {
                if (convertedText == null)
                {
                    text = null;
                    return false;
                }

                text = convertedText;
                convertedText = null;
                return true;
            }
        }

        private static byte[] ReadWindowProperty(IntPtr display, ulong windowId, ulong property)
        {
            const long smallPropertyLength = 1024;

            LibX11.XGetWindowProperty
            (
                display,
                windowId,
                property,
                0,
                smallPropertyLength,
                1,
                0 /* AnyPropertyType */,
                out ulong type,
                out int format,
                out ulong nItems,
                out ulong bytes_after_return,
                out IntPtr bufferPtr
            );

            if (bytes_after_return != 0)
            {
                if (bufferPtr != IntPtr.Zero)
                {
                    LibX11.XFree(bufferPtr);
                }

                ulong actualLength = bytes_after_return + 4 * smallPropertyLength;

                if (bytes_after_return > int.MaxValue || actualLength > int.MaxValue)
                {
                    throw new InvalidOperationException
                    (
                        $"Property {property} is too long: {nameof(bytes_after_return)} = {bytes_after_return}."
                    );
                }

                LibX11.XGetWindowProperty
                (
                    display,
                    windowId,
                    property,
                    0,
                    (long) (actualLength + 3) / 4,
                    1,
                    0 /* AnyPropertyType */,
                    out type,
                    out format,
                    out nItems,
                    out bytes_after_return,
                    out bufferPtr
                );
            }

            try
            {
                if ((nItems != 0) && (format <= 0 || format % 8 != 0))
                {
                    throw new InvalidOperationException($"{nameof(LibX11.XGetWindowProperty)} returned an invalid format: {format}.");
                }

                int chunkSize = (int) nItems * format / 8;
                byte[] res = new byte [chunkSize];
                if (chunkSize != 0)
                {
                    if (bufferPtr == IntPtr.Zero)
                    {
                        throw new InvalidOperationException($"{nameof(LibX11.XGetWindowProperty)} returned an empty buffer for a non-empty value.");
                    }

                    Marshal.Copy(bufferPtr, res, 0, chunkSize);
                }

                return res;
            }
            finally
            {
                if (bufferPtr != IntPtr.Zero)
                {
                    LibX11.XFree(bufferPtr);
                }
            }
        }
    }
}