using System;
using System.Threading.Tasks;

namespace NWindows.Win32
{
    internal static class PixelConverter
    {
        public static void ConvertArgb32ToPArgb32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => ConvertArgb32ToPArgb32(source + y * sourceStride, dest + y * destStride, width));
        }

        public static void ConvertPArgb32ToArgb32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => ConvertPArgb32ToArgb32(source + y * sourceStride, dest + y * destStride, width));
        }

        private static unsafe void ConvertArgb32ToPArgb32(IntPtr source, IntPtr dest, int width)
        {
            uint* sourcePtr = (uint*) source.ToPointer();
            uint* destPtr = (uint*) dest.ToPointer();
            for (int i = 0; i < width; i++, sourcePtr++, destPtr++)
            {
                uint c = *sourcePtr;
                byte a = (byte) (c >> 24);
                if (a == 0xFF)
                {
                    *destPtr = c;
                }
                else if (a == 0)
                {
                    *destPtr = 0;
                }
                else
                {
                    // todo: add rounding
                    byte r = (byte) (c >> 16);
                    r = (byte) (r * a / 255);

                    byte g = (byte) (c >> 8);
                    g = (byte) (g * a / 255);

                    byte b = (byte) c;
                    b = (byte) (b * a / 255);

                    *destPtr = (uint) ((a << 24) | (r << 16) | (g << 8) | b);
                }
            }
        }

        private static unsafe void ConvertPArgb32ToArgb32(IntPtr source, IntPtr dest, int width)
        {
            uint* sourcePtr = (uint*) source.ToPointer();
            uint* destPtr = (uint*) dest.ToPointer();
            for (int i = 0; i < width; i++, sourcePtr++, destPtr++)
            {
                uint c = *sourcePtr;
                byte a = (byte) (c >> 24);
                if (a == 0xFF)
                {
                    *destPtr = c;
                }
                else if (a == 0)
                {
                    // todo: should it be 0x00000000 or 0xFF000000
                    *destPtr = 0;
                }
                else
                {
                    // todo: add rounding
                    byte r = (byte) (c >> 16);
                    r = r >= a ? (byte) 0xFF : (byte) ((r * 255 + 127) / a);

                    byte g = (byte) (c >> 8);
                    g = g >= a ? (byte) 0xFF : (byte) ((g * 255 + 127) / a);

                    byte b = (byte) c;
                    b = b >= a ? (byte) 0xFF : (byte) ((b * 255 + 127) / a);

                    *destPtr = (uint) ((a << 24) | (r << 16) | (g << 8) | b);
                }
            }
        }
    }
}