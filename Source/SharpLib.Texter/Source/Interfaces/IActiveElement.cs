using SharpLib.Texter.Document;

namespace SharpLib.Texter.Snippets
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