using System.Collections.Generic;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using ICSharpCode.AvalonEdit.Document;

#endif

namespace ICSharpCode.AvalonEdit.Editing
{
    public interface IReadOnlySectionProvider
    {
        #region Методы

        bool CanInsert(int offset);

        IEnumerable<ISegment> GetDeletableSegments(ISegment segment);

        #endregion
    }
}