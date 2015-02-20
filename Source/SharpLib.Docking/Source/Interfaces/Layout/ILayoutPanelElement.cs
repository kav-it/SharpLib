namespace SharpLib.Docking.Layout
{
    public interface ILayoutPanelElement : ILayoutElement
    {
        #region Свойства

        bool IsVisible { get; }

        #endregion
    }
}