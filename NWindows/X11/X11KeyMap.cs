using System.Collections.Generic;

namespace NWindows.X11
{
    internal static class X11KeyMap
    {
        private static readonly Dictionary<ulong, NKeyCode> keyCodes = new Dictionary<ulong, NKeyCode>();

        static X11KeyMap()
        {
            keyCodes.Add(0x0020, NKeyCode.Space);
            keyCodes.Add(0x0027, NKeyCode.Apostrophe);
            keyCodes.Add(0x002c, NKeyCode.Comma);
            keyCodes.Add(0x002d, NKeyCode.Minus);
            keyCodes.Add(0x002e, NKeyCode.Period);
            keyCodes.Add(0x002f, NKeyCode.Slash);
            keyCodes.Add(0x0030, NKeyCode.Key0);
            keyCodes.Add(0x0031, NKeyCode.Key1);
            keyCodes.Add(0x0032, NKeyCode.Key2);
            keyCodes.Add(0x0033, NKeyCode.Key3);
            keyCodes.Add(0x0034, NKeyCode.Key4);
            keyCodes.Add(0x0035, NKeyCode.Key5);
            keyCodes.Add(0x0036, NKeyCode.Key6);
            keyCodes.Add(0x0037, NKeyCode.Key7);
            keyCodes.Add(0x0038, NKeyCode.Key8);
            keyCodes.Add(0x0039, NKeyCode.Key9);
            keyCodes.Add(0x003b, NKeyCode.Semicolon);
            keyCodes.Add(0x003d, NKeyCode.Plus);
            keyCodes.Add(0x005b, NKeyCode.LeftBracket);
            keyCodes.Add(0x005c, NKeyCode.Backslash);
            keyCodes.Add(0x005d, NKeyCode.RightBracket);
            keyCodes.Add(0x0060, NKeyCode.Grave);
            keyCodes.Add(0x0061, NKeyCode.A);
            keyCodes.Add(0x0062, NKeyCode.B);
            keyCodes.Add(0x0063, NKeyCode.C);
            keyCodes.Add(0x0064, NKeyCode.D);
            keyCodes.Add(0x0065, NKeyCode.E);
            keyCodes.Add(0x0066, NKeyCode.F);
            keyCodes.Add(0x0067, NKeyCode.G);
            keyCodes.Add(0x0068, NKeyCode.H);
            keyCodes.Add(0x0069, NKeyCode.I);
            keyCodes.Add(0x006a, NKeyCode.J);
            keyCodes.Add(0x006b, NKeyCode.K);
            keyCodes.Add(0x006c, NKeyCode.L);
            keyCodes.Add(0x006d, NKeyCode.M);
            keyCodes.Add(0x006e, NKeyCode.N);
            keyCodes.Add(0x006f, NKeyCode.O);
            keyCodes.Add(0x0070, NKeyCode.P);
            keyCodes.Add(0x0071, NKeyCode.Q);
            keyCodes.Add(0x0072, NKeyCode.R);
            keyCodes.Add(0x0073, NKeyCode.S);
            keyCodes.Add(0x0074, NKeyCode.T);
            keyCodes.Add(0x0075, NKeyCode.U);
            keyCodes.Add(0x0076, NKeyCode.V);
            keyCodes.Add(0x0077, NKeyCode.W);
            keyCodes.Add(0x0078, NKeyCode.X);
            keyCodes.Add(0x0079, NKeyCode.Y);
            keyCodes.Add(0x007A, NKeyCode.Z);
            keyCodes.Add(0xff08, NKeyCode.Backspace);
            keyCodes.Add(0xff09, NKeyCode.Tab);
            keyCodes.Add(0xff0d, NKeyCode.Enter);
            keyCodes.Add(0xff13, NKeyCode.Pause);
            keyCodes.Add(0xff14, NKeyCode.ScrollLock);
            keyCodes.Add(0xff1b, NKeyCode.Escape);
            keyCodes.Add(0xff50, NKeyCode.Home);
            keyCodes.Add(0xff51, NKeyCode.LeftArrow);
            keyCodes.Add(0xff52, NKeyCode.UpArrow);
            keyCodes.Add(0xff53, NKeyCode.RightArrow);
            keyCodes.Add(0xff54, NKeyCode.DownArrow);
            keyCodes.Add(0xff55, NKeyCode.PageUp);
            keyCodes.Add(0xff56, NKeyCode.PageDown);
            keyCodes.Add(0xff57, NKeyCode.End);
            keyCodes.Add(0xff61, NKeyCode.PrintScreen);
            keyCodes.Add(0xff63, NKeyCode.Insert);
            keyCodes.Add(0xff67, NKeyCode.Applications);
            keyCodes.Add(0xff7f, NKeyCode.NumPadNumLock);
            keyCodes.Add(0xff8d, NKeyCode.NumPadEnter);
            keyCodes.Add(0xff95, NKeyCode.NumPadHome);
            keyCodes.Add(0xff96, NKeyCode.NumPadLeftArrow);
            keyCodes.Add(0xff97, NKeyCode.NumPadUpArrow);
            keyCodes.Add(0xff98, NKeyCode.NumPadRightArrow);
            keyCodes.Add(0xff99, NKeyCode.NumPadDownArrow);
            keyCodes.Add(0xff9a, NKeyCode.NumPadPageUp);
            keyCodes.Add(0xff9b, NKeyCode.NumPadPageDown);
            keyCodes.Add(0xff9c, NKeyCode.NumPadEnd);
            keyCodes.Add(0xff9d, NKeyCode.NumPadClear);
            keyCodes.Add(0xff9e, NKeyCode.NumPadInsert);
            keyCodes.Add(0xff9f, NKeyCode.NumPadDelete);
            keyCodes.Add(0xffaa, NKeyCode.NumPadMultiply);
            keyCodes.Add(0xffab, NKeyCode.NumPadAdd);
            keyCodes.Add(0xffad, NKeyCode.NumPadSubtract);
            keyCodes.Add(0xffae, NKeyCode.NumPadDecimal);
            keyCodes.Add(0xffaf, NKeyCode.NumPadDivide);
            keyCodes.Add(0xffb0, NKeyCode.NumPad0);
            keyCodes.Add(0xffb1, NKeyCode.NumPad1);
            keyCodes.Add(0xffb2, NKeyCode.NumPad2);
            keyCodes.Add(0xffb3, NKeyCode.NumPad3);
            keyCodes.Add(0xffb4, NKeyCode.NumPad4);
            keyCodes.Add(0xffb5, NKeyCode.NumPad5);
            keyCodes.Add(0xffb6, NKeyCode.NumPad6);
            keyCodes.Add(0xffb7, NKeyCode.NumPad7);
            keyCodes.Add(0xffb8, NKeyCode.NumPad8);
            keyCodes.Add(0xffb9, NKeyCode.NumPad9);
            keyCodes.Add(0xffbe, NKeyCode.F1);
            keyCodes.Add(0xffbf, NKeyCode.F2);
            keyCodes.Add(0xffc0, NKeyCode.F3);
            keyCodes.Add(0xffc1, NKeyCode.F4);
            keyCodes.Add(0xffc2, NKeyCode.F5);
            keyCodes.Add(0xffc3, NKeyCode.F6);
            keyCodes.Add(0xffc4, NKeyCode.F7);
            keyCodes.Add(0xffc5, NKeyCode.F8);
            keyCodes.Add(0xffc6, NKeyCode.F9);
            keyCodes.Add(0xffc7, NKeyCode.F10);
            keyCodes.Add(0xffc8, NKeyCode.F11);
            keyCodes.Add(0xffc9, NKeyCode.F12);
            keyCodes.Add(0xffe1, NKeyCode.LeftShift);
            keyCodes.Add(0xffe2, NKeyCode.RightShift);
            keyCodes.Add(0xffe3, NKeyCode.LeftControl);
            keyCodes.Add(0xffe4, NKeyCode.RightControl);
            keyCodes.Add(0xffe5, NKeyCode.CapsLock);
            keyCodes.Add(0xffe9, NKeyCode.LeftAlt);
            keyCodes.Add(0xffea, NKeyCode.RightAlt);
            keyCodes.Add(0xffeb, NKeyCode.LeftWindows);
            keyCodes.Add(0xffec, NKeyCode.RightWindows);
            keyCodes.Add(0xffff, NKeyCode.Delete);
        }

        public static NKeyCode GetKeyCode(ulong keySym)
        {
            if (!keyCodes.TryGetValue(keySym, out var keyCode))
            {
                return NKeyCode.Unknown;
            }

            return keyCode;
        }
    }
}