using System.Runtime.InteropServices;

namespace NWindows.Win32.DirectX
{
    using FLOAT = System.Single;

    [StructLayout(LayoutKind.Sequential)]
    internal class D2D1_BRUSH_PROPERTIES
    {
        public D2D1_BRUSH_PROPERTIES(FLOAT opacity, D2D1_MATRIX_3X2_F transform)
        {
            this.opacity = opacity;
            this.transform = transform;
        }

        private readonly FLOAT opacity;
        private readonly D2D1_MATRIX_3X2_F transform;

        public static D2D1_BRUSH_PROPERTIES Create(float opacity)
        {
            return new D2D1_BRUSH_PROPERTIES(opacity, D2D1_MATRIX_3X2_F.Identity);
        }
    }
}