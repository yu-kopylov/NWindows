using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    [ComImport, Guid("2cd906a9-12e2-11dc-9fed-001143a055f9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1SolidColorBrush : ID2D1Brush
    {
        void __ID2D1Brush__ID2D1Resource__GetFactory();

        void __ID2D1Brush__SetOpacity();
        void __ID2D1Brush__SetTransform();
        void __ID2D1Brush__GetOpacity();
        void __ID2D1Brush__GetTransform();

        [PreserveSig] void SetColor([In] ref D2D1_COLOR_F color);
        [PreserveSig] D2D1_COLOR_F GetColor();
    }
}