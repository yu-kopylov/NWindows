using System.Drawing;

namespace NWindows.Win32.DirectX
{
    using FLOAT = System.Single;

    internal struct D2D1_RECT_F
    {
        public D2D1_RECT_F(Rectangle rect)
        {
            this.left = rect.Left;
            this.top = rect.Top;
            this.right = rect.Right;
            this.bottom = rect.Bottom;
        }

        private readonly FLOAT left;
        private readonly FLOAT top;
        private readonly FLOAT right;
        private readonly FLOAT bottom;
    }
}