using System;
using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    using HRESULT = System.Int32;

    internal static class D2D1Api
    {
        [DllImport("d2d1.dll")]
        public static extern HRESULT D2D1CreateFactory
        (
            D2D1_FACTORY_TYPE factoryType,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [In] D2D1_FACTORY_OPTIONS pFactoryOptions,
            out ID2D1Factory factory
        );
    }
}