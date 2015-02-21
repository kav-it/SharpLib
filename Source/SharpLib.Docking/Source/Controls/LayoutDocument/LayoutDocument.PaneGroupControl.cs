using System.Windows;
using System.Windows.Controls;

using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    public class LayoutDocumentPaneGroupControl : LayoutGridControl<ILayoutDocumentPane>
    {
        #region Поля

        private readonly LayoutDocumentPaneGroup _model;

        #endregion

        #region Конструктор

        internal LayoutDocumentPaneGroupControl(LayoutDocumentPaneGroup model)
            : base(model, model.Orientation)
        {
            _model = model;
        }

        #endregion

        #region Методы

        protected override void OnFixChildrenDockLengths()
        {
            #region Setup DockWidth/Height for children

            if (_model.Orientation == Orientation.Horizontal)
            {
                foreach (var t in _model.Children)
                {
                    var childModel = t as ILayoutPositionableElement;
                    if (childModel != null && !childModel.DockWidth.IsStar)
                    {
                        childModel.DockWidth = new GridLength(1.0, GridUnitType.Star);
                    }
                }
            }
            else
            {
                foreach (var t in _model.Children)
                {
                    var childModel = t as ILayoutPositionableElement;
                    if (childModel != null && !childModel.DockHeight.IsStar)
                    {
                        childModel.DockHeight = new GridLength(1.0, GridUnitType.Star);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}