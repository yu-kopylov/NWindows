using System;
using System.Runtime.InteropServices;
using NWindows.X11;

namespace NWindows.Win32
{
    public class Win32ImageCodec : IImageCodec
    {
        public IImage LoadFromFile(string filename)
        {
            IWICImagingFactory imagingFactory = new IWICImagingFactory();
            IWICBitmapDecoder decoder = null;
            IWICBitmapFrameDecode frame = null;
            IWICFormatConverter formatConverter = null;

            try
            {
                decoder = imagingFactory.CreateDecoderFromFilename(
                    filename,
                    Guid.Empty,
                    WICFileAccessMask.GENERIC_READ,
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