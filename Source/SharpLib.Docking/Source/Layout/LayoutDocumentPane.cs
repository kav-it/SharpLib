﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace SharpLib.Docking
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutDocumentPane : LayoutPositionableGroup<LayoutContent>, ILayoutDocumentPane, ILayoutContentSelector, ILayoutPaneSerializable
    {
        #region Поля

        private string _id;

        private int _selectedIndex;

        #endregion

        #region Свойства

        public int SelectedContentIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value < 0 || value >= Children.Count)
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

        public IEnumerable<LayoutContent> ChildrenSorted
        {
            get
            {
                var listSorted = Children.ToList();
                listSorted.Sort();
                return listSorted;
            }
        }

        string ILayoutPaneSerializable.Id
        {
            get { return _id; }
            set { _id = value; }
        }

        #endregion

        #region Конструктор

        public LayoutDocumentPane()
        {
            _selectedIndex = -1;
        }

        public LayoutDocumentPane(LayoutContent firstChild): this()
        {
            Children.Add(firstChild);
        }

        #endregion

        #region Методы

        protected override bool GetVisibility()
        {
            if (Parent is LayoutDocumentPaneGroup)
            {
                return ChildrenCount > 0;
            }

            return true;
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
            if (SelectedContentIndex >= ChildrenCount)
            {
                SelectedContentIndex = Children.Count - 1;
            }
            if (SelectedContentIndex == -1 && ChildrenCount > 0)
            {
                if (Root == null)
                {
                    SelectedContentIndex = 0;
                }
                else
                {
                    var childrenToSelect = Children.OrderByDescending(c => c.LastActivationTimeStamp.GetValueOrDefault()).First();
                    SelectedContentIndex = Children.IndexOf(childrenToSelect);
                    childrenToSelect.IsActive = true;
                }
            }

            base.OnChildrenCollectionChanged();

            RaisePropertyChanged("ChildrenSorted");
        }

        public int IndexOf(LayoutContent content)
        {
            return Children.IndexOf(content);
        }

        protected override void OnIsVisibleChanged()
        {
            UpdateParentVisibility();
            base.OnIsVisibleChanged();
        }

        private void UpdateParentVisibility()
        {
            var parentPane = Parent as ILayoutElementWithVisibility;
            if (parentPane != null)
            {
                parentPane.ComputeVisibility();
            }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            if (_id != null)
            {
                writer.WriteAttributeString("Id", _id);
            }

            WriteXmlInternalDesc(writer);

            base.WriteXml(writer);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToAttribute("Id"))
            {
                _id = reader.Value;
            }

            ReadXmlInternalDesc(reader);

            base.ReadXml(reader);
        }

        public override void ConsoleDump(int tab)
        {
            // Trace.Write(new string(' ', tab * 4));
            // Trace.WriteLine("DocumentPane()");

            foreach (LayoutContent child in Children)
            {
                child.ConsoleDump(tab + 1);
            }
        }

        #endregion
    }
}