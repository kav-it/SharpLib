using System;
using System.Collections.Generic;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Editing
{
    internal sealed class NoReadOnlySections : IReadOnlySectionProvider
    {
        #region Поля

        public static readonly NoReadOnlySections Instance = new NoReadOnlySections();

        #endregion

        #region Методы

        public bool CanInsert(int offset)
        {
            return true;
        }

        public IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException("segment");
            }

            return ExtensionMethods.Sequence(segment);
        }

        #endregion
    }
}