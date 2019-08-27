namespace NWindows
{
    internal interface INativeApplication
    {
        IImageCodec ImageCodec { get; }

        void Init();
        void Run(BasicWindow window);
    }
}