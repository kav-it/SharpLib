using System.Collections.Generic;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Editing
{
    public interface IReadOnlySectionProvider
    {
        #region Методы

        bool CanInsert(int offset);

        IEnumerable<ISegment> GetDeletableSegments(ISegment segment);

        #endregion
    }
}