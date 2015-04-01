using System;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    public class CompletionListBox : ListBox
    {
        #region Поля

        internal ScrollViewer scrollViewer;

        #endregion

        #region Свойства

        public int FirstVisibleItem
        {
            get
            {
                if (scrollViewer == null || scrollViewer.ExtentHeight == 0)
                {
                    return 0;
                }
                return (int)(Items.Count * scrollViewer.VerticalOffset / scrollViewer.ExtentHeight);
            }
            set
            {
                value = value.CoerceValue(0, Items.Count - VisibleItemCount);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset((double)value / Items.Count * scrollViewer.ExtentHeight);
                }
            }
        }

        public int VisibleItemCount
        {
            get
            {
                if (scrollViewer == null || scrollViewer.ExtentHeight == 0)
                {
                    return 10;
                }
                return Math.Max(
                    3,
                    (int)Math.Ceiling(Items.Count * scrollViewer.ViewportHeight
                                      / scrollViewer.ExtentHeight));
            }
        }

        #endregion

        #region Методы

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            scrollViewer = null;
            if (VisualChildrenCount > 0)
            {
                var border = GetVisualChild(0) as Border;
                if (border != null)
                {
                    scrollViewer = border.Child as ScrollViewer;
                }
            }
        }

        public void ClearSelection()
        {
            SelectedIndex = -1;
        }

        public void SelectIndex(int index)
        {
            if (index >= Items.Count)
            {
                index = Items.Count - 1;
            }
            if (index < 0)
            {
                index = 0;
            }
            SelectedIndex = index;
            ScrollIntoView(SelectedItem);
        }

        public void CenterViewOn(int index)
        {
            FirstVisibleItem = index - VisibleItemCount / 2;
        }

        #endregion
    }
}