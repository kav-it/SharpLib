using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SharpLib.Docking.Layout;

namespace SharpLib.Docking.Controls
{
    public class LayoutAnchorSideControl : Control, ILayoutControl
    {
        #region Поля

        public static readonly DependencyProperty IsBottomSideProperty;

        public static readonly DependencyProperty IsLeftSideProperty;

        public static readonly DependencyProperty IsRightSideProperty;

        public static readonly DependencyProperty IsTopSideProperty;

        private static readonly DependencyPropertyKey _isBottomSidePropertyKey;

        private static readonly DependencyPropertyKey _isLeftSidePropertyKey;

        private static readonly DependencyPropertyKey _isRightSidePropertyKey;

        private static readonly DependencyPropertyKey _isTopSidePropertyKey;

        private readonly ObservableCollection<LayoutAnchorGroupControl> _childViews;

        private readonly LayoutAnchorSide _model;

        #endregion

        #region Свойства

        public ILayoutElement Model
        {
            get { return _model; }
        }

        public ObservableCollection<LayoutAnchorGroupControl> Children
        {
            get { return _childViews; }
        }

        public bool IsLeftSide
        {
            get { return (bool)GetValue(IsLeftSideProperty); }
        }

        public bool IsTopSide
        {
            get { return (bool)GetValue(IsTopSideProperty); }
        }

        public bool IsRightSide
        {
            get { return (bool)GetValue(IsRightSideProperty); }
        }

        public bool IsBottomSide
        {
            get { return (bool)GetValue(IsBottomSideProperty); }
        }

        #endregion

        #region Конструктор

        static LayoutAnchorSideControl()
        {
            _isBottomSidePropertyKey = DependencyProperty.RegisterReadOnly("IsBottomSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
            _isLeftSidePropertyKey = DependencyProperty.RegisterReadOnly("IsLeftSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
            _isRightSidePropertyKey = DependencyProperty.RegisterReadOnly("IsRightSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));
            _isTopSidePropertyKey = DependencyProperty.RegisterReadOnly("IsTopSide", typeof(bool), typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(false));

            IsRightSideProperty = _isRightSidePropertyKey.DependencyProperty;
            IsBottomSideProperty = _isBottomSidePropertyKey.DependencyProperty;
            IsLeftSideProperty = _isLeftSidePropertyKey.DependencyProperty;
            IsTopSideProperty = _isTopSidePropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutAnchorSideControl), new FrameworkPropertyMetadata(typeof(LayoutAnchorSideControl)));
        }

        internal LayoutAnchorSideControl(LayoutAnchorSide model)
        {
            _childViews = new ObservableCollection<LayoutAnchorGroupControl>();
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            _model = model;

            CreateChildrenViews();

            _model.Children.CollectionChanged += (s, e) => OnModelChildrenCollectionChanged(e);

            UpdateSide();
        }

        #endregion

        #region Методы

        private void CreateChildrenViews()
        {
            var manager = _model.Root.Manager;
            foreach (var childModel in _model.Children)
            {
                _childViews.Add(manager.CreateUIElementForModel(childModel) as LayoutAnchorGroupControl);
            }
        }

        private void OnModelChildrenCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (var childModel in e.OldItems)
                {
                    _childViews.Remove(_childViews.First(cv => cv.Model == childModel));
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                _childViews.Clear();
            }

            if (e.NewItems != null &&
                (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                 e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                var manager = _model.Root.Manager;
                int insertIndex = e.NewStartingIndex;
                foreach (LayoutAnchorGroup childModel in e.NewItems)
                {
                    _childViews.Insert(insertIndex++, manager.CreateUIElementForModel(childModel) as LayoutAnchorGroupControl);
                }
            }
        }

        private void UpdateSide()
        {
            switch (_model.Side)
            {
                case AnchorSide.Left:
                    SetIsLeftSide(true);
                    break;
                case AnchorSide.Top:
                    SetIsTopSide(true);
                    break;
                case AnchorSide.Right:
                    SetIsRightSide(true);
                    break;
                case AnchorSide.Bottom:
                    SetIsBottomSide(true);
                    break;
            }
        }

        protected void SetIsLeftSide(bool value)
        {
            SetValue(_isLeftSidePropertyKey, value);
        }

        protected void SetIsTopSide(bool value)
        {
            SetValue(_isTopSidePropertyKey, value);
        }

        protected void SetIsRightSide(bool value)
        {
            SetValue(_isRightSidePropertyKey, value);
        }

        protected void SetIsBottomSide(bool value)
        {
            SetValue(_isBottomSidePropertyKey, value);
        }

        #endregion
    }
}