using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering
{
    internal sealed class TextLayer : Layer
    {
        #region Поля

        private readonly List<VisualLineDrawingVisual> visuals = new List<VisualLineDrawingVisual>();

        internal int index;

        #endregion

        #region Свойства

        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }

        #endregion

        #region Конструктор

        public TextLayer(TextView textView)
            : base(textView, KnownLayer.Text)
        {
        }

        #endregion

        #region Методы

        internal void SetVisualLines(ICollection<VisualLine> visualLines)
        {
            foreach (VisualLineDrawingVisual v in visuals)
            {
                if (v.VisualLine.IsDisposed)
                {
                    RemoveVisualChild(v);
                }
            }
            visuals.Clear();
            foreach (VisualLine newLine in visualLines)
            {
                var v = newLine.Render();
                if (!v.IsAdded)
                {
                    AddVisualChild(v);
                    v.IsAdded = true;
                }
                visuals.Add(v);
            }
            InvalidateArrange();
        }

        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }

        protected override void ArrangeCore(Rect finalRect)
        {
            textView.ArrangeTextLayer(visuals);
        }

        #endregion
    }
}