using System.Collections.Generic;
using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class KeyboardExampleControl : Control
    {
        private readonly List<string> lastKeys = new List<string>();

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            canvas.FillRectangle(Color.LightBlue, 0, 0, Area.Width, Area.Height);
            FontConfig arial = new FontConfig("Arial", 16);
            for (int i = 0; i < lastKeys.Count; i++)
            {
                canvas.DrawString(Color.Black, arial, 1, 1 + 18 * i, lastKeys[i]);
            }
        }

        public void HandleKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            lastKeys.Add($"[D] {keyCode}{(autoRepeat ? " (R)" : "")}({modifierKey})");
            TruncateList();
            Invalidate();
        }

        public void HandleKeyUp(NKeyCode keyCode)
        {
            lastKeys.Add($"[U] {keyCode}");
            TruncateList();
            Invalidate();
        }

        public void HandleTextInput(string text)
        {
            lastKeys.Add($"[T] {text}");
            TruncateList();
            Invalidate();
        }

        private void TruncateList()
        {
            if (lastKeys.Count > 10)
            {
                lastKeys.RemoveAt(0);
            }
        }
    }
}