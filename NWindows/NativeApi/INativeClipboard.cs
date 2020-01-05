namespace NWindows.NativeApi
{
    public interface INativeClipboard
    {
        bool TryGetText(out string text);
    }
}