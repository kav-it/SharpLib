using System.Collections.Generic;
#if NREFACTORY
using ICSharpCode.NRefactory.Editor;
#else
using SharpLib.Notepad.Document;

#endif

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