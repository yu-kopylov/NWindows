using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NWindows.NativeApi;

namespace NWindows.X11
{
    internal class GdkPixBufImageCodec : INativeImageCodec
    {
        private readonly IntPtr display;
        private readonly IntPtr visual;

        public GdkPixBufImageCodec(IntPtr display, IntPtr visual)
        {
            this.display = display;
            this.visual = visual;
        }

        public INativeImage LoadImageFromStream(Stream stream)
        {
            // todo: implement
            return null;
            // NBitmap bitmap = LoadBitmapFromFile(filename);
            // return CreateImageFromBitmap(bitmap);
        }

        private NBitmap LoadBitmapFromFile(string filename)
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
                bool hasAlpha = LibGdkPixBuf.gdk_pixbuf_get_has_alpha(pixbuf);

                ConvertPixbufToBitmapDelegate convertStride;

                if (nChannels == 4 && hasAlpha)
                {
                    convertStride = From32RGBATo32ARGB;
                }
                else if (nChannels == 3 && !hasAlpha)
                {
                    convertStride = From24RGBTo32ARGB;
                }
                else
                {
                    throw new IOException($"Unsupported number of channels: {nChannels} (has alpha: {hasAlpha}).");
                }

                int bitsPerChanel = LibGdkPixBuf.gdk_pixbuf_get_bits_per_sample(pixbuf);
                if (bitsPerChanel != 8)
                {
                    throw new IOException($"Unsupported number of bits per chanel: {bitsPerChanel}.");
                }

                int width = LibGdkPixBuf.gdk_pixbuf_get_width(pixbuf);
                int height = LibGdkPixBuf.gdk_pixbuf_get_height(pixbuf);
                int stride = LibGdkPixBuf.gdk_pixbuf_get_rowstride(pixbuf);

                if (stride < width * nChannels)
                {
                    throw new IOException(
                        $"Unexpected row stride. Channels: {nChannels}. Bits per chanel: {bitsPerChanel}. Width: {width}. Stride: {stride}."
                    );
                }

                IntPtr pixbufDataPtr = LibGdkPixBuf.gdk_pixbuf_get_pixels(pixbuf);
                byte[] pixbufData = new byte[height * stride];
                Marshal.Copy(pixbufDataPtr, pixbufData, 0, pixbufData.Length);

                NBitmap bitmap = new NBitmap(width, height);

                Parallel.For(0, height, y => convertStride(pixbufData, stride * y, bitmap, y, width));

                return bitmap;
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

        private INativeImage CreateImageFromBitmap(NBitmap bitmap)
        {
            int imageDataSize = bitmap.Width * bitmap.Height;
            int[] imageData = new int[imageDataSize];
            Parallel.For(0, bitmap.Height, y =>
            {
                for (int x = 0, ofs = y * bitmap.Width; x < bitmap.Width; x++, ofs++)
                {
                    byte a = bitmap[x, y].A;
                    byte r = (byte) (bitmap[x, y].R * a / 255);
                    byte g = (byte) (bitmap[x, y].G * a / 255);
                    byte b = (byte) (bitmap[x, y].B * a / 255);
                    imageData[ofs] = (a << 24) | (r << 16) | (g << 8) | b;
                }
            });

            IntPtr nativeImageData = IntPtr.Zero;
            try
            {
                nativeImageData = Marshal.AllocHGlobal(4 * imageDataSize);
                Marshal.Copy(imageData, 0, nativeImageData, imageDataSize);

                IntPtr xImage = LibX11.XCreateImage
                (
                    display,
                    visual,
                    X11Application.RequiredColorDepth,
                    XImageFormat.ZPixmap,
                    0,
                    nativeImageData,
                    (uint) bitmap.Width,
                    (uint) bitmap.Height,
                    X11Application.RequiredColorDepth,
                    bitmap.Width * 4
                );
                X11Image image = new X11Image(xImage, bitmap.Width, bitmap.Height);
                nativeImageData = IntPtr.Zero;
                return image;
            }
            finally
            {
                if (nativeImageData != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(nativeImageData);
                }
            }
        }

        private delegate void ConvertPixbufToBitmapDelegate(byte[] pixbufData, int dataOffset, NBitmap bitmap, int y, int width);

        private static void From32RGBATo32ARGB(byte[] pixbufData, int dataOffset, NBitmap bitmap, int y, int width)
        {
            for (int x = 0; x < width; x++, dataOffset += 4)
            {
                bitmap[x, y] = Color32.FromARGB
                (
                    pixbufData[dataOffset + 3],
                    pixbufData[dataOffset + 0],
                    pixbufData[dataOffset + 1],
                    pixbufData[dataOffset + 2]
                );
            }
        }

        private static void From24RGBTo32ARGB(byte[] pixbufData, int dataOffset, NBitmap bitmap, int y, int width)
        {
            for (int x = 0; x < width; x++, dataOffset += 3)
            {
                bitmap[x, y] = Color32.FromARGB
                (
                    0xFF,
                    pixbufData[dataOffset + 0],
                    pixbufData[dataOffset + 1],
                    pixbufData[dataOffset + 2]
                );
            }
        }
    }
}