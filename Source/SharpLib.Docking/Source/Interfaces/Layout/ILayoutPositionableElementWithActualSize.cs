namespace SharpLib.Docking.Layout
{
    internal interface ILayoutPositionableElementWithActualSize
    {
        #region ��������

        double ActualWidth { get; set; }

        double ActualHeight { get; set; }

        #endregion
    }
}