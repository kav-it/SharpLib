namespace SharpLib.Docking
{
    public interface ILayoutPanelElement : ILayoutElement
    {
        #region Свойства

        bool IsVisible { get; }

        #endregion
    }
}