using System;
using System.Collections.Generic;
using System.Drawing;

namespace NWindows.Examples.Controls
{
    public class TextBox : Control
    {
        private const int MaxLogLength = 16;

        private readonly Color selectionColor = Color.LightSkyBlue;
        private readonly Color cursorColor = Color.Black;

        private Color textColor = Color.Black;
        private FontConfig font = new FontConfig("Arial", 16);

        private readonly LinkedList<TextBoxState> undoLog = new LinkedList<TextBoxState>();
        private readonly LinkedList<TextBoxState> redoLog = new LinkedList<TextBoxState>();
        private TextBoxState state = new TextBoxState {Text = "TextBox Test", CursorOffset = 4, SelectionFrom = 4, SelectionTo = 4 + 5};
        private ActionType lastActionType = ActionType.Undefined;

        private bool coordinatesCalculated;
        private int textOffsetX;
        private int cursorX;
        private int selectionFromX;
        private int selectionToX;

        public Color TextColor
        {
            get { return textColor; }
            set
            {
                if (textColor != value)
                {
                    textColor = value;
                    Invalidate();
                }
            }
        }

        public FontConfig Font
        {
            get { return font; }
            set
            {
                // todo: compare fonts by value
                if (font != value)
                {
                    font = value;
                    coordinatesCalculated = false;
                    Invalidate();
                }
            }
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            UpdateCoordinates();

            // todo: use font to get height
            int textHeight = 18;

            if (state.HasSelection)
            {
                // todo: check +/- 1 in width
                canvas.FillRectangle(selectionColor, textOffsetX + selectionFromX, 0, selectionToX - selectionFromX, textHeight);
            }

            canvas.FillRectangle(cursorColor, textOffsetX + cursorX, 0, 1, textHeight);
            canvas.DrawString(textColor, font, textOffsetX, 0, state.Text);
        }

        private void UpdateCoordinates()
        {
            if (coordinatesCalculated)
            {
                return;
            }

            coordinatesCalculated = true;

            if (state.HasSelection)
            {
                selectionFromX = Application.Graphics.MeasureString(font, state.Text.Substring(0, state.SelectionFrom)).Width;
                selectionToX = Application.Graphics.MeasureString(font, state.Text.Substring(0, state.SelectionTo)).Width;
            }

            cursorX = Application.Graphics.MeasureString(font, state.Text.Substring(0, state.CursorOffset)).Width;

            // todo: recalculate offset when TextBox is resized
            if (textOffsetX + cursorX >= Area.Width)
            {
                textOffsetX = Area.Width - 1 - cursorX;
            }
            else if (textOffsetX < 0)
            {
                // todo: check edge-cases
                int textWidth = Application.Graphics.MeasureString(font, state.Text).Width;
                if (textOffsetX + textWidth + 1 < Area.Width)
                {
                    textOffsetX = Math.Min(0, Area.Width - textWidth - 1);
                }
            }

            if (textOffsetX + cursorX < 0)
            {
                textOffsetX = -cursorX;
            }
        }

        public override void OnMouseButtonDown(NMouseButton button, Point point, NModifierKey modifierKey)
        {
            if (button == NMouseButton.Left)
            {
                UpdateCoordinates();
                int offset = XToOffset(point.X - textOffsetX);

                SaveState(ActionType.Undefined);
                state.SetCursorOffset(offset);
                coordinatesCalculated = false;
                Invalidate();
            }
        }

        private int XToOffset(int x)
        {
            List<int> codepointOffsets = GetCodepointOffsets(state.Text);
            codepointOffsets.Add(state.Text.Length);
            int a = 0, b = codepointOffsets.Count - 1;

            while (a < b)
            {
                int c = (a + b + 1) / 2;
                int width = Application.Graphics.MeasureString(font, state.Text.Substring(0, codepointOffsets[c])).Width;

                if (x == width)
                {
                    a = c;
                    break;
                }

                if (x > width)
                {
                    a = c;
                }
                else
                {
                    b = c - 1;
                }
            }

            if (a + 1 < codepointOffsets.Count)
            {
                int width1 = Application.Graphics.MeasureString(font, state.Text.Substring(0, codepointOffsets[a])).Width;
                int width2 = Application.Graphics.MeasureString(font, state.Text.Substring(0, codepointOffsets[a + 1])).Width;
                if (x > (width1 + 2 * width2) / 3)
                {
                    a++;
                }
            }

            return codepointOffsets[a];
        }

