using System;
using NWindows.NativeApi;

namespace NWindows
{
    public class NClipboard
    {
        private readonly INativeClipboard nativeClipboard;

        internal NClipboard(INativeClipboard nativeClipboard)
        {
            this.nativeClipboard = nativeClipboard;
        }

        public void PutText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            nativeClipboard.PutText(text);
        }

        public bool TryGetText(out string text)
        {
            return nativeClipboard.TryGetText(out text);
        }
    }
}