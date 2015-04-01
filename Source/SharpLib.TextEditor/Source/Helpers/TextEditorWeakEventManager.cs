using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit
{
    public static class TextEditorWeakEventManager
    {
        #region Вложенный класс: DocumentChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, ITextEditorComponent>
        {
            #region Методы

            protected override void StartListening(ITextEditorComponent source)
            {
                source.DocumentChanged += DeliverEvent;
            }

            protected override void StopListening(ITextEditorComponent source)
            {
                source.DocumentChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: OptionChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class OptionChanged : WeakEventManagerBase<OptionChanged, ITextEditorComponent>
        {
            #region Методы

            protected override void StartListening(ITextEditorComponent source)
            {
                source.OptionChanged += DeliverEvent;
            }

            protected override void StopListening(ITextEditorComponent source)
            {
                source.OptionChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion
    }
}