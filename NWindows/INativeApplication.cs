namespace NWindows
{
    internal interface INativeApplication
    {
        IImageCodec ImageCodec { get; }
        void Run(BasicWindow window);
    }
}