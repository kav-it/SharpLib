namespace SharpLib.Notepad.Search
{
    public class Localization
    {
        #region Свойства

        public virtual string MatchCaseText
        {
            get { return "Match case"; }
        }

        public virtual string MatchWholeWordsText
        {
            get { return "Match whole words"; }
        }

        public virtual string UseRegexText
        {
            get { return "Use regular expressions"; }
        }

        public virtual string FindNextText
        {
            get { return "Find next (F3)"; }
        }

        public virtual string FindPreviousText
        {
            get { return "Find previous (Shift+F3)"; }
        }

        public virtual string ErrorText
        {
            get { return "Error: "; }
        }

        public virtual string NoMatchesFoundText
        {
            get { return "No matches found!"; }
        }

        #endregion
    }
}