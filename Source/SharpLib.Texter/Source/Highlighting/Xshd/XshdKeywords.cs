﻿using System;
using System.Collections.Generic;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Highlighting.Xshd
{
    [Serializable]
    public class XshdKeywords : XshdElement
    {
        #region Поля

        private readonly NullSafeCollection<string> words = new NullSafeCollection<string>();

        #endregion

        #region Свойства

        public XshdReference<XshdColor> ColorReference { get; set; }

        public IList<string> Words
        {
            get { return words; }
        }

        #endregion

        #region Методы

        public override object AcceptVisitor(IXshdVisitor visitor)
        {
            return visitor.VisitKeywords(this);
        }

        #endregion
    }
}