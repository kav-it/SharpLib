using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using SharpLib.Docking.Commands;

namespace SharpLib.Docking.Controls
{
    public class LayoutAnchorableItem : LayoutItem
    {
        #region Поля

        public static readonly DependencyProperty AutoHideCommandProperty;

        public static readonly DependencyProperty CanHideProperty;

        public static readonly DependencyProperty DockCommandProperty;

        public static readonly DependencyProperty HideCommandProperty;

        private readonly ReentrantFlag _visibilityReentrantFlag;

        private LayoutAnchorable _anchorable;

        private ICommand _defaultAutoHideCommand;

        private ICommand _defaultDockCommand;

        private ICommand _defaultHideCommand;

        #endregion

        #region Свойства

        public ICommand HideCommand
        {
            get { return (ICommand)GetValue(HideCommandProperty); }
            set { SetValue(HideCommandProperty, value); }
        }

        public ICommand AutoHideCommand
        {
            get { return (ICommand)GetValue(AutoHideCommandProperty); }
            set { SetValue(AutoHideCommandProperty, value); }
        }

        public ICommand DockCommand
        {
            get { return (ICommand)GetValue(DockCommandProperty); }
            set { SetValue(DockCommandProperty, value); }
        }

        public bool CanHide
        {
            get { return (bool)GetValue(CanHideProperty); }
            set { SetValue(CanHideProperty, value); }
        }

        #endregion

        #region Конструктор

        static LayoutAnchorableItem()
        {
            AutoHideCommandProperty = DependencyProperty.Register("AutoHideCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null, OnAutoHideCommandChanged, CoerceAutoHideCommandValue));
            CanHideProperty = DependencyProperty.Register("CanHide", typeof(bool), typeof(LayoutAnchorableItem), new FrameworkPropertyMetadata(true, OnCanHideChanged));
            DockCommandProperty = DependencyProperty.Register("DockCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null, OnDockCommandChanged, CoerceDockCommandValue));
            HideCommandProperty = DependencyProperty.Register("HideCommand", typeof(ICommand), typeof(LayoutAnchorableItem),
                new FrameworkPropertyMetadata(null, OnHideCommandChanged, CoerceHideCommandValue));
        }

        internal LayoutAnchorableItem()
        {
            _visibilityReentrantFlag = new ReentrantFlag();
        }

        #endregion

        #region Методы

        private static void OnHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem)d).RaiseHideCommandChanged(e);
        }

        private static object CoerceHideCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnAutoHideCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem)d).RaiseAutoHideCommandChanged(e);
        }

        private static object CoerceAutoHideCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnDockCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem)d).RaiseDockCommandChanged(e);
        }

        private static object CoerceDockCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnCanHideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableItem)d).RaiseCanHideChanged(e);
        }

        private bool CanExecuteAutoHideCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }

            if (LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null)
            {
                return false;
            }

            return _anchorable.CanAutoHide;
        }

        private void ExecuteAutoHideCommand(object parameter)
        {
            if (_anchorable != null && _anchorable.Root != null && _anchorable.Root.Manager != null)
            {
                _anchorable.Root.Manager.ExecuteAutoHideCommand(_anchorable);
            }
        }

        private bool CanExecuteDockCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }
            return LayoutElement.FindParent<LayoutAnchorableFloatingWindow>() != null;
        }

        private void ExecuteDockCommand(object parameter)
        {
            LayoutElement.Root.Manager.ExecuteDockCommand(_anchorable);
        }

        private bool CanExecuteHideCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }

            return _anchorable.CanHide || _anchorable.CanClose;
        }

        private void ExecuteHideCommand(object parameter)
        {
            if (_anchorable != null && _anchorable.Root != null && _anchorable.Root.Manager != null)
            {
                if (_anchorable.CanClose)
                {
                    _anchorable.Root.Manager.ExecuteCloseCommand(_anchorable);
                }
                else
                { 
                    _anchorable.Root.Manager.ExecuteHideCommand(_anchorable);
                }
            }
        }

        private void AnchorableIsVisibleChanged(object sender, EventArgs e)
        {
            if (_anchorable != null && _anchorable.Root != null)
            {
                if (_visibilityReentrantFlag.CanEnter)
                {
                    using (_visibilityReentrantFlag.Enter())
                    {
                        Visibility = _anchorable.IsVisible ? Visibility.Visible : Visibility.Hidden;
                    }
                }
            }
        }

        internal override void Attach(LayoutContent model)
        {
            _anchorable = model as LayoutAnchorable;
            if (_anchorable != null)
            {
                _anchorable.IsVisibleChanged += AnchorableIsVisibleChanged;
            }

            base.Attach(model);
        }

        internal override void Detach()
        {
            _anchorable.IsVisibleChanged -= AnchorableIsVisibleChanged;
            _anchorable = null;
            base.Detach();
        }

        protected override void Close()
        {
            var dockingManager = _anchorable.Root.Manager;
            dockingManager.ExecuteCloseCommand(_anchorable);
        }

        protected override void InitDefaultCommands()
        {
            _defaultHideCommand = new RelayCommand(ExecuteHideCommand, CanExecuteHideCommand);
            _defaultAutoHideCommand = new RelayCommand(ExecuteAutoHideCommand, CanExecuteAutoHideCommand);
            _defaultDockCommand = new RelayCommand(ExecuteDockCommand, CanExecuteDockCommand);

            base.InitDefaultCommands();
        }

        protected override void ClearDefaultBindings()
        {
            if (HideCommand == _defaultHideCommand)
            {
                BindingOperations.ClearBinding(this, HideCommandProperty);
            }
            if (AutoHideCommand == _defaultAutoHideCommand)
            {
                BindingOperations.ClearBinding(this, AutoHideCommandProperty);
            }
            if (DockCommand == _defaultDockCommand)
            {
                BindingOperations.ClearBinding(this, DockCommandProperty);
            }

            base.ClearDefaultBindings();
        }

        protected override void SetDefaultBindings()
        {
            if (HideCommand == null)
            {
                HideCommand = _defaultHideCommand;
            }
            if (AutoHideCommand == null)
            {
                AutoHideCommand = _defaultAutoHideCommand;
            }
            if (DockCommand == null)
            {
                DockCommand = _defaultDockCommand;
            }

            Visibility = _anchorable.IsVisible ? Visibility.Visible : Visibility.Hidden;
            base.SetDefaultBindings();
        }

        protected virtual void RaiseHideCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseAutoHideCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseDockCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseCanHideChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_anchorable != null)
            {
                _anchorable.CanHide = (bool)e.NewValue;
            }
        }

        protected override void RaiseVisibilityChanged()
        {
            if (_anchorable != null && _anchorable.Root != null)
            {
                if (_visibilityReentrantFlag.CanEnter)
                {
                    using (_visibilityReentrantFlag.Enter())
                    {
                        if (Visibility == Visibility.Hidden)
                        {
                            _anchorable.Hide(false);
                        }
                        else if (Visibility == Visibility.Visible)
                        {
                            _anchorable.Show();
                        }
                    }
                }
            }

            base.RaiseVisibilityChanged();
        }

        #endregion
    }
}