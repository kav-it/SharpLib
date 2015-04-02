using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace SharpLib.Texter.Rendering
{
    internal class Layer : UIElement
    {
        #region Поля

        protected readonly KnownLayer knownLayer;

        protected readonly TextView textView;

        #endregion

        #region Конструктор

        public Layer(TextView textView, KnownLayer knownLayer)
        {
            Debug.Assert(textView != null);
            this.textView = textView;
            this.knownLayer = knownLayer;
            Focusable = false;
        }

        #endregion

        #region Методы

        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            return null;
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            textView.RenderBackground(drawingContext, knownLayer);
        }

        #endregion
    }
}