using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public static class TextViewWeakEventManager
    {
        #region Вложенный класс: DocumentChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, TextView>
        {
            #region Методы

            protected override void StartListening(TextView source)
            {
                source.DocumentChanged += DeliverEvent;
            }

            protected override void StopListening(TextView source)
            {
                source.DocumentChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: ScrollOffsetChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class ScrollOffsetChanged : WeakEventManagerBase<ScrollOffsetChanged, TextView>
        {
            #region Методы

            protected override void StartListening(TextView source)
            {
                source.ScrollOffsetChanged += DeliverEvent;
            }

            protected override void StopListening(TextView source)
            {
                source.ScrollOffsetChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: VisualLinesChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class VisualLinesChanged : WeakEventManagerBase<VisualLinesChanged, TextView>
        {
            #region Методы

            protected override void StartListening(TextView source)
            {
                source.VisualLinesChanged += DeliverEvent;
            }

            protected override void StopListening(TextView source)
            {
                source.VisualLinesChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion
    }
}