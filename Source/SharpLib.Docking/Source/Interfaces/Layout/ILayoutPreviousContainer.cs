namespace SharpLib.Docking.Layout
{
    internal interface ILayoutPreviousContainer
    {
        #region Свойства

        ILayoutContainer PreviousContainer { get; set; }

        string PreviousContainerId { get; set; }

        #endregion
    }
}