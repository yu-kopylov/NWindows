using System.Collections.Generic;

namespace NWindows.X11
{
    public class X11FontConfigComparer : IEqualityComparer<FontConfig>
    {
        public static IEqualityComparer<FontConfig> Instance { get; } = new X11FontConfigComparer();

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
                   && x.IsItalic == y.IsItalic;
        }

        public int GetHashCode(FontConfig obj)
        {
            unchecked
            {
                var hashCode = (obj.FontFamily != null ? obj.FontFamily.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Size.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsBold.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsItalic.GetHashCode();
                return hashCode;
            }
        }
    }
}