using System;
using System.Collections.Generic;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Search
{
    public interface ISearchStrategy : IEquatable<ISearchStrategy>
    {
        #region Методы

        IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length);

        ISearchResult FindNext(ITextSource document, int offset, int length);

        #endregion
    }
}