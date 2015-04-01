using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public interface IBackgroundRenderer
    {
        #region Свойства

        KnownLayer Layer { get; }

        #endregion

        #region Методы

        void Draw(TextView textView, DrawingContext drawingContext);

        #endregion
    }
}