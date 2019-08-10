namespace NWindows.X11
{
    public class X11Window : INativeWindow
    {
        private readonly ulong windowId;

        public X11Window(ulong windowId)
        {
            this.windowId = windowId;
        }
    }
}