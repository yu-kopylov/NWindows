using System;
using System.IO;
using System.Runtime.InteropServices;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal class Win32ImageCodec : INativeImageCodec
    {
        public INativeImage LoadImageFromStream(Stream stream)
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
                return LoadImageFromMemory(pinnedContentPtr, (uint) streamContent.Length);
            }
            finally
            {
                pinnedContent.Free();
            }
        }

        private INativeImage LoadImageFromMemory(IntPtr sourceBuffer, uint sourceBufferSize)
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

                if (frame.GetPixelFormat() == WICPixelFormat.GUID_WICPixelFormat32bppPBGRA)
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
                byte[] pixels = new byte[4 * width * height];
                GCHandle pixelsHandle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
                try
                {
                    source.CopyPixels(null, 4 * width, 4 * width * height, pixelsHandle.AddrOfPinnedObject());
                }
                finally
                {
                    pixelsHandle.Free();
                }

                return new W32Image((int) width, (int) height, pixels);
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

        internal class DisposableComObject<T> : IDisposable
        {
            public T Object { get; }

            public DisposableComObject(T obj)
            {
                Object = obj;
            }

            public void Dispose()
            {
                Marshal.ReleaseComObject(Object);
            }
        }
    }
}