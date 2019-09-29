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
                    MakeCursorVisible();
                    Invalidate();
                }
            }
        }

        public override void Paint(ICanvas canvas, Rectangle area)
        {
            // todo: measure text
            int textHeight = 18;

            if (state.HasSelection)
            {
                int selectionFromX = state.SelectionFrom * 8;
                int selectionToX = state.SelectionTo * 8;
                // todo: check +/- 1 in width
                canvas.FillRectangle(selectionColor, selectionFromX, 0, selectionToX - selectionFromX, textHeight);
            }

            int cursorX = state.CursorOffset * 8;
            canvas.FillRectangle(cursorColor, cursorX, 0, 1, textHeight);

            canvas.DrawString(textColor, font, 0, 0, state.Text);
        }

        public void OnKeyDown(NKeyCode keyCode, NModifierKey modifierKey, bool autoRepeat)
        {
            if (keyCode == NKeyCode.A && modifierKey == NModifierKey.Control)
            {
                state.SelectAll();
                MakeCursorVisible();
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
                MakeCursorVisible();
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
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.LeftArrow && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.MoveCursorLeft();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.RightArrow && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.MoveCursorRight();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.LeftArrow && modifierKey == NModifierKey.Shift)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.SelectLeft();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.RightArrow && modifierKey == NModifierKey.Shift)
            {
                // todo: handle num-pad arrow keys
                SaveState(ActionType.Undefined);
                state.SelectRight();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.Home && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad home key
                SaveState(ActionType.Undefined);
                state.GoHome();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.End && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Undefined);
                state.GoEnd();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.Backspace && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Deletion);
                state.DeleteLeft();
                MakeCursorVisible();
                Invalidate();
            }
            else if (keyCode == NKeyCode.Delete && modifierKey == NModifierKey.None)
            {
                // todo: handle num-pad end key
                SaveState(ActionType.Deletion);
                state.DeleteRight();
                MakeCursorVisible();
                Invalidate();
            }
        }

        public void OnTextInput(string text)
        {
            // todo: limit text length
            SaveState(ActionType.Typing);
            state.EnterText(text);
            MakeCursorVisible();
            Invalidate();
        }

        private void MakeCursorVisible()
        {
            // todo: implement
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
            public string Text { get; set; }
            public int CursorOffset { get; set; }
            public int SelectionFrom { get; set; }
            public int SelectionTo { get; set; }

            public bool HasSelection => SelectionFrom != SelectionTo;

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
                    CursorOffset--;
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
                    CursorOffset++;
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
                            CursorOffset--;
                            SelectionFrom = CursorOffset;
                        }
                        else
                        {
                            CursorOffset--;
                            SelectionTo = CursorOffset;
                        }
                    }
                    else
                    {
                        SelectionTo = CursorOffset;
                        CursorOffset--;
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
                            CursorOffset++;
                            SelectionFrom = CursorOffset;
                        }
                        else
                        {
                            CursorOffset++;
                            SelectionTo = CursorOffset;
                        }
                    }
                    else
                    {
                        SelectionFrom = CursorOffset;
                        CursorOffset++;
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
                        Text = Text.Substring(0, CursorOffset - 1) + Text.Substring(CursorOffset);
                        CursorOffset--;
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
                        Text = Text.Substring(0, CursorOffset) + Text.Substring(CursorOffset + 1);
                    }
                }
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