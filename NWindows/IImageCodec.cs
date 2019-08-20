using NWindows.X11;

namespace NWindows
{
    public interface IImageCodec
    {
        IImage LoadFromFile(string filename);
    }
}