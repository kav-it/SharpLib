using System.Windows.Media.TextFormatting;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
    public interface ITextRunConstructionContext
    {
        #region Свойства

        TextDocument Document { get; }

        TextView TextView { get; }

        VisualLine VisualLine { get; }

        TextRunProperties GlobalTextRunProperties { get; }

        #endregion

        #region Методы

        StringSegment GetText(int offset, int length);

        #endregion
    }
}