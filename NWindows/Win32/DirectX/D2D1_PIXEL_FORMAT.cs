namespace NWindows.Win32.DirectX
{
    internal struct D2D1_PIXEL_FORMAT
    {
        public static D2D1_PIXEL_FORMAT Unknown { get; } = new D2D1_PIXEL_FORMAT(DXGI_FORMAT.DXGI_FORMAT_UNKNOWN, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_UNKNOWN);

        private D2D1_PIXEL_FORMAT(DXGI_FORMAT format, D2D1_ALPHA_MODE alphaMode)
        {
            this.format = format;
            this.alphaMode = alphaMode;
        }

        private readonly DXGI_FORMAT format;
        private readonly D2D1_ALPHA_MODE alphaMode;
    }
}