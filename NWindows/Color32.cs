using System.Drawing;
using System.Runtime.InteropServices;

namespace NWindows
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct Color32
    {
        [FieldOffset(0)] private readonly uint argb;

        private Color32(uint argb)
        {
            this.argb = argb;
        }

        public byte A => (byte) (argb >> 24);
        public byte R => (byte) (argb >> 16);
        public byte G => (byte) (argb >> 8);
        public byte B => (byte) argb;

        public static Color32 FromARGB(byte a, byte r, byte g, byte b)
        {
            return new Color32((uint) (a << 24 | r << 16 | g << 8 | b));
        }

        public Color ToColor()
        {
            return Color.FromArgb((int) argb);
        }

        public static Color32 FromColor(Color color)
        {
            return new Color32((uint) color.ToArgb());
        }
    }
}