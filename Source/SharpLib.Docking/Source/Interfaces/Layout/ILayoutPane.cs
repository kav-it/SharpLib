namespace SharpLib.Docking.Layout
{
    public interface ILayoutPane : ILayoutContainer, ILayoutElementWithVisibility
    {
        #region Методы

        void MoveChild(int oldIndex, int newIndex);

        void RemoveChildAt(int childIndex);

        #endregion
    }
}