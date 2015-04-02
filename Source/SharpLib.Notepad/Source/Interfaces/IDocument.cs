using System;

namespace SharpLib.Notepad.Document
{
    public interface IDocument : ITextSource, IServiceProvider
    {
        #region Свойства

        new string Text { get; set; }

        int LineCount { get; }

        string FileName { get; }

        #endregion

        #region События

        event EventHandler ChangeCompleted;

        event EventHandler FileNameChanged;

        event EventHandler<TextChangeEventArgs> TextChanged;

        event EventHandler<TextChangeEventArgs> TextChanging;

        #endregion

        #region Методы

        IDocumentLine GetLineByNumber(int lineNumber);

        IDocumentLine GetLineByOffset(int offset);

        int GetOffset(int line, int column);

        int GetOffset(TextLocation location);

        TextLocation GetLocation(int offset);

        void Insert(int offset, string text);

        void Insert(int offset, ITextSource text);

        void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType);

        void Insert(int offset, ITextSource text, AnchorMovementType defaultAnchorMovementType);

        void Remove(int offset, int length);

        void Replace(int offset, int length, string newText);

        void Replace(int offset, int length, ITextSource newText);

        void StartUndoableAction();

        void EndUndoableAction();

        IDisposable OpenUndoGroup();

        ITextAnchor CreateAnchor(int offset);

        #endregion
    }
}