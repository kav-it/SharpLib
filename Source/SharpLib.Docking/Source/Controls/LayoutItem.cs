﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using SharpLib.Docking.Commands;
using SharpLib.Docking;

namespace SharpLib.Docking.Controls
{
    public abstract class LayoutItem : FrameworkElement
    {
        #region Поля

        public static readonly DependencyProperty ActivateCommandProperty;

        public static readonly DependencyProperty CanCloseProperty;

        public static readonly DependencyProperty CanFloatProperty;

        public static readonly DependencyProperty CloseAllButThisCommandProperty;

        public static readonly DependencyProperty CloseCommandProperty;

        public static readonly DependencyProperty ContentIdProperty;

        public static readonly DependencyProperty DockAsDocumentCommandProperty;

        public static readonly DependencyProperty FloatCommandProperty;

        public static readonly DependencyProperty IconSourceProperty;

        public static readonly DependencyProperty IsActiveProperty;

        public static readonly DependencyProperty IsSelectedProperty;

        public static readonly DependencyProperty MoveToNextTabGroupCommandProperty;

        public static readonly DependencyProperty MoveToPreviousTabGroupCommandProperty;

        public static readonly DependencyProperty NewHorizontalTabGroupCommandProperty;

        public static readonly DependencyProperty NewVerticalTabGroupCommandProperty;

        public static readonly DependencyProperty TitleProperty;

        private readonly ReentrantFlag _isActiveReentrantFlag;

        private readonly ReentrantFlag _isSelectedReentrantFlag;

        private ICommand _defaultActivateCommand;

        private ICommand _defaultCloseAllButThisCommand;

        private ICommand _defaultCloseCommand;

        private ICommand _defaultDockAsDocumentCommand;

        private ICommand _defaultFloatCommand;

        private ICommand _defaultMoveToNextTabGroupCommand;

        private ICommand _defaultMoveToPreviousTabGroupCommand;

        private ICommand _defaultNewHorizontalTabGroupCommand;

        private ICommand _defaultNewVerticalTabGroupCommand;

        private ContentPresenter _view;

        #endregion

        #region Свойства

        public LayoutContent LayoutElement { get; private set; }

        public object Model { get; private set; }

        public ContentPresenter View
        {
            get
            {
                if (_view == null)
                {
                    _view = new ContentPresenter();

                    _view.SetBinding(ContentPresenter.ContentProperty, new Binding("Content")
                    {
                        Source = LayoutElement
                    });
                    _view.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding("LayoutItemTemplate")
                    {
                        Source = LayoutElement.Root.Manager
                    });
                    _view.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding("LayoutItemTemplateSelector")
                    {
                        Source = LayoutElement.Root.Manager
                    });
                    LayoutElement.Root.Manager.InternalAddLogicalChild(_view);
                }

