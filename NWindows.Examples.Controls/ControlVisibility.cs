using System;

namespace NWindows.Examples.Controls
{
    public enum ControlVisibility
    {
        Visible = 0,
        Hidden = 1,
        Collapsed = 2
    }

    public static class ControlVisibilityExtensions
    {
        public static ControlVisibility Combine(this ControlVisibility visibility1, ControlVisibility visibility2)
        {
            return (ControlVisibility) Math.Max((int) visibility1, (int) visibility2);
        }
    }
}