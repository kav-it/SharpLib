namespace SharpLib.Log
{
    public abstract class LayoutBasedFilter : Filter
    {
        #region ��������

        [RequiredParameter]
        public Layout Layout { get; set; }

        #endregion
    }
}