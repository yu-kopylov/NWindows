using System;
using System.Drawing;

namespace NWindows.Win32
{
    public class Win32Canvas : ICanvas
    {
        private readonly IntPtr hdc;

        public Win32Canvas(IntPtr hdc)
        {
            this.hdc = hdc;
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            // todo: choose W32/GDI/GDI+ method
//            FillRectangleW32(color, x, y, width, height);
//            FillRectangleGDI(color, x, y, width, height);
            FillRectangleGDIPlus(color, x, y, width, height);
        }

        private void FillRectangleW32(Color color, int x, int y, int width, int height)
        {
            // todo: check that width and height are exact
            RECT rect = new RECT {left = x, top = y, right = x + width, bottom = y + height};
            var brush = Gdi32API.CreateSolidBrush(ToCOLORREF(color));
            Win32API.FillRect(hdc, ref rect, brush);
            Gdi32API.DeleteObject(brush);
        }

        private void FillRectangleGDI(Color color, int x, int y, int width, int height)
        {
            var brush = Gdi32API.CreateSolidBrush(ToCOLORREF(color));
            var pen = Gdi32API.CreatePen(GdiPenStyle.PS_SOLID, 0, ToCOLORREF(color));

            var originalBrush = Gdi32API.SelectObject(hdc, brush);
            var originalPen = Gdi32API.SelectObject(hdc, pen);

            // todo: check that width and height are exact
            Gdi32API.Rectangle(hdc, x, y, x + width, y + height);

            Gdi32API.SelectObject(hdc, originalBrush);
            Gdi32API.SelectObject(hdc, originalPen);

            Gdi32API.DeleteObject(brush);
            Gdi32API.DeleteObject(pen);
        }

        private uint ToCOLORREF(Color color)
        {
            return (uint) (color.B << 16 | color.G << 8 | color.R);
        }

        private void FillRectangleGDIPlus(Color color, int x, int y, int width, int height)
        {
            GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateFromHDC(hdc, out var graphics));
            try
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipCreateSolidFill(color.ToArgb(), out var brush));
                try
                {
                    GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipFillRectangleI(graphics, brush, x, y, width, height));
                }
                finally
                {
                    GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteBrush(brush));
                }
            }
            finally
            {
                GdiPlusAPI.CheckStatus(GdiPlusAPI.GdipDeleteGraphics(graphics));
            }
        }
    }
}