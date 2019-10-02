using System;

namespace NWindows.X11
{
    public struct XftRange
    {
        private readonly IntPtr font;
        private readonly int start;
        private readonly int end;

        public XftRange(IntPtr font, int start, int end)
        {
            this.font = font;
            this.start = start;
            this.end = end;
        }

        public IntPtr Font
        {
            get { return font; }
        }

        public int Start
        {
            get { return start; }
        }

        public int End
        {
            get { return end; }
        }
    }
}