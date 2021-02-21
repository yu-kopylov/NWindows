namespace NWindows.X11
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Atom = System.UInt64;
    using Bool = System.Int32;
    using XFixed = System.Int32;
    using XDouble = System.Double;
    using Colormap = System.UInt64;
    using Drawable = System.UInt64;
    using PictFormat = System.UInt64;
    using Picture = System.UInt64;
    using DisplayPtr = System.IntPtr;
    using PictFormatPtr = System.IntPtr;
    using Pixmap = System.UInt64;

    internal static class LibXRender
    {
        [DllImport("libXrender.so.1")]
        public static extern PictFormatPtr XRenderFindStandardFormat(DisplayPtr dpy, StandardPictFormat format);

        [DllImport("libXrender.so.1")]
        public static extern Picture XRenderCreatePicture(
            DisplayPtr dpy,
            Drawable drawable,
            PictFormatPtr format,
            XRenderPictureAttributeMask valuemask,
            ref XRenderPictureAttributes attributes
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderFreePicture(DisplayPtr dpy, Picture picture);

        [DllImport("libXrender.so.1")]
        public static extern void XRenderSetPictureClipRectangles(
            DisplayPtr dpy,
            Picture picture,
            int xOrigin,
            int yOrigin,
            [In] XRectangle[] rects,
            int n
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderFillRectangle(
            DisplayPtr dpy,
            PictOp op,
            Picture dst,
            [In] ref XRenderColor color,
            int x,
            int y,
            uint width,
            uint height
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderComposite(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture mask,
            Picture dst,
            int src_x,
            int src_y,
            int mask_x,
            int mask_y,
            int dst_x,
            int dst_y,
            uint width,
            uint height
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderCompositeTrapezoids(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture dst,
            [In] ref XRenderPictFormat maskFormat,
            int xSrc,
            int ySrc,
            [In] XTrapezoid[] traps,
            int ntrap
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderCompositeTriangles(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture dst,
            [In] /* XRenderPictFormat */ IntPtr maskFormat,
            int xSrc,
            int ySrc,
            [In] XTriangle[] triangles,
            int ntriangle
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderCompositeTriStrip(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture dst,
            [In] /* XRenderPictFormat */ IntPtr maskFormat,
            int xSrc,
            int ySrc,
            [In] XPointFixed[] points,
            int npoint
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderCompositeTriFan(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture dst,
            [In] /* XRenderPictFormat */ IntPtr maskFormat,
            int xSrc,
            int ySrc,
            [In] XPointFixed[] points,
            int npoint
        );

        [DllImport("libXrender.so.1")]
        public static extern void XRenderCompositeDoublePoly(
            DisplayPtr dpy,
            PictOp op,
            Picture src,
            Picture dst,
            [In] /* XRenderPictFormat */ IntPtr maskFormat,
            int xSrc,
            int ySrc,
            int xDst,
            int yDst,
            XPointDouble[] fpoints,
            int npoints,
            int winding
        );

        [DllImport("libXrender.so.1")]
        public static extern Picture XRenderCreateSolidFill(DisplayPtr dpy, [In] ref XRenderColor color);
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

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XRenderColor
    {
        public readonly ushort red;
        public readonly ushort green;
        public readonly ushort blue;
        public readonly ushort alpha;

        public XRenderColor(Color color) : this()
        {
            red = (ushort) (color.R * 0x101 * color.A / 255);
            green = (ushort) (color.G * 0x101 * color.A / 255);
            blue = (ushort) (color.B * 0x101 * color.A / 255);
            alpha = (ushort) (color.A * 0x101);
        }
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

    internal enum PictOp
    {
        PictOpClear = 0,
        PictOpSrc = 1,
        PictOpDst = 2,
        PictOpOver = 3,
        PictOpOverReverse = 4,
        PictOpIn = 5,
        PictOpInReverse = 6,
        PictOpOut = 7,
        PictOpOutReverse = 8,
        PictOpAtop = 9,
        PictOpAtopReverse = 10,
        PictOpXor = 11,
        PictOpAdd = 12,
        PictOpSaturate = 13
    }

    [Flags]
    internal enum XRenderPictureAttributeMask : ulong
    {
        CPRepeat = (1 << 0),
        CPAlphaMap = (1 << 1),
        CPAlphaXOrigin = (1 << 2),
        CPAlphaYOrigin = (1 << 3),
        CPClipXOrigin = (1 << 4),
        CPClipYOrigin = (1 << 5),
        CPClipMask = (1 << 6),
        CPGraphicsExposure = (1 << 7),
        CPSubwindowMode = (1 << 8),
        CPPolyEdge = (1 << 9),
        CPPolyMode = (1 << 10),
        CPDither = (1 << 11),
        CPComponentAlpha = (1 << 12),
        CPLastBit = 11
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XRenderPictureAttributes
    {
        public Bool repeat;
        public Picture alpha_map;
        public int alpha_x_origin;
        public int alpha_y_origin;
        public int clip_x_origin;
        public int clip_y_origin;
        public Pixmap clip_mask;
        public Bool graphics_exposures;
        public int subwindow_mode;
        public XRenderPolyEdge poly_edge;
        public XRenderPolyMode poly_mode;
        public Atom dither;
        public Bool component_alpha;
    }

    internal enum XRenderPolyEdge
    {
        Sharp = 0,
        Smooth = 1
    }

    internal enum XRenderPolyMode
    {
        Precise = 0,
        Imprecise = 1
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XPointFixed
    {
        public XFixed x;
        public XFixed y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XLineFixed
    {
        public XPointFixed p1;
        public XPointFixed p2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XPointDouble
    {
        public XDouble x;
        public XDouble y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XTrapezoid
    {
        public XFixed top;
        public XFixed bottom;
        public XLineFixed left;
        public XLineFixed right;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct XTriangle
    {
        public XPointFixed p1;
        public XPointFixed p2;
        public XPointFixed p3;
    }
}