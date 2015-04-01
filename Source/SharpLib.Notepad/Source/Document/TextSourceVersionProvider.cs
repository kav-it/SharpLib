using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Document
{
#if !NREFACTORY

    public class TextSourceVersionProvider
    {
        #region Поля

        private Version currentVersion;

        #endregion

        #region Свойства

        public ITextSourceVersion CurrentVersion
        {
            get { return currentVersion; }
        }

        #endregion

        #region Конструктор

        public TextSourceVersionProvider()
        {
            currentVersion = new Version(this);
        }

        #endregion

        #region Методы

        public void AppendChange(TextChangeEventArgs change)
        {
            if (change == null)
            {
                throw new ArgumentNullException("change");
            }
            currentVersion.change = change;
            currentVersion.next = new Version(currentVersion);
            currentVersion = currentVersion.next;
        }

        #endregion

        #region Вложенный класс: Version

        [DebuggerDisplay("Version #{id}")]
        private sealed class Version : ITextSourceVersion
        {
            #region Поля

            private readonly int id;

            private readonly TextSourceVersionProvider provider;

            internal TextChangeEventArgs change;

            internal Version next;

            #endregion

            #region Конструктор

            internal Version(TextSourceVersionProvider provider)
            {
                this.provider = provider;
            }

            internal Version(Version prev)
            {
                provider = prev.provider;
                id = unchecked(prev.id + 1);
            }

            #endregion

            #region Методы

            public bool BelongsToSameDocumentAs(ITextSourceVersion other)
            {
                var o = other as Version;
                return o != null && provider == o.provider;
            }

            public int CompareAge(ITextSourceVersion other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException("other");
                }
                var o = other as Version;
                if (o == null || provider != o.provider)
                {
                    throw new ArgumentException("Versions do not belong to the same document.");
                }

                return Math.Sign(unchecked(id - o.id));
            }

            public IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other)
            {
                int result = CompareAge(other);
                var o = (Version)other;
                if (result < 0)
                {
                    return GetForwardChanges(o);
                }
                if (result > 0)
                {
                    return o.GetForwardChanges(this).Reverse().Select(change => change.Invert());
                }
                return Empty<TextChangeEventArgs>.Array;
            }

            private IEnumerable<TextChangeEventArgs> GetForwardChanges(Version other)
            {
                for (var node = this; node != other; node = node.next)
                {
                    yield return node.change;
                }
            }

            public int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement)
            {
                int offset = oldOffset;
                foreach (var e in GetChangesTo(other))
                {
                    offset = e.GetNewOffset(offset, movement);
                }
                return offset;
            }

            #endregion
        }

        #endregion
    }
#endif
}