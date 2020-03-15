using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class X11Graphics : INativeGraphics, IDisposable
    {
        private readonly IntPtr display;
        private readonly int screen;
        private readonly ulong rootWindow;
        private readonly IntPtr visual;
        private readonly ulong colorMap;
        private readonly XRenderPictFormat pictFormat;
        private readonly IntPtr pictFormatPtr;

        private readonly X11ObjectCache objectCache;

        private X11Graphics(IntPtr display, int screen, ulong rootWindow, IntPtr visual, ulong colorMap, XRenderPictFormat pictFormat, IntPtr pictFormatPtr)
        {
            this.display = display;
            this.screen = screen;
            this.rootWindow = rootWindow;
            this.visual = visual;
            this.colorMap = colorMap;
            this.pictFormat = pictFormat;
            this.pictFormatPtr = pictFormatPtr;
            this.objectCache = new X11ObjectCache(display, screen);
        }

        internal IntPtr Display
        {
            get { return display; }
        }

        internal int Screen
        {
            get { return screen; }
        }

        internal ulong RootWindow
        {
            get { return rootWindow; }
        }

        internal IntPtr Visual
        {
            get { return visual; }
        }

        internal ulong ColorMap
        {
            get { return colorMap; }
        }

        internal XRenderPictFormat PictFormat
        {
            get { return pictFormat; }
        }

        internal IntPtr PictFormatPtr
        {
            get { return pictFormatPtr; }
        }

        public void Dispose()
        {
            objectCache.Clear();
        }

        public static X11Graphics Create(IntPtr display)
        {
            int defaultScreen = LibX11.XDefaultScreen(display);
            ulong defaultRootWindow = LibX11.XDefaultRootWindow(display);

            //todo: is is safe to keep pictFormatPtr after reading it into XRenderPictFormat
            IntPtr pictFormatPtr = LibXRender.XRenderFindStandardFormat(display, StandardPictFormat.PictStandardARGB32);

            if (pictFormatPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Display does not support 32-bit ARGB.");
            }

            XRenderPictFormat pictFormat = Marshal.PtrToStructure<XRenderPictFormat>(pictFormatPtr);

            int status = LibX11.XMatchVisualInfo
            (
                display,
                defaultScreen,
                pictFormat.depth,
                VisualClass.TrueColor,
                out XVisualInfo visualInfo
            );
            if (status == 0)
            {
                throw new InvalidOperationException($"TrueColor visual with depth {pictFormat.depth} does not exist.");
            }

            ulong colorMap = LibX11.XCreateColormap(display, defaultRootWindow, visualInfo.visual, CreateColormapOption.AllocNone);

            return new X11Graphics(display, defaultScreen, defaultRootWindow, visualInfo.visual, colorMap, pictFormat, pictFormatPtr);
        }

        public Size MeasureString(FontConfig font, string text)
        {
            XftFontExt fontExt = objectCache.GetXftFont(font);
            byte[] utf32Text = Encoding.UTF32.GetBytes(text);

            int width = 0;
            GCHandle utf32TextHandle = GCHandle.Alloc(utf32Text, GCHandleType.Pinned);
            try
            {
                IntPtr utf32TextPtr = utf32TextHandle.AddrOfPinnedObject();
                foreach (var range in fontExt.GetRanges(utf32Text))
                {
                    LibXft.XftTextExtents32(
                        display,
                        range.Font,
                        utf32TextPtr + range.Start,
                        (range.End - range.Start) / 4,
                        out var extents
                    );
                    width += extents.xOff;
                }
            }
            finally
            {
                utf32TextHandle.Free();
            }

            var fontInfo = Marshal.PtrToStructure<XftFont>(fontExt.MainFont);
            return new Size(width, fontInfo.height);
        }

        internal X11Image CreateImage(int width, int height)
        {
            return X11Image.Create(display, visual, rootWindow, width, height);
        }

        internal X11Canvas CreateCanvas(ulong drawableId)
        {
            return X11Canvas.CreateForDrawable(display, screen, objectCache, visual, colorMap, pictFormatPtr, drawableId);
        }
    }
}