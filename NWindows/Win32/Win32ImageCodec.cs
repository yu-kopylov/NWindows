using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32ImageCodec : INativeImageCodec
    {
        public INativeImage LoadImageFromStream(Stream stream)
        {
            return LoadBitmapFromStream(stream, WICPixelFormat.GUID_WICPixelFormat32bppPBGRA, CreateImage);
        }

        public T LoadBitmapFromStream<T>(Stream stream, CreateBitmapDelegate<T> createBitmap)
        {
            return LoadBitmapFromStream(stream, WICPixelFormat.GUID_WICPixelFormat32bppBGRA, createBitmap);
        }

        private static INativeImage CreateImage(INativeBitmapSource source)
        {
            byte[] pixels = new byte[4 * source.Width * source.Height];
            GCHandle pixelsHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
            try
            {
                source.CopyToBitmap(new Rectangle(0, 0, source.Width, source.Height), pixelsHandle.AddrOfPinnedObject(), 4 * source.Width);
            }
            finally
            {
                pixelsHandle.Free();
            }

            return new W32Image(source.Width, source.Height, pixels);
        }

        private T LoadBitmapFromStream<T>(Stream stream, Guid pixelFormat, CreateBitmapDelegate<T> createImage)
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
                IntPtr pinnedContentPtr = pinnedContent.AddrOfPinnedObject();
                return LoadBitmapFromMemory(pinnedContentPtr, (uint) streamContent.Length, pixelFormat, createImage);
            }
            finally
            {
                pinnedContent.Free();
            }
        }

        private T LoadBitmapFromMemory<T>(IntPtr sourceBuffer, uint sourceBufferSize, Guid pixelFormat, CreateBitmapDelegate<T> createBitmap)
        {
            IWICImagingFactory imagingFactory = null;
            IWICStream wicStream = null;
            IWICBitmapDecoder decoder = null;
            IWICBitmapFrameDecode frame = null;
            IWICFormatConverter formatConverter = null;

            try
            {
                imagingFactory = new IWICImagingFactory();

                wicStream = imagingFactory.CreateStream();
                wicStream.InitializeFromMemory(sourceBuffer, sourceBufferSize);

                decoder = imagingFactory.CreateDecoderFromStream(
                    wicStream,
                    Guid.Empty,
                    WICDecodeOptions.WICDecodeMetadataCacheOnLoad
                );

                frame = decoder.GetFrame(0);

                IWICBitmapSource source;

                if (frame.GetPixelFormat() == pixelFormat)
                {
                    source = frame;
                }
                else
                {
                    formatConverter = imagingFactory.CreateFormatConverter();
                    formatConverter.Initialize(
                        frame,
                        WICPixelFormat.GUID_WICPixelFormat32bppPBGRA,
                        WICBitmapDitherType.WICBitmapDitherTypeNone,
                        null,
                        0.0f,
                        WICBitmapPaletteType.WICBitmapPaletteTypeCustom
                    );
                    source = formatConverter;
                }

                source.GetSize(out uint width, out uint height);
                if (width * 4UL > int.MaxValue || height * 4UL > int.MaxValue || 4UL * width * height > int.MaxValue)
                {
                    throw new IOException($"Image is too large: {width}x{height}.");
                }

                WicBitmapSource bitmapSource = new WicBitmapSource(source, (int) width, (int) height);
                return createBitmap(bitmapSource);
            }
            finally
            {
                SafeRelease(formatConverter);
                SafeRelease(frame);
                SafeRelease(decoder);
                SafeRelease(wicStream);
                SafeRelease(imagingFactory);
            }
        }

        private void SafeRelease(object comObject)
        {
            if (comObject != null)
            {
                Marshal.ReleaseComObject(comObject);
            }
        }

        private class WicBitmapSource : INativeBitmapSource
        {
            private readonly IWICBitmapSource source;

            public WicBitmapSource(IWICBitmapSource source, int width, int height)
            {
                this.source = source;
                Width = width;
                Height = height;
            }

            public int Width { get; }
            public int Height { get; }

            public void CopyToBitmap(Rectangle sourceArea, IntPtr bitmap, int stride)
            {
                NativeBitmapSourceParameterValidation.CopyToBitmap(this, sourceArea, bitmap, stride, out int requiredBufferSize);

                if (sourceArea.Width <= 0 || sourceArea.Height <= 0)
                {
                    return;
                }

                WICRect rect = new WICRect(sourceArea.X, sourceArea.Y, sourceArea.Width, sourceArea.Height);
                source.CopyPixels(rect, (uint) stride, (uint) requiredBufferSize, bitmap);
            }
        }
    }
}