namespace SharpLib.Docking
{
    internal interface ILayoutPreviousContainer
    {
        #region Свойства

        ILayoutContainer PreviousContainer { get; set; }

        string PreviousContainerId { get; set; }

        #endregion
    }
}