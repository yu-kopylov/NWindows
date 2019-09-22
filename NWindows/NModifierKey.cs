using System;

namespace NWindows
{
    [Flags]
    public enum NModifierKey
    {
        None = 0,

        Shift = 1,
        Control = 2,
        Alt = 4
    }
}