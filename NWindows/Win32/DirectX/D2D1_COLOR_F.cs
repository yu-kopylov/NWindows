namespace NWindows.Win32.DirectX
{
    internal struct D2D1_COLOR_F
    {
        private D2D1_COLOR_F(float a, float r, float g, float b)
        {
            this.a = a;
            this.r = r;
            this.g = g;
            this.b = b;
        }

        private readonly float r;
        private readonly float g;
        private readonly float b;
        private readonly float a;

        public static D2D1_COLOR_F FromArgb(uint argb)
        {
            float a = ((argb >> 24) & 0xFF) / 255.0f;
            float r = ((argb >> 16) & 0xFF) / 255.0f;
            float g = ((argb >> 8) & 0xFF) / 255.0f;
            float b = (argb & 0xFF) / 255.0f;
            return new D2D1_COLOR_F(a, r, g, b);
        }
    }
}