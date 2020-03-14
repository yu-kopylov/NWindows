using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    [ComImport, Guid("2cd90691-12e2-11dc-9fed-001143a055f9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Resource
    {
        [PreserveSig] ID2D1Factory GetFactory();
    }
}