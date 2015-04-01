using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.AvalonEdit.Utils
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    [Serializable]
    public sealed class ImmutableStack<T> : IEnumerable<T>
    {
        #region Поля

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "ImmutableStack is immutable")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly ImmutableStack<T> Empty = new ImmutableStack<T>();

        private readonly ImmutableStack<T> next;

        private readonly T value;

        #endregion

        #region Свойства

        public bool IsEmpty
        {
            get { return next == null; }
        }

        #endregion

        #region Конструктор

        private ImmutableStack()
        {
        }

        private ImmutableStack(T value, ImmutableStack<T> next)
        {
            this.value = value;
            this.next = next;
        }

        #endregion

        #region Методы

        public ImmutableStack<T> Push(T item)
        {
            return new ImmutableStack<T>(item, this);
        }

        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Operation not valid on empty stack.");
            }
            return value;
        }

        public T PeekOrDefault()
        {
            return value;
        }

        public ImmutableStack<T> Pop()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Operation not valid on empty stack.");
            }
            return next;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var t = this;
            while (!t.IsEmpty)
            {
                yield return t.value;
                t = t.next;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var b = new StringBuilder("[Stack");
            foreach (T val in this)
            {
                b.Append(' ');
                b.Append(val);
            }
            b.Append(']');
            return b.ToString();
        }

        #endregion
    }
}