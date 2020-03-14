using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    using FLOAT = System.Single;

    [ComImport, Guid("2cd906a8-12e2-11dc-9fed-001143a055f9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Brush : ID2D1Resource
    {
        void __ID2D1Resource__GetFactory();

        [PreserveSig] void SetOpacity(FLOAT opacity);
        [PreserveSig] void SetTransform([In] ref D2D1_MATRIX_3X2_F transform);
        [PreserveSig] FLOAT GetOpacity();
        [PreserveSig] void GetTransform(out D2D1_MATRIX_3X2_F transform);
    }
}