using System;
using System.Text.RegularExpressions;

namespace SharpLib.Notepad.Rendering
{
    public class LinkElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
    {
        #region Поля

        internal static readonly Regex defaultLinkRegex = new Regex(@"\b(https?://|ftp://|www\.)[\w\d\._/\-~%@()+:?&=#!]*[\w\d/]");

        internal static readonly Regex defaultMailRegex = new Regex(@"\b[\w\d\.\-]+\@[\w\d\.\-]+\.[a-z]{2,6}\b");

        private readonly Regex linkRegex;

        #endregion

        #region Свойства

        public bool RequireControlModifierForClick { get; set; }

        #endregion

        #region Конструктор

        public LinkElementGenerator()
        {
            linkRegex = defaultLinkRegex;
            RequireControlModifierForClick = true;
        }

        protected LinkElementGenerator(Regex regex)
            : this()
        {
            if (regex == null)
            {
                throw new ArgumentNullException("regex");
            }
            linkRegex = regex;
        }

        #endregion

        #region Методы

        void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
        {
            RequireControlModifierForClick = options.RequireControlModifierForHyperlinkClick;
        }

        private Match GetMatch(int startOffset, out int matchOffset)
        {
            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var relevantText = CurrentContext.GetText(startOffset, endOffset - startOffset);
            var m = linkRegex.Match(relevantText.Text, relevantText.Offset, relevantText.Count);
            matchOffset = m.Success ? m.Index - relevantText.Offset + startOffset : -1;
            return m;
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            int matchOffset;
            GetMatch(startOffset, out matchOffset);
            return matchOffset;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            int matchOffset;
            var m = GetMatch(offset, out matchOffset);
            if (m.Success && matchOffset == offset)
            {
                return ConstructElementFromMatch(m);
            }
            return null;
        }

        protected virtual VisualLineElement ConstructElementFromMatch(Match m)
        {
            var uri = GetUriFromMatch(m);
            if (uri == null)
            {
                return null;
            }
            var linkText = new VisualLineLinkText(CurrentContext.VisualLine, m.Length);
            linkText.NavigateUri = uri;
            linkText.RequireControlModifierForClick = RequireControlModifierForClick;
            return linkText;
        }

        protected virtual Uri GetUriFromMatch(Match match)
        {
            string targetUrl = match.Value;
            if (targetUrl.StartsWith("www.", StringComparison.Ordinal))
            {
                targetUrl = "http://" + targetUrl;
            }
            if (Uri.IsWellFormedUriString(targetUrl, UriKind.Absolute))
            {
                return new Uri(targetUrl);
            }

            return null;
        }

        #endregion
    }
}