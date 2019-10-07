using System;

namespace NWindows
{
    [Flags]
    public enum NMouseButton
    {
        Unknown = 0,
        Left = 1,
        Right = 2,
        Middle = 4,
        X1 = 8,
        X2 = 16
    }
}