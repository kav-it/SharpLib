using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing
{
    internal sealed class ReadOnlySectionDocument : IReadOnlySectionProvider
    {
        #region Поля

        public static readonly ReadOnlySectionDocument Instance = new ReadOnlySectionDocument();

        #endregion

        #region Методы

        public bool CanInsert(int offset)
        {
            return false;
        }

        public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
        {
            return Enumerable.Empty<ISegment>();
        }

        #endregion
    }
}