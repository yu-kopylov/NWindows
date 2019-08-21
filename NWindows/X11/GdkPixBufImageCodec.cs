using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NWindows.X11
{
    public class GdkPixBufImageCodec : IImageCodec
    {
        private readonly IntPtr display;
        private readonly IntPtr visual;

        public GdkPixBufImageCodec(IntPtr display, IntPtr visual)
        {
            this.display = display;
            this.visual = visual;
        }

        public IImage LoadFromFile(string filename)
        {
            IntPtr pixbuf = LibGdkPixBuf.gdk_pixbuf_new_from_file(filename, out IntPtr errorPtr);
            try
            {
                if (pixbuf == IntPtr.Zero)
                {
                    string gdkErrorMessage;
                    if (errorPtr != IntPtr.Zero)
                    {
                        GError error = Marshal.PtrToStructure<GError>(errorPtr);
                        gdkErrorMessage = error.message;
                    }
                    else
                    {
                        gdkErrorMessage = "Unknown error.";
                    }

                    throw new IOException($"{nameof(LibGdkPixBuf.gdk_pixbuf_new_from_file)} error: {gdkErrorMessage}");
                }

                var colorSpace = LibGdkPixBuf.gdk_pixbuf_get_colorspace(pixbuf);
                if (colorSpace != GdkColorspace.GDK_COLORSPACE_RGB)
                {
                    throw new IOException($"Unsupported color space: {colorSpace}.");
                }

                var nChannels = LibGdkPixBuf.gdk_pixbuf_get_n_channels(pixbuf);
                if (nChannels != 4)
                {
                    throw new IOException($"Unsupported number of channels: {nChannels}.");
                }

                if (!LibGdkPixBuf.gdk_pixbuf_get_has_alpha(pixbuf))
                {
                    throw new IOException($"Images without alpha channel are not supported.");
                }

                int bitsPerChanel = LibGdkPixBuf.gdk_pixbuf_get_bits_per_sample(pixbuf);
                if (bitsPerChanel != X11Application.RequiredBitsPerChannel)
                {
                    throw new IOException($"Unsupported image depth: {bitsPerChanel}.");
                }

                int width = LibGdkPixBuf.gdk_pixbuf_get_width(pixbuf);
                int height = LibGdkPixBuf.gdk_pixbuf_get_height(pixbuf);
                int rowStride = LibGdkPixBuf.gdk_pixbuf_get_rowstride(pixbuf);

                if (rowStride != width * 4)
                {
                    throw new IOException(
                        $"Unexpected row stride. Channels: {nChannels}. Bits per chanel: {bitsPerChanel}. Width: {width}. Stride: {rowStride}."
                    );
                }

                IntPtr pixbufPixels = LibGdkPixBuf.gdk_pixbuf_get_pixels(pixbuf);

                int pixelDataSize = height * width;
                int[] pixelData = new int[pixelDataSize];
                Marshal.Copy(pixbufPixels, pixelData, 0, pixelDataSize);
                for (int i = 0; i < pixelDataSize; i++)
                {
                    uint c = (uint) pixelData[i];
                    byte r = (byte) c;
                    byte g = (byte) (c >> 8);
                    byte b = (byte) (c >> 16);
                    byte a = (byte) (c >> 24);
                    r = (byte) (r * a / 255);
                    g = (byte) (g * a / 255);
                    b = (byte) (b * a / 255);
                    pixelData[i] = (a << 24) | (r << 16) | (g << 8) | b;
                }

                // todo: make sure to release imagePixels even if XDestroyImage fails
                IntPtr imagePixels = Marshal.AllocHGlobal(4 * pixelDataSize);
                Marshal.Copy(pixelData, 0, imagePixels, pixelDataSize);

                IntPtr xImage = LibX11.XCreateImage
                (
                    display,
                    visual,
                    X11Application.RequiredColorDepth,
                    XImageFormat.ZPixmap,
                    0,
                    imagePixels,
                    (uint) width,
                    (uint) height,
                    X11Application.RequiredColorDepth,
                    rowStride
                );
                return new X11Image(xImage, width, height);
            }
            finally
            {
                if (pixbuf != IntPtr.Zero)
                {
                    LibGdkPixBuf.g_object_unref(pixbuf);
                }

                if (errorPtr != IntPtr.Zero)
                {
                    LibGdkPixBuf.g_error_free(errorPtr);
                }
            }
        }
    }
}