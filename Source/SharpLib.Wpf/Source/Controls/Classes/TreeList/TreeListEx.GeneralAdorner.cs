using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SharpLib.Wpf.Controls
{
    public class TreeListExGeneralAdorner : Adorner
    {
        #region ����

        private FrameworkElement _child;

        #endregion

        #region ��������

        public FrameworkElement Child
        {
            get { return _child; }
            set
            {
                if (!Equals(_child, value))
                {
                    RemoveVisualChild(_child);
                    RemoveLogicalChild(_child);
                    _child = value;
                    AddLogicalChild(value);
                    AddVisualChild(value);
                    InvalidateMeasure();
                }
            }
        }

        protected override int VisualChildrenCount
        {
            get { return _child == null ? 0 : 1; }
        }

        #endregion

        #region �����������

        public TreeListExGeneralAdorner(UIElement target) : base(target)
        {
        }

        #endregion

        #region ������

        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (_child != null)
            {
                _child.Measure(constraint);
                return _child.DesiredSize;
            }
            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_child != null)
            {
                _child.Arrange(new Rect(finalSize));
                return finalSize;
            }
            return new Size();
        }

        #endregion
    }
}