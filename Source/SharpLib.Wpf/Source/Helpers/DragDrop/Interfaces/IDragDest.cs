namespace SharpLib.Wpf.Dragging
{
    public interface IDragDest
    {
        #region Методы

        void DragOver(IDropInfo dropInfo);

        void Drop(IDropInfo dropInfo);

        #endregion
    }
}