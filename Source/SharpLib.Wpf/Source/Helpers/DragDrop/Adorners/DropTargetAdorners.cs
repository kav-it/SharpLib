using System;

namespace SharpLib.Wpf.Dragging
{
    public class DropTargetAdorners
    {
        #region Свойства

        public static Type Highlight
        {
            get { return typeof(DropTargetHighlightAdorner); }
        }

        public static Type Insert
        {
            get { return typeof(DropTargetInsertionAdorner); }
        }

        #endregion
    }
}