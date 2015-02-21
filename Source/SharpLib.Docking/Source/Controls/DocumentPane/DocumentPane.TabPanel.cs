using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class DocumentPaneTabPanel : Panel
    {
        #region Конструктор

        public DocumentPaneTabPanel()
        {
            FlowDirection = FlowDirection.LeftToRight;
        }

        #endregion

        #region Методы

        protected override Size MeasureOverride(Size availableSize)
        {
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != Visibility.Collapsed);

            var desideredSize = new Size();
            foreach (FrameworkElement child in Children)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                desideredSize.Width += child.DesiredSize.Width;

                desideredSize.Height = Math.Max(desideredSize.Height, child.DesiredSize.Height);
            }

            return new Size(Math.Min(desideredSize.Width, availableSize.Width), desideredSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            restart:
            var visibleChildren = Children.Cast<UIElement>().Where(ch => ch.Visibility != System.Windows.Visibility.Collapsed);
            double offset = 0.0;
            bool skipAllOthers = false;
            foreach (TabItem doc in visibleChildren)
            {
                var layoutContent = doc.Content as LayoutContent;
                if (skipAllOthers || offset + doc.DesiredSize.Width > finalSize.Width)
                {
                    if (layoutContent.IsSelected)
                    {
                        var parentContainer = layoutContent.Parent;
                        var parentSelector = layoutContent.Parent as ILayoutContentSelector;
                        var parentPane = layoutContent.Parent as ILayoutPane;
                        int contentIndex = parentSelector.IndexOf(layoutContent);
                        if (contentIndex > 0 &&
                            parentContainer.ChildrenCount > 1)
                        {
                            parentPane.MoveChild(contentIndex, 0);
                            parentSelector.SelectedContentIndex = 0;
                            goto restart;
                        }
                    }
                    doc.Visibility = Visibility.Hidden;
                    skipAllOthers = true;
                }
                else
                {
                    doc.Visibility = Visibility.Visible;
                    doc.Arrange(new Rect(offset, 0.0, doc.DesiredSize.Width, finalSize.Height));
                    offset += doc.ActualWidth + doc.Margin.Left + doc.Margin.Right;
                }
            }

            return finalSize;
        }

        #endregion
    }
}