using System.Drawing;

namespace NWindows.Win32
{
    internal static class Win32EventHandler
    {
        static readonly HandleWin32Event[] eventHandlers = new HandleWin32Event[1024];

        static Win32EventHandler()
        {
            eventHandlers[(int) Win32MessageType.WM_ACTIVATE] = HandleActivate;
            eventHandlers[(int) Win32MessageType.WM_KEYDOWN] = HandleKeyDown;
            eventHandlers[(int) Win32MessageType.WM_SYSKEYDOWN] = HandleKeyDown;
            eventHandlers[(int) Win32MessageType.WM_KEYUP] = HandleKeyUp;
            eventHandlers[(int) Win32MessageType.WM_SYSKEYUP] = HandleKeyUp;
            eventHandlers[(int) Win32MessageType.WM_LBUTTONDOWN] = HandleLeftMouseButtonDown;
            eventHandlers[(int) Win32MessageType.WM_RBUTTONDOWN] = HandleRightMouseButtonDown;
            eventHandlers[(int) Win32MessageType.WM_MBUTTONDOWN] = HandleMiddleMouseButtonDown;
            eventHandlers[(int) Win32MessageType.WM_XBUTTONDOWN] = HandleXMouseButtonDown;
            eventHandlers[(int) Win32MessageType.WM_LBUTTONUP] = HandleLeftMouseButtonUp;
            eventHandlers[(int) Win32MessageType.WM_RBUTTONUP] = HandleRightMouseButtonUp;
            eventHandlers[(int) Win32MessageType.WM_MBUTTONUP] = HandleMiddleMouseButtonUp;
            eventHandlers[(int) Win32MessageType.WM_XBUTTONUP] = HandleXMouseButtonUp;
        }

        public static bool HandleWindowEvent(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            int messageTypeIndex = (int) messageType;
            if (messageTypeIndex < 0 || messageTypeIndex >= eventHandlers.Length)
            {
                return false;
            }

            HandleWin32Event eventHandler = eventHandlers[messageTypeIndex];
            if (eventHandler == null)
            {
                return false;
            }

            eventHandler(window, messageType, wParam, lParam);
            return true;
        }

        private delegate void HandleWin32Event(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam);

        private static void HandleActivate(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            bool activated = (wParam & 0xFFFF) != 0;
            if (activated)
            {
                window.StartupInfo.OnActivated();
            }
            else
            {
                window.StartupInfo.OnDeactivated();
            }
        }

        private static void HandleKeyDown(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            bool autoRepeat = (lParam & 0x40000000) != 0;
            window.StartupInfo.OnKeyDown(Win32KeyMap.GetKeyCode(lParam, wParam), GetModifierKey(), autoRepeat);
        }

        private static void HandleKeyUp(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            window.StartupInfo.OnKeyUp(Win32KeyMap.GetKeyCode(lParam, wParam));
        }

        private static void HandleLeftMouseButtonDown(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Left, window, messageType, wParam, lParam);
        }

        private static void HandleRightMouseButtonDown(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Right, window, messageType, wParam, lParam);
        }

        private static void HandleMiddleMouseButtonDown(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Middle, window, messageType, wParam, lParam);
        }

        private static void HandleXMouseButtonDown(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(GetXMouseButton(wParam), window, messageType, wParam, lParam);
        }

        private static void HandleMouseButtonDown(NMouseButton mouseButton, Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            int x = (short) (lParam & 0xFFFF);
            int y = (short) ((lParam >> 16) & 0xFFFF);

            Win32API.SetCapture(window.WindowHandle);

            window.StartupInfo.OnMouseButtonDown(mouseButton, new Point(x, y), GetModifierKey());
        }

        private static void HandleLeftMouseButtonUp(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Left, window, messageType, wParam, lParam);
        }

        private static void HandleRightMouseButtonUp(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Right, window, messageType, wParam, lParam);
        }

        private static void HandleMiddleMouseButtonUp(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Middle, window, messageType, wParam, lParam);
        }

        private static void HandleXMouseButtonUp(Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(GetXMouseButton(wParam), window, messageType, wParam, lParam);
        }

        private static void HandleMouseButtonUp(NMouseButton mouseButton, Win32Window window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            int x = (short) (lParam & 0xFFFF);
            int y = (short) ((lParam >> 16) & 0xFFFF);

            if (!AnyMouseButtonIsDown())
            {
                Win32API.ReleaseCapture();
            }

            window.StartupInfo.OnMouseButtonUp(mouseButton, new Point(x, y));
        }

        private static NMouseButton GetXMouseButton(uint wParam)
        {
            int button = (short) ((wParam >> 16) & 0xFFFF);

            if (button == 1)
            {
                return NMouseButton.X1;
            }

            if (button == 2)
            {
                return NMouseButton.X2;
            }

            return NMouseButton.Unknown;
        }

        private static bool AnyMouseButtonIsDown()
        {
            if ((Win32API.GetKeyState(W32VirtualKey.VK_LBUTTON) & 0x8000) != 0)
            {
                return true;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_RBUTTON) & 0x8000) != 0)
            {
                return true;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_MBUTTON) & 0x8000) != 0)
            {
                return true;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_XBUTTON1) & 0x8000) != 0)
            {
                return true;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_XBUTTON2) & 0x8000) != 0)
            {
                return true;
            }

            return false;
        }

        private static NModifierKey GetModifierKey()
        {
            NModifierKey modifierKey = NModifierKey.None;

            if ((Win32API.GetKeyState(W32VirtualKey.VK_SHIFT) & 0x8000) != 0)
            {
                modifierKey |= NModifierKey.Shift;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_CONTROL) & 0x8000) != 0)
            {
                modifierKey |= NModifierKey.Control;
            }

            if ((Win32API.GetKeyState(W32VirtualKey.VK_MENU) & 0x8000) != 0)
            {
                modifierKey |= NModifierKey.Alt;
            }

            return modifierKey;
        }
    }
}