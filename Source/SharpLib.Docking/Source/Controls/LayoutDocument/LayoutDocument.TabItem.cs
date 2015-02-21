using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    public class LayoutDocumentTabItem : Control
    {
        #region Поля

        public static readonly DependencyProperty LayoutItemProperty;

        public static readonly DependencyProperty ModelProperty;

        private static readonly DependencyPropertyKey _layoutItemPropertyKey;

        private bool _isMouseDown;

        private Point _mouseDownPoint;

        private List<TabItem> _otherTabs;

        private List<Rect> _otherTabsScreenArea;

        private DocumentPaneTabPanel _parentDocumentTabPanel;

        private Rect _parentDocumentTabPanelScreenArea;

        #endregion

        #region Свойства

        public LayoutContent Model
        {
            get { return (LayoutContent)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public LayoutItem LayoutItem
        {
            get { return (LayoutItem)GetValue(LayoutItemProperty); }
        }

        #endregion

        #region Конструктор

        static LayoutDocumentTabItem()
        {
            _layoutItemPropertyKey = DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata((LayoutItem)null));
            ModelProperty = DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata(null, OnModelChanged));
            LayoutItemProperty = _layoutItemPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentTabItem), new FrameworkPropertyMetadata(typeof(LayoutDocumentTabItem)));
        }

        #endregion

        #region Методы

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutDocumentTabItem)d).OnModelChanged(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                SetLayoutItem(Model.Root.Manager.GetLayoutItemFromModel(Model));
            }
            else
            {
                SetLayoutItem(null);
            }
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(_layoutItemPropertyKey, value);
        }

        private void UpdateDragDetails()
        {
            _parentDocumentTabPanel = this.FindLogicalAncestor<DocumentPaneTabPanel>();
            _parentDocumentTabPanelScreenArea = _parentDocumentTabPanel.GetScreenArea();
            _otherTabs = _parentDocumentTabPanel.Children.Cast<TabItem>().Where(ch =>
                ch.Visibility != System.Windows.Visibility.Collapsed).ToList();
            var currentTabScreenArea = this.FindLogicalAncestor<TabItem>().GetScreenArea();
            _otherTabsScreenArea = _otherTabs.Select(ti =>
            {
                var screenArea = ti.GetScreenArea();
                return new Rect(screenArea.Left, screenArea.Top, currentTabScreenArea.Width, screenArea.Height);
            }).ToList();
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Model.IsActive = true;

            if (e.ClickCount == 1)
            {
                _mouseDownPoint = e.GetPosition(this);
                _isMouseDown = true;
            }
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isMouseDown)
            {
                var ptMouseMove = e.GetPosition(this);

                if (Math.Abs(ptMouseMove.X - _mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(ptMouseMove.Y - _mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    UpdateDragDetails();
                    CaptureMouse();
                    _isMouseDown = false;
                }
            }

            if (IsMouseCaptured)
            {
                var mousePosInScreenCoord = this.PointToScreenDPI(e.GetPosition(this));
                if (!_parentDocumentTabPanelScreenArea.Contains(mousePosInScreenCoord))
                {
                    ReleaseMouseCapture();
                    var manager = Model.Root.Manager;
                    manager.StartDraggingFloatingWindowForContent(Model);
                }
                else
                {
                    int indexOfTabItemWithMouseOver = _otherTabsScreenArea.FindIndex(r => r.Contains(mousePosInScreenCoord));
                    if (indexOfTabItemWithMouseOver >= 0)
                    {
                        var targetModel = _otherTabs[indexOfTabItemWithMouseOver].Content as LayoutContent;
                        var container = Model.Parent;
                        var containerPane = Model.Parent as ILayoutPane;
                        var childrenList = container.Children.ToList();
                        containerPane.MoveChild(childrenList.IndexOf(Model), childrenList.IndexOf(targetModel));
                        Model.IsActive = true;
                        _parentDocumentTabPanel.UpdateLayout();
                        UpdateDragDetails();
                    }
                }
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }
            _isMouseDown = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _isMouseDown = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            _isMouseDown = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (LayoutItem.CloseCommand.CanExecute(null))
                {
                    LayoutItem.CloseCommand.Execute(null);
                }
            }

            base.OnMouseDown(e);
        }

        #endregion
    }
}