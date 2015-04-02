using System.Windows.Input;

namespace SharpLib.Texter.Search
{
    public static class SearchCommands
    {
        #region Поля

        public static readonly RoutedCommand CloseSearchPanel;

        public static readonly RoutedCommand FindNext;

        public static readonly RoutedCommand FindPrevious;

        #endregion

        #region Конструктор

        static SearchCommands()
        {
            CloseSearchPanel = new RoutedCommand("CloseSearchPanel", typeof(SearchPanel), new InputGestureCollection
            {
                new KeyGesture(Key.Escape)
            });
            FindNext = new RoutedCommand("FindNext", typeof(SearchPanel), new InputGestureCollection
            {
                new KeyGesture(Key.F3)
            });
            FindPrevious = new RoutedCommand("FindPrevious", typeof(SearchPanel), new InputGestureCollection
            {
                new KeyGesture(Key.F3, ModifierKeys.Shift)
            });
        }

        #endregion
    }
}