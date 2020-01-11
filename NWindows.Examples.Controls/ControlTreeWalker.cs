namespace NWindows.Examples.Controls
{
    public static class ControlTreeWalker
    {
        public static Control GetNextControl(Control control)
        {
            if (control.Children.Count != 0)
            {
                return control.Children.First;
            }

            for (; control.Parent != null; control = control.Parent)
            {
                if (control.Parent.Children.TryGetNextValue(control, out var nextSibling))
                {
                    return nextSibling;
                }
            }

            return control;
        }

        public static Control GetPreviousControl(Control control)
        {
            if (control.Parent != null)
            {
                if (control.Parent.Children.TryGetPreviousValue(control, out var prevSibling))
                {
                    return GetLastChild(prevSibling);
                }

                return control.Parent;
            }

            return GetLastChild(control);
        }

        private static Control GetLastChild(Control control)
        {
            while (control.Children.Count != 0)
            {
                control = control.Children.Last;
            }

            return control;
        }
    }
}