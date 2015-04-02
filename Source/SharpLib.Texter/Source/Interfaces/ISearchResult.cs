using SharpLib.Texter.Document;

namespace SharpLib.Texter.Search
{
    public interface ISearchResult : ISegment
    {
        #region Методы

        string ReplaceWith(string replacement);

        #endregion
    }
}