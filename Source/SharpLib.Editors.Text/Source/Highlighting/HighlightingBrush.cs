using System;
using System.Windows.Media;

using SharpLib.Notepad.Rendering;

namespace SharpLib.Notepad.Highlighting
{
    [Serializable]
    public abstract class HighlightingBrush
    {
        #region Методы

        public abstract Brush GetBrush(ITextRunConstructionContext context);

        public virtual Color? GetColor(ITextRunConstructionContext context)
        {
            var scb = GetBrush(context) as SolidColorBrush;
            if (scb != null)
            {
                return scb.Color;
            }
            return null;
        }

        #endregion
    }
}