        private static List<int> GetCodepointOffsets(string text)
        {
            List<int> res = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                res.Add(i);
                if ((i + 1) < text.Length && char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
                {
                    i++;
                }
            }

            return res;
        }

        public void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            if (keyCode == NKeyCode.A && modifierKey == NModifierKey.Control)
            {
                state.SelectAll();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.Z && modifierKey == NModifierKey.Control)
            {
                if (undoLog.Count != 0)
                {
                    redoLog.AddFirst(state);
                    state = undoLog.Last.Value;
                    undoLog.RemoveLast();
                }

                lastActionType = ActionType.Undefined;
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.Z && modifierKey == (NModifierKey.Control | NModifierKey.Shift))
            {
                if (redoLog.Count != 0)
                {
                    undoLog.AddLast(state);
                    state = redoLog.First.Value;
                    redoLog.RemoveFirst();
                }

                lastActionType = ActionType.Undefined;
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.LeftArrow && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.MoveCursorLeft();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.RightArrow && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.MoveCursorRight();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.LeftArrow && modifierKey == NModifierKey.Shift)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.SelectLeft();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.RightArrow && modifierKey == NModifierKey.Shift)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.SelectRight();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.Home && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad home key
                SaveState(ActionType.Undefined);
                state.GoHome();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.End && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Undefined);
                state.GoEnd();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.Backspace && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Deletion);
                state.DeleteLeft();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.Delete && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Deletion);
                state.DeleteRight();
                coordinatesCalculated = false;
                Invalidate();
            }
            else if (keyCode == NKeyCode.C && modifierKey == NModifierKey.Control)
            {
                if (state.HasSelection)
                {
                    Application.Clipboard.PutText(state.SelectedText);
                }
            }
            else if (keyCode == NKeyCode.X && modifierKey == NModifierKey.Control)
            {
                if (state.HasSelection)
                {
                    Application.Clipboard.PutText(state.SelectedText);
                    SaveState(ActionType.Undefined);
                    state.DeleteRight();
                    coordinatesCalculated = false;
                    Invalidate();
                }
            }
            else if (keyCode == NKeyCode.V && modifierKey == NModifierKey.Control)
            {
                if (Application.Clipboard.TryGetText(out var text))
                {
                    SaveState(ActionType.Undefined);
                    state.EnterText(text);
                    coordinatesCalculated = false;
                    Invalidate();
                }
            }
        }

        public void OnTextInput(string text)
        {
            // todo: limit text length
            SaveState(ActionType.Typing);
            state.EnterText(text);
            coordinatesCalculated = false;
            Invalidate();
        }

        private void SaveState(ActionType actionType)
        {
            if (lastActionType != actionType)
            {
                lastActionType = actionType;
                if (actionType != ActionType.Undefined)
                {
                    undoLog.AddLast(state);
                    while (undoLog.Count > MaxLogLength)
                    {
                        undoLog.RemoveFirst();
                    }

                    redoLog.Clear();
                    state = state.Copy();
                }
            }
        }

        private class TextBoxState
        {
            // todo: make sure that text is never null
            public string Text { get; set; } = string.Empty;
            public int CursorOffset { get; set; }
            public int SelectionFrom { get; set; }
            public int SelectionTo { get; set; }

            public bool HasSelection => SelectionFrom != SelectionTo;

            public string SelectedText => HasSelection ? Text.Substring(SelectionFrom, SelectionTo - SelectionFrom) : string.Empty;

            public TextBoxState Copy()
            {
                return new TextBoxState
                {
                    Text = Text,
                    CursorOffset = CursorOffset,
                    SelectionFrom = SelectionFrom,
                    SelectionTo = SelectionTo
                };
            }

            public void EnterText(string text)
            {
                if (HasSelection)
                {
                    Text = Text.Substring(0, SelectionFrom) + text + Text.Substring(SelectionTo);
                    CursorOffset = SelectionFrom + text.Length;
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }
                else
                {
                    Text = Text.Substring(0, CursorOffset) + text + Text.Substring(CursorOffset);
                    CursorOffset = CursorOffset + text.Length;
                }
            }

            public void SelectAll()
            {
                CursorOffset = Text.Length;
                SelectionFrom = 0;
                SelectionTo = Text.Length;
            }

            public void MoveCursorLeft()
            {
                if (HasSelection)
                {
                    CursorOffset = SelectionFrom;
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }
                else if (CursorOffset > 0)
                {
                    CursorOffset = PrevCodepointOffset(CursorOffset);
                }
            }

            public void MoveCursorRight()
            {
                if (HasSelection)
                {
                    CursorOffset = SelectionTo;
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }
                else if (CursorOffset < Text.Length)
                {
                    CursorOffset = NextCodepointOffset(CursorOffset);
                }
            }

            public void SelectLeft()
            {
                if (CursorOffset > 0)
                {
                    if (HasSelection)
                    {
                        if (CursorOffset == SelectionFrom)
                        {
                            CursorOffset = PrevCodepointOffset(CursorOffset);
                            SelectionFrom = CursorOffset;
                        }
                        else
                        {
                            CursorOffset = PrevCodepointOffset(CursorOffset);
                            SelectionTo = CursorOffset;
                        }
                    }
                    else
                    {
                        SelectionTo = CursorOffset;
                        CursorOffset = PrevCodepointOffset(CursorOffset);
                        SelectionFrom = CursorOffset;
                    }
                }
            }

            public void SelectRight()
            {
                if (CursorOffset < Text.Length)
                {
                    if (HasSelection)
                    {
                        if (CursorOffset == SelectionFrom)
                        {
                            CursorOffset = NextCodepointOffset(CursorOffset);
                            SelectionFrom = CursorOffset;
                        }
                        else
                        {
                            CursorOffset = NextCodepointOffset(CursorOffset);
                            SelectionTo = CursorOffset;
                        }
                    }
                    else
                    {
                        SelectionFrom = CursorOffset;
                        CursorOffset = NextCodepointOffset(CursorOffset);
                        SelectionTo = CursorOffset;
                    }
                }
            }

            public void GoHome()
            {
                if (HasSelection)
                {
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }

                if (CursorOffset > 0)
                {
                    CursorOffset = 0;
                }
            }

            public void GoEnd()
            {
                if (HasSelection)
                {
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }

                if (CursorOffset < Text.Length)
                {
                    CursorOffset = Text.Length;
                }
            }

            public void DeleteLeft()
            {
                if (HasSelection)
                {
                    Text = Text.Substring(0, SelectionFrom) + Text.Substring(SelectionTo);
                    CursorOffset = SelectionFrom;
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }
                else
                {
                    if (CursorOffset > 0)
                    {
                        int newOffset = PrevCodepointOffset(CursorOffset);
                        Text = Text.Substring(0, newOffset) + Text.Substring(CursorOffset);
                        CursorOffset = newOffset;
                    }
                }
            }

            public void DeleteRight()
            {
                if (HasSelection)
                {
                    Text = Text.Substring(0, SelectionFrom) + Text.Substring(SelectionTo);
                    CursorOffset = SelectionFrom;
                    SelectionFrom = -1;
                    SelectionTo = -1;
                }
                else
                {
                    if (CursorOffset < Text.Length)
                    {
                        Text = Text.Substring(0, CursorOffset) + Text.Substring(NextCodepointOffset(CursorOffset));
                    }
                }
            }

            public void SetCursorOffset(int offset)
            {
                CursorOffset = offset;
                SelectionFrom = -1;
                SelectionTo = -1;
            }

            private int PrevCodepointOffset(int offset)
            {
                return offset - (HasSurrogatePairAt(offset - 2) ? 2 : 1);
            }

            private int NextCodepointOffset(int offset)
            {
                return offset + (HasSurrogatePairAt(offset) ? 2 : 1);
            }

            private bool HasSurrogatePairAt(int offset)
            {
                return
                    offset >= 0 &&
                    offset + 1 < Text.Length &&
                    char.IsHighSurrogate(Text[offset]) &&
                    char.IsLowSurrogate(Text[offset + 1]);
            }
        }

        private enum ActionType
        {
            Undefined,
            Typing,
            Deletion
        }
    }
}