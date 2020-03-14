using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    internal class D2D1_FACTORY_OPTIONS
    {
        private D2D1_FACTORY_OPTIONS(D2D1_DEBUG_LEVEL debugLevel)
        {
            this.debugLevel = debugLevel;
        }

        private readonly D2D1_DEBUG_LEVEL debugLevel;

        public static D2D1_FACTORY_OPTIONS Create(D2D1_DEBUG_LEVEL debugLevel)
        {
            return new D2D1_FACTORY_OPTIONS(debugLevel);
        }
    }
}