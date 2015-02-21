using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace SharpLib.Docking
{
    [Serializable]
    public abstract class LayoutGroup<T> : LayoutGroupBase, ILayoutGroup, IXmlSerializable where T : class, ILayoutElement
    {
        #region Поля

        private readonly ObservableCollection<T> _children = new ObservableCollection<T>();

        private bool _isVisible = true;

        #endregion

        #region Свойства

        public ObservableCollection<T> Children
        {
            get { return _children; }
        }

        IEnumerable<ILayoutElement> ILayoutContainer.Children
        {
            get { return _children; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            protected set
            {
                if (_isVisible != value)
                {
                    RaisePropertyChanging("IsVisible");
                    _isVisible = value;
                    OnIsVisibleChanged();
                    RaisePropertyChanged("IsVisible");
                }
            }
        }

        public int ChildrenCount
        {
            get { return _children.Count; }
        }

        #endregion

        #region Конструктор

        internal LayoutGroup()
        {
            _children.CollectionChanged += _children_CollectionChanged;
        }

        #endregion

        #region Методы

        private void _children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
                e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (LayoutElement element in e.OldItems)
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
                    foreach (LayoutElement element in e.NewItems)
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

            ComputeVisibility();
            OnChildrenCollectionChanged();
            NotifyChildrenTreeChanged(ChildrenTreeChange.DirectChildrenChanged);
            RaisePropertyChanged("ChildrenCount");
        }

        protected virtual void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
        }

        private void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
            {
                parentPane.ComputeVisibility();
            }
        }

        public void ComputeVisibility()
        {
            IsVisible = GetVisibility();
        }

        protected abstract bool GetVisibility();

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            base.OnParentChanged(oldValue, newValue);

            ComputeVisibility();
        }

        public void MoveChild(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
            {
                return;
            }
            _children.Move(oldIndex, newIndex);
            ChildMoved(oldIndex, newIndex);
        }

        protected virtual void ChildMoved(int oldIndex, int newIndex)
        {
        }

        public void RemoveChildAt(int childIndex)
        {
            _children.RemoveAt(childIndex);
        }

        public int IndexOfChild(ILayoutElement element)
        {
            return _children.Cast<ILayoutElement>().ToList().IndexOf(element);
        }

        public void InsertChildAt(int index, ILayoutElement element)
        {
            _children.Insert(index, (T)element);
        }

        public void RemoveChild(ILayoutElement element)
        {
            _children.Remove((T)element);
        }

        public void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement)
        {
            int index = _children.IndexOf((T)oldElement);
            _children.Insert(index, (T)newElement);
            _children.RemoveAt(index + 1);
        }

        public void ReplaceChildAt(int index, ILayoutElement element)
        {
            _children[index] = (T)element;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                reader.Read();
                ComputeVisibility();
                return;
            }
            string localName = reader.LocalName;
            reader.Read();
            while (true)
            {
                if (reader.LocalName == localName &&
                    reader.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    break;
                }

                XmlSerializer serializer = null;
                if (reader.LocalName == "LayoutAnchorablePaneGroup")
                {
                    serializer = new XmlSerializer(typeof(LayoutAnchorablePaneGroup));
                }
                else if (reader.LocalName == "LayoutAnchorablePane")
                {
                    serializer = new XmlSerializer(typeof(LayoutAnchorablePane));
                }
                else if (reader.LocalName == "LayoutAnchorable")
                {
                    serializer = new XmlSerializer(typeof(LayoutAnchorable));
                }
                else if (reader.LocalName == "LayoutDocumentPaneGroup")
                {
                    serializer = new XmlSerializer(typeof(LayoutDocumentPaneGroup));
                }
                else if (reader.LocalName == "LayoutDocumentPane")
                {
                    serializer = new XmlSerializer(typeof(LayoutDocumentPane));
                }
                else if (reader.LocalName == "LayoutDocument")
                {
                    serializer = new XmlSerializer(typeof(LayoutDocument));
                }
                else if (reader.LocalName == "LayoutAnchorGroup")
                {
                    serializer = new XmlSerializer(typeof(LayoutAnchorGroup));
                }
                else if (reader.LocalName == "LayoutPanel")
                {
                    serializer = new XmlSerializer(typeof(LayoutPanel));
                }

                if (serializer != null)
                {
                    Children.Add((T)serializer.Deserialize(reader));
                }
            }

            reader.ReadEndElement();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (var child in Children)
            {
                var type = child.GetType();
                var serializer = new XmlSerializer(type);
                serializer.Serialize(writer, child);
            }
        }

        #endregion
    }
}