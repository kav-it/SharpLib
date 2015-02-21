using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [Serializable]
    public class LayoutAnchorable : LayoutContent
    {
        #region Поля

        private double _autohideHeight;

        private double _autohideMinHeight = 100.0;

        private double _autohideMinWidth = 100.0;

        private double _autohideWidth;

        private bool _canAutoHide = true;

        private bool _canHide = true;

        #endregion

        #region Свойства

        [XmlIgnore]
        public bool IsVisible
        {
            get { return Parent != null && !(Parent is LayoutRoot); }
            set
            {
                if (value)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }

        [XmlIgnore]
        public bool IsHidden
        {
            get { return (Parent is LayoutRoot); }
        }

        public double AutoHideWidth
        {
            get { return _autohideWidth; }
            set
            {
                if (_autohideWidth.NotEqualEx(value))
                {
                    RaisePropertyChanging("AutoHideWidth");
                    value = Math.Max(value, _autohideMinWidth);
                    _autohideWidth = value;
                    RaisePropertyChanged("AutoHideWidth");
                }
            }
        }

        public double AutoHideMinWidth
        {
            get { return _autohideMinWidth; }
            set
            {
                if (_autohideMinWidth.NotEqualEx(value))
                {
                    RaisePropertyChanging("AutoHideMinWidth");
                    if (value < 0)
                    {
                        throw new ArgumentException("value");
                    }
                    _autohideMinWidth = value;
                    RaisePropertyChanged("AutoHideMinWidth");
                }
            }
        }

        public double AutoHideHeight
        {
            get { return _autohideHeight; }
            set
            {
                if (_autohideHeight.NotEqualEx(value))
                {
                    RaisePropertyChanging("AutoHideHeight");
                    value = Math.Max(value, _autohideMinHeight);
                    _autohideHeight = value;
                    RaisePropertyChanged("AutoHideHeight");
                }
            }
        }

        public double AutoHideMinHeight
        {
            get { return _autohideMinHeight; }
            set
            {
                if (_autohideMinHeight.NotEqualEx(value))
                {
                    RaisePropertyChanging("AutoHideMinHeight");
                    if (value < 0)
                    {
                        throw new ArgumentException("value");
                    }
                    _autohideMinHeight = value;
                    RaisePropertyChanged("AutoHideMinHeight");
                }
            }
        }

        public bool IsAutoHidden
        {
            get { return Parent is LayoutAnchorGroup; }
        }

        public bool CanHide
        {
            get { return _canHide; }
            set
            {
                if (_canHide != value)
                {
                    _canHide = value;
                    RaisePropertyChanged("CanHide");
                }
            }
        }

        public bool CanAutoHide
        {
            get { return _canAutoHide; }
            set
            {
                if (_canAutoHide != value)
                {
                    _canAutoHide = value;
                    RaisePropertyChanged("CanAutoHide");
                }
            }
        }

        #endregion

        #region События

        public event EventHandler<CancelEventArgs> Hiding;

        public event EventHandler IsVisibleChanged;

        #endregion

        #region Методы

        private void NotifyIsVisibleChanged()
        {
            if (IsVisibleChanged != null)
            {
                IsVisibleChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            UpdateParentVisibility();
            RaisePropertyChanged("IsVisible");
            NotifyIsVisibleChanged();
            RaisePropertyChanged("IsHidden");
            RaisePropertyChanged("IsAutoHidden");
            base.OnParentChanged(oldValue, newValue);
        }

        private void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
            {
                parentPane.ComputeVisibility();
            }
        }

        public void Hide(bool cancelable = true)
        {
            if (!IsVisible)
            {
                IsSelected = true;
                IsActive = true;
                return;
            }

            if (cancelable)
            {
                var args = new CancelEventArgs();
                OnHiding(args);
                if (args.Cancel)
                {
                    return;
                }
            }

            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");

            {
                var parentAsGroup = Parent as ILayoutGroup;
                PreviousContainer = parentAsGroup;
                if (parentAsGroup != null)
                {
                    PreviousContainerIndex = parentAsGroup.IndexOfChild(this);
                }
            }
            Root.Hidden.Add(this);
            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
            NotifyIsVisibleChanged();
        }

        protected virtual void OnHiding(CancelEventArgs args)
        {
            if (Hiding != null)
            {
                Hiding(this, args);
            }
        }

        public void Show()
        {
            if (IsVisible)
            {
                return;
            }

            if (!IsHidden)
            {
                throw new InvalidOperationException();
            }

            RaisePropertyChanging("IsHidden");
            RaisePropertyChanging("IsVisible");

            bool added = false;
            var root = Root;
            if (root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                {
                    added = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root as LayoutRoot, this, PreviousContainer);
                }
            }

            if (!added && PreviousContainer != null)
            {
                var previousContainerAsLayoutGroup = PreviousContainer as ILayoutGroup;
                if (previousContainerAsLayoutGroup != null)
                {
                    previousContainerAsLayoutGroup.InsertChildAt(PreviousContainerIndex < previousContainerAsLayoutGroup.ChildrenCount
                        ? PreviousContainerIndex
                        : previousContainerAsLayoutGroup.ChildrenCount, this);
                }
                IsSelected = true;
                IsActive = true;
            }

            if (root != null && root.Manager != null)
            {
                if (root.Manager.LayoutUpdateStrategy != null)
                {
                    root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root as LayoutRoot, this);
                }
            }

            PreviousContainer = null;
            PreviousContainerIndex = -1;

            RaisePropertyChanged("IsVisible");
            RaisePropertyChanged("IsHidden");
            NotifyIsVisibleChanged();
        }

        protected override void InternalDock()
        {
            var root = Root as LayoutRoot;
            LayoutAnchorablePane anchorablePane = null;

            if (root != null && (root.ActiveContent != null && !Equals(root.ActiveContent, this)))
            {
                anchorablePane = root.ActiveContent.Parent as LayoutAnchorablePane;
            }

            if (anchorablePane == null)
            {
                anchorablePane = root.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(pane => !pane.IsHostedInFloatingWindow && pane.GetSide() == AnchorSide.Right);
            }

            if (anchorablePane == null)
            {
                anchorablePane = root.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault();
            }

            bool added = false;
            if (root != null && root.Manager.LayoutUpdateStrategy != null)
            {
                added = root.Manager.LayoutUpdateStrategy.BeforeInsertAnchorable(root, this, anchorablePane);
            }

            if (!added)
            {
                if (anchorablePane == null)
                {
                    var mainLayoutPanel = new LayoutPanel
                    {
                        Orientation = Orientation.Horizontal
                    };

                    if (root != null && root.RootPanel != null)
                    {
                        mainLayoutPanel.Children.Add(root.RootPanel);
                    }

                    if (root != null)
                    {
                        root.RootPanel = mainLayoutPanel;
                    }
                    anchorablePane = new LayoutAnchorablePane
                    {
                        DockWidth = new GridLength(200.0, GridUnitType.Pixel)
                    };
                    mainLayoutPanel.Children.Add(anchorablePane);
                }

                anchorablePane.Children.Add(this);
                added = true;
            }

            if (root != null && root.Manager.LayoutUpdateStrategy != null)
            {
                root.Manager.LayoutUpdateStrategy.AfterInsertAnchorable(root, this);
            }

            base.InternalDock();
        }

        public void AddToLayout(DockingManager manager, AnchorableShowStrategy strategy)
        {
            if (IsVisible || IsHidden)
            {
                throw new InvalidOperationException();
            }

            bool most = (strategy & AnchorableShowStrategy.Most) == AnchorableShowStrategy.Most;
            bool left = (strategy & AnchorableShowStrategy.Left) == AnchorableShowStrategy.Left;
            bool right = (strategy & AnchorableShowStrategy.Right) == AnchorableShowStrategy.Right;
            bool top = (strategy & AnchorableShowStrategy.Top) == AnchorableShowStrategy.Top;
            bool bottom = (strategy & AnchorableShowStrategy.Bottom) == AnchorableShowStrategy.Bottom;

            if (!most)
            {
                var side = AnchorSide.Left;
                if (left)
                {
                    side = AnchorSide.Left;
                }
                if (right)
                {
                    side = AnchorSide.Right;
                }
                if (top)
                {
                    side = AnchorSide.Top;
                }
                if (bottom)
                {
                    side = AnchorSide.Bottom;
                }

                var anchorablePane = manager.Layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == side);
                if (anchorablePane != null)
                {
                    anchorablePane.Children.Add(this);
                }
                else
                {
                    most = true;
                }
            }

            if (most)
            {
                if (manager.Layout.RootPanel == null)
                {
                    manager.Layout.RootPanel = new LayoutPanel
                    {
                        Orientation = (left || right ? Orientation.Horizontal : Orientation.Vertical)
                    };
                }

                if (left || right)
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Vertical &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Horizontal;

                    if (left)
                    {
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    }
                    else
                    {
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                    }
                }
                else
                {
                    if (manager.Layout.RootPanel.Orientation == Orientation.Horizontal &&
                        manager.Layout.RootPanel.ChildrenCount > 1)
                    {
                        manager.Layout.RootPanel = new LayoutPanel(manager.Layout.RootPanel);
                    }

                    manager.Layout.RootPanel.Orientation = Orientation.Vertical;

                    if (top)
                    {
                        manager.Layout.RootPanel.Children.Insert(0, new LayoutAnchorablePane(this));
                    }
                    else
                    {
                        manager.Layout.RootPanel.Children.Add(new LayoutAnchorablePane(this));
                    }
                }
            }
        }

        public void ToggleAutoHide()
        {
            if (IsAutoHidden)
            {
                var parentGroup = Parent as LayoutAnchorGroup;
                if (parentGroup != null)
                {
                    var parentSide = parentGroup.Parent as LayoutAnchorSide;
                    var previousContainer = ((ILayoutPreviousContainer)parentGroup).PreviousContainer as LayoutAnchorablePane;

                    if (previousContainer == null)
                    {
                        var layoutAnchorSide = parentGroup.Parent as LayoutAnchorSide;
                        if (layoutAnchorSide != null)
                        {
                            var side = layoutAnchorSide.Side;
                            switch (side)
                            {
                                case AnchorSide.Right:
                                    if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        parentGroup.Root.RootPanel.Children.Add(previousContainer);
                                    }
                                    else
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        var panel = new LayoutPanel
                                        {
                                            Orientation = Orientation.Horizontal
                                        };
                                        var root = parentGroup.Root as LayoutRoot;
                                        var oldRootPanel = parentGroup.Root.RootPanel;
                                        if (root != null)
                                        {
                                            root.RootPanel = panel;
                                        }
                                        panel.Children.Add(oldRootPanel);
                                        panel.Children.Add(previousContainer);
                                    }
                                    break;
                                case AnchorSide.Left:
                                    if (parentGroup.Root.RootPanel.Orientation == Orientation.Horizontal)
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                                    }
                                    else
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        var panel = new LayoutPanel
                                        {
                                            Orientation = Orientation.Horizontal
                                        };
                                        var root = parentGroup.Root as LayoutRoot;
                                        var oldRootPanel = parentGroup.Root.RootPanel;
                                        if (root != null)
                                        {
                                            root.RootPanel = panel;
                                        }
                                        panel.Children.Add(previousContainer);
                                        panel.Children.Add(oldRootPanel);
                                    }
                                    break;
                                case AnchorSide.Top:
                                    if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        parentGroup.Root.RootPanel.Children.Insert(0, previousContainer);
                                    }
                                    else
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        var panel = new LayoutPanel
                                        {
                                            Orientation = Orientation.Vertical
                                        };
                                        var root = parentGroup.Root as LayoutRoot;
                                        var oldRootPanel = parentGroup.Root.RootPanel;
                                        if (root != null)
                                        {
                                            root.RootPanel = panel;
                                        }
                                        panel.Children.Add(previousContainer);
                                        panel.Children.Add(oldRootPanel);
                                    }
                                    break;
                                case AnchorSide.Bottom:
                                    if (parentGroup.Root.RootPanel.Orientation == Orientation.Vertical)
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        parentGroup.Root.RootPanel.Children.Add(previousContainer);
                                    }
                                    else
                                    {
                                        previousContainer = new LayoutAnchorablePane();
                                        var panel = new LayoutPanel
                                        {
                                            Orientation = Orientation.Vertical
                                        };
                                        var root = parentGroup.Root as LayoutRoot;
                                        var oldRootPanel = parentGroup.Root.RootPanel;
                                        if (root != null)
                                        {
                                            root.RootPanel = panel;
                                        }
                                        panel.Children.Add(oldRootPanel);
                                        panel.Children.Add(previousContainer);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var root = parentGroup.Root as LayoutRoot;
                        foreach (var cnt in root.Descendents().OfType<ILayoutPreviousContainer>().Where(c => Equals(c.PreviousContainer, parentGroup)))
                        {
                            cnt.PreviousContainer = previousContainer;
                        }
                    }

                    foreach (var anchorableToToggle in parentGroup.Children.ToArray())
                    {
                        if (previousContainer != null)
                        {
                            previousContainer.Children.Add(anchorableToToggle);
                        }
                    }

                    if (parentSide != null)
                    {
                        parentSide.Children.Remove(parentGroup);
                    }
                }
            }
            else if (Parent is LayoutAnchorablePane)
            {
                var root = Root;
                var parentPane = Parent as LayoutAnchorablePane;

                var newAnchorGroup = new LayoutAnchorGroup();

                ((ILayoutPreviousContainer)newAnchorGroup).PreviousContainer = parentPane;

                foreach (var anchorableToImport in parentPane.Children.ToArray())
                {
                    newAnchorGroup.Children.Add(anchorableToImport);
                }

                var anchorSide = parentPane.GetSide();

                switch (anchorSide)
                {
                    case AnchorSide.Right:
                        root.RightSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Left:
                        root.LeftSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Top:
                        root.TopSide.Children.Add(newAnchorGroup);
                        break;
                    case AnchorSide.Bottom:
                        root.BottomSide.Children.Add(newAnchorGroup);
                        break;
                }
            }
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("CanHide"))
            {
                CanHide = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("CanAutoHide"))
            {
                CanAutoHide = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("AutoHideWidth"))
            {
                AutoHideWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("AutoHideHeight"))
            {
                AutoHideHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("AutoHideMinWidth"))
            {
                AutoHideMinWidth = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }
            if (reader.MoveToAttribute("AutoHideMinHeight"))
            {
                AutoHideMinHeight = double.Parse(reader.Value, CultureInfo.InvariantCulture);
            }

            base.ReadXml(reader);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!CanHide)
            {
                writer.WriteAttributeString("CanHide", CanHide.ToString());
            }
            if (!CanAutoHide)
            {
                writer.WriteAttributeString("CanAutoHide", CanAutoHide.ToString(CultureInfo.InvariantCulture));
            }
            if (AutoHideWidth > 0)
            {
                writer.WriteAttributeString("AutoHideWidth", AutoHideWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (AutoHideHeight > 0)
            {
                writer.WriteAttributeString("AutoHideHeight", AutoHideHeight.ToString(CultureInfo.InvariantCulture));
            }
            if (AutoHideMinWidth.NotEqualEx(25.0))
            {
                writer.WriteAttributeString("AutoHideMinWidth", AutoHideMinWidth.ToString(CultureInfo.InvariantCulture));
            }
            if (AutoHideMinHeight.NotEqualEx(25.0))
            {
                writer.WriteAttributeString("AutoHideMinHeight", AutoHideMinHeight.ToString(CultureInfo.InvariantCulture));
            }

            base.WriteXml(writer);
        }

        public override void ConsoleDump(int tab)
        {
            // System.Diagnostics.Trace.Write(new string(' ', tab * 4));
            // System.Diagnostics.Trace.WriteLine("Anchorable()");
        }

        #endregion
    }
}