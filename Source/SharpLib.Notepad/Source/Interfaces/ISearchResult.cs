using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Search
{
    public interface ISearchResult : ISegment
    {
        #region Методы

        string ReplaceWith(string replacement);

        #endregion
    }
}