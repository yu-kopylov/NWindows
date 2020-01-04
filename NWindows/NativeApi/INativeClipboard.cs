namespace NWindows.NativeApi
{
    public interface INativeClipboard
    {
        bool GetText(out string text);
    }
}