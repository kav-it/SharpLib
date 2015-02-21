namespace SharpLib.Docking
{
    internal interface ILayoutPositionableElementWithActualSize
    {
        #region Свойства

        double ActualWidth { get; set; }

        double ActualHeight { get; set; }

        #endregion
    }
}