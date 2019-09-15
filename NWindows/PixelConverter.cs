using System;
using System.Threading.Tasks;

namespace NWindows
{
    internal static class PixelConverter
    {
        public static void Convert_ARGB_32_To_PARGB_32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => Convert_ARGB_32_To_PARGB_32(source + y * sourceStride, dest + y * destStride, width));
        }

        public static void Convert_PARGB_32_To_ARGB_32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => Convert_PARGB_32_To_ARGB_32(source + y * sourceStride, dest + y * destStride, width));
        }

        public static void Convert_RGBA_32BE_To_ARGB_32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => Convert_RGBA_32BE_To_ARGB_32(source + y * sourceStride, dest + y * destStride, width));
        }

        public static void Convert_RGBA_32BE_To_PARGB_32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => Convert_RGBA_32BE_To_PARGB_32(source + y * sourceStride, dest + y * destStride, width));
        }

        public static void Convert_RGB_24BE_To_ARGB_32(IntPtr source, int sourceStride, IntPtr dest, int destStride, int width, int height)
        {
            Parallel.For(0, height, y => Convert_RGB_24BE_To_ARGB_32(source + y * sourceStride, dest + y * destStride, width));
        }

        private static unsafe void Convert_ARGB_32_To_PARGB_32(IntPtr source, IntPtr dest, int width)
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

        private static unsafe void Convert_PARGB_32_To_ARGB_32(IntPtr source, IntPtr dest, int width)
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

        private static unsafe void Convert_RGBA_32BE_To_ARGB_32(IntPtr source, IntPtr dest, int width)
        {
            byte* sourcePtr = (byte*) source.ToPointer();
            uint* destPtr = (uint*) dest.ToPointer();
            for (int i = 0; i < width; i++, destPtr++)
            {
                byte r = *sourcePtr++;
                byte g = *sourcePtr++;
                byte b = *sourcePtr++;
                byte a = *sourcePtr++;

                *destPtr = (uint) ((a << 24) | (r << 16) | (g << 8) | b);
            }
        }

        private static unsafe void Convert_RGBA_32BE_To_PARGB_32(IntPtr source, IntPtr dest, int width)
        {
            byte* sourcePtr = (byte*) source.ToPointer();
            uint* destPtr = (uint*) dest.ToPointer();
            for (int i = 0; i < width; i++, destPtr++)
            {
                byte r = *sourcePtr++;
                byte g = *sourcePtr++;
                byte b = *sourcePtr++;
                byte a = *sourcePtr++;

                if (a == 0)
                {
                    *destPtr = 0;
                }
                else
                {
                    r = (byte) (r * a / 255);
                    g = (byte) (g * a / 255);
                    b = (byte) (b * a / 255);

                    *destPtr = (uint) ((a << 24) | (r << 16) | (g << 8) | b);
                }
            }
        }

        private static unsafe void Convert_RGB_24BE_To_ARGB_32(IntPtr source, IntPtr dest, int width)
        {
            byte* sourcePtr = (byte*) source.ToPointer();
            uint* destPtr = (uint*) dest.ToPointer();
            for (int i = 0; i < width; i++, destPtr++)
            {
                byte r = *sourcePtr++;
                byte g = *sourcePtr++;
                byte b = *sourcePtr++;

                *destPtr = (uint) (0xFF000000 | (r << 16) | (g << 8) | b);
            }
        }
    }
}