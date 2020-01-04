using NWindows.NativeApi;

namespace NWindows.X11
{
    public class X11Clipboard : INativeClipboard
    {
        public bool GetText(out string text)
        {
            // todo: implement
            text = null;
            return false;
        }
    }
}