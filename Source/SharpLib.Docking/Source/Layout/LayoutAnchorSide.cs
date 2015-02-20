using System;
using System.Windows.Markup;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
    {
        #region Поля

        private AnchorSide _side;

        #endregion

        #region Свойства

        public AnchorSide Side
        {
            get { return _side; }
            private set
            {
                if (_side == value)
                {
                    return;
                }
                RaisePropertyChanging("Side");
                _side = value;
                RaisePropertyChanged("Side");
            }
        }

        #endregion

        #region Методы

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            base.OnParentChanged(oldValue, newValue);

            UpdateSide();
        }

        private void UpdateSide()
        {
            if (Equals(Root.LeftSide, this))
            {
                Side = AnchorSide.Left;
            }
            else if (Equals(Root.TopSide, this))
            {
                Side = AnchorSide.Top;
            }
            else if (Equals(Root.RightSide, this))
            {
                Side = AnchorSide.Right;
            }
            else if (Equals(Root.BottomSide, this))
            {
                Side = AnchorSide.Bottom;
            }
        }

        #endregion
    }
}