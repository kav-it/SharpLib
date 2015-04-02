using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

using SharpLib.Texter.Editing;

namespace SharpLib.Texter.Search
{
    public class SearchInputHandler : TextAreaInputHandler
    {
        #region Поля

        private readonly SearchPanel _panel;

        #endregion

        #region События

        public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged
        {
            add { _panel.SearchOptionsChanged += value; }
            remove { _panel.SearchOptionsChanged -= value; }
        }

        #endregion

        #region Конструктор

        [Obsolete("Use SearchPanel.Install instead")]
        public SearchInputHandler(TextArea textArea)
            : base(textArea)
        {
            RegisterCommands(CommandBindings);
            _panel = SearchPanel.Install(textArea);
        }

        internal SearchInputHandler(TextArea textArea, SearchPanel panel)
            : base(textArea)
        {
            RegisterCommands(CommandBindings);
            _panel = panel;
        }

        #endregion

        #region Методы

        internal void RegisterGlobalCommands(CommandBindingCollection commandBindings)
        {
            commandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
            commandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
            commandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
        }

        private void RegisterCommands(ICollection<CommandBinding> commandBindings)
        {
            commandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
            commandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
            commandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
            commandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, ExecuteCloseSearchPanel, CanExecuteWithOpenSearchPanel));
        }

        private void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
        {
            _panel.Open();
            if (!(TextArea.Selection.IsEmpty || TextArea.Selection.IsMultiline))
            {
                _panel.SearchPattern = TextArea.Selection.GetText();
            }
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate { _panel.Reactivate(); });
        }

        private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
        {
            if (_panel.IsClosed)
            {
                e.CanExecute = false;

                e.ContinueRouting = true;
            }
            else
            {
                e.CanExecute = true;
                e.Handled = true;
            }
        }

        private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_panel.IsClosed)
            {
                _panel.FindNext();
                e.Handled = true;
            }
        }

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_panel.IsClosed)
            {
                _panel.FindPrevious();
                e.Handled = true;
            }
        }

        private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_panel.IsClosed)
            {
                _panel.Close();
                e.Handled = true;
            }
        }

        #endregion
    }
}