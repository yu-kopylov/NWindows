using System;
using System.Drawing;

namespace NWindows.Win32.DirectX
{
    using HWND = IntPtr;

    internal struct D2D1_HWND_RENDER_TARGET_PROPERTIES
    {
        private D2D1_HWND_RENDER_TARGET_PROPERTIES(HWND hwnd, D2D1_SIZE_U pixelSize, D2D1_PRESENT_OPTIONS presentOptions)
        {
            this.hwnd = hwnd;
            this.pixelSize = pixelSize;
            this.presentOptions = presentOptions;
        }

        private readonly HWND hwnd;
        private readonly D2D1_SIZE_U pixelSize;
        private readonly D2D1_PRESENT_OPTIONS presentOptions;

        public static D2D1_HWND_RENDER_TARGET_PROPERTIES Create(HWND hwnd, Size pixelSize)
        {
            return new D2D1_HWND_RENDER_TARGET_PROPERTIES(hwnd, D2D1_SIZE_U.FromSize(pixelSize), D2D1_PRESENT_OPTIONS.D2D1_PRESENT_OPTIONS_NONE);
        }
    }
}