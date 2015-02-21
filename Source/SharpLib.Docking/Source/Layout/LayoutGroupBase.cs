using System;
using System.Xml.Serialization;

namespace SharpLib.Docking
{
    [Serializable]
    public abstract class LayoutGroupBase : LayoutElement
    {
        #region События

        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler ChildrenCollectionChanged;

        [field: NonSerialized]
        [field: XmlIgnore]
        public event EventHandler<ChildrenTreeChangedEventArgs> ChildrenTreeChanged;

        #endregion

        #region Методы

        protected virtual void OnChildrenCollectionChanged()
        {
            if (ChildrenCollectionChanged != null)
            {
                ChildrenCollectionChanged(this, EventArgs.Empty);
            }
        }

        protected void NotifyChildrenTreeChanged(ChildrenTreeChange change)
        {
            OnChildrenTreeChanged(change);
            var parentGroup = Parent as LayoutGroupBase;
            if (parentGroup != null)
            {
                parentGroup.NotifyChildrenTreeChanged(ChildrenTreeChange.TreeChanged);
            }
        }

        protected virtual void OnChildrenTreeChanged(ChildrenTreeChange change)
        {
            if (ChildrenTreeChanged != null)
            {
                ChildrenTreeChanged(this, new ChildrenTreeChangedEventArgs(change));
            }
        }

        #endregion
    }
}