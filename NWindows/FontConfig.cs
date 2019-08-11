namespace NWindows
{
    public class FontConfig
    {
        public FontConfig(string fontFamily, float size)
        {
            this.FontFamily = fontFamily;
            this.Size = size;
        }

        private FontConfig(string fontFamily, float size, bool bold, bool italic, bool underline, bool strikeout)
        {
            FontFamily = fontFamily;
            Size = size;
            IsBold = bold;
            IsItalic = italic;
            IsUnderline = underline;
            IsStrikeout = strikeout;
        }

        public string FontFamily { get; }

        public float Size { get; }

        public bool IsBold { get; }

        public bool IsItalic { get; }

        public bool IsUnderline { get; }

        public bool IsStrikeout { get; }

        public FontConfig Bold()
        {
            return new FontConfig(FontFamily, Size, true, IsItalic, IsUnderline, IsStrikeout);
        }

        public FontConfig Italic()
        {
            return new FontConfig(FontFamily, Size, IsBold, true, IsUnderline, IsStrikeout);
        }

        public FontConfig Underline()
        {
            return new FontConfig(FontFamily, Size, IsBold, IsItalic, true, IsStrikeout);
        }

        public FontConfig Strikeout()
        {
            return new FontConfig(FontFamily, Size, IsBold, IsItalic, IsUnderline, true);
        }
    }
}