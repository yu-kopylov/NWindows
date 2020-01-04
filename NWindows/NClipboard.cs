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

        public bool GetText(out string text)
        {
            return nativeClipboard.GetText(out text);
        }
    }
}