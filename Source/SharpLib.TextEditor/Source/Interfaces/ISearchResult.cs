using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Search
{
    public interface ISearchResult : ISegment
    {
        #region Методы

        string ReplaceWith(string replacement);

        #endregion
    }
}