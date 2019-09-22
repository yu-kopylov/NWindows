using System;

namespace NWindows.Win32
{
    internal static class W32KeyMap
    {
        private const byte VK_SHIFT = 0x10;

        private static readonly NKeyCode[] keyCodes = new NKeyCode[512];

        static W32KeyMap()
        {
            keyCodes[0x008] = NKeyCode.Backspace;
            keyCodes[0x009] = NKeyCode.Tab;
            keyCodes[0x00C] = NKeyCode.NumPadClear;
            keyCodes[0x00D] = NKeyCode.Enter;
            keyCodes[0x011] = NKeyCode.LeftControl;
            keyCodes[0x012] = NKeyCode.LeftAlt;
            keyCodes[0x013] = NKeyCode.Pause;
            keyCodes[0x014] = NKeyCode.CapsLock;
            keyCodes[0x01B] = NKeyCode.Escape;
            keyCodes[0x020] = NKeyCode.Space;

            keyCodes[0x021] = NKeyCode.NumPadPageUp;
            keyCodes[0x022] = NKeyCode.NumPadPageDown;
            keyCodes[0x023] = NKeyCode.NumPadEnd;
            keyCodes[0x024] = NKeyCode.NumPadHome;
            keyCodes[0x025] = NKeyCode.NumPadLeftArrow;
            keyCodes[0x026] = NKeyCode.NumPadUpArrow;
            keyCodes[0x027] = NKeyCode.NumPadRightArrow;
            keyCodes[0x028] = NKeyCode.NumPadDownArrow;
            keyCodes[0x02D] = NKeyCode.NumPadInsert;
            keyCodes[0x02E] = NKeyCode.NumPadDelete;

            keyCodes[0x030] = NKeyCode.Key0;
            keyCodes[0x031] = NKeyCode.Key1;
            keyCodes[0x032] = NKeyCode.Key2;
            keyCodes[0x033] = NKeyCode.Key3;
            keyCodes[0x034] = NKeyCode.Key4;
            keyCodes[0x035] = NKeyCode.Key5;
            keyCodes[0x036] = NKeyCode.Key6;
            keyCodes[0x037] = NKeyCode.Key7;
            keyCodes[0x038] = NKeyCode.Key8;
            keyCodes[0x039] = NKeyCode.Key9;

            keyCodes[0x041] = NKeyCode.A;
            keyCodes[0x042] = NKeyCode.B;
            keyCodes[0x043] = NKeyCode.C;
            keyCodes[0x044] = NKeyCode.D;
            keyCodes[0x045] = NKeyCode.E;
            keyCodes[0x046] = NKeyCode.F;
            keyCodes[0x047] = NKeyCode.G;
            keyCodes[0x048] = NKeyCode.H;
            keyCodes[0x049] = NKeyCode.I;
            keyCodes[0x04A] = NKeyCode.J;
            keyCodes[0x04B] = NKeyCode.K;
            keyCodes[0x04C] = NKeyCode.L;
            keyCodes[0x04D] = NKeyCode.M;
            keyCodes[0x04E] = NKeyCode.N;
            keyCodes[0x04F] = NKeyCode.O;
            keyCodes[0x050] = NKeyCode.P;
            keyCodes[0x051] = NKeyCode.Q;
            keyCodes[0x052] = NKeyCode.R;
            keyCodes[0x053] = NKeyCode.S;
            keyCodes[0x054] = NKeyCode.T;
            keyCodes[0x055] = NKeyCode.U;
            keyCodes[0x056] = NKeyCode.V;
            keyCodes[0x057] = NKeyCode.W;
            keyCodes[0x058] = NKeyCode.X;
            keyCodes[0x059] = NKeyCode.Y;
            keyCodes[0x05A] = NKeyCode.Z;

            keyCodes[0x060] = NKeyCode.NumPad0;
            keyCodes[0x061] = NKeyCode.NumPad1;
            keyCodes[0x062] = NKeyCode.NumPad2;
            keyCodes[0x063] = NKeyCode.NumPad3;
            keyCodes[0x064] = NKeyCode.NumPad4;
            keyCodes[0x065] = NKeyCode.NumPad5;
            keyCodes[0x066] = NKeyCode.NumPad6;
            keyCodes[0x067] = NKeyCode.NumPad7;
            keyCodes[0x068] = NKeyCode.NumPad8;
            keyCodes[0x069] = NKeyCode.NumPad9;
            keyCodes[0x06A] = NKeyCode.NumPadMultiply;
            keyCodes[0x06B] = NKeyCode.NumPadAdd;
            keyCodes[0x06D] = NKeyCode.NumPadSubtract;
            keyCodes[0x06E] = NKeyCode.NumPadDecimal;

            keyCodes[0x070] = NKeyCode.F1;
            keyCodes[0x071] = NKeyCode.F2;
            keyCodes[0x072] = NKeyCode.F3;
            keyCodes[0x073] = NKeyCode.F4;
            keyCodes[0x074] = NKeyCode.F5;
            keyCodes[0x075] = NKeyCode.F6;
            keyCodes[0x076] = NKeyCode.F7;
            keyCodes[0x077] = NKeyCode.F8;
            keyCodes[0x078] = NKeyCode.F9;
            keyCodes[0x079] = NKeyCode.F10;
            keyCodes[0x07A] = NKeyCode.F11;
            keyCodes[0x07B] = NKeyCode.F12;

            keyCodes[0x091] = NKeyCode.ScrollLock;

            keyCodes[0x0A0] = NKeyCode.LeftShift;
            keyCodes[0x0A1] = NKeyCode.RightShift;

            keyCodes[0x0BA] = NKeyCode.Semicolon;
            keyCodes[0x0BB] = NKeyCode.Plus;
            keyCodes[0x0BC] = NKeyCode.Comma;
            keyCodes[0x0BD] = NKeyCode.Minus;
            keyCodes[0x0BE] = NKeyCode.Period;
            keyCodes[0x0BF] = NKeyCode.Slash;
            keyCodes[0x0C0] = NKeyCode.Grave;

            keyCodes[0x0DB] = NKeyCode.LeftBracket;
            keyCodes[0x0DC] = NKeyCode.Backslash;
            keyCodes[0x0DD] = NKeyCode.RightBracket;
            keyCodes[0x0DE] = NKeyCode.Apostrophe;

            keyCodes[0x10D] = NKeyCode.NumPadEnter;

            keyCodes[0x111] = NKeyCode.RightControl;
            keyCodes[0x112] = NKeyCode.RightAlt;

            keyCodes[0x125] = NKeyCode.LeftArrow;
            keyCodes[0x126] = NKeyCode.UpArrow;
            keyCodes[0x127] = NKeyCode.RightArrow;
            keyCodes[0x128] = NKeyCode.DownArrow;

            keyCodes[0x121] = NKeyCode.PageUp;
            keyCodes[0x122] = NKeyCode.PageDown;
            keyCodes[0x123] = NKeyCode.End;
            keyCodes[0x124] = NKeyCode.Home;
            keyCodes[0x12C] = NKeyCode.PrintScreen;
            keyCodes[0x12D] = NKeyCode.Insert;
            keyCodes[0x12E] = NKeyCode.Delete;

            keyCodes[0x15B] = NKeyCode.LeftWindows;
            keyCodes[0x15C] = NKeyCode.RightWindows;
            keyCodes[0x15D] = NKeyCode.Applications;

            keyCodes[0x190] = NKeyCode.NumPadNumLock;

            keyCodes[0x16F] = NKeyCode.NumPadDivide;
        }

        public static NKeyCode GetKeyCode(IntPtr lParam, IntPtr wParam)
        {
            uint lParam32 = (uint) lParam.ToInt64();
            uint wParam32 = (uint) wParam.ToInt64();

            byte virtualKey = (byte) wParam32;
            bool isExtended = (lParam32 & 0x01000000) != 0;

            if (virtualKey == VK_SHIFT)
            {
                byte scanCode = (byte) (lParam32 >> 16);
                virtualKey = (byte) Win32API.MapVirtualKeyW(scanCode, VirtualKeyMapType.MAPVK_VSC_TO_VK_EX);
                return GetKeyCode(virtualKey, isExtended);
            }

            return GetKeyCode(virtualKey, isExtended);
        }

        private static NKeyCode GetKeyCode(byte virtualKey, bool isExtended)
        {
            int index = virtualKey;

            if (isExtended)
            {
                index |= 0x100;
            }

            return keyCodes[index];
        }
    }
}