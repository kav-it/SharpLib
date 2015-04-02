using System.Text.RegularExpressions;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Search
{
    internal class SearchResult : TextSegment, ISearchResult
    {
        #region Свойства

        public Match Data { get; set; }

        #endregion

        #region Методы

        public string ReplaceWith(string replacement)
        {
            return Data.Result(replacement);
        }

        #endregion
    }
}