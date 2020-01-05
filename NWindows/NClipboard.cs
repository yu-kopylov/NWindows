using NWindows.NativeApi;

namespace NWindows
{
    public class NClipboard
    {
        private readonly INativeClipboard nativeClipboard;

        public NClipboard(INativeClipboard nativeClipboard)
        {
            this.nativeClipboard = nativeClipboard;
        }

        public bool TryGetText(out string text)
        {
            return nativeClipboard.TryGetText(out text);
        }
    }
}