                return _view;
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        public string ContentId
        {
            get { return (string)GetValue(ContentIdProperty); }
            set { SetValue(ContentIdProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public bool CanFloat
        {
            get { return (bool)GetValue(CanFloatProperty); }
            set { SetValue(CanFloatProperty, value); }
        }

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        public ICommand FloatCommand
        {
            get { return (ICommand)GetValue(FloatCommandProperty); }
            set { SetValue(FloatCommandProperty, value); }
        }

        public ICommand DockAsDocumentCommand
        {
            get { return (ICommand)GetValue(DockAsDocumentCommandProperty); }
            set { SetValue(DockAsDocumentCommandProperty, value); }
        }

        public ICommand CloseAllButThisCommand
        {
            get { return (ICommand)GetValue(CloseAllButThisCommandProperty); }
            set { SetValue(CloseAllButThisCommandProperty, value); }
        }

        public ICommand ActivateCommand
        {
            get { return (ICommand)GetValue(ActivateCommandProperty); }
            set { SetValue(ActivateCommandProperty, value); }
        }

        public ICommand NewVerticalTabGroupCommand
        {
            get { return (ICommand)GetValue(NewVerticalTabGroupCommandProperty); }
            set { SetValue(NewVerticalTabGroupCommandProperty, value); }
        }

        public ICommand NewHorizontalTabGroupCommand
        {
            get { return (ICommand)GetValue(NewHorizontalTabGroupCommandProperty); }
            set { SetValue(NewHorizontalTabGroupCommandProperty, value); }
        }

        public ICommand MoveToNextTabGroupCommand
        {
            get { return (ICommand)GetValue(MoveToNextTabGroupCommandProperty); }
            set { SetValue(MoveToNextTabGroupCommandProperty, value); }
        }

        public ICommand MoveToPreviousTabGroupCommand
        {
            get { return (ICommand)GetValue(MoveToPreviousTabGroupCommandProperty); }
            set { SetValue(MoveToPreviousTabGroupCommandProperty, value); }
        }

        #endregion

        #region Конструктор

        static LayoutItem()
        {
            ActivateCommandProperty = DependencyProperty.Register("ActivateCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnActivateCommandChanged, CoerceActivateCommandValue));
            CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(true, OnCanCloseChanged));
            CanFloatProperty = DependencyProperty.Register("CanFloat", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(true, OnCanFloatChanged));
            CloseAllButThisCommandProperty = DependencyProperty.Register("CloseAllButThisCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnCloseAllButThisCommandChanged, CoerceCloseAllButThisCommandValue));
            CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnCloseCommandChanged, CoerceCloseCommandValue));
            ContentIdProperty = DependencyProperty.Register("ContentId", typeof(string), typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnContentIdChanged));
            DockAsDocumentCommandProperty = DependencyProperty.Register("DockAsDocumentCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnDockAsDocumentCommandChanged, CoerceDockAsDocumentCommandValue));
            FloatCommandProperty = DependencyProperty.Register("FloatCommand", typeof(ICommand), typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnFloatCommandChanged, CoerceFloatCommandValue));
            IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnIconSourceChanged));
            IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(false, OnIsActiveChanged));
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LayoutItem), new FrameworkPropertyMetadata(false, OnIsSelectedChanged));
            MoveToNextTabGroupCommandProperty = DependencyProperty.Register("MoveToNextTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnMoveToNextTabGroupCommandChanged));
            MoveToPreviousTabGroupCommandProperty = DependencyProperty.Register("MoveToPreviousTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnMoveToPreviousTabGroupCommandChanged));
            NewHorizontalTabGroupCommandProperty = DependencyProperty.Register("NewHorizontalTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnNewHorizontalTabGroupCommandChanged));
            NewVerticalTabGroupCommandProperty = DependencyProperty.Register("NewVerticalTabGroupCommand", typeof(ICommand), typeof(LayoutItem),
                new FrameworkPropertyMetadata(null, OnNewVerticalTabGroupCommandChanged));
            TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnTitleChanged));

            ToolTipProperty.OverrideMetadata(typeof(LayoutItem), new FrameworkPropertyMetadata(null, OnToolTipChanged));
            VisibilityProperty.OverrideMetadata(typeof(LayoutItem), new FrameworkPropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        }

        internal LayoutItem()
        {
            _isSelectedReentrantFlag = new ReentrantFlag();
            _isActiveReentrantFlag = new ReentrantFlag();
        }

        #endregion

        #region Методы

        private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseIconSourceChanged(e);
        }

        private static void OnContentIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseContentIdChanged(e);
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseTitleChanged(e);
        }

        private static void OnToolTipChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)s).RaiseToolTipChanged();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseIsSelectedChanged(e);
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseIsActiveChanged(e);
        }

        private static void OnCanCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseCanCloseChanged(e);
        }

        private static void OnVisibilityChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)s).RaiseVisibilityChanged();
        }

        private static void OnCanFloatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseCanFloatChanged(e);
        }

        private static void OnCloseCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseCloseCommandChanged(e);
        }

        private static object CoerceCloseCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnCloseAllButThisCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseCloseAllButThisCommandChanged(e);
        }

        private static object CoerceCloseAllButThisCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceActivateCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnNewVerticalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseNewVerticalTabGroupCommandChanged(e);
        }

        private static void OnNewHorizontalTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseNewHorizontalTabGroupCommandChanged(e);
        }

        private static void OnMoveToNextTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseMoveToNextTabGroupCommandChanged(e);
        }

        private static void OnMoveToPreviousTabGroupCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseMoveToPreviousTabGroupCommandChanged(e);
        }

        private static void OnFloatCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseFloatCommandChanged(e);
        }

        private static object CoerceFloatCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static object CoerceDockAsDocumentCommandValue(DependencyObject d, object value)
        {
            return value;
        }

        private static void OnActivateCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseActivateCommandChanged(e);
        }

        private static void OnDockAsDocumentCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutItem)d).RaiseDockAsDocumentCommandChanged(e);
        }

        private void LayoutElement_IsActiveChanged(object sender, EventArgs e)
        {
            if (_isActiveReentrantFlag.CanEnter)
            {
                using (_isActiveReentrantFlag.Enter())
                {
                    var bnd = BindingOperations.GetBinding(this, IsActiveProperty);
                    IsActive = LayoutElement.IsActive;
                    var bnd2 = BindingOperations.GetBinding(this, IsActiveProperty);
                }
            }
        }

        private void LayoutElement_IsSelectedChanged(object sender, EventArgs e)
        {
            if (_isSelectedReentrantFlag.CanEnter)
            {
                using (_isSelectedReentrantFlag.Enter())
                {
                    IsSelected = LayoutElement.IsSelected;
                }
            }
        }

        private void RaiseToolTipChanged()
        {
            if (LayoutElement != null)
            {
                LayoutElement.ToolTip = ToolTip;
            }
        }

        private void ExecuteCloseCommand(object parameter)
        {
            Close();
        }

        private bool CanExecuteCloseCommand(object parameter)
        {
            return LayoutElement != null && LayoutElement.CanClose;
        }

        private bool CanExecuteFloatCommand(object anchorable)
        {
            return LayoutElement != null && LayoutElement.CanFloat && LayoutElement.FindParent<LayoutFloatingWindow>() == null;
        }

        private void ExecuteFloatCommand(object parameter)
        {
            LayoutElement.Root.Manager.ExecuteFloatCommand(LayoutElement);
        }

        private bool CanExecuteDockAsDocumentCommand(object parameter)
        {
            return LayoutElement != null && LayoutElement.FindParent<LayoutDocumentPane>() == null;
        }

        private void ExecuteDockAsDocumentCommand(object parameter)
        {
            LayoutElement.Root.Manager.ExecuteDockAsDocumentCommand(LayoutElement);
        }

        private bool CanExecuteCloseAllButThisCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }
            var root = LayoutElement.Root;
            if (root == null)
            {
                return false;
            }

            return LayoutElement.Root.Manager.Layout.
                Descendents().OfType<LayoutContent>().Any(d => d != LayoutElement && (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow));
        }

        private void ExecuteCloseAllButThisCommand(object parameter)
        {
            LayoutElement.Root.Manager.ExecuteCloseAllButThisCommand(LayoutElement);
        }

        private bool CanExecuteActivateCommand(object parameter)
        {
            return LayoutElement != null;
        }

        private void ExecuteActivateCommand(object parameter)
        {
            LayoutElement.Root.Manager.ExecuteContentActivateCommand(LayoutElement);
        }

        private bool CanExecuteNewVerticalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return ((parentDocumentGroup == null ||
                     parentDocumentGroup.ChildrenCount == 1 ||
                     parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                     parentDocumentGroup.Orientation == Orientation.Horizontal) &&
                    parentDocumentPane != null &&
                    parentDocumentPane.ChildrenCount > 1);
        }

        private void ExecuteNewVerticalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                var grandParent = parentDocumentPane.Parent;
                parentDocumentGroup = new LayoutDocumentPaneGroup
                {
                    Orientation = Orientation.Horizontal
                };
                grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                parentDocumentGroup.Children.Add(parentDocumentPane);
            }
            parentDocumentGroup.Orientation = Orientation.Horizontal;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private bool CanExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return ((parentDocumentGroup == null ||
                     parentDocumentGroup.ChildrenCount == 1 ||
                     parentDocumentGroup.Root.Manager.AllowMixedOrientation ||
                     parentDocumentGroup.Orientation == Orientation.Vertical) &&
                    parentDocumentPane != null &&
                    parentDocumentPane.ChildrenCount > 1);
        }

        private void ExecuteNewHorizontalTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;

            if (parentDocumentGroup == null)
            {
                var grandParent = parentDocumentPane.Parent;
                parentDocumentGroup = new LayoutDocumentPaneGroup
                {
                    Orientation = Orientation.Vertical
                };
                grandParent.ReplaceChild(parentDocumentPane, parentDocumentGroup);
                parentDocumentGroup.Children.Add(parentDocumentPane);
            }
            parentDocumentGroup.Orientation = Orientation.Vertical;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            parentDocumentGroup.InsertChildAt(indexOfParentPane + 1, new LayoutDocumentPane(layoutElement));
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private bool CanExecuteMoveToNextTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }

            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return (parentDocumentGroup != null &&
                    parentDocumentPane != null &&
                    parentDocumentGroup.ChildrenCount > 1 &&
                    parentDocumentGroup.IndexOfChild(parentDocumentPane) < parentDocumentGroup.ChildrenCount - 1 &&
                    parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) + 1] is LayoutDocumentPane);
        }

        private void ExecuteMoveToNextTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane + 1] as LayoutDocumentPane;
            nextDocumentPane.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        private bool CanExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            if (LayoutElement == null)
            {
                return false;
            }
            var parentDocumentGroup = LayoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = LayoutElement.Parent as LayoutDocumentPane;
            return (parentDocumentGroup != null &&
                    parentDocumentPane != null &&
                    parentDocumentGroup.ChildrenCount > 1 &&
                    parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                    parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is LayoutDocumentPane);
        }

        private void ExecuteMoveToPreviousTabGroupCommand(object parameter)
        {
            var layoutElement = LayoutElement;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;
            nextDocumentPane.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.Root.CollectGarbage();
        }

        protected virtual void InitDefaultCommands()
        {
            _defaultCloseCommand = new RelayCommand(ExecuteCloseCommand, CanExecuteCloseCommand);
            _defaultFloatCommand = new RelayCommand(ExecuteFloatCommand, CanExecuteFloatCommand);
            _defaultDockAsDocumentCommand = new RelayCommand(ExecuteDockAsDocumentCommand, CanExecuteDockAsDocumentCommand);
            _defaultCloseAllButThisCommand = new RelayCommand(ExecuteCloseAllButThisCommand, CanExecuteCloseAllButThisCommand);
            _defaultActivateCommand = new RelayCommand(ExecuteActivateCommand, CanExecuteActivateCommand);
            _defaultNewVerticalTabGroupCommand = new RelayCommand(ExecuteNewVerticalTabGroupCommand, CanExecuteNewVerticalTabGroupCommand);
            _defaultNewHorizontalTabGroupCommand = new RelayCommand(ExecuteNewHorizontalTabGroupCommand, CanExecuteNewHorizontalTabGroupCommand);
            _defaultMoveToNextTabGroupCommand = new RelayCommand(ExecuteMoveToNextTabGroupCommand, CanExecuteMoveToNextTabGroupCommand);
            _defaultMoveToPreviousTabGroupCommand = new RelayCommand(ExecuteMoveToPreviousTabGroupCommand, CanExecuteMoveToPreviousTabGroupCommand);
        }

        protected virtual void ClearDefaultBindings()
        {
            if (CloseCommand == _defaultCloseCommand)
            {
                BindingOperations.ClearBinding(this, CloseCommandProperty);
            }
            if (FloatCommand == _defaultFloatCommand)
            {
                BindingOperations.ClearBinding(this, FloatCommandProperty);
            }
            if (DockAsDocumentCommand == _defaultDockAsDocumentCommand)
            {
                BindingOperations.ClearBinding(this, DockAsDocumentCommandProperty);
            }
            if (CloseAllButThisCommand == _defaultCloseAllButThisCommand)
            {
                BindingOperations.ClearBinding(this, CloseAllButThisCommandProperty);
            }
            if (ActivateCommand == _defaultActivateCommand)
            {
                BindingOperations.ClearBinding(this, ActivateCommandProperty);
            }
            if (NewVerticalTabGroupCommand == _defaultNewVerticalTabGroupCommand)
            {
                BindingOperations.ClearBinding(this, NewVerticalTabGroupCommandProperty);
            }
            if (NewHorizontalTabGroupCommand == _defaultNewHorizontalTabGroupCommand)
            {
                BindingOperations.ClearBinding(this, NewHorizontalTabGroupCommandProperty);
            }
            if (MoveToNextTabGroupCommand == _defaultMoveToNextTabGroupCommand)
            {
                BindingOperations.ClearBinding(this, MoveToNextTabGroupCommandProperty);
            }
            if (MoveToPreviousTabGroupCommand == _defaultMoveToPreviousTabGroupCommand)
            {
                BindingOperations.ClearBinding(this, MoveToPreviousTabGroupCommandProperty);
            }
        }

        protected virtual void SetDefaultBindings()
        {
            if (CloseCommand == null)
            {
                CloseCommand = _defaultCloseCommand;
            }
            if (FloatCommand == null)
            {
                FloatCommand = _defaultFloatCommand;
            }
            if (DockAsDocumentCommand == null)
            {
                DockAsDocumentCommand = _defaultDockAsDocumentCommand;
            }
            if (CloseAllButThisCommand == null)
            {
                CloseAllButThisCommand = _defaultCloseAllButThisCommand;
            }
            if (ActivateCommand == null)
            {
                ActivateCommand = _defaultActivateCommand;
            }
            if (NewVerticalTabGroupCommand == null)
            {
                NewVerticalTabGroupCommand = _defaultNewVerticalTabGroupCommand;
            }
            if (NewHorizontalTabGroupCommand == null)
            {
                NewHorizontalTabGroupCommand = _defaultNewHorizontalTabGroupCommand;
            }
            if (MoveToNextTabGroupCommand == null)
            {
                MoveToNextTabGroupCommand = _defaultMoveToNextTabGroupCommand;
            }
            if (MoveToPreviousTabGroupCommand == null)
            {
                MoveToPreviousTabGroupCommand = _defaultMoveToPreviousTabGroupCommand;
            }

            IsSelected = LayoutElement.IsSelected;
            IsActive = LayoutElement.IsActive;
        }

        protected virtual void RaiseTitleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
            {
                LayoutElement.Title = (string)e.NewValue;
            }
        }

        protected virtual void RaiseIconSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
            {
                LayoutElement.IconSource = IconSource;
            }
        }

        protected virtual void RaiseVisibilityChanged()
        {
            if (LayoutElement != null && Visibility == Visibility.Collapsed)
            {
                LayoutElement.Close();
            }
        }

        protected virtual void RaiseContentIdChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
            {
                LayoutElement.ContentId = (string)e.NewValue;
            }
        }

        protected virtual void RaiseIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isSelectedReentrantFlag.CanEnter)
            {
                using (_isSelectedReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                    {
                        LayoutElement.IsSelected = (bool)e.NewValue;
                    }
                }
            }
        }

        protected virtual void RaiseIsActiveChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_isActiveReentrantFlag.CanEnter)
            {
                using (_isActiveReentrantFlag.Enter())
                {
                    if (LayoutElement != null)
                    {
                        LayoutElement.IsActive = (bool)e.NewValue;
                    }
                }
            }
        }

        protected virtual void RaiseCanCloseChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
            {
                LayoutElement.CanClose = (bool)e.NewValue;
            }
        }

        protected virtual void RaiseCanFloatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LayoutElement != null)
            {
                LayoutElement.CanFloat = (bool)e.NewValue;
            }
        }

        protected virtual void RaiseCloseCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected abstract void Close();

        protected virtual void RaiseFloatCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void Float()
        {
        }

        protected virtual void RaiseDockAsDocumentCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseCloseAllButThisCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseActivateCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseNewVerticalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseNewHorizontalTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseMoveToNextTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void RaiseMoveToPreviousTabGroupCommandChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        internal virtual void Detach()
        {
            LayoutElement.IsSelectedChanged -= LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged -= LayoutElement_IsActiveChanged;
            LayoutElement = null;
            Model = null;
        }

        internal virtual void Attach(LayoutContent model)
        {
            LayoutElement = model;
            Model = model.Content;

            InitDefaultCommands();

            LayoutElement.IsSelectedChanged += LayoutElement_IsSelectedChanged;
            LayoutElement.IsActiveChanged += LayoutElement_IsActiveChanged;

            DataContext = this;
        }

        internal void _ClearDefaultBindings()
        {
            ClearDefaultBindings();
        }

        internal void _SetDefaultBindings()
        {
            SetDefaultBindings();
        }

        #endregion
    }
}