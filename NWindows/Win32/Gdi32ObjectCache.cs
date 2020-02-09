using System;
using System.Collections.Generic;
using System.Drawing;
using NWindows.Utils;

namespace NWindows.Win32
{
    internal class Gdi32ObjectCache
    {
        private readonly Cache<uint, IntPtr> pens;
        private readonly Cache<uint, IntPtr> brushes;
        private readonly Cache<FontConfig, IntPtr> fonts;

        public Gdi32ObjectCache()
        {
            pens = new Cache<uint, IntPtr>(64, CreateSolidPen, GdiDeleteObject, EqualityComparer<uint>.Default);
            brushes = new Cache<uint, IntPtr>(64, CreateSolidBrush, GdiDeleteObject, EqualityComparer<uint>.Default);
            fonts = new Cache<FontConfig, IntPtr>(64, CreateFont, GdiDeleteObject, Gdi32FontConfigComparer.Instance);
        }

        public void Clear()
        {
            pens.Clear();
            brushes.Clear();
            fonts.Clear();
        }

        private void GdiDeleteObject(IntPtr ptr)
        {
            Gdi32API.DeleteObject(ptr);
        }

        public IntPtr GetSolidPen(Color color)
        {
            uint cref = Gdi32API.ToCOLORREF(color);
            return pens.Get(cref);
        }

        private IntPtr CreateSolidPen(uint color)
        {
            return Gdi32API.CreatePen(GdiPenStyle.PS_SOLID, 0, color);
        }

        public IntPtr GetSolidBrush(Color color)
        {
            uint cref = Gdi32API.ToCOLORREF(color);
            return brushes.Get(cref);
        }

        private IntPtr CreateSolidBrush(uint color)
        {
            return Gdi32API.CreateSolidBrush(color);
        }

        public IntPtr GetFont(FontConfig font)
        {
            return fonts.Get(font);
        }

        private IntPtr CreateFont(FontConfig font)
        {
            const uint DEFAULT_CHARSET = 1;
            const uint OUT_DEFAULT_PRECIS = 0;
            const uint CLIP_DEFAULT_PRECIS = 0;
            const uint CLEARTYPE_QUALITY = 5;
            const uint DEFAULT_PITCH = 0;

            IntPtr fontPtr = Gdi32API.CreateFontW(
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

            return fontPtr;
        }
    }
}