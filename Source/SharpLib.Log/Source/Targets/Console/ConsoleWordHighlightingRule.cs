using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpLib.Log
{
    [LogConfigurationItem]
    public class ConsoleWordHighlightingRule
    {
        #region Поля

        private Regex _compiledRegex;

        #endregion

        #region Свойства

        public string Regex { get; set; }

        public string Text { get; set; }

        [DefaultValue(false)]
        public bool WholeWords { get; set; }

        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        public Regex CompiledRegex
        {
            get
            {
                if (_compiledRegex == null)
                {
                    string regexpression = Regex;

                    if (regexpression == null && Text != null)
                    {
                        regexpression = System.Text.RegularExpressions.Regex.Escape(Text);
                        if (WholeWords)
                        {
                            regexpression = "\b" + regexpression + "\b";
                        }
                    }

                    RegexOptions regexOptions = RegexOptions.Compiled;
                    if (IgnoreCase)
                    {
                        regexOptions |= RegexOptions.IgnoreCase;
                    }

                    _compiledRegex = new Regex(regexpression, regexOptions);
                }

                return _compiledRegex;
            }
        }

        [DefaultValue("NoChange")]
        public ConsoleOutputColor ForegroundColor { get; set; }

        [DefaultValue("NoChange")]
        public ConsoleOutputColor BackgroundColor { get; set; }

        #endregion

        #region Конструктор

        public ConsoleWordHighlightingRule()
        {
            BackgroundColor = ConsoleOutputColor.NoChange;
            ForegroundColor = ConsoleOutputColor.NoChange;
        }

        public ConsoleWordHighlightingRule(string text, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
        {
            Text = text;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        #endregion

        #region Методы

        internal string MatchEvaluator(Match m)
        {
            StringBuilder result = new StringBuilder();

            result.Append('\a');
            result.Append((char)((int)ForegroundColor + 'A'));
            result.Append((char)((int)BackgroundColor + 'A'));
            result.Append(m.Value);
            result.Append('\a');
            result.Append('X');

            return result.ToString();
        }

        internal string ReplaceWithEscapeSequences(string message)
        {
            return CompiledRegex.Replace(message, MatchEvaluator);
        }

        #endregion
    }
}