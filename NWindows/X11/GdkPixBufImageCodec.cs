using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
            using (GdkPixBufBitmapSource source = LoadSourceFromStream(stream))
            {
                X11Image disposableImage = CreateX11Image(source.Width, source.Height);
                try
                {
                    source.CopyToImage(disposableImage);
                    X11Image image = disposableImage;
                    disposableImage = null;
                    return image;
                }
                finally
                {
                    disposableImage?.Dispose();
                }
            }
        }

        public T LoadBitmapFromStream<T>(Stream stream, CreateBitmapDelegate<T> create)
        {
            using (GdkPixBufBitmapSource source = LoadSourceFromStream(stream))
            {
                return create(source);
            }
        }

        public INativeImage CreateImage(int width, int height)
        {
            // todo: what is the best place for this validation
            if (width < 0 || height < 0)
            {
                throw new ArgumentException($"Image dimensions cannot be negative ({width} x {height}).");
            }

            if (width * 4L > int.MaxValue || height * 4L > int.MaxValue || 4L * width * height > int.MaxValue)
            {
                throw new ArgumentException($"Image dimensions are too large ({width} x {height}).");
            }

            return CreateX11Image(width, height);
        }

        public X11Image CreateX11Image(int width, int height)
        {
            IntPtr nativeImageData = IntPtr.Zero;
            try
            {
                nativeImageData = Marshal.AllocHGlobal(4 * width * height);

                IntPtr xImage = LibX11.XCreateImage
                (
                    display,
                    visual,
                    X11Application.RequiredColorDepth,
                    XImageFormat.ZPixmap,
                    0,
                    nativeImageData,
                    (uint) width,
                    (uint) height,
                    X11Application.RequiredColorDepth,
                    width * 4
                );
                X11Image image = new X11Image(width, height, xImage, nativeImageData);
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

        private GdkPixBufBitmapSource LoadSourceFromStream(Stream stream)
        {
            byte[] streamContent;
            using (var mem = new MemoryStream())
            {
                stream.CopyTo(mem);
                streamContent = mem.ToArray();
            }

            GCHandle pinnedContent = GCHandle.Alloc(streamContent, GCHandleType.Pinned);
            try
            {
                return LoadSourceFromMemory(pinnedContent.AddrOfPinnedObject(), streamContent.Length);
            }
            finally
            {
                pinnedContent.Free();
            }
        }

        private GdkPixBufBitmapSource LoadSourceFromMemory(IntPtr sourceBuffer, int sourceBufferSize)
        {
            IntPtr gdkStream = LibGdkPixBuf.g_memory_input_stream_new_from_data(sourceBuffer, sourceBufferSize, IntPtr.Zero);
            try
            {
                return LoadSourceFromGdkStream(gdkStream);
            }
            finally
            {
                LibGdkPixBuf.g_object_unref(gdkStream);
            }
        }

        private GdkPixBufBitmapSource LoadSourceFromGdkStream(IntPtr gdkStream)
        {
            IntPtr pixbuf = LibGdkPixBuf.gdk_pixbuf_new_from_stream(gdkStream, IntPtr.Zero, out IntPtr errorPtr);
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

                    throw new IOException($"{nameof(LibGdkPixBuf.gdk_pixbuf_new_from_stream)} error: {gdkErrorMessage}");
                }

                var source = GdkPixBufBitmapSource.Create(pixbuf);
                pixbuf = IntPtr.Zero;
                return source;
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

        private class GdkPixBufBitmapSource : INativeBitmapSource, IDisposable
        {
            private enum PixelFormat
            {
                RGBA_32,
                RGB_24
            }

            private readonly IntPtr pixbuf;
            private readonly IntPtr dataPtr;
            private readonly int width;
            private readonly int height;
            private readonly int stride;
            private readonly PixelFormat pixelFormat;

            private GdkPixBufBitmapSource(IntPtr pixbuf, IntPtr dataPtr, int width, int height, int stride, PixelFormat pixelFormat)
            {
                this.pixbuf = pixbuf;
                this.dataPtr = dataPtr;
                this.width = width;
                this.height = height;
                this.stride = stride;
                this.pixelFormat = pixelFormat;
            }

            public int Width => width;
            public int Height => height;

            public void CopyToBitmap(Rectangle imageArea, IntPtr bitmap, int bitmapStride)
            {
                NativeBitmapSourceParameterValidation.CopyToBitmap(this, imageArea, bitmap, bitmapStride, out _);

                if (pixelFormat == PixelFormat.RGBA_32)
                {
                    IntPtr pixelsPtr = dataPtr + imageArea.Y * stride + imageArea.X * 4;
                    PixelConverter.Convert_RGBA_32BE_To_ARGB_32(pixelsPtr, stride, bitmap, bitmapStride, imageArea.Width, imageArea.Height);
                }
                else if (pixelFormat == PixelFormat.RGB_24)
                {
                    IntPtr pixelsPtr = dataPtr + imageArea.Y * stride + imageArea.X * 3;
                    PixelConverter.Convert_RGB_24BE_To_ARGB_32(pixelsPtr, stride, bitmap, bitmapStride, imageArea.Width, imageArea.Height);
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected pixel format: {pixelFormat}.");
                }
            }

            public void CopyToImage(X11Image image)
            {
                if (image.Height != height || image.Width != width)
                {
                    throw new InvalidOperationException($"Source size ({width} x {height}) does not match image size ({image.Width} x {image.Height}).");
                }

                if (pixelFormat == PixelFormat.RGBA_32)
                {
                    PixelConverter.Convert_RGBA_32BE_To_PARGB_32(dataPtr, stride, image.ImageData, 4 * width, width, height);
                }
                else if (pixelFormat == PixelFormat.RGB_24)
                {
                    PixelConverter.Convert_RGB_24BE_To_ARGB_32(dataPtr, stride, image.ImageData, 4 * width, width, height);
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected pixel format: {pixelFormat}.");
                }
            }

            public void Dispose()
            {
                LibGdkPixBuf.g_object_unref(pixbuf);
            }

            public static GdkPixBufBitmapSource Create(IntPtr pixbuf)
            {
                var colorSpace = LibGdkPixBuf.gdk_pixbuf_get_colorspace(pixbuf);
                if (colorSpace != GdkColorspace.GDK_COLORSPACE_RGB)
                {
                    throw new IOException($"Unsupported color space: {colorSpace}.");
                }

                var nChannels = LibGdkPixBuf.gdk_pixbuf_get_n_channels(pixbuf);
                bool hasAlpha = LibGdkPixBuf.gdk_pixbuf_get_has_alpha(pixbuf);

                PixelFormat pixelFormat;
                if (nChannels == 4 && hasAlpha)
                {
                    pixelFormat = PixelFormat.RGBA_32;
                }
                else if (nChannels == 3 && !hasAlpha)
                {
                    pixelFormat = PixelFormat.RGB_24;
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

                IntPtr dataPtr = LibGdkPixBuf.gdk_pixbuf_get_pixels(pixbuf);

                return new GdkPixBufBitmapSource(pixbuf, dataPtr, width, height, stride, pixelFormat);
            }
        }
    }
}