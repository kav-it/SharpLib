using System;

namespace SharpLib.Notepad.Highlighting.Xshd
{
    [Serializable]
    public struct XshdReference<T> : IEquatable<XshdReference<T>> where T : XshdElement
    {
        #region Поля

        private readonly T inlineElement;

        private readonly string referencedDefinition;

        private readonly string referencedElement;

        #endregion

        #region Свойства

        public string ReferencedDefinition
        {
            get { return referencedDefinition; }
        }

        public string ReferencedElement
        {
            get { return referencedElement; }
        }

        public T InlineElement
        {
            get { return inlineElement; }
        }

        #endregion

        #region Конструктор

        public XshdReference(string referencedDefinition, string referencedElement)
        {
            if (referencedElement == null)
            {
                throw new ArgumentNullException("referencedElement");
            }
            this.referencedDefinition = referencedDefinition;
            this.referencedElement = referencedElement;
            inlineElement = null;
        }

        public XshdReference(T inlineElement)
        {
            if (inlineElement == null)
            {
                throw new ArgumentNullException("inlineElement");
            }
            referencedDefinition = null;
            referencedElement = null;
            this.inlineElement = inlineElement;
        }

        #endregion

        #region Методы

        public object AcceptVisitor(IXshdVisitor visitor)
        {
            if (inlineElement != null)
            {
                return inlineElement.AcceptVisitor(visitor);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is XshdReference<T>)
            {
                return Equals((XshdReference<T>)obj);
            }
            return false;
        }

        public bool Equals(XshdReference<T> other)
        {
            return referencedDefinition == other.referencedDefinition
                   && referencedElement == other.referencedElement
                   && inlineElement == other.inlineElement;
        }

        public override int GetHashCode()
        {
            return GetHashCode(referencedDefinition) ^ GetHashCode(referencedElement) ^ GetHashCode(inlineElement);
        }

        private static int GetHashCode(object o)
        {
            return o != null ? o.GetHashCode() : 0;
        }

        #endregion

        public static bool operator ==(XshdReference<T> left, XshdReference<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(XshdReference<T> left, XshdReference<T> right)
        {
            return !left.Equals(right);
        }
    }
}