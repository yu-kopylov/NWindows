using System;
using System.Drawing;
using System.Text;

namespace NWindows.X11
{
    public class X11Window : INativeWindow
    {
        private readonly IntPtr display;
        private readonly ulong windowId;

        public X11Window(IntPtr display, ulong windowId)
        {
            this.display = display;
            this.windowId = windowId;
        }

        public void SetTitle(string title)
        {
            byte[] windowTitleASCII = Encoding.ASCII.GetBytes(title);
            LibX11.XChangeProperty(
                display,
                windowId,
                LibX11.XInternAtom(display, "WM_NAME", 0),
                LibX11.XInternAtom(display, "STRING", 0),
                XChangePropertyFormat.Byte,
                XChangePropertyMode.PropModeReplace,
                windowTitleASCII,
                windowTitleASCII.Length
            );

            byte[] windowTitleUTF8 = Encoding.UTF8.GetBytes(title);
            LibX11.XChangeProperty(
                display,
                windowId,
                LibX11.XInternAtom(display, "_NET_WM_NAME", 0),
                LibX11.XInternAtom(display, "UTF8_STRING", 0),
                XChangePropertyFormat.Byte,
                XChangePropertyMode.PropModeReplace,
                windowTitleUTF8,
                windowTitleUTF8.Length
            );
        }

        public void Invalidate(Rectangle area)
        {
            // todo: implement
        }
    }
}