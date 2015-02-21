using System;

namespace SharpLib.Docking
{
    public interface ILayoutGroup : ILayoutContainer
    {
        #region События

        event EventHandler ChildrenCollectionChanged;

        #endregion

        #region Методы

        int IndexOfChild(ILayoutElement element);

        void InsertChildAt(int index, ILayoutElement element);

        void RemoveChildAt(int index);

        void ReplaceChildAt(int index, ILayoutElement element);

        #endregion
    }
}