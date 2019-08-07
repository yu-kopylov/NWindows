using System;

namespace NWindows.X11
{
    public class X11Window : INativeWindow
    {
        private readonly IntPtr display;
        private readonly IntPtr pictFormatPtr;
        private readonly ulong windowId;

        public X11Window(IntPtr display, IntPtr pictFormatPtr, ulong windowId)
        {
            this.display = display;
            this.pictFormatPtr = pictFormatPtr;
            this.windowId = windowId;
        }

        public ICanvas CreateCanvas()
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
    }
}