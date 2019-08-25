namespace NWindows
{
    public interface IImageCodec
    {
        IImage LoadFromFile(string filename);
    }
}