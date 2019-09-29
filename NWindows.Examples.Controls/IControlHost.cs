using System.Drawing;

namespace NWindows.Examples.Controls
{
    public interface IControlHost
    {
        NApplication Application { get; }

        void Invalidate(Rectangle area);
    }
}