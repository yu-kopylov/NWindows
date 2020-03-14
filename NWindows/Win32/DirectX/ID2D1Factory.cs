using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    [ComImport, Guid("06152247-6f50-465a-9245-118bfd3b6007"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Factory
    {
        void __Stub__ReloadSystemMetrics();
        void __Stub__GetDesktopDpi();
        void __Stub__CreateRectangleGeometry();
        void __Stub__CreateRoundedRectangleGeometry();
        void __Stub__CreateEllipseGeometry();
        void __Stub__CreateGeometryGroup();
        void __Stub__CreateTransformedGeometry();
        void __Stub__CreatePathGeometry();
        void __Stub__CreateStrokeStyle();
        void __Stub__CreateDrawingStateBlock();
        void __Stub__CreateWicBitmapRenderTarget();
        ID2D1RenderTarget CreateHwndRenderTarget([In] ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, [In] ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties);
        void __Stub__CreateDxgiSurfaceRenderTarget();
        void __Stub__CreateDCRenderTarget();
    }
}