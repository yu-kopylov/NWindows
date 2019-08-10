using System;
using System.Drawing;

namespace NWindows.X11
{
    internal class X11Canvas : ICanvas, IDisposable
    {
        private readonly IntPtr display;
        private readonly ulong pictureId;

        private X11Canvas(IntPtr display, ulong pictureId)
        {
            this.display = display;
            this.pictureId = pictureId;
        }

        public static X11Canvas CreateForWindow(IntPtr display, IntPtr pictFormatPtr, ulong windowId)
        {
            XRenderPictureAttributes attr = new XRenderPictureAttributes();
            ulong pictureId = LibXRender.XRenderCreatePicture
            (
                display,
                windowId,
                pictFormatPtr,
                0,
                ref attr
            );
            return new X11Canvas(display, pictureId);
        }

        public void Dispose()
        {
            LibXRender.XRenderFreePicture(display, pictureId);
        }

        public void FillRectangle(Color color, int x, int y, int width, int height)
        {
            if (width < 0 || height < 0)
            {
                return;
            }

            XRenderColor xColor = new XRenderColor(color);
            LibXRender.XRenderFillRectangle(display, PictOp.PictOpOver, pictureId, ref xColor, x, y, (uint) width, (uint) height);
        }
    }
}