﻿using System.Windows.Input;

namespace ICSharpCode.AvalonEdit
{
    public static class AvalonEditCommands
    {
        #region Поля

        public static readonly RoutedCommand ConvertLeadingSpacesToTabs = new RoutedCommand("ConvertLeadingSpacesToTabs", typeof(TextEditor));

        public static readonly RoutedCommand ConvertLeadingTabsToSpaces = new RoutedCommand("ConvertLeadingTabsToSpaces", typeof(TextEditor));

        public static readonly RoutedCommand ConvertSpacesToTabs = new RoutedCommand("ConvertSpacesToTabs", typeof(TextEditor));

        public static readonly RoutedCommand ConvertTabsToSpaces = new RoutedCommand("ConvertTabsToSpaces", typeof(TextEditor));

        public static readonly RoutedCommand ConvertToLowercase = new RoutedCommand("ConvertToLowercase", typeof(TextEditor));

        public static readonly RoutedCommand ConvertToTitleCase = new RoutedCommand("ConvertToTitleCase", typeof(TextEditor));

        public static readonly RoutedCommand ConvertToUppercase = new RoutedCommand("ConvertToUppercase", typeof(TextEditor));

        public static readonly RoutedCommand DeleteLine = new RoutedCommand(
            "DeleteLine", typeof(TextEditor),
            new InputGestureCollection
            {
                new KeyGesture(Key.D, ModifierKeys.Control)
            });

        public static readonly RoutedCommand IndentSelection = new RoutedCommand(
            "IndentSelection", typeof(TextEditor),
            new InputGestureCollection
            {
                new KeyGesture(Key.I, ModifierKeys.Control)
            });

        public static readonly RoutedCommand InvertCase = new RoutedCommand("InvertCase", typeof(TextEditor));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        public static readonly RoutedCommand RemoveLeadingWhitespace = new RoutedCommand("RemoveLeadingWhitespace", typeof(TextEditor));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
            Justification = "WPF uses 'Whitespace'")]
        public static readonly RoutedCommand RemoveTrailingWhitespace = new RoutedCommand("RemoveTrailingWhitespace", typeof(TextEditor));

        #endregion
    }
}