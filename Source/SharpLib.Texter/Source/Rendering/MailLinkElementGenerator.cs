using System;
using System.Text.RegularExpressions;

namespace SharpLib.Texter.Rendering
{
    internal sealed class MailLinkElementGenerator : LinkElementGenerator
    {
        #region Конструктор

        public MailLinkElementGenerator()
            : base(defaultMailRegex)
        {
        }

        #endregion

        #region Методы

        protected override Uri GetUriFromMatch(Match match)
        {
            return new Uri("mailto:" + match.Value);
        }

        #endregion
    }
}