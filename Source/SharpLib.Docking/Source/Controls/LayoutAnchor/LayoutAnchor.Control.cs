using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    public class LayoutAnchorControl : Control, ILayoutControl
    {
        #region Поля

        public static readonly DependencyProperty SideProperty;

        private static readonly DependencyPropertyKey _sidePropertyKey;

        private readonly LayoutAnchorable _model;

        private DispatcherTimer _openUpTimer;

        #endregion

        #region Свойства

        public ILayoutElement Model
        {
            get { return _model; }
        }

        public AnchorSide Side
        {
            get { return (AnchorSide)GetValue(SideProperty); }
        }

        #endregion

        #region Конструктор

        static LayoutAnchorControl()
        {
            _sidePropertyKey = DependencyProperty.RegisterReadOnly("Side", typeof(AnchorSide), typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(AnchorSide.Left));
            SideProperty = _sidePropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorControl)));
            IsHitTestVisibleProperty.AddOwner(typeof(LayoutAnchorControl), new FrameworkPropertyMetadata(true));
        }

        internal LayoutAnchorControl(LayoutAnchorable model)
        {
            _model = model;
            _model.IsActiveChanged += _model_IsActiveChanged;
            _model.IsSelectedChanged += _model_IsSelectedChanged;

            SetSide(_model.FindParent<LayoutAnchorSide>().Side);
        }

        #endregion

        #region Методы

        private void _model_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_model.IsAutoHidden)
            {
                _model.IsSelectedChanged -= _model_IsSelectedChanged;
            }
            else if (_model.IsSelected)
            {
                _model.Root.Manager.ShowAutoHideWindow(this);
                _model.IsSelected = false;
            }
        }

        private void _model_IsActiveChanged(object sender, EventArgs e)
        {
            if (!_model.IsAutoHidden)
            {
                _model.IsActiveChanged -= _model_IsActiveChanged;
            }
            else if (_model.IsActive)
            {
                _model.Root.Manager.ShowAutoHideWindow(this);
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (!e.Handled)
            {
                _model.Root.Manager.ShowAutoHideWindow(this);
                _model.IsActive = true;
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!e.Handled)
            {
                _openUpTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                _openUpTimer.Interval = TimeSpan.FromMilliseconds(400);
                _openUpTimer.Tick += _openUpTimer_Tick;
                _openUpTimer.Start();
            }
        }

        private void _openUpTimer_Tick(object sender, EventArgs e)
        {
            _openUpTimer.Tick -= _openUpTimer_Tick;
            _openUpTimer.Stop();
            _openUpTimer = null;
            _model.Root.Manager.ShowAutoHideWindow(this);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            if (_openUpTimer != null)
            {
                _openUpTimer.Tick -= _openUpTimer_Tick;
                _openUpTimer.Stop();
                _openUpTimer = null;
            }
            base.OnMouseLeave(e);
        }

        protected void SetSide(AnchorSide value)
        {
            SetValue(_sidePropertyKey, value);
        }

        #endregion
    }
}