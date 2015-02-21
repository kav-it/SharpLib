using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class LayoutDocumentPaneControl : TabControl, ILayoutControl
    {
        #region Поля

        private readonly List<object> _logicalChildren = new List<object>();

        private readonly LayoutDocumentPane _model;

        #endregion

        #region Свойства

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get { return _logicalChildren.GetEnumerator(); }
        }

        public ILayoutElement Model
        {
            get { return _model; }
        }

        #endregion

        #region Конструктор

        static LayoutDocumentPaneControl()
        {
            FocusableProperty.OverrideMetadata(typeof(LayoutDocumentPaneControl), new FrameworkPropertyMetadata(false));
        }

        internal LayoutDocumentPaneControl(LayoutDocumentPane model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            _model = model;
            SetBinding(ItemsSourceProperty, new Binding("Model.Children")
            {
                Source = this
            });
            SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection")
            {
                Source = this
            });

            LayoutUpdated += OnLayoutUpdated;
        }

        #endregion

        #region Методы

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var modelWithAtcualSize = _model as ILayoutPositionableElementWithActualSize;
            modelWithAtcualSize.ActualWidth = ActualWidth;
            modelWithAtcualSize.ActualHeight = ActualHeight;
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            System.Diagnostics.Trace.WriteLine(string.Format("OnGotKeyboardFocus({0}, {1})", e.Source, e.NewFocus));
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (_model.SelectedContent != null)
            {
                _model.SelectedContent.IsActive = true;
            }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!e.Handled && _model.SelectedContent != null)
            {
                _model.SelectedContent.IsActive = true;
            }
        }

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (!e.Handled && _model.SelectedContent != null)
            {
                _model.SelectedContent.IsActive = true;
            }
        }

        #endregion
    }
}