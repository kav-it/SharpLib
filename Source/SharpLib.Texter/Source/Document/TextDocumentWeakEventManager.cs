using System;

using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Document
{
    public static class TextDocumentWeakEventManager
    {
        #region Вложенный класс: Changed

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Changed : WeakEventManagerBase<Changed, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.Changed += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.Changed -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: Changing

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class Changing : WeakEventManagerBase<Changing, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.Changing += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.Changing -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: LineCountChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [Obsolete("The TextDocument.LineCountChanged event will be removed in a future version. Use PropertyChangedEventManager instead.")]
        public sealed class LineCountChanged : WeakEventManagerBase<LineCountChanged, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.LineCountChanged += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.LineCountChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: TextChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class TextChanged : WeakEventManagerBase<TextChanged, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.TextChanged += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.TextChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: TextLengthChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        [Obsolete("The TextDocument.TextLengthChanged event will be removed in a future version. Use PropertyChangedEventManager instead.")]
        public sealed class TextLengthChanged : WeakEventManagerBase<TextLengthChanged, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.TextLengthChanged += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.TextLengthChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: UpdateFinished

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class UpdateFinished : WeakEventManagerBase<UpdateFinished, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.UpdateFinished += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.UpdateFinished -= DeliverEvent;
            }

            #endregion
        }

        #endregion

        #region Вложенный класс: UpdateStarted

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class UpdateStarted : WeakEventManagerBase<UpdateStarted, TextDocument>
        {
            #region Методы

            protected override void StartListening(TextDocument source)
            {
                source.UpdateStarted += DeliverEvent;
            }

            protected override void StopListening(TextDocument source)
            {
                source.UpdateStarted -= DeliverEvent;
            }

            #endregion
        }

        #endregion
    }
}