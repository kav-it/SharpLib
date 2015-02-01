using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SharpLib.Wpf.Dragging
{
    internal class DragAdorner : Adorner
    {
        #region Поля

        private readonly AdornerLayer _adornerLayer;

        private readonly UIElement _adornment;

        private Point _mousePosition;

        #endregion

        #region Свойства

        public Point MousePosition
        {
            get { return _mousePosition; }
            set
            {
                if (_mousePosition != value)
                {
                    _mousePosition = value;
                    _adornerLayer.Update(AdornedElement);
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion

        #region Конструктор

        public DragAdorner(UIElement adornedElement, UIElement adornment)
            : base(adornedElement)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            _adornerLayer.Add(this);
            _adornment = adornment;
            IsHitTestVisible = false;
        }

        #endregion

        #region Методы

        public void Detatch()
        {
            _adornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _adornment.Measure(constraint);
            return _adornment.DesiredSize;
        }

        #endregion
    }
}