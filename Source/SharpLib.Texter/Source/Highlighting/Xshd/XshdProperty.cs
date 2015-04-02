using System;

namespace SharpLib.Texter.Highlighting.Xshd
{
    [Serializable]
    public class XshdProperty : XshdElement
    {
        #region Свойства

        public string Name { get; set; }

        public string Value { get; set; }

        #endregion

        #region Методы

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return null;
        }

        #endregion
    }
}