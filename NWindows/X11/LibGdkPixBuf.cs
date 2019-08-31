using System.Runtime.InteropServices;

namespace NWindows.X11
{
    using gboolean = System.Boolean;
    using gint = System.Int32;
    using gpointer = System.IntPtr;
    using gssize = System.Int64;
    using guchar_ptr = System.IntPtr;
    using GCancellablePtr = System.IntPtr;
    using GDestroyNotify = System.IntPtr;
    using GdkPixbufPtr = System.IntPtr;
    using GErrorPtr = System.IntPtr;
    using GInputStreamPtr = System.IntPtr;
    using GQuark = System.UInt32;
    using VoidPtr = System.IntPtr;

    internal class LibGdkPixBuf
    {
        [DllImport("libgdk_pixbuf-2.0")]
        public static extern void g_object_unref(gpointer obj);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern void g_error_free(GErrorPtr error);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern GInputStreamPtr g_memory_input_stream_new_from_data(VoidPtr data, gssize len, GDestroyNotify destroy);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern GdkPixbufPtr gdk_pixbuf_new_from_stream(GInputStreamPtr stream, GCancellablePtr cancellable, out GErrorPtr error);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern GdkColorspace gdk_pixbuf_get_colorspace(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern int gdk_pixbuf_get_width(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern int gdk_pixbuf_get_height(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern int gdk_pixbuf_get_n_channels(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern int gdk_pixbuf_get_bits_per_sample(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern gboolean gdk_pixbuf_get_has_alpha(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern int gdk_pixbuf_get_rowstride(GdkPixbufPtr pixbuf);

        [DllImport("libgdk_pixbuf-2.0")]
        public static extern guchar_ptr gdk_pixbuf_get_pixels(GdkPixbufPtr pixbuf);
    }

    internal enum GdkColorspace
    {
        GDK_COLORSPACE_RGB
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct GError
    {
        public readonly GQuark domain;

        public readonly gint code;

        // todo: handle UTF-8 encoding better?
        [MarshalAs(UnmanagedType.LPStr)] public readonly string message;
    }
}