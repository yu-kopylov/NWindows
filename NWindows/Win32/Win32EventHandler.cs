﻿using System.Drawing;
using NWindows.NativeApi;

namespace NWindows.Win32
{
    internal static class Win32EventHandler
    {
        static readonly HandleWin32Event[] eventHandlers = new HandleWin32Event[1024];

        static Win32EventHandler()
        {
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

        public static bool HandleWindowEvent(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
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

        private delegate void HandleWin32Event(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam);

        private static void HandleKeyDown(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            bool autoRepeat = (lParam & 0x40000000) != 0;
            window.OnKeyDown(Win32KeyMap.GetKeyCode(lParam, wParam), GetModifierKey(), autoRepeat);
        }

        private static void HandleKeyUp(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            window.OnKeyUp(Win32KeyMap.GetKeyCode(lParam, wParam));
        }

        private static void HandleLeftMouseButtonDown(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Left, window, messageType, wParam, lParam);
        }

        private static void HandleRightMouseButtonDown(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Right, window, messageType, wParam, lParam);
        }

        private static void HandleMiddleMouseButtonDown(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(NMouseButton.Middle, window, messageType, wParam, lParam);
        }

        private static void HandleXMouseButtonDown(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonDown(GetXMouseButton(wParam), window, messageType, wParam, lParam);
        }

        private static void HandleMouseButtonDown(NMouseButton mouseButton, INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            int x = (short) (lParam & 0xFFFF);
            int y = (short) ((lParam >> 16) & 0xFFFF);
            window.OnMouseButtonDown(mouseButton, new Point(x, y), GetModifierKey());
        }

        private static void HandleLeftMouseButtonUp(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Left, window, messageType, wParam, lParam);
        }

        private static void HandleRightMouseButtonUp(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Right, window, messageType, wParam, lParam);
        }

        private static void HandleMiddleMouseButtonUp(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(NMouseButton.Middle, window, messageType, wParam, lParam);
        }

        private static void HandleXMouseButtonUp(INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            HandleMouseButtonUp(GetXMouseButton(wParam), window, messageType, wParam, lParam);
        }

        private static void HandleMouseButtonUp(NMouseButton mouseButton, INativeWindowStartupInfo window, Win32MessageType messageType, uint wParam, uint lParam)
        {
            int x = (short) (lParam & 0xFFFF);
            int y = (short) ((lParam >> 16) & 0xFFFF);
            window.OnMouseButtonUp(mouseButton, new Point(x, y));
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