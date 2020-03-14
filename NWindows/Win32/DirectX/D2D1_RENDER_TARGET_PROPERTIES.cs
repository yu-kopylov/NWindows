namespace NWindows.Win32.DirectX
{
    using FLOAT = System.Single;

    internal struct D2D1_RENDER_TARGET_PROPERTIES
    {
        public static D2D1_RENDER_TARGET_PROPERTIES Default { get; } = new D2D1_RENDER_TARGET_PROPERTIES
        (
            D2D1_RENDER_TARGET_TYPE.D2D1_RENDER_TARGET_TYPE_DEFAULT,
            D2D1_PIXEL_FORMAT.Unknown,
            0f,
            0f,
            D2D1_RENDER_TARGET_USAGE.D2D1_RENDER_TARGET_USAGE_NONE,
            D2D1_FEATURE_LEVEL.D2D1_FEATURE_LEVEL_DEFAULT
        );

        private D2D1_RENDER_TARGET_PROPERTIES(
            D2D1_RENDER_TARGET_TYPE type,
            D2D1_PIXEL_FORMAT pixelFormat,
            float dpiX,
            float dpiY,
            D2D1_RENDER_TARGET_USAGE usage,
            D2D1_FEATURE_LEVEL minLevel
        )
        {
            this.type = type;
            this.pixelFormat = pixelFormat;
            this.dpiX = dpiX;
            this.dpiY = dpiY;
            this.usage = usage;
            this.minLevel = minLevel;
        }

        private readonly D2D1_RENDER_TARGET_TYPE type;
        private readonly D2D1_PIXEL_FORMAT pixelFormat;
        private readonly FLOAT dpiX;
        private readonly FLOAT dpiY;
        private readonly D2D1_RENDER_TARGET_USAGE usage;
        private readonly D2D1_FEATURE_LEVEL minLevel;
    }
}