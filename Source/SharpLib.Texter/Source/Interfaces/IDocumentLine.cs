namespace SharpLib.Texter.Document
{
    public interface IDocumentLine : ISegment
    {
        #region Свойства

        int TotalLength { get; }

        int DelimiterLength { get; }

        int LineNumber { get; }

        IDocumentLine PreviousLine { get; }

        IDocumentLine NextLine { get; }

        bool IsDeleted { get; }

        #endregion
    }
}