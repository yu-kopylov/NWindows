using System;
using System.Drawing;

namespace NWindows.Win32.DirectX
{
    internal struct D2D1_SIZE_U
    {
        private D2D1_SIZE_U(uint width, uint height)
        {
            this.width = width;
            this.height = height;
        }

        private readonly UInt32 width;
        private readonly UInt32 height;

        public static D2D1_SIZE_U FromSize(Size size)
        {
            // todo: check conversion
            return new D2D1_SIZE_U((uint) size.Width, (uint) size.Height);
        }
    }
}