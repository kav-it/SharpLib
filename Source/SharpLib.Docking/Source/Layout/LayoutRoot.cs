using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("RootPanel")]
    [Serializable]
    public class LayoutRoot : LayoutElement, ILayoutContainer, ILayoutRoot
    {
        #region Поля

        [field: NonSerialized]
        private WeakReference _activeContent;

        private bool _activeContentSet;

        private LayoutAnchorSide _bottomSide;

        private ObservableCollection<LayoutFloatingWindow> _floatingWindows;

        private ObservableCollection<LayoutAnchorable> _hiddenAnchorables;

        [field: NonSerialized]
        private WeakReference _lastFocusedDocument;

        [field: NonSerialized]
        private bool _lastFocusedDocumentSet;

        private LayoutAnchorSide _leftSide;

        [NonSerialized]
        private DockingManager _manager;

        private LayoutAnchorSide _rightSide;

        private LayoutPanel _rootPanel;

        private LayoutAnchorSide _topSide;

        #endregion

        #region Свойства

        public LayoutPanel RootPanel
        {
            get { return _rootPanel; }
            set
            {
                if (Equals(_rootPanel, value))
                {
                    return;
                }
                RaisePropertyChanging("RootPanel");
                if (_rootPanel != null && Equals(_rootPanel.Parent, this))
                {
                    _rootPanel.Parent = null;
                }
                _rootPanel = value;

                if (_rootPanel == null)
                {
                    _rootPanel = new LayoutPanel(new LayoutDocumentPane());
                }

                if (_rootPanel != null)
                {
                    _rootPanel.Parent = this;
                }
                RaisePropertyChanged("RootPanel");
            }
        }

        public LayoutAnchorSide TopSide
        {
            get { return _topSide; }
            set
            {
                if (Equals(_topSide, value))
                {
                    return;
                }
                RaisePropertyChanging("TopSide");
                _topSide = value;
                if (_topSide != null)
                {
                    _topSide.Parent = this;
                }
                RaisePropertyChanged("TopSide");
            }
        }

        public LayoutAnchorSide RightSide
        {
            get { return _rightSide; }
            set
            {
                if (Equals(_rightSide, value))
                {
                    return;
                }
                RaisePropertyChanging("RightSide");
                _rightSide = value;
                if (_rightSide != null)
                {
                    _rightSide.Parent = this;
                }
                RaisePropertyChanged("RightSide");
            }
        }

        public LayoutAnchorSide LeftSide
        {
            get { return _leftSide; }
            set
            {
                if (Equals(_leftSide, value))
                {
                    return;
                }
                RaisePropertyChanging("LeftSide");
                _leftSide = value;
                if (_leftSide != null)
                {
                    _leftSide.Parent = this;
                }
                RaisePropertyChanged("LeftSide");
            }
        }

        public LayoutAnchorSide BottomSide
        {
            get { return _bottomSide; }
            set
            {
                if (Equals(_bottomSide, value))
                {
                    return;
                }
                RaisePropertyChanging("BottomSide");
                _bottomSide = value;
                if (_bottomSide != null)
                {
                    _bottomSide.Parent = this;
                }
                RaisePropertyChanged("BottomSide");
            }
        }

        public ObservableCollection<LayoutFloatingWindow> FloatingWindows
        {
            get
            {
                if (_floatingWindows == null)
                {
                    _floatingWindows = new ObservableCollection<LayoutFloatingWindow>();
                    _floatingWindows.CollectionChanged += _floatingWindows_CollectionChanged;
                }

                return _floatingWindows;
            }
        }

        public ObservableCollection<LayoutAnchorable> Hidden
        {
            get
            {
                if (_hiddenAnchorables == null)
                {
                    _hiddenAnchorables = new ObservableCollection<LayoutAnchorable>();
                    _hiddenAnchorables.CollectionChanged += _hiddenAnchorables_CollectionChanged;
                }

                return _hiddenAnchorables;
            }
        }

        public IEnumerable<ILayoutElement> Children
        {
            get
            {
                if (RootPanel != null)
                {
                    yield return RootPanel;
                }
                if (_floatingWindows != null)
                {
                    foreach (var floatingWindow in _floatingWindows)
                    {
                        yield return floatingWindow;
                    }
                }
                if (TopSide != null)
                {
                    yield return TopSide;
                }
                if (RightSide != null)
                {
                    yield return RightSide;
                }
                if (BottomSide != null)
                {
                    yield return BottomSide;
                }
                if (LeftSide != null)
                {
                    yield return LeftSide;
                }
                if (_hiddenAnchorables != null)
                {
                    foreach (var hiddenAnchorable in _hiddenAnchorables)
                    {
                        yield return hiddenAnchorable;
                    }
                }
            }
        }

        public int ChildrenCount
        {
            get
            {
                return 5 +
                       (_floatingWindows != null ? _floatingWindows.Count : 0) +
                       (_hiddenAnchorables != null ? _hiddenAnchorables.Count : 0);
            }
        }

        [XmlIgnore]
        public LayoutContent ActiveContent
        {
            get { return _activeContent.GetValueOrDefault<LayoutContent>(); }
            set
            {
                var currentValue = ActiveContent;
                if (!Equals(currentValue, value))
                {
                    InternalSetActiveContent(currentValue, value);
                }
            }
        }

        [XmlIgnore]
        public LayoutContent LastFocusedDocument
        {
            get { return _lastFocusedDocument.GetValueOrDefault<LayoutContent>(); }
            private set
            {
                var currentValue = LastFocusedDocument;
                if (!Equals(currentValue, value))
                {
                    RaisePropertyChanging("LastFocusedDocument");
                    if (currentValue != null)
                    {
                        currentValue.IsLastFocusedDocument = false;
                    }
                    _lastFocusedDocument = new WeakReference(value);
                    currentValue = LastFocusedDocument;
                    if (currentValue != null)
                    {
                        currentValue.IsLastFocusedDocument = true;
                    }
                    _lastFocusedDocumentSet = currentValue != null;
                    RaisePropertyChanged("LastFocusedDocument");
                }
            }
        }

        [XmlIgnore]
        public DockingManager Manager
        {
            get { return _manager; }
            internal set
            {
                if (Equals(_manager, value))
                {
                    return;
                }
                RaisePropertyChanging("Manager");
                _manager = value;
                RaisePropertyChanged("Manager");
            }
        }

        #endregion

        #region События

        public event EventHandler<LayoutElementEventArgs> ElementAdded;

        public event EventHandler<LayoutElementEventArgs> ElementRemoved;

        public event EventHandler Updated;

        #endregion

        #region Конструктор

        public LayoutRoot()
        {
            RightSide = new LayoutAnchorSide();
            LeftSide = new LayoutAnchorSide();
            TopSide = new LayoutAnchorSide();
            BottomSide = new LayoutAnchorSide();
            RootPanel = new LayoutPanel(new LayoutDocumentPane());
        }

        #endregion

        #region Методы

        private void _floatingWindows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                                       e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (LayoutFloatingWindow element in e.OldItems)
                {
                    if (Equals(element.Parent, this))
                    {
                        element.Parent = null;
                    }
                }
            }

            if (e.NewItems != null && (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                                       e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace))
            {
                foreach (LayoutFloatingWindow element in e.NewItems)
                {
                    element.Parent = this;
                }
            }
        }

        private void _hiddenAnchorables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (LayoutAnchorable element in e.OldItems)
                    {
                        if (Equals(element.Parent, this))
                        {
                            element.Parent = null;
                        }
                    }
                }
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (LayoutAnchorable element in e.NewItems)
                    {
                        if (!Equals(element.Parent, this))
                        {
                            if (element.Parent != null)
                            {
                                element.Parent.RemoveChild(element);
                            }
                            element.Parent = this;
                        }
                    }
                }
            }
        }

        public void RemoveChild(ILayoutElement element)
        {
            if (Equals(element, RootPanel))
            {
                RootPanel = null;
            }
            else if (_floatingWindows != null && _floatingWindows.Contains(element))
            {
                _floatingWindows.Remove(element as LayoutFloatingWindow);
            }
            else if (_hiddenAnchorables != null && _hiddenAnchorables.Contains(element))
            {
                _hiddenAnchorables.Remove(element as LayoutAnchorable);
            }
            else if (Equals(element, TopSide))
            {
                TopSide = null;
            }
            else if (Equals(element, RightSide))
            {
                RightSide = null;
            }
            else if (Equals(element, BottomSide))
            {
                BottomSide = null;
            }
            else if (Equals(element, LeftSide))
            {
                LeftSide = null;
            }
        }

        public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            if (Equals(oldElement, RootPanel))
            {
                RootPanel = (LayoutPanel)newElement;
            }
            else if (_floatingWindows != null && _floatingWindows.Contains(oldElement))
            {
                int index = _floatingWindows.IndexOf(oldElement as LayoutFloatingWindow);
                _floatingWindows.Remove(oldElement as LayoutFloatingWindow);
                _floatingWindows.Insert(index, newElement as LayoutFloatingWindow);
            }
            else if (_hiddenAnchorables != null && _hiddenAnchorables.Contains(oldElement))
            {
                int index = _hiddenAnchorables.IndexOf(oldElement as LayoutAnchorable);
                _hiddenAnchorables.Remove(oldElement as LayoutAnchorable);
                _hiddenAnchorables.Insert(index, newElement as LayoutAnchorable);
            }
            else if (Equals(oldElement, TopSide))
            {
                TopSide = (LayoutAnchorSide)newElement;
            }
            else if (Equals(oldElement, RightSide))
            {
                RightSide = (LayoutAnchorSide)newElement;
            }
            else if (Equals(oldElement, BottomSide))
            {
                BottomSide = (LayoutAnchorSide)newElement;
            }
            else if (Equals(oldElement, LeftSide))
            {
                LeftSide = (LayoutAnchorSide)newElement;
            }
        }

        private void InternalSetActiveContent(LayoutContent currentValue, LayoutContent newActiveContent)
        {
            RaisePropertyChanging("ActiveContent");
            if (currentValue != null)
            {
                currentValue.IsActive = false;
            }
            _activeContent = new WeakReference(newActiveContent);
            currentValue = ActiveContent;
            if (currentValue != null)
            {
                currentValue.IsActive = true;
            }
            RaisePropertyChanged("ActiveContent");
            _activeContentSet = currentValue != null;
            if (currentValue != null)
            {
                if (currentValue.Parent is LayoutDocumentPane || currentValue is LayoutDocument)
                {
                    LastFocusedDocument = currentValue;
                }
            }
            else
            {
                LastFocusedDocument = null;
            }
        }

        private void UpdateActiveContentProperty()
        {
            var activeContent = ActiveContent;
            if (_activeContentSet && (activeContent == null || !Equals(activeContent.Root, this)))
            {
                _activeContentSet = false;
                InternalSetActiveContent(activeContent, null);
            }
        }

        public void CollectGarbage()
        {
            bool exitFlag;

            do
            {
                exitFlag = true;

                foreach (var content in this.Descendents()
                    .OfType<ILayoutPreviousContainer>()
                    .Where(c => c.PreviousContainer != null && (c.PreviousContainer.Parent == null || !Equals(c.PreviousContainer.Parent.Root, this))))
                {
                    content.PreviousContainer = null;
                }

                foreach (var emptyPane in this.Descendents().OfType<ILayoutPane>().Where(p => p.ChildrenCount == 0))
                {
                    ILayoutPane pane = emptyPane;
                    foreach (var contentReferencingEmptyPane in this.Descendents().OfType<LayoutContent>()
                        .Where(c => ((ILayoutPreviousContainer)c).PreviousContainer == pane && !c.IsFloating))
                    {
                        if (contentReferencingEmptyPane is LayoutAnchorable &&
                            !((LayoutAnchorable)contentReferencingEmptyPane).IsVisible)
                        {
                            continue;
                        }

                        ((ILayoutPreviousContainer)contentReferencingEmptyPane).PreviousContainer = null;
                        contentReferencingEmptyPane.PreviousContainerIndex = -1;
                    }

                    if (emptyPane is LayoutDocumentPane &&
                        this.Descendents().OfType<LayoutDocumentPane>().Count(c => !Equals(c, emptyPane)) == 0)
                    {
                        continue;
                    }

                    if (this.Descendents().OfType<ILayoutPreviousContainer>().All(c => c.PreviousContainer != emptyPane))
                    {
                        var parentGroup = emptyPane.Parent;
                        parentGroup.RemoveChild(emptyPane);
                        exitFlag = false;
                        break;
                    }
                }

                if (!exitFlag)
                {
                    foreach (var emptyPaneGroup in this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        exitFlag = false;
                        break;
                    }
                }

                if (!exitFlag)
                {
                    foreach (var emptyPaneGroup in this.Descendents().OfType<LayoutPanel>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        exitFlag = false;
                        break;
                    }
                }

                if (!exitFlag)
                {
                    foreach (var emptyPaneGroup in this.Descendents().OfType<LayoutFloatingWindow>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        exitFlag = false;
                        break;
                    }
                }

                if (!exitFlag)
                {
                    foreach (var emptyPaneGroup in this.Descendents().OfType<LayoutAnchorGroup>().Where(p => p.ChildrenCount == 0))
                    {
                        var parentGroup = emptyPaneGroup.Parent;
                        parentGroup.RemoveChild(emptyPaneGroup);
                        exitFlag = false;
                        break;
                    }
                }
            } while (!exitFlag);

            do
            {
                exitFlag = true;

                foreach (var paneGroupToCollapse in this.Descendents().OfType<LayoutAnchorablePaneGroup>().Where(p => p.ChildrenCount == 1 && p.Children[0] is LayoutAnchorablePaneGroup).ToArray())
                {
                    var singleChild = paneGroupToCollapse.Children[0] as LayoutAnchorablePaneGroup;
                    if (singleChild != null)
                    {
                        paneGroupToCollapse.Orientation = singleChild.Orientation;
                        paneGroupToCollapse.RemoveChild(singleChild);
                        while (singleChild.ChildrenCount > 0)
                        {
                            paneGroupToCollapse.InsertChildAt(
                                paneGroupToCollapse.ChildrenCount, singleChild.Children[0]);
                        }
                    }

                    exitFlag = false;
                    break;
                }
            } while (!exitFlag);

            do
            {
                exitFlag = true;

                foreach (var paneGroupToCollapse in this.Descendents().OfType<LayoutDocumentPaneGroup>().Where(p => p.ChildrenCount == 1 && p.Children[0] is LayoutDocumentPaneGroup).ToArray())
                {
                    var singleChild = paneGroupToCollapse.Children[0] as LayoutDocumentPaneGroup;
                    if (singleChild != null)
                    {
                        paneGroupToCollapse.Orientation = singleChild.Orientation;
                        paneGroupToCollapse.RemoveChild(singleChild);
                        while (singleChild.ChildrenCount > 0)
                        {
                            paneGroupToCollapse.InsertChildAt(
                                paneGroupToCollapse.ChildrenCount, singleChild.Children[0]);
                        }
                    }

                    exitFlag = false;
                    break;
                }
            } while (!exitFlag);

            do
            {
                exitFlag = true;

                foreach (var panelToCollapse in this.Descendents().OfType<LayoutPanel>().Where(p => p.ChildrenCount == 1 && p.Children[0] is LayoutPanel).ToArray())
                {
                    var singleChild = panelToCollapse.Children[0] as LayoutPanel;
                    if (singleChild != null)
                    {
                        panelToCollapse.Orientation = singleChild.Orientation;
                        panelToCollapse.RemoveChild(singleChild);
                        while (singleChild.ChildrenCount > 0)
                        {
                            panelToCollapse.InsertChildAt(
                                panelToCollapse.ChildrenCount, singleChild.Children[0]);
                        }
                    }

                    exitFlag = false;
                    break;
                }
            } while (!exitFlag);

            UpdateActiveContentProperty();
        }

        internal void FireLayoutUpdated()
        {
            if (Updated != null)
            {
                Updated(this, EventArgs.Empty);
            }
        }

        internal void OnLayoutElementAdded(LayoutElement element)
        {
            if (ElementAdded != null)
            {
                ElementAdded(this, new LayoutElementEventArgs(element));
            }
        }

        internal void OnLayoutElementRemoved(LayoutElement element)
        {
            if (element.Descendents().OfType<LayoutContent>().Any(c => Equals(c, LastFocusedDocument)))
            {
                LastFocusedDocument = null;
            }
            if (element.Descendents().OfType<LayoutContent>().Any(c => Equals(c, ActiveContent)))
            {
                ActiveContent = null;
            }
            if (ElementRemoved != null)
            {
                ElementRemoved(this, new LayoutElementEventArgs(element));
            }
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("RootPanel()");

            RootPanel.ConsoleDump(tab + 1);

            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("FloatingWindows()");

            foreach (LayoutFloatingWindow fw in FloatingWindows)
            {
                fw.ConsoleDump(tab + 1);
            }

            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("Hidden()");

            foreach (LayoutAnchorable hidden in Hidden)
            {
                hidden.ConsoleDump(tab + 1);
            }
        }

        #endregion
    }
}