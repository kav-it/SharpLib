using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Editing
{
    public class TextAreaDefaultInputHandler : TextAreaInputHandler
    {
        #region Свойства

        public TextAreaInputHandler CaretNavigation { get; private set; }

        public TextAreaInputHandler Editing { get; private set; }

        public ITextAreaInputHandler MouseSelection { get; private set; }

        #endregion

        #region Конструктор

        public TextAreaDefaultInputHandler(TextArea textArea)
            : base(textArea)
        {
            NestedInputHandlers.Add(CaretNavigation = CaretNavigationCommandHandler.Create(textArea));
            NestedInputHandlers.Add(Editing = EditingCommandHandler.Create(textArea));
            NestedInputHandlers.Add(MouseSelection = new SelectionMouseHandler(textArea));

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo, CanExecuteRedo));
        }

        #endregion

        #region Методы

        internal static KeyBinding CreateFrozenKeyBinding(ICommand command, ModifierKeys modifiers, Key key)
        {
            var kb = new KeyBinding(command, key, modifiers);

            var f = kb as Freezable;
            if (f != null)
            {
                f.Freeze();
            }
            return kb;
        }

        internal static void WorkaroundWPFMemoryLeak(List<InputBinding> inputBindings)
        {
            var dummyElement = new UIElement();
            dummyElement.InputBindings.AddRange(inputBindings);
        }

        private UndoStack GetUndoStack()
        {
            var document = TextArea.Document;
            if (document != null)
            {
                return document.UndoStack;
            }
            return null;
        }

        private void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            var undoStack = GetUndoStack();
            if (undoStack != null)
            {
                if (undoStack.CanUndo)
                {
                    undoStack.Undo();
                    TextArea.Caret.BringCaretToView();
                }
                e.Handled = true;
            }
        }

        private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            var undoStack = GetUndoStack();
            if (undoStack != null)
            {
                e.Handled = true;
                e.CanExecute = undoStack.CanUndo;
            }
        }

        private void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
        {
            var undoStack = GetUndoStack();
            if (undoStack != null)
            {
                if (undoStack.CanRedo)
                {
                    undoStack.Redo();
                    TextArea.Caret.BringCaretToView();
                }
                e.Handled = true;
            }
        }

        private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            var undoStack = GetUndoStack();
            if (undoStack != null)
            {
                e.Handled = true;
                e.CanExecute = undoStack.CanRedo;
            }
        }

        #endregion
    }
}