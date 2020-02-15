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

        private readonly ulong XA_ATOM;
        private readonly ulong XA_CLIPBOARD;
        private readonly ulong XA_TARGETS;
        private readonly ulong XA_UTF8_STRING;
        private readonly ulong XA_NWINDOWS_CONVERTED_CLIPBOARD;
        private readonly ulong XA_NWINDOWS_CLIPBOARD_STOP;

        private readonly object selectionConversionLock = new object();
        private readonly AutoResetEvent selectionConvertedEvent = new AutoResetEvent(false);
        private readonly Thread thread;

        private string convertedText;
        private string bufferText;

        private X11Clipboard(IntPtr display, ulong windowId)
        {
            this.display = display;
            this.windowId = windowId;

            thread = new Thread(Run);
            thread.Name = nameof(X11Clipboard);
            thread.IsBackground = true;

            XA_ATOM = LibX11.XInternAtom(display, "ATOM", 0);
            XA_CLIPBOARD = LibX11.XInternAtom(display, "CLIPBOARD", 0);
            XA_TARGETS = LibX11.XInternAtom(display, "TARGETS", 0);
            XA_UTF8_STRING = LibX11.XInternAtom(display, "UTF8_STRING", 0);
            XA_NWINDOWS_CONVERTED_CLIPBOARD = LibX11.XInternAtom(display, "NWINDOWS_CONVERTED_CLIPBOARD", 0);
            XA_NWINDOWS_CLIPBOARD_STOP = LibX11.XInternAtom(display, "NWINDOWS_CLIPBOARD_STOP", 0);
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
                clipboard.Start();
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

        public void Start()
        {
            thread.Start();
        }

        public void Stop()
        {
            lock (selectionConversionLock)
            {
                var message = XEvent.CreateClientMessage(windowId, XA_NWINDOWS_CLIPBOARD_STOP);
                LibX11.XSendEvent(display, windowId, 0, XEventMask.NoEventMask, ref message);
                LibX11.XFlush(display);
            }

            thread.Join(TimeSpan.FromSeconds(30));
        }

        private void Run()
        {
            bool stopped = false;
            while (!stopped)
            {
                LibX11.XNextEvent(display, out XEvent evt);
                // todo: log and handle exceptions
                if (evt.type == XEventType.SelectionClear)
                {
                    HandleSelectionClear(evt.SelectionClearEvent);
                }
                else if (evt.type == XEventType.SelectionRequest)
                {
                    HandleSelectionRequest(evt.SelectionRequestEvent);
                }
                else if (evt.type == XEventType.SelectionNotify)
                {
                    HandleSelectionNotify(evt.SelectionEvent);
                }
                else if (evt.type == XEventType.ClientMessage)
                {
                    if (evt.ClientMessageEvent.message_type == XA_NWINDOWS_CLIPBOARD_STOP)
                    {
                        stopped = true;
                    }
                }
            }

            LibX11.XCloseDisplay(display);
        }

        private void HandleSelectionClear(XSelectionClearEvent evt)
        {
            if (evt.window != windowId)
            {
                return;
            }

            lock (selectionConversionLock)
            {
                bufferText = null;
            }
        }

        private void HandleSelectionRequest(XSelectionRequestEvent evt)
        {
            if (evt.owner != windowId)
            {
                return;
            }

            var targetProperty = evt.property;
            if (targetProperty == 0)
            {
                // Owners receiving ConvertSelection requests with a property argument of None are talking to an obsolete client.
                // They should choose the target atom as the property name to be used for the reply.
                targetProperty = evt.target;
            }

            bool dataSent = ConvertOwnSelection(evt.requestor, evt.selection, evt.target, targetProperty);
            var response = XEvent.CreateSelectionNotify(evt.requestor, evt.selection, evt.target, dataSent ? targetProperty : 0, evt.time);
            LibX11.XSendEvent(display, evt.requestor, 0, XEventMask.NoEventMask, ref response);
        }

        private bool ConvertOwnSelection(ulong requestor, ulong selection, ulong targetType, ulong targetProperty)
        {
            if (selection != XA_CLIPBOARD)
            {
                return false;
            }

            string textToConvert;
            lock (selectionConversionLock)
            {
                textToConvert = bufferText;
                if (textToConvert == null)
                {
                    return false;
                }
            }

            if (targetType == XA_TARGETS)
            {
                ulong[] targets = {XA_TARGETS, XA_UTF8_STRING};

                LibX11.XChangeProperty
                (
                    display,
                    requestor,
                    targetProperty,
                    XA_ATOM,
                    XChangePropertyFormat.Atom,
                    XChangePropertyMode.PropModeReplace,
                    targets,
                    targets.Length
                );

                return true;
            }

            if (targetType == XA_UTF8_STRING)
            {
                byte[] text = Encoding.UTF8.GetBytes(textToConvert);
                LibX11.XChangeProperty
                (
                    display,
                    requestor,
                    targetProperty,
                    XA_UTF8_STRING,
                    XChangePropertyFormat.Byte,
                    XChangePropertyMode.PropModeReplace,
                    text,
                    text.Length
                );

                return true;
            }

            return false;
        }

        private void HandleSelectionNotify(XSelectionEvent evt)
        {
            lock (selectionConversionLock)
            {
                if (evt.property == XA_NWINDOWS_CONVERTED_CLIPBOARD)
                {
                    // todo: handle INCR Properties
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

        public void PutText(string text)
        {
            lock (selectionConversionLock)
            {
                bufferText = text;
                LibX11.XSetSelectionOwner(display, XA_CLIPBOARD, windowId, 0);
                LibX11.XFlush(display);
            }
        }

        public bool TryGetText(out string text)
        {
            lock (selectionConversionLock)
            {
                selectionConvertedEvent.Reset();

                // todo: review usage of XLib under lock

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