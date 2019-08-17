using System;
using System.Collections.Generic;
using System.Drawing;

namespace NWindows.Win32
{
    internal class Gdi32ObjectCache : IDisposable
    {
        private readonly Dictionary<uint, IntPtr> pens = new Dictionary<uint, IntPtr>();
        private readonly Dictionary<uint, IntPtr> brushes = new Dictionary<uint, IntPtr>();
        private readonly Dictionary<FontConfig, IntPtr> fonts = new Dictionary<FontConfig, IntPtr>(Gdi32FontConfigComparer.Instance);

        public void Dispose()
        {
            foreach (IntPtr penPtr in pens.Values)
            {
                Gdi32API.DeleteObject(penPtr);
            }

            foreach (IntPtr brushPtr in brushes.Values)
            {
                Gdi32API.DeleteObject(brushPtr);
            }

            foreach (IntPtr fontPtr in fonts.Values)
            {
                Gdi32API.DeleteObject(fontPtr);
            }
        }

        public IntPtr GetSolidPen(Color color)
        {
            uint cref = Gdi32API.ToCOLORREF(color);
            if (pens.TryGetValue(cref, out IntPtr pen))
            {
                return pen;
            }

            pen = Gdi32API.CreatePen(GdiPenStyle.PS_SOLID, 0, cref);
            pens.Add(cref, pen);
            return pen;
        }

        public IntPtr GetSolidBrush(Color color)
        {
            uint cref = Gdi32API.ToCOLORREF(color);
            if (brushes.TryGetValue(cref, out IntPtr brush))
            {
                return brush;
            }

            brush = Gdi32API.CreateSolidBrush(cref);
            brushes.Add(cref, brush);
            return brush;
        }
        
        public IntPtr GetFont(FontConfig font)
        {
            const uint DEFAULT_CHARSET = 1;
            const uint OUT_DEFAULT_PRECIS = 0;
            const uint CLIP_DEFAULT_PRECIS = 0;
            const uint CLEARTYPE_QUALITY = 5;
            const uint DEFAULT_PITCH = 0;

            if (fonts.TryGetValue(font, out IntPtr fontPtr))
            {
                return fontPtr;
            }

            fontPtr = Gdi32API.CreateFontW(
                //todo: use int?
                -Convert.ToInt32(font.Size),
                0, 0, 0,
                font.IsBold ? 700 : 400,
                font.IsItalic ? 1u : 0,
                font.IsUnderline ? 1u : 0,
                font.IsStrikeout ? 1u : 0,
                DEFAULT_CHARSET,
                OUT_DEFAULT_PRECIS,
                CLIP_DEFAULT_PRECIS,
                CLEARTYPE_QUALITY,
                DEFAULT_PITCH,
                font.FontFamily
            );

            if (fontPtr == IntPtr.Zero)
            {
                throw new InvalidOperationException($"Failed to create font: '{font.FontFamily}', {font.Size:0.0}.");
            }

            fonts.Add(font, fontPtr);

            return fontPtr;
        }
    }
}