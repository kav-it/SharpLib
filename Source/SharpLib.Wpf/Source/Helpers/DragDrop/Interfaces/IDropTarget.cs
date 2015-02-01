namespace SharpLib.Wpf.Dragging
{
    public interface IDropTarget
    {
        #region Методы

        void DragOver(IDropInfo dropInfo);

        void Drop(IDropInfo dropInfo);

        #endregion
    }
}