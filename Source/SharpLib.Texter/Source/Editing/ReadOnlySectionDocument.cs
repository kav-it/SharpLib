﻿using System.Collections.Generic;
using System.Linq;

using SharpLib.Texter.Document;

namespace SharpLib.Texter.Editing
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