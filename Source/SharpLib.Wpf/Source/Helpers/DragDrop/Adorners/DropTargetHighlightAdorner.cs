using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpLib.Wpf.Dragging
{
    public class DropTargetHighlightAdorner : DropTargetAdorner
    {
        #region Конструктор

        public DropTargetHighlightAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region Методы

        protected override void OnRender(DrawingContext drawingContext)
        {
            var visualTargetItem = DropInfo.VisualTargetItem;
            if (visualTargetItem != null)
            {
                var rect = Rect.Empty;

                var tvItem = visualTargetItem as TreeViewItem;
                if (tvItem != null && VisualTreeHelper.GetChildrenCount(tvItem) > 0)
                {
                    var descendant = VisualTreeHelper.GetDescendantBounds(tvItem);
                    rect = new Rect(tvItem.TranslatePoint(new Point(), AdornedElement), new Size(descendant.Width + 4, tvItem.ActualHeight));
                }
                if (rect.IsEmpty)
                {
                    rect = new Rect(visualTargetItem.TranslatePoint(new Point(), AdornedElement), VisualTreeHelper.GetDescendantBounds(visualTargetItem).Size);
                }
                drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
            }
        }

        #endregion
    }
}