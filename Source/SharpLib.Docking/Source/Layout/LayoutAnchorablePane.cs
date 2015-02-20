using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorablePane : LayoutPositionableGroup<LayoutAnchorable>, ILayoutAnchorablePane, ILayoutContentSelector, ILayoutPaneSerializable
    {
        #region Поля

        [XmlIgnore]
        private bool _autoFixSelectedContent = true;

        private string _id;

        private string _name;

        private int _selectedIndex = -1;

        #endregion

        #region Свойства

        public int SelectedContentIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value < 0 ||
                    value >= Children.Count)
                {
                    value = -1;
                }

                if (_selectedIndex != value)
                {
                    RaisePropertyChanging("SelectedContentIndex");
                    RaisePropertyChanging("SelectedContent");
                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                    {
                        Children[_selectedIndex].IsSelected = false;
                    }

                    _selectedIndex = value;

                    if (_selectedIndex >= 0 &&
                        _selectedIndex < Children.Count)
                    {
                        Children[_selectedIndex].IsSelected = true;
                    }

                    RaisePropertyChanged("SelectedContentIndex");
                    RaisePropertyChanged("SelectedContent");
                }
            }
        }

        public LayoutContent SelectedContent
        {
            get { return _selectedIndex == -1 ? null : Children[_selectedIndex]; }
        }

        public bool IsDirectlyHostedInFloatingWindow
        {
            get
            {
                var parentFloatingWindow = this.FindParent<LayoutAnchorableFloatingWindow>();
                if (parentFloatingWindow != null)
                {
                    return parentFloatingWindow.IsSinglePane;
                }

                return false;
            }
        }

        public bool IsHostedInFloatingWindow
        {
            get { return this.FindParent<LayoutFloatingWindow>() != null; }
        }

        string ILayoutPaneSerializable.Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public bool CanHide
        {
            get { return Children.All(a => a.CanHide); }
        }

        public bool CanClose
        {
            get { return Children.All(a => a.CanClose); }
        }

        #endregion

        #region Конструктор

        public LayoutAnchorablePane()
        {
        }

        public LayoutAnchorablePane(LayoutAnchorable anchorable)
        {
            Children.Add(anchorable);
        }

        #endregion

        #region Методы

        protected override bool GetVisibility()
        {
            return Children.Count > 0 && Children.Any(c => c.IsVisible);
        }

        protected override void ChildMoved(int oldIndex, int newIndex)
        {
            if (_selectedIndex == oldIndex)
            {
                RaisePropertyChanging("SelectedContentIndex");
                _selectedIndex = newIndex;
                RaisePropertyChanged("SelectedContentIndex");
            }

            base.ChildMoved(oldIndex, newIndex);
        }

        protected override void OnChildrenCollectionChanged()
        {
            AutoFixSelectedContent();
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].IsSelected)
                {
                    SelectedContentIndex = i;
                    break;
                }
            }

            RaisePropertyChanged("CanClose");
            RaisePropertyChanged("CanHide");
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
            base.OnChildrenCollectionChanged();
        }

        private void AutoFixSelectedContent()
        {
            if (_autoFixSelectedContent)
            {
                if (SelectedContentIndex >= ChildrenCount)
                {
                    SelectedContentIndex = Children.Count - 1;
                }

                if (SelectedContentIndex == -1 && ChildrenCount > 0)
                {
                    SelectedContentIndex = 0;
                }
            }
        }

        public int IndexOf(LayoutContent content)
        {
            var anchorableChild = content as LayoutAnchorable;
            if (anchorableChild == null)
            {
                return -1;
            }

            return Children.IndexOf(anchorableChild);
        }

        internal void UpdateIsDirectlyHostedInFloatingWindow()
        {
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            var oldGroup = oldValue as ILayoutGroup;
            if (oldGroup != null)
            {
                oldGroup.ChildrenCollectionChanged -= OnParentChildrenCollectionChanged;
            }

            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");

            var newGroup = newValue as ILayoutGroup;
            if (newGroup != null)
            {
                newGroup.ChildrenCollectionChanged += OnParentChildrenCollectionChanged;
            }

            base.OnParentChanged(oldValue, newValue);
        }

        private void OnParentChildrenCollectionChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("IsDirectlyHostedInFloatingWindow");
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (_id != null)
            {
                writer.WriteAttributeString("Id", _id);
            }
            if (_name != null)
            {
                writer.WriteAttributeString("Name", _name);
            }

            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
            {
                _id = reader.Value;
            }
            if (reader.MoveToAttribute("Name"))
            {
                _name = reader.Value;
            }

            _autoFixSelectedContent = false;
            base.ReadXml(reader);
            _autoFixSelectedContent = true;
            AutoFixSelectedContent();
        }

        public override void ConsoleDump(int tab)
        {
            Trace.Write(new string(' ', tab * 4));
            Trace.WriteLine("AnchorablePane()");

            foreach (LayoutAnchorable child in Children)
            {
                child.ConsoleDump(tab + 1);
            }
        }

        #endregion
    }
}