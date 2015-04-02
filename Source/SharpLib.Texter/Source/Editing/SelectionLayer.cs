using System;
using System.Windows;
using System.Windows.Media;

using SharpLib.Texter.Rendering;

namespace SharpLib.Texter.Editing
{
    internal sealed class SelectionLayer : Layer, IWeakEventListener
    {
        #region Поля

        private readonly TextArea textArea;

        #endregion

        #region Конструктор

        public SelectionLayer(TextArea textArea)
            : base(textArea.TextView, KnownLayer.Selection)
        {
            IsHitTestVisible = false;

            this.textArea = textArea;
            TextViewWeakEventManager.VisualLinesChanged.AddListener(textView, this);
            TextViewWeakEventManager.ScrollOffsetChanged.AddListener(textView, this);
        }

        #endregion

        #region Методы

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(TextViewWeakEventManager.VisualLinesChanged)
                || managerType == typeof(TextViewWeakEventManager.ScrollOffsetChanged))
            {
                InvalidateVisual();
                return true;
            }
            return false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var geoBuilder = new BackgroundGeometryBuilder();
            geoBuilder.AlignToMiddleOfPixels = true;
            geoBuilder.ExtendToFullWidthAtLineEnd = textArea.Selection.EnableVirtualSpace;
            geoBuilder.CornerRadius = textArea.SelectionCornerRadius;
            foreach (var segment in textArea.Selection.Segments)
            {
                geoBuilder.AddSegment(textView, segment);
            }
            var geometry = geoBuilder.CreateGeometry();
            if (geometry != null)
            {
                drawingContext.DrawGeometry(textArea.SelectionBrush, textArea.SelectionBorder, geometry);
            }
        }

        #endregion
    }
}