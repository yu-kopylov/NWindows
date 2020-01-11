using System.Collections.Generic;
using System.Drawing;
using NWindows.Examples.Controls;

namespace NWindows.Examples
{
    public class InputExampleControl : Control
    {
        private readonly List<string> lastKeys = new List<string>();

        protected override void OnPaint(ICanvas canvas, Rectangle area)
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
            lastKeys.Add($"[D][K] {keyCode}{(autoRepeat ? " (R)" : "")}({modifierKey})");
            TruncateList();
            Invalidate();
        }

        public void HandleKeyUp(NKeyCode keyCode)
        {
            lastKeys.Add($"[U][K] {keyCode}");
            TruncateList();
            Invalidate();
        }

        public void HandleTextInput(string text)
        {
            lastKeys.Add($"[T][X] {text}");
            TruncateList();
            Invalidate();
        }

        public void HandleMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            lastKeys.Add($"[D][M] {button} at {point} ({modifierKey})");
            TruncateList();
            Invalidate();
        }

        public void HandleMouseButtonUp(NMouseButton button, Point point)
        {
            lastKeys.Add($"[U][M] {button} at {point}");
            TruncateList();
            Invalidate();
        }

        private void TruncateList()
        {
            if (lastKeys.Count > 12)
            {
                lastKeys.RemoveAt(0);
            }
        }
    }
}