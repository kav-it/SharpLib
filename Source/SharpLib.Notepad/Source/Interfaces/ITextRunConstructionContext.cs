using System.Windows.Media.TextFormatting;

using SharpLib.Notepad.Document;
using SharpLib.Notepad.Utils;

namespace SharpLib.Notepad.Rendering
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