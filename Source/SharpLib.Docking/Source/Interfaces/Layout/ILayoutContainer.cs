using System.Collections.Generic;

namespace SharpLib.Docking
{
    public interface ILayoutContainer : ILayoutElement
    {
        #region Свойства

        IEnumerable<ILayoutElement> Children { get; }

        int ChildrenCount { get; }

        #endregion

        #region Методы

        void RemoveChild(ILayoutElement element);

        void ReplaceChild(ILayoutElement oldElement, ILayoutElement newElement);

        #endregion
    }
}