using System;
using System.Collections.Generic;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Highlighting.Xshd
{
    [Serializable]
    public class XshdRuleSet : XshdElement
    {
        #region Поля

        private readonly NullSafeCollection<XshdElement> elements = new NullSafeCollection<XshdElement>();

        #endregion

        #region Свойства

        public string Name { get; set; }

        public bool? IgnoreCase { get; set; }

        public IList<XshdElement> Elements
        {
            get { return elements; }
        }

        #endregion

        #region Методы

        public void AcceptElements(IXshdVisitor visitor)
        {
            foreach (XshdElement element in Elements)
            {
                element.AcceptVisitor(visitor);
            }
        }

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return visitor.VisitRuleSet(this);
        }

        #endregion
    }
}