using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpLib.Docking.Layout
{
    [Serializable]
    [XmlInclude(typeof(LayoutAnchorableFloatingWindow))]
    [XmlInclude(typeof(LayoutDocumentFloatingWindow))]
    public abstract class LayoutFloatingWindow : LayoutElement, ILayoutContainer
    {
        #region Свойства

        public abstract IEnumerable<ILayoutElement> Children { get; }

        public abstract int ChildrenCount { get; }

        public abstract bool IsValid { get; }

        #endregion

        #region Методы

        public abstract void RemoveChild(ILayoutElement element);

        public abstract void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

        #endregion
    }
}