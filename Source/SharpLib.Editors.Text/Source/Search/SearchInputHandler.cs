using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;

using SharpLib.Notepad.Editing;

namespace SharpLib.Notepad.Search
{
    public class SearchInputHandler : TextAreaInputHandler
    {
        #region Поля

        private readonly SearchPanel panel;

        #endregion

        #region События

        public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged
        {
            add { panel.SearchOptionsChanged += value; }
            remove { panel.SearchOptionsChanged -= value; }
        }

        #endregion

        #region Конструктор

        [Obsolete("Use SearchPanel.Install instead")]
        public SearchInputHandler(TextArea textArea)
            : base(textArea)
        {
            RegisterCommands(CommandBindings);
            panel = SearchPanel.Install(textArea);
        }

        internal SearchInputHandler(TextArea textArea, SearchPanel panel)
            : base(textArea)
        {
            RegisterCommands(CommandBindings);
            this.panel = panel;
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
            panel.Open();
            if (!(TextArea.Selection.IsEmpty || TextArea.Selection.IsMultiline))
            {
                panel.SearchPattern = TextArea.Selection.GetText();
            }
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate { panel.Reactivate(); });
        }

        private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
        {
            if (panel.IsClosed)
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
            if (!panel.IsClosed)
            {
                panel.FindNext();
                e.Handled = true;
            }
        }

        private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
        {
            if (!panel.IsClosed)
            {
                panel.FindPrevious();
                e.Handled = true;
            }
        }

        private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e)
        {
            if (!panel.IsClosed)
            {
                panel.Close();
                e.Handled = true;
            }
        }

        #endregion
    }
}