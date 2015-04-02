using System.Windows.Media.TextFormatting;

using SharpLib.Texter.Document;
using SharpLib.Texter.Utils;

namespace SharpLib.Texter.Rendering
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