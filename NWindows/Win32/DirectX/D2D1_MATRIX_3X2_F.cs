namespace NWindows.Win32.DirectX
{
    using FLOAT = System.Single;

    internal struct D2D1_MATRIX_3X2_F
    {
        public static D2D1_MATRIX_3X2_F Identity { get; } = new D2D1_MATRIX_3X2_F(1f, 0f, 0f, 1f, 0f, 0f);

        private D2D1_MATRIX_3X2_F(float m11, float m12, float m21, float m22, float dx, float dy)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.dx = dx;
            this.dy = dy;
        }

        private readonly FLOAT m11;
        private readonly FLOAT m12;
        private readonly FLOAT m21;
        private readonly FLOAT m22;
        private readonly FLOAT dx;
        private readonly FLOAT dy;
    }
}