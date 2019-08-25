namespace NWindows.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using DWORD = System.UInt32;
    using HBITMAP = System.IntPtr;
    using HICON = System.IntPtr;
    using HPALETTE = System.IntPtr;
    using UINT = System.UInt32;
    using ULONG_PTR = System.IntPtr;
    using WINBOOL = System.Boolean;

    [ComImport, Guid("cacaf262-9370-4615-a13b-9f5539da4c0a")]
    internal class WICImagingFactory
    {
    }

    [ComImport, Guid("ec5ec8a9-c395-4314-9c77-54d7a935ff70"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CoClass(typeof(WICImagingFactory))]
    internal interface IWICImagingFactory
    {
        IWICBitmapDecoder CreateDecoderFromFilename(
            [MarshalAs(UnmanagedType.LPWStr)] string wzFilename,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pguidVendor,
            WICFileAccessMask dwDesiredAccess,
            WICDecodeOptions metadataOptions
        );

        IWICBitmapDecoder CreateDecoderFromStream(
            IStream pIStream,
            [In] ref Guid? pguidVendor,
            WICDecodeOptions metadataOptions
        );

        IWICBitmapDecoder CreateDecoderFromFileHandle(
            ULONG_PTR hFile,
            [In] ref Guid? pguidVendor,
            WICDecodeOptions metadataOptions
        );

        IWICComponentInfo CreateComponentInfo([In] ref Guid clsidComponent);

        IWICBitmapDecoder CreateDecoder([In] ref Guid guidContainerFormat, [In] ref Guid? pguidVendor);

        IWICBitmapEncoder CreateEncoder([In] ref Guid guidContainerFormat, [In] ref Guid? pguidVendor);

        IWICPalette CreatePalette();

        IWICFormatConverter CreateFormatConverter();

        IWICBitmapScaler CreateBitmapScaler();

        IWICBitmapClipper CreateBitmapClipper();

        IWICBitmapFlipRotator CreateBitmapFlipRotator();

        IWICStream CreateStream();

        IWICColorContext CreateColorContext();

        IWICColorTransform CreateColorTransformer();

        IWICBitmap CreateBitmap(
            UINT uiWidth,
            UINT uiHeight,
            [In] ref Guid pixelFormat,
            WICBitmapCreateCacheOption option
        );

        IWICBitmap CreateBitmapFromSource(IWICBitmapSource piBitmapSource, WICBitmapCreateCacheOption option);

        IWICBitmap CreateBitmapFromSourceRect(
            IWICBitmapSource piBitmapSource,
            UINT x,
            UINT y,
            UINT width,
            UINT height
        );

        IWICBitmap CreateBitmapFromMemory(
            UINT uiWidth,
            UINT uiHeight,
            [In] ref Guid pixelFormat,
            UINT cbStride,
            UINT cbBufferSize,
            byte[] pbBuffer
        );

        IWICBitmap CreateBitmapFromHBITMAP(HBITMAP hBitmap, HPALETTE hPalette, WICBitmapAlphaChannelOption options);

        IWICBitmap CreateBitmapFromHICON(HICON hIcon);

        IEnumUnknown CreateComponentEnumerator(DWORD componentTypes, DWORD options);

        IWICFastMetadataEncoder CreateFastMetadataEncoderFromDecoder(IWICBitmapDecoder pIDecoder);

        IWICFastMetadataEncoder CreateFastMetadataEncoderFromFrameDecode(IWICBitmapFrameDecode pIFrameDecoder);

        IWICMetadataQueryWriter CreateQueryWriter([In] ref Guid guidMetadataFormat, [In] ref Guid? pguidVendor);

        IWICMetadataQueryWriter CreateQueryWriterFromReader(IWICMetadataQueryReader pIQueryReader, [In] ref Guid? pguidVendor);
    }

    [ComImport, Guid("9edde9e7-8dee-47ea-99df-e6faf2ed44bf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWICBitmapDecoder
    {
        DWORD QueryCapability(IStream pIStream);

        void Initialize(IStream pIStream, WICDecodeOptions cacheOptions);

        Guid GetContainerFormat();

        IWICBitmapDecoderInfo GetDecoderInfo();

        IWICPalette CopyPalette();

        IWICMetadataQueryReader GetMetadataQueryReader();

        IWICBitmapSource GetPreview();

        UINT GetColorContexts(UINT cCount, IWICColorContext[] ppIColorContexts);

        IWICBitmapSource GetThumbnail();

        UINT GetFrameCount();

        IWICBitmapFrameDecode GetFrame(UINT index);
    }

    internal interface IWICBitmapDecoderInfo
    {
    }

    internal interface IWICBitmapEncoder
    {
    }

    [ComImport, Guid("3b16811b-6a43-4ec9-a813-3d930c13b940"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWICBitmapFrameDecode : IWICBitmapSource
    {
        void GetSize(out UINT puiWidth, out UINT puiHeight);

        Guid GetPixelFormat();

        void GetResolution(out double pDpiX, out double pDpiY);

        void CopyPalette(IWICPalette pIPalette);

        void CopyPixels(
            WICRect prc,
            uint cbStride,
            uint cbBufferSize,
            IntPtr pbBuffer
        );

        IWICMetadataQueryReader GetMetadataQueryReader();

        int GetColorContexts(UINT cCount, IWICColorContext[] ppIColorContexts, out UINT res);

        IWICBitmapSource GetThumbnail();
    }

    [ComImport, Guid("00000120-a8f2-4877-ba0a-fd2b6645fb94"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWICBitmapSource
    {
        void GetSize(out UINT puiWidth, out UINT puiHeight);

        Guid GetPixelFormat();

        void GetResolution(out double pDpiX, out double pDpiY);

        void CopyPalette(IWICPalette pIPalette);

        void CopyPixels(
            WICRect prc,
            uint cbStride,
            uint cbBufferSize,
            IntPtr pbBuffer
        );
    }

    internal interface IWICBitmap
    {
    }

    internal interface IWICBitmapClipper
    {
    }

    internal interface IWICBitmapFlipRotator
    {
    }

    internal interface IWICBitmapScaler
    {
    }

    internal interface IWICColorContext
    {
    }

    internal interface IWICColorTransform
    {
    }

    internal interface IWICPalette
    {
    }

    internal interface IWICStream
    {
    }

    [ComImport, Guid("00000301-a8f2-4877-ba0a-fd2b6645fb94"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IWICFormatConverter : IWICBitmapSource
    {
        void GetSize(out UINT puiWidth, out UINT puiHeight);

        Guid GetPixelFormat();

        void GetResolution(out double pDpiX, out double pDpiY);

        void CopyPalette(IWICPalette pIPalette);

        void CopyPixels(
            WICRect prc,
            uint cbStride,
            uint cbBufferSize,
            IntPtr pbBuffer
        );

        void Initialize(
            IWICBitmapSource pISource,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid dstFormat,
            WICBitmapDitherType dither,
            IWICPalette pIPalette,
            double alphaThresholdPercent,
            WICBitmapPaletteType paletteTranslate
        );

        WINBOOL CanConvert(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid srcPixelFormat,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid dstPixelFormat
        );
    }

    internal interface IWICMetadataQueryReader
    {
    }

    internal interface IWICComponentInfo
    {
    }

    internal interface IWICMetadataQueryWriter
    {
    }

    internal interface IWICFastMetadataEncoder
    {
    }

    internal interface IEnumUnknown
    {
    }

    internal enum WICDecodeOptions
    {
        WICDecodeMetadataCacheOnDemand = 0x0,
        WICDecodeMetadataCacheOnLoad = 0x1,
        WICMETADATACACHEOPTION_FORCE_DWORD = 0x7fffffff
    }

    internal enum WICBitmapCreateCacheOption
    {
        WICBitmapNoCache = 0x0,
        WICBitmapCacheOnDemand = 0x1,
        WICBitmapCacheOnLoad = 0x2,
        WICBITMAPCREATECACHEOPTION_FORCE_DWORD = 0x7fffffff
    }

    internal enum WICBitmapAlphaChannelOption
    {
        WICBitmapUseAlpha = 0x0,
        WICBitmapUsePremultipliedAlpha = 0x1,
        WICBitmapIgnoreAlpha = 0x2,
        WICBITMAPALPHACHANNELOPTIONS_FORCE_DWORD = 0x7fffffff
    }

    internal enum WICBitmapDitherType
    {
        WICBitmapDitherTypeNone = 0x0,
        WICBitmapDitherTypeSolid = 0x0,
        WICBitmapDitherTypeOrdered4x4 = 0x1,
        WICBitmapDitherTypeOrdered8x8 = 0x2,
        WICBitmapDitherTypeOrdered16x16 = 0x3,
        WICBitmapDitherTypeSpiral4x4 = 0x4,
        WICBitmapDitherTypeSpiral8x8 = 0x5,
        WICBitmapDitherTypeDualSpiral4x4 = 0x6,
        WICBitmapDitherTypeDualSpiral8x8 = 0x7,
        WICBitmapDitherTypeErrorDiffusion = 0x8,
        WICBITMAPDITHERTYPE_FORCE_DWORD = 0x7fffffff
    }

    enum WICBitmapPaletteType
    {
        WICBitmapPaletteTypeCustom = 0x0,
        WICBitmapPaletteTypeMedianCut = 0x1,
        WICBitmapPaletteTypeFixedBW = 0x2,
        WICBitmapPaletteTypeFixedHalftone8 = 0x3,
        WICBitmapPaletteTypeFixedHalftone27 = 0x4,
        WICBitmapPaletteTypeFixedHalftone64 = 0x5,
        WICBitmapPaletteTypeFixedHalftone125 = 0x6,
        WICBitmapPaletteTypeFixedHalftone216 = 0x7,
        WICBitmapPaletteTypeFixedWebPalette = WICBitmapPaletteTypeFixedHalftone216,
        WICBitmapPaletteTypeFixedHalftone252 = 0x8,
        WICBitmapPaletteTypeFixedHalftone256 = 0x9,
        WICBitmapPaletteTypeFixedGray4 = 0xa,
        WICBitmapPaletteTypeFixedGray16 = 0xb,
        WICBitmapPaletteTypeFixedGray256 = 0xc,
        WICBITMAPPALETTETYPE_FORCE_DWORD = 0x7fffffff
    }

    internal enum WICFileAccessMask : uint
    {
        GENERIC_READ = 0x80000000U,
        GENERIC_WRITE = 0x40000000U
    }

    internal static class WICPixelFormat
    {
        public static readonly Guid GUID_WICPixelFormat32bppPBGRA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x10);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class WICRect
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public WICRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}