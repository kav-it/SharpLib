using System.Collections.Generic;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Editing
{
    public interface IReadOnlySectionProvider
    {
        #region Методы

        bool CanInsert(int offset);

        IEnumerable<ISegment> GetDeletableSegments(ISegment segment);

        #endregion
    }
}