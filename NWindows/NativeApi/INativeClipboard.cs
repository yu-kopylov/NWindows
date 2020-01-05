namespace NWindows.NativeApi
{
    public interface INativeClipboard
    {
        void PutText(string text);
        bool TryGetText(out string text);
    }
}