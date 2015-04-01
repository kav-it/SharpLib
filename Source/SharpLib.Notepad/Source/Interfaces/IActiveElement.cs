using SharpLib.Notepad.Document;

namespace SharpLib.Notepad.Snippets
{
    public interface IActiveElement
    {
        #region Свойства

        bool IsEditable { get; }

        ISegment Segment { get; }

        #endregion

        #region Методы

        void OnInsertionCompleted();

        void Deactivate(SnippetEventArgs e);

        #endregion
    }
}