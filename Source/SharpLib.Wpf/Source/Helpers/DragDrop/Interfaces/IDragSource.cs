namespace SharpLib.Wpf.Dragging
{
    public interface IDragSource
    {
        #region Методы

        void StartDrag(IDragInfo dragInfo);

        bool CanStartDrag(IDragInfo dragInfo);

        void Dropped(IDropInfo dropInfo);

        void DragCancelled();

        #endregion
    }
}