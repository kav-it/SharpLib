namespace ICSharpCode.AvalonEdit.Document
{
    internal interface ISegmentTree
    {
        #region ������

        void Add(TextSegment s);

        void Remove(TextSegment s);

        void UpdateAugmentedData(TextSegment s);

        #endregion
    }
}