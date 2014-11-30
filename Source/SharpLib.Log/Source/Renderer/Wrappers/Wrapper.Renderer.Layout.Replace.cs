using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpLib.Log
{
    [LayoutRenderer("replace")]
    [ThreadAgnostic]
    public sealed class ReplaceLayoutRendererWrapper : WrapperLayoutRendererBase
    {
        #region Поля

        private Regex _regex;

        #endregion

        #region Свойства

        public string SearchFor { get; set; }

        public bool Regex { get; set; }

        public string ReplaceWith { get; set; }

        public string ReplaceGroupName { get; set; }

        public bool IgnoreCase { get; set; }

        public bool WholeWords { get; set; }

        #endregion

        #region Методы

        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();
            string regexString = SearchFor;

            if (!Regex)
            {
                regexString = System.Text.RegularExpressions.Regex.Escape(regexString);
            }

            RegexOptions regexOptions = RegexOptions.Compiled;

            if (IgnoreCase)
            {
                regexOptions |= RegexOptions.IgnoreCase;
            }

            if (WholeWords)
            {
                regexString = "\\b" + regexString + "\\b";
            }

            _regex = new Regex(regexString, regexOptions);
        }

        protected override string Transform(string text)
        {
            var replacer = new Replacer(text, ReplaceGroupName, ReplaceWith);

            return string.IsNullOrEmpty(ReplaceGroupName)
                ? _regex.Replace(text, ReplaceWith)
                : _regex.Replace(text, replacer.EvaluateMatch);
        }

        public static string ReplaceNamedGroup(string input, string groupName, string replacement, Match match)
        {
            var sb = new StringBuilder(input);
            var matchStart = match.Index;
            var matchLength = match.Length;

            var captures = match.Groups[groupName].Captures.OfType<Capture>().OrderByDescending(c => c.Index);
            foreach (var capt in captures)
            {
                if (capt == null)
                {
                    continue;
                }

                matchLength += replacement.Length - capt.Length;

                sb.Remove(capt.Index, capt.Length);
                sb.Insert(capt.Index, replacement);
            }

            var end = matchStart + matchLength;
            sb.Remove(end, sb.Length - end);
            sb.Remove(0, matchStart);

            return sb.ToString();
        }

        #endregion

        #region Вложенный класс: Replacer

        [ThreadAgnostic]
        public class Replacer
        {
            #region Поля

            private readonly string _replaceGroupName;

            private readonly string _replaceWith;

            private readonly string _text;

            #endregion

            #region Конструктор

            internal Replacer(string text, string replaceGroupName, string replaceWith)
            {
                _text = text;
                _replaceGroupName = replaceGroupName;
                _replaceWith = replaceWith;
            }

            #endregion

            #region Методы

            internal string EvaluateMatch(Match match)
            {
                return ReplaceNamedGroup(_text, _replaceGroupName, _replaceWith, match);
            }

            #endregion
        }

        #endregion
    }
}