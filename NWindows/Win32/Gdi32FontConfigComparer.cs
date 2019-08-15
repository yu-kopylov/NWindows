using System.Collections.Generic;

namespace NWindows.Win32
{
    public class Gdi32FontConfigComparer : IEqualityComparer<FontConfig>
    {
        public static IEqualityComparer<FontConfig> Instance { get; } = new Gdi32FontConfigComparer();

        public bool Equals(FontConfig x, FontConfig y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return string.Equals(x.FontFamily, y.FontFamily)
                   && x.Size.Equals(y.Size)
                   && x.IsBold == y.IsBold
                   && x.IsItalic == y.IsItalic
                   && x.IsUnderline == y.IsUnderline
                   && x.IsStrikeout == y.IsStrikeout;
        }

        public int GetHashCode(FontConfig obj)
        {
            unchecked
            {
                var hashCode = (obj.FontFamily != null ? obj.FontFamily.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Size.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsBold.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsItalic.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsUnderline.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsStrikeout.GetHashCode();
                return hashCode;
            }
        }
    }
}