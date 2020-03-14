using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    using D2D1_TAG = System.UInt64;

    [ComImport, Guid("2cd90694-12e2-11dc-9fed-001143a055f9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1RenderTarget : ID2D1Resource
    {
        void __ID2D1Resource__GetFactory();

        void __stub__CreateBitmap();
        void __Stub__CreateBitmapFromWicBitmap();
        void __Stub__CreateSharedBitmap();
        void __Stub__CreateBitmapBrush();
        ID2D1SolidColorBrush CreateSolidColorBrush([In] ref D2D1_COLOR_F color, [In] D2D1_BRUSH_PROPERTIES brushProperties);
        void __Stub__CreateGradientStopCollection();
        void __Stub__CreateLinearGradientBrush();
        void __Stub__CreateRadialGradientBrush();
        void __Stub__CreateCompatibleRenderTarget();
        void __Stub__CreateLayer();
        void __Stub__CreateMesh();
        void __Stub__DrawLine();
        void __Stub__DrawRectangle();
        [PreserveSig] void FillRectangle([In] ref D2D1_RECT_F rect, ID2D1Brush brush);
        void __Stub__DrawRoundedRectangle();
        void __Stub__FillRoundedRectangle();
        void __Stub__DrawEllipse();
        void __Stub__FillEllipse();
        void __Stub__DrawGeometry();
        void __Stub__FillGeometry();
        void __Stub__FillMesh();
        void __Stub__FillOpacityMask();
        void __Stub__DrawBitmap();
        void __Stub__DrawText();
        void __Stub__DrawTextLayout();
        void __Stub__DrawGlyphRun();
        [PreserveSig] void SetTransform([In] ref D2D1_MATRIX_3X2_F transform);
        void __Stub__GetTransform();
        void __Stub__SetAntialiasMode();
        void __Stub__GetAntialiasMode();
        void __Stub__SetTextAntialiasMode();
        void __Stub__GetTextAntialiasMode();
        void __Stub__SetTextRenderingParams();
        void __Stub__GetTextRenderingParams();
        void __Stub__SetTags();
        void __Stub__GetTags();
        void __Stub__PushLayer();
        void __Stub__PopLayer();
        void __Stub__Flush();
        void __Stub__SaveDrawingState();
        void __Stub__RestoreDrawingState();
        void __Stub__PushAxisAlignedClip();
        void __Stub__PopAxisAlignedClip();
        [PreserveSig] void Clear([In] ref D2D1_COLOR_F clearColor);
        [PreserveSig] void BeginDraw();
        void EndDraw(out D2D1_TAG tag1, out D2D1_TAG tag2);
        void __Stub__GetPixelFormat();
        void __Stub__SetDpi();
        void __Stub__GetDpi();
        void __Stub__GetSize();
        void __Stub__GetPixelSize();
        void __Stub__GetMaximumBitmapSize();
        void __Stub__IsSupported();
    }
}