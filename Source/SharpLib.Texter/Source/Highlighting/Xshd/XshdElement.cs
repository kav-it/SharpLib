using System;

namespace SharpLib.Texter.Highlighting.Xshd
{
    [Serializable]
    public abstract class XshdElement
    {
        #region Свойства

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        #endregion

        #region Методы

        public abstract object AcceptVisitor(IXshdVisitor visitor);

        #endregion
    }
}