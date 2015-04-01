using System;

namespace SharpLib.Notepad.Search
{
    public class SearchOptionsChangedEventArgs : EventArgs
    {
        #region Свойства

        public string SearchPattern { get; private set; }

        public bool MatchCase { get; private set; }

        public bool UseRegex { get; private set; }

        public bool WholeWords { get; private set; }

        #endregion

        #region Конструктор

        public SearchOptionsChangedEventArgs(string searchPattern, bool matchCase, bool useRegex, bool wholeWords)
        {
            SearchPattern = searchPattern;
            MatchCase = matchCase;
            UseRegex = useRegex;
            WholeWords = wholeWords;
        }

        #endregion
    }
}