using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpLib.Wpf.Dragging
{
    public class DropTargetInsertionAdorner : DropTargetAdorner
    {
        #region Поля

        private static readonly Pen _pen;

        private static readonly PathGeometry _triangle;

        #endregion

        #region Конструктор

        static DropTargetInsertionAdorner()
        {
            const int TRIANGLE_SIZE = 5;

            _pen = new Pen(Brushes.Gray, 2);
            _pen.Freeze();

            var firstLine = new LineSegment(new Point(0, -TRIANGLE_SIZE), false);
            firstLine.Freeze();
            var secondLine = new LineSegment(new Point(0, TRIANGLE_SIZE), false);
            secondLine.Freeze();

            var figure = new PathFigure
            {
                StartPoint = new Point(TRIANGLE_SIZE, 0)
            };
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            _triangle = new PathGeometry();
            _triangle.Figures.Add(figure);
            _triangle.Freeze();
        }

        public DropTargetInsertionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        #endregion

        #region Методы

        protected override void OnRender(DrawingContext drawingContext)
        {
            var itemsControl = DropInfo.VisualTarget as ItemsControl;

            if (itemsControl != null)
            {
                ItemsControl itemParent = DropInfo.VisualTargetItem != null 
                    ? ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem) 
                    : itemsControl;

                var index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);

                var lastItemInGroup = false;
                var targetGroup = DropInfo.TargetGroup;
                if (targetGroup != null && targetGroup.IsBottomLevel && DropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
                {
                    var indexOf = targetGroup.Items.IndexOf(DropInfo.TargetItem);
                    lastItemInGroup = indexOf == targetGroup.ItemCount - 1;
                    if (lastItemInGroup && DropInfo.InsertIndex != itemParent.Items.Count)
                    {
                        index--;
                    }
                }

                var itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

                if (itemContainer == null)
                {
                    return;
                }

                var itemRect = new Rect(itemContainer.TranslatePoint(new Point(), AdornedElement), itemContainer.RenderSize);
                var rotation = 0;

                Point point2;
                Point point1;
                if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
                {
                    if (DropInfo.InsertIndex == itemParent.Items.Count || lastItemInGroup)
                    {
                        itemRect.Y += itemContainer.RenderSize.Height;
                    }

                    point1 = new Point(itemRect.X, itemRect.Y);
                    point2 = new Point(itemRect.Right, itemRect.Y);
                }
                else
                {
                    var itemRectX = itemRect.X;

                    if (DropInfo.VisualTargetFlowDirection == FlowDirection.LeftToRight && DropInfo.InsertIndex == itemParent.Items.Count)
                    {
                        itemRectX += itemContainer.RenderSize.Width;
                    }
                    else if (DropInfo.VisualTargetFlowDirection == FlowDirection.RightToLeft && DropInfo.InsertIndex != itemParent.Items.Count)
                    {
                        itemRectX += itemContainer.RenderSize.Width;
                    }

                    point1 = new Point(itemRectX, itemRect.Y);
                    point2 = new Point(itemRectX, itemRect.Bottom);
                    rotation = 90;
                }

                drawingContext.DrawLine(_pen, point1, point2);
                DrawTriangle(drawingContext, point1, rotation);
                DrawTriangle(drawingContext, point2, 180 + rotation);
            }
        }

        private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
        {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(rotation));

            drawingContext.DrawGeometry(_pen.Brush, null, _triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }

        #endregion
    }
}