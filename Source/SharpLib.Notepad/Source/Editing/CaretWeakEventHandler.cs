using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Editing
{
    public static class CaretWeakEventManager
    {
        #region Вложенный класс: PositionChanged

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public sealed class PositionChanged : WeakEventManagerBase<PositionChanged, Caret>
        {
            #region Методы

            protected override void StartListening(Caret source)
            {
                source.PositionChanged += DeliverEvent;
            }

            protected override void StopListening(Caret source)
            {
                source.PositionChanged -= DeliverEvent;
            }

            #endregion
        }

        #endregion
    }
}