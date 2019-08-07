using System;
using System.Runtime.InteropServices;
using Colormap = System.UInt64;
using PictFormat = System.UInt64;
using Picture = System.UInt64;

namespace NWindows.X11
{
    internal static class LibXRender
    {
        [DllImport("libXrender.so.1")]
        public static extern IntPtr XRenderFindStandardFormat(IntPtr dpy, StandardPictFormat format);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal class XRenderPictFormat
    {
        public readonly PictFormat id;
        public readonly int type;
        public readonly int depth;
        public readonly XRenderDirectFormat direct;
        public readonly Colormap colormap;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal class XRenderDirectFormat
    {
        public readonly short red;
        public readonly short redMask;
        public readonly short green;
        public readonly short greenMask;
        public readonly short blue;
        public readonly short blueMask;
        public readonly short alpha;
        public readonly short alphaMask;
    }

    internal enum StandardPictFormat
    {
        PictStandardARGB32 = 0,
        PictStandardRGB24 = 1,
        PictStandardA8 = 2,
        PictStandardA4 = 3,
        PictStandardA1 = 4,
        PictStandardNUM = 5
    }
}