namespace SharpLib.Log
{
    public abstract class LayoutBasedFilter : Filter
    {
        #region Свойства

        [RequiredParameter]
        public Layout Layout { get; set; }

        #endregion
    }